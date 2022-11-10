using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Objects
{
    public static class ObjectUtil
    {
        public static bool IsNull(object obj)
        {
            return obj is null || obj is DBNull;
        }

        public static bool IsNullOrBlank(object obj)
        {
            if (IsNull(obj)) return true;
            return obj is string stringValue && stringValue.Trim().Length == 0;
        }

        public static void MapProperties(object src, object dest, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, bool tryMapAll = false, bool nullOverwrites = false, bool ignoreCase = false, bool ignoreErrors = false)
        {
            if (src == null) throw new ValidationException("Argument '" + nameof(src) + "' cannot be null");
            if (dest == null) throw new ValidationException("Argument '" + nameof(dest) + "' cannot be null");

            var srcType = src.GetType();
            var destType = dest.GetType();

            if (!srcType.IsClass) throw new ValidationException("'" + nameof(src) + "' is of type " + srcType.FullName + " which is not a reference type");
            if (!destType.IsClass) throw new ValidationException("'" + nameof(dest) + "' is of type " + destType.FullName + " which is not a reference type");

            //
            // Case 1 of 2 - same types
            //
            if (srcType == destType)
            {
                var srcPropValues = GetInstancePropertyValues(src, bindingFlags: bindingFlags, filter: (prop) => prop.CanRead && prop.CanWrite);
                foreach (var srcPV in srcPropValues)
                {
                    if (srcPV.Value == null && !nullOverwrites)
                        continue;
                    try
                    {
                        ((PropertyInfo)srcPV.Member).SetValue(dest, srcPV.Value);
                    }
                    catch (Exception ex)
                    {
                        if (ignoreErrors)
                            continue;
                        throw new ObjectMappingException(srcType, destType, srcPV.Member, ex);
                    }
                }
            }

            //
            // Case 2 of 2 - different types
            //
            else
            {
                var srcPropValues = GetInstancePropertyValues(src, bindingFlags: bindingFlags, filter: (prop) => prop.CanRead);
                var destProps = GetInstanceProperties(destType, bindingFlags: bindingFlags, filter: (prop) => prop.CanWrite);

                foreach (var srcPV in srcPropValues)
                {
                    var srcProp = (PropertyInfo)srcPV.Member;
                    var matchingDestProps = destProps
                        .Where
                        (
                            destProp =>
                            (
                                ignoreCase
                                ? destProp.Name.Equals(srcProp.Name, StringComparison.OrdinalIgnoreCase)
                                : destProp.Name.Equals(srcProp.Name)
                            )
                        )
                        .ToList();
                    try
                    {
                        switch (matchingDestProps.Count)
                        {
                            case 0:
                                if (ignoreErrors || !tryMapAll)
                                    break;
                                throw new ObjectMappingException(srcType, destType, srcPV.Member, "Destination property '" + srcProp.Name + "' not found");
                            case 1:
                                matchingDestProps[0].SetValue(dest, srcPV.Value);
                                break;
                            default:
                                if (ignoreErrors)
                                    break;
                                throw new ObjectMappingException(srcType, destType, srcPV.Member, "More than one destination property matching '" + srcProp.Name + "' exists" + (ignoreCase ? "" : " (case-sensitive)"));
                        }
                    }
                    catch (ObjectMappingException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new ObjectMappingException(srcType, destType, srcPV.Member, ex);
                    }
                }
            }
        }

        public static object Duplicate(object src, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, DuplicateMode duplicateMode = DuplicateMode.MapProperties, bool ignoreErrors = false)
        {
            var dest = GetInstance(src.GetType());
            if ((duplicateMode & DuplicateMode.MapProperties) == DuplicateMode.MapProperties)
            {
                MapProperties(src, dest, bindingFlags: bindingFlags, ignoreErrors: ignoreErrors);
            }
            return dest;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *            PROPERTIES - LISTS
         * * * * * * * * * * * * * * * * * * * * * * * */

        [Obsolete("This method is too generic - use GetInstanceProperties(instance), GetStaticProperties(Type) or GetAllProperties(Type)", false)]
        public static IList<PropertyInfo> GetProperties(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return GetAllProperties(instance.GetType(), bindingFlags: bindingFlags ?? (BindingFlags.Instance | BindingFlags.Public), filter: filter);
        }

        [Obsolete("This method is too generic - use GetInstancePropertiesOfType<T>(instance), GetStaticPropertiesOfType<T>(Type) or GetAllPropertiesOfType<T>(Type)", false)]
        public static IList<PropertyInfo> GetPropertiesOfType<T>(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false)
        {
            bool _filter(PropertyInfo prop)
            {
                // add type matching to the user-supplied filter, if any
                if (exactTypeMatchOnly ? typeof(T) == prop.PropertyType : typeof(T).IsAssignableFrom(prop.PropertyType))
                {
                    return filter?.Invoke(prop) ?? true;
                }
                return false;
            }
            var props = GetProperties(instance, bindingFlags: bindingFlags, filter: _filter);
            return props;
        }

        public static IList<PropertyInfo> GetInstanceProperties(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return GetAllProperties(instance.GetType(), bindingFlags: BindingFlags.Instance | (bindingFlags ?? BindingFlags.Public), filter: filter);
        }

        public static IList<PropertyInfo> GetInstanceProperties(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return GetAllProperties(type, bindingFlags: BindingFlags.Instance | (bindingFlags ?? BindingFlags.Public), filter: filter);
        }

        public static IList<PropertyInfo> GetInstanceProperties<TType>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where TType : class
        {
            return GetAllProperties(typeof(TType), bindingFlags: BindingFlags.Instance | (bindingFlags ?? BindingFlags.Public), filter: filter);
        }

        public static IList<PropertyInfo> GetInstancePropertiesOfType<T>(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false)
        {
            bool _filter(PropertyInfo prop)
            {
                // add type matching to the user-supplied filter, if any
                if (exactTypeMatchOnly ? typeof(T) == prop.PropertyType : typeof(T).IsAssignableFrom(prop.PropertyType))
                {
                    return filter?.Invoke(prop) ?? true;
                }
                return false;
            }
            var props = GetInstanceProperties(instance, bindingFlags: bindingFlags, filter: _filter);
            return props;
        }

        [Obsolete("This method is an antipattern - use GetStaticProperties(Type) instead", false)]
        public static IList<PropertyInfo> GetStaticProperties(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return GetAllProperties(instance.GetType(), bindingFlags: BindingFlags.Static | (bindingFlags ?? BindingFlags.Public), filter: filter);
        }

        public static IList<PropertyInfo> GetStaticProperties(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return GetAllProperties(type, bindingFlags: BindingFlags.Static | (bindingFlags ?? BindingFlags.Public), filter: filter);
        }

        public static IList<PropertyInfo> GetStaticProperties<TType>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where TType : class
        {
            return GetAllProperties(typeof(TType), bindingFlags: BindingFlags.Static | (bindingFlags ?? BindingFlags.Public), filter: filter);
        }

        public static IList<PropertyInfo> GetStaticPropertiesOfType<T>(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false)
        {
            bool _filter(PropertyInfo prop)
            {
                // add type matching to the user-supplied filter, if any
                if (exactTypeMatchOnly ? typeof(T) == prop.PropertyType : typeof(T).IsAssignableFrom(prop.PropertyType))
                {
                    return filter?.Invoke(prop) ?? true;
                }
                return false;
            }
            var props = GetStaticProperties(type, bindingFlags: bindingFlags, filter: _filter);
            return props;
        }

        public static IList<PropertyInfo> GetStaticPropertiesOfType<TType, T>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false) where TType : class
        {
            bool _filter(PropertyInfo prop)
            {
                // add type matching to the user-supplied filter, if any
                if (exactTypeMatchOnly ? typeof(T) == prop.PropertyType : typeof(T).IsAssignableFrom(prop.PropertyType))
                {
                    return filter?.Invoke(prop) ?? true;
                }
                return false;
            }
            var props = GetStaticProperties<TType>(bindingFlags: bindingFlags, filter: _filter);
            return props;
        }

        public static IList<PropertyInfo> GetAllProperties(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return GetAllProperties(instance.GetType(), bindingFlags: bindingFlags, filter: filter);
        }

        public static IList<PropertyInfo> GetAllProperties(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            if (!type.IsClass)
                throw new ValidationException(type.FullName + " is not a reference type");
            var allBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var props = new List<PropertyInfo>(type.GetProperties(bindingFlags ?? allBindingFlags));
            return filter != null
                ? props.Where(filter).ToList()
                : props;
        }

        public static IList<PropertyInfo> GetAllProperties<TType>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where TType : class
        {
            return GetAllProperties(typeof(TType), bindingFlags: bindingFlags, filter: filter);
        }

        public static IList<PropertyInfo> GetAllPropertiesOfType<T>(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false)
        {
            bool _filter(PropertyInfo prop)
            {
                // add type matching to the user-supplied filter, if any
                if (exactTypeMatchOnly ? typeof(T) == prop.PropertyType : typeof(T).IsAssignableFrom(prop.PropertyType))
                {
                    return filter?.Invoke(prop) ?? true;
                }
                return false;
            }
            var props = GetAllProperties(type, bindingFlags: bindingFlags, filter: _filter);
            return props;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *            PROPERTIES - SINGLE
         * * * * * * * * * * * * * * * * * * * * * * * */

        public static PropertyInfo GetProperty(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetProperty(instance.GetType(), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
        }

        public static PropertyInfo GetProperty(Type type, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            if (!type.IsClass)
                throw new ValidationException(type.FullName + " is not a reference type");
            var matchingProps = GetAllProperties
            (
                type,
                bindingFlags: bindingFlags ?? BindingFlags.Public,
                filter: p => SearchCriteria.Equals(name, ignoreCase: ignoreCase).Evaluate(p.Name)
            );
            switch (matchingProps.Count)
            {
                case 0:
                    if (!required) return null;
                    throw new ObjectMemberException("Required property '" + name + "' not found in " + type.FullName);
                case 1:
                    return matchingProps[0];
                default:
                    throw new ObjectMemberException(matchingProps.Count + " properties in " + type.FullName + " match this name: " + name + (ignoreCase ? " (ignore case)" : ""));
            }
        }

        public static PropertyInfo GetProperty<TType>(string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false) where TType : class
        {
            return GetProperty(typeof(TType), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
        }

        public static PropertyInfo GetInstanceProperty(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetProperty(instance.GetType(), name, bindingFlags: BindingFlags.Instance | (bindingFlags ?? BindingFlags.Public), ignoreCase: ignoreCase, required: required);
        }

        public static PropertyInfo GetInstanceProperty(Type type, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetProperty(type, name, bindingFlags: BindingFlags.Instance | (bindingFlags ?? BindingFlags.Public), ignoreCase: ignoreCase, required: required);
        }

        public static PropertyInfo GetInstanceProperty<TType>(string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false) where TType : class
        {
            return GetProperty(typeof(TType), name, bindingFlags: BindingFlags.Instance | (bindingFlags ?? BindingFlags.Public), ignoreCase: ignoreCase, required: required);
        }

        [Obsolete("This method is an antipattern - use GetStaticProperty(Type) instead", false)]
        public static PropertyInfo GetStaticProperty(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetProperty(instance.GetType(), name, bindingFlags: BindingFlags.Static | (bindingFlags ?? BindingFlags.Public), ignoreCase: ignoreCase, required: required);
        }

        public static PropertyInfo GetStaticProperty(Type type, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetProperty(type, name, bindingFlags: BindingFlags.Static | (bindingFlags ?? BindingFlags.Public), ignoreCase: ignoreCase, required: required);
        }

        public static PropertyInfo GetStaticProperty<TType>(string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false) where TType : class
        {
            return GetProperty(typeof(TType), name, bindingFlags: BindingFlags.Static | (bindingFlags ?? BindingFlags.Public), ignoreCase: ignoreCase, required: required);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *            PROPERTY VALUES - LIST
         * * * * * * * * * * * * * * * * * * * * * * * */

        [Obsolete("This method is too generic - use GetInstancePropertyValues(object) and GetStaticPropertyValues(Type) instead", false)]
        public static IList<MemberValue> GetPropertyValues(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var memberValues = new List<MemberValue>();
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Instance) == BindingFlags.Instance)
            {
                memberValues.AddRange(GetInstancePropertyValues(instance.GetType(), bindingFlags: bindingFlags, filter: filter));
            }
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Static) == BindingFlags.Static)
            {
                memberValues.AddRange(GetStaticPropertyValues(instance.GetType(), bindingFlags: bindingFlags, filter: filter));
            }
            return memberValues;
        }

        [Obsolete("This method is an antipattern - use GetStaticPropertyValues(Type) instead", false)]
        public static IList<MemberValue> GetPropertyValues(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var memberValues = new List<MemberValue>();
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Static) == BindingFlags.Static)
            {
                memberValues.AddRange(GetStaticPropertyValues(type, bindingFlags: bindingFlags, filter: filter));
            }
            return memberValues;
        }

        [Obsolete("This method is an antipattern - use GetStaticPropertyValues(Type) instead", false)]
        public static IList<MemberValue> GetPropertyValues<TType>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where TType : class
        {
            var memberValues = new List<MemberValue>();
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Static) == BindingFlags.Static)
            {
                memberValues.AddRange(GetStaticPropertyValues<TType>(bindingFlags: bindingFlags, filter: filter));
            }
            return memberValues;
        }

        public static IList<MemberValue> GetInstancePropertyValues(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var props = GetInstanceProperties(instance, bindingFlags: bindingFlags, filter: filter);
            var memberValues = props
                .Select(prop => new MemberValue(prop, prop.GetValue(instance)))
                .ToList();
            return memberValues;
        }

        public static IList<MemberValue<T>> GetInstancePropertyValuesOfType<T>(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false)
        {
            var props = GetInstancePropertiesOfType<T>(instance, bindingFlags: bindingFlags, filter: filter, exactTypeMatchOnly: exactTypeMatchOnly);
            var memberValues = props
                .Select(prop => new MemberValue<T>(prop, prop.GetValue(instance)))
                .ToList();
            return memberValues;
        }

        [Obsolete("This method is an antipattern - use GetStaticPropertyValues(Type) instead", false)]
        public static IList<MemberValue> GetStaticPropertyValues(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var props = GetStaticProperties(instance, bindingFlags: bindingFlags, filter: filter);
            var memberValues = props
                .Select(prop => new MemberValue(prop, prop.GetValue(null)))
                .ToList();
            return memberValues;
        }

        public static IList<MemberValue> GetStaticPropertyValues(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var props = GetStaticProperties(type, bindingFlags: bindingFlags, filter: filter);
            var memberValues = props
                .Select(prop => new MemberValue(prop, prop.GetValue(null)))
                .ToList();
            return memberValues;
        }

        public static IList<MemberValue> GetStaticPropertyValues<TType>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where TType : class
        {
            var props = GetStaticProperties<TType>(bindingFlags: bindingFlags, filter: filter);
            var memberValues = props
                .Select(prop => new MemberValue(prop, prop.GetValue(null)))
                .ToList();
            return memberValues;
        }


        public static IList<MemberValue<T>> GetStaticPropertyValuesOfType<T>(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false)
        {
            var props = GetStaticPropertiesOfType<T>(type, bindingFlags: bindingFlags, filter: filter, exactTypeMatchOnly: exactTypeMatchOnly);
            var memberValues = props
                .Select(prop => new MemberValue<T>(prop, prop.GetValue(null)))
                .ToList();
            return memberValues;
        }

        public static IList<MemberValue<T>> GetStaticPropertyValuesOfType<TType, T>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null, bool exactTypeMatchOnly = false) where TType : class
        {
            var props = GetStaticPropertiesOfType<TType, T>(bindingFlags: bindingFlags, filter: filter, exactTypeMatchOnly: exactTypeMatchOnly);
            var memberValues = props
                .Select(prop => new MemberValue<T>(prop, prop.GetValue(null)))
                .ToList();
            return memberValues;
        }

        public static IList<MemberValue> GetAllPropertyValues(object instance, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var memberValues = new List<MemberValue>();
            memberValues.AddRange(GetInstancePropertyValues(instance, bindingFlags: bindingFlags, filter: filter));
            memberValues.AddRange(GetStaticPropertyValues(instance.GetType(), bindingFlags: bindingFlags, filter: filter));
            return memberValues;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *            PROPERTY VALUES - SINGLE
         * * * * * * * * * * * * * * * * * * * * * * * */

        [Obsolete("This method is too generic - use GetInstancePropertyValue(object) or GetStaticPropertyValue(Type) instead", false)]
        public static MemberValue GetPropertyValue(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            MemberValue memberValue = null;
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Instance) == BindingFlags.Instance)
            {
                memberValue = GetInstancePropertyValue(instance, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: false);
            }
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Static) == BindingFlags.Static)
            {
                memberValue = GetStaticPropertyValue(instance.GetType(), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            }
            return memberValue;
        }

        [Obsolete("This method is an antipattern - use GetStaticPropertyValue(Type) instead", false)]
        public static MemberValue GetPropertyValue(Type type, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            MemberValue memberValue = null;
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Instance) == BindingFlags.Instance)
            {
                var prop = GetInstanceProperty(type, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: false);
                if (prop != null)
                    throw new ObjectMemberException("Member \"" + name + "\" requires an instance to get the value");
            }
            if (!bindingFlags.HasValue || (bindingFlags.Value & BindingFlags.Static) == BindingFlags.Static)
            {
                memberValue = GetStaticPropertyValue(type, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            }
            return memberValue;
        }

        public static MemberValue GetInstancePropertyValue(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            var prop = GetInstanceProperty(instance, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            if (prop == null)
                return null;
            var memberValue = new MemberValue(prop, prop.GetValue(instance));
            return memberValue;
        }

        public static MemberValue<T> GetInstancePropertyValue<T>(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            var prop = GetInstanceProperty(instance, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            if (prop == null)
                return null;
            var memberValue = new MemberValue<T>(prop, prop.GetValue(instance));
            return memberValue;
        }

        [Obsolete("This method is an antipattern - use GetStaticPropertyValue(Type) instead", false)]
        public static MemberValue GetStaticPropertyValue(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetStaticPropertyValue(instance.GetType(), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
        }

        [Obsolete("This method is an antipattern - use GetStaticPropertyValue(Type) instead", false)]
        public static MemberValue<T> GetStaticPropertyValue<T>(object instance, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            return GetStaticPropertyValue<T>(instance.GetType(), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
        }

        public static MemberValue GetStaticPropertyValue(Type type, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            var prop = GetStaticProperty(type, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            if (prop == null)
                return null;
            var memberValue = new MemberValue(prop, prop.GetValue(null));
            return memberValue;
        }

        public static MemberValue<T> GetStaticPropertyValue<T>(Type type, string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            var prop = GetStaticProperty(type, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            if (prop == null)
                return null;
            var memberValue = new MemberValue<T>(prop, prop.GetValue(null));
            return memberValue;
        }

        public static MemberValue GetStaticPropertyValue<TType>(string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false) where TType : class
        {
            return GetStaticPropertyValue(typeof(TType), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
        }

        public static MemberValue<T> GetStaticPropertyValue<TType, T>(string name, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false) where TType : class
        {
            return GetStaticPropertyValue<T>(typeof(TType), name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
        }

        public static MemberValue GetNestedInstancePropertyValue(object instance, string fullName, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            MemberValue nestedPropertyValue = null;
            var nameParts = fullName.Split('.');      // e.g. MyLibraryClass.'OldestBook.Age' => [ "OldestBook", "Age" ]
            for (int i = 0; i < nameParts.Length; i++)
            {
                nestedPropertyValue = GetInstancePropertyValue(instance, nameParts[i], bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
                if (nestedPropertyValue == null || nestedPropertyValue.Value == null)
                    return null;
                instance = nestedPropertyValue.Value;
            }
            return nestedPropertyValue;
        }

        public static MemberValue<T> GetNestedInstancePropertyValue<T>(object instance, string fullName, BindingFlags? bindingFlags = null, bool ignoreCase = false, bool required = false)
        {
            var memberValue = GetNestedInstancePropertyValue(instance, fullName, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: required);
            if (memberValue == null)
                return null;
            return new MemberValue<T>(memberValue.Member, memberValue.Value);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *            TYPES / INSTANCES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Finds the runtime type respresented by the fully qualified class name
        /// </summary>
        /// <param name="className"></param>
        /// <param name="assemblyName"></param>
        /// <param name="suppressErrors"></param>
        /// <returns></returns>
        public static Type GetType(string className, string assemblyName = null, bool ignoreCase = false, bool suppressErrors = false)
        {
            if (className == null)
            {
                if (suppressErrors)
                    return null;
                throw new ArgumentNullException(nameof(className));
            }
            try
            {
                // step 1 - direct type loading (seldom works)
                Type type = Type.GetType(className, throwOnError: !suppressErrors, ignoreCase: ignoreCase);
                if (type != null)
                    return type;

                // step 2 - load type from loaded assemblies (e.g. System.Core, Horseshoe.NET, etc.)
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(className);
                    if (type != null) 
                        return type;
                }

                // step 3 - load assembly and type
                if (assemblyName != null)
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        type = assembly.GetType(className);
                        if (type != null) 
                            return type;
                    }
                }

                throw new ValidationException("Ensure you are using the full class name and that the proper assembly is accessible (e.g. is present in the executable's path or installed in the GAC)");
            }
            catch (Exception ex)
            {
                if (suppressErrors)
                    return null;
                throw new UtilityException("Type \"" + className + "\" could not be loaded: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type
        /// </summary>
        /// <param name="type">a type</param>
        /// <param name="args">constructor args</param>
        /// <param name="nonPublic"><c>true</c> if a public or nonpublic default constructor can match, <c>false</c> if only a public default constructor can match</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(Type type, object[] args = null, bool nonPublic = false)
        {
            if (!type.IsClass) 
                throw new ValidationException(type.FullName + " is not a reference type");
            return args != null && args.Any()
                ? Activator.CreateInstance(type, args)
                : Activator.CreateInstance(type, nonPublic: nonPublic);
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type cast as the supplied type parameter
        /// </summary>
        /// <param name="type">a type</param>
        /// <param name="args">constructor args</param>
        /// <param name="nonPublic"><c>true</c> if a public or nonpublic default constructor can match, <c>false</c> if only a public default constructor can match</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static T GetInstance<T>(Type type, object[] args = null, bool nonPublic = false) where T : class
        {
            if (!type.IsClass) 
                throw new ValidationException(type.FullName + " is not a reference type");
            object instance = args != null && args.Any()
                ? Activator.CreateInstance(type, args)
                : Activator.CreateInstance(type, nonPublic: nonPublic);
            return (T)instance;
        }

        /// <summary>
        /// Dynamically creates an instance of the class represented by the supplied class name.
        /// </summary>
        /// <param name="className">A fully qualified class name to instantiate</param>
        /// <param name="assemblyName">An assembly name from which to draw types</param>
        /// <param name="args">constructor args</param>
        /// <param name="nonPublic"><c>true</c> if a public or nonpublic default constructor can match, <c>false</c> if only a public default constructor can match</param>
        /// <param name="suppressErrors">Whether to return null or throw an exception for class names that are invalid or not found</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(string className, string assemblyName = null, object[] args = null, bool nonPublic = false, bool suppressErrors = false)
        {
            var type = GetType(className, assemblyName: assemblyName, suppressErrors: suppressErrors);
            if (type == null)
                return null;
            return GetInstance(type, args: args, nonPublic: nonPublic);
        }

        /// <summary>
        /// Dynamically creates a typed instance of the class represented by the supplied class name and cast as the supplied type parameter.
        /// </summary>
        /// <typeparam name="T">A type (the return type)</typeparam>
        /// <param name="className">A fully qualified class name to instantiate</param>
        /// <param name="assemblyName">An assembly name from which to draw types</param>
        /// <param name="args">constructor args</param>
        /// <param name="nonPublic"><c>true</c> if a public or nonpublic default constructor can match, <c>false</c> if only a public default constructor can match</param>
        /// <param name="suppressErrors">Whether to return null or throw an exception for class names that are invalid or not found</param>
        /// <returns>A dynamically created PublicInstance of the supplied type</returns>
        public static T GetInstance<T>(string className, string assemblyName = null, object[] args = null, bool nonPublic = false, bool suppressErrors = false) where T : class
        {
            var instance = GetInstance(className, assemblyName: assemblyName, args: args, nonPublic: nonPublic, suppressErrors: suppressErrors);
            return (T)instance;
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type using the default constructor, if one does not exist this method throws an exception
        /// </summary>
        /// <typeparam name="T">A type (the return type)</typeparam>
        /// <param name="nonPublic"><c>true</c> if a public or nonpublic default constructor can match, <c>false</c> if only a public default constructor can match</param>
        /// <returns>A dynamically created instance of the supplied type parameter</returns>
        public static T GetDefaultInstance<T>(bool nonPublic = false) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), nonPublic: nonPublic);
        }

        /// <summary>
        /// Gets the default value for the supplied type, <c>null</c> for reference types
        /// </summary>
        /// <param name="type">A type</param>
        /// <returns></returns>
        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *            MORE PROPERTY VALUES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Sets the value of the indicated instance property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="ignoreCase"></param>
        public static void SetInstancePropertyValue(object instance, string name, object value, BindingFlags? bindingFlags = null, bool ignoreCase = false)
        {
            var type = instance.GetType();
            if (!type.IsClass)
                throw new ValidationException(type.FullName + " is not a reference type");
            PropertyInfo prop = GetInstanceProperty(instance, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: true);
            prop.SetValue(instance, value);
        }

        /// <summary>
        /// Sets the value of the indicated static property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="ignoreCase"></param>
        public static void SetStaticPropertyValue(Type type, string name, object value, BindingFlags? bindingFlags = null, bool ignoreCase = false)
        {
            if (!type.IsClass)
                throw new ValidationException(type.FullName + " is not a reference type");
            PropertyInfo prop = GetStaticProperty(type, name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: true);
            prop.SetValue(null, value);
        }

        /// <summary>
        /// Sets the value of the indicated static property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="ignoreCase"></param>
        public static void SetStaticPropertyValue<TType>(string name, object value, BindingFlags? bindingFlags = null, bool ignoreCase = false) where TType : class
        {
            PropertyInfo prop = GetStaticProperty<TType>(name, bindingFlags: bindingFlags, ignoreCase: ignoreCase, required: true);
            prop.SetValue(null, value);
        }
    }
}
