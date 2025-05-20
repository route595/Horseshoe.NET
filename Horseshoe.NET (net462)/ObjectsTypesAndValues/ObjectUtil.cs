using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Comparison;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for object handling, reflection and manipulation.
    /// </summary>
    public static class ObjectUtil
    {
        /// <summary>
        /// Reports whether an object is <c>null</c> (including <c>DBNull</c>)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsNull(object obj)
        {
            return obj is null || obj is DBNull;
        }

        /// <summary>
        /// Sets the properties of <c>dest</c> with the property values of <c>src</c>
        /// </summary>
        /// <param name="src">The source <c>object</c></param>
        /// <param name="dest">The target <c>object</c></param>
        /// <param name="tryMapAll"></param>
        /// <param name="preventNullOverwrite">If <c>true</c>, prevents a non-null destination property from being overwritten by null, default is <c>false</c> and this is not commonly used.</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <param name="ignoreErrors">If <c>true</c>, bypasses mapping errors leaving the values unchanged, default is <c>false</c>.</param>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ObjectMappingException"></exception>
        public static void MapProperties(object src, object dest, bool tryMapAll = false, bool preventNullOverwrite = false, bool ignoreCase = false, bool ignoreErrors = false)
        {
            if (src == null)
                throw new ValidationException("Argument '" + nameof(src) + "' cannot be null");
            if (dest == null)
                throw new ValidationException("Argument '" + nameof(dest) + "' cannot be null");

            var srcType = src.GetType();
            var destType = dest.GetType();

            if (!srcType.IsClass)
                throw new ValidationException("'" + nameof(src) + "' is of type " + srcType.FullName + " which is not a reference type: " + (srcType.IsInterface ? "interface" : (srcType.IsEnum ? "enum" : (srcType.IsValueType ? "value type" : "unknown"))));
            if (!destType.IsClass)
                throw new ValidationException("'" + nameof(dest) + "' is of type " + destType.FullName + " which is not a reference type: " + (destType.IsInterface ? "interface" : (destType.IsEnum ? "enum" : (destType.IsValueType ? "value type" : "unknown"))));

            //
            // Case 1 of 2 - same type
            //
            if (srcType == destType)
            {
                var srcPropValues = src.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead && p.CanWrite)
                    .Select(p => new PropertyValue(p, p.GetValue(src)))
                    .ToArray();
                foreach (var srcPV in srcPropValues)
                {
                    if (preventNullOverwrite && srcPV.Value == null && srcPV.Property.GetValue(dest) != null)
                        continue;
                    try
                    {
                        srcPV.Property.SetValue(dest, srcPV.Value);
                    }
                    catch (Exception ex)
                    {
                        if (ignoreErrors)
                            continue;
                        throw new ObjectMappingException(srcType, destType, srcPV.Property, ex);
                    }
                }
            }

            //
            // Case 2 of 2 - different types
            //
            else
            {
                var srcPropValues = src.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead)
                    .Select(p => new PropertyValue(p, p.GetValue(src)))
                    .ToArray();
                var destProps = destType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanWrite)
                    .ToArray();

                foreach (var srcPV in srcPropValues)
                {
                    var srcProp = srcPV.Property;
                    var matchingDestProps = destProps
                        .Where
                        (
                            destProp => ignoreCase
                                ? destProp.Name.Equals(srcProp.Name, StringComparison.OrdinalIgnoreCase)
                                : destProp.Name.Equals(srcProp.Name)
                        )
                        .ToList();
                    try
                    {
                        switch (matchingDestProps.Count)
                        {
                            case 0:
                                if (ignoreErrors || !tryMapAll)
                                    break;
                                throw new ObjectMappingException(srcType, destType, srcPV.Property, "Destination property '" + srcProp.Name + "' not found");
                            case 1:
                                if (preventNullOverwrite && srcPV.Value == null && matchingDestProps[0].GetValue(dest) != null)
                                    continue;
                                matchingDestProps[0].SetValue(dest, srcPV.Value);
                                break;
                            default:
                                if (ignoreErrors)
                                    break;
                                throw new ObjectMappingException(srcType, destType, srcPV.Property, "More than one destination property matching '" + srcProp.Name + "' exists" + (ignoreCase ? "" : " (case-sensitive)"));
                        }
                    }
                    catch (ObjectMappingException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new ObjectMappingException(srcType, destType, srcPV.Property, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a shallow duplicate of an <c>object</c>.
        /// </summary>
        /// <param name="src">The source <c>object</c>.</param>
        /// <param name="args">Constructor args, optional for types with a no-arg constructor.</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <param name="ignoreErrors">If <c>true</c>, bypasses mapping errors leaving the values unchanged, default is <c>false</c>.</param>
        /// <returns>A new <c>object</c> identical to <c>src</c>.</returns>
        public static object Duplicate(object src, object[] args = null, bool nonPublic = false, bool ignoreErrors = false)
        {
            var dest = args != null && args.Any()
                ? TypeUtil.GetInstance(src.GetType(), args: args)
                : TypeUtil.GetInstance(src.GetType(), nonPublic: nonPublic);
            MapProperties(src, dest, ignoreErrors: ignoreErrors);
            return dest;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *         GET / SET PROPERTY VALUES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the public instance properties of <c>instance</c>, values included
        /// </summary>
        /// <param name="instance">An <c>object</c></param>
        /// <param name="propertyTypeFilter">Optional. A C# type to compare against property types, this includes subtypes if <c>strictPropertyTypeFilter == false</c>.  All other properties are filtered out.</param>
        /// <param name="strictPropertyTypeFilter">Optional. If <c>propertyTypeFilter</c> is supplied, this parameter indicates whether to exclude subtypes, default is <c>false</c>.</param>
        /// <param name="propertyNameCompareMode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="propertyNameCriteria">Optional. The property name criteria search value(s) if filtering on property name.</param>
        /// <param name="propertyNameIgnoreCase">
        /// <para>
        /// Set to <c>true</c> (recommended) to ignore the letter case of the property names being compared by this filter, default is <c>false</c>.
        /// </para>
        /// <para>
        /// While operating systems like Windows are not case-sensitive, others are.  So are <c>string</c>s in practically every programming
        /// language.  As such, Horseshoe.NET requires opt-in for case-insensitivity, i.e. setting this parameter to <c>true</c>.
        /// </para>
        /// </param>
        /// <param name="propertyFilter"></param>
        /// <returns>A property value array</returns>
        public static PropertyValue[] GetInstancePropertyValues(object instance, Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, LikeMode propertyNameCompareMode = default, StringValues propertyNameCriteria = default, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null)
        {
            if (instance == null)
                return null;
            return TypeUtil.GetInstanceProperties(instance.GetType())
                .OfPropertyType(propertyTypeFilter, strictType: strictPropertyTypeFilter)
                .NamedLike(propertyNameCriteria, propertyNameCompareMode, ignoreCase: propertyNameIgnoreCase)
                .Where(p => propertyFilter?.Invoke(p) ?? true)
                .Select(prop => new PropertyValue(prop, prop.GetValue(instance)))
                .ToArray();
        }

        /// <summary>
        /// Gets the public instance property of <c>instance</c> whose name matches <c>propertyName</c>, value included
        /// </summary>
        /// <param name="instance">An <c>object</c></param>
        /// <param name="propertyName">The property name</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property value</returns>
        public static PropertyValue GetInstancePropertyValue(object instance, string propertyName, bool ignoreCase = false)
        {
            var prop = TypeUtil.GetInstanceProperty(instance.GetType(), propertyName, ignoreCase: ignoreCase);
            return new PropertyValue(prop, prop.GetValue(instance));
        }

        /// <summary>
        /// Sets the value of the indicated instance property
        /// </summary>
        /// <param name="instance">An <c>object</c> whose property to set</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="newValue">An <c>object</c></param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <param name="strict">If a property matching the supplied name does not exist then <c>strict == true</c> causes an exception to be thrown, default is <c>false</c>.</param>
        public static void SetInstancePropertyValue(object instance, string propertyName, object newValue, bool ignoreCase = false, bool strict = false)
        {
            if (instance == null)
                throw new ValidationException(nameof(instance) + " cannot be null");
            try
            {
                PropertyInfo prop = TypeUtil.GetInstanceProperty(instance.GetType(), propertyName, ignoreCase: ignoreCase);
                prop.SetValue(instance, newValue);
            }
            catch(ObjectMemberException omex)
            {
                if (!omex.IsStrictSensitive || strict)
                    throw;
            }
        }

        /// <summary>
        /// Experimental.  Gets the value of a nested property, identified in dot (.) notation, for example: <c>"myProperty.someValue"</c> which would reference <c>myInstance.myProperty.someValue</c>.
        /// </summary>
        /// <param name="instance">An <c>object</c></param>
        /// <param name="fullName">The full nested property name, e.g. <c>"myProperty.someValue"</c></param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property value</returns>
        public static PropertyValue GetNestedInstancePropertyValue(object instance, string fullName, bool ignoreCase = false)
        {
            PropertyValue nestedPropertyValue = null;
            var nameParts = fullName.Split('.');      // e.g. MyLibraryClass.'OldestBook.Age' => [ "OldestBook", "Age" ]
            for (int i = 0; i < nameParts.Length; i++)
            {
                nestedPropertyValue = GetInstancePropertyValue(instance, nameParts[i], ignoreCase: ignoreCase);
                if (nestedPropertyValue == null || nestedPropertyValue.Value == null)
                    return null;
                instance = nestedPropertyValue.Value;
            }
            return nestedPropertyValue;
        }

        /// <summary>
        /// Experimental.  Gets the value of a nested property, identified in dot (.) notation, for example: <c>"myProperty.someValue"</c> which would reference <c>myInstance.myProperty.someValue</c>.
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="instance">An <c>object</c></param>
        /// <param name="fullName">The full nested property name, e.g. <c>"myProperty.someValue"</c></param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property value</returns>
        public static PropertyValue<T> GetNestedInstancePropertyValue<T>(object instance, string fullName, bool ignoreCase = false)
        {
            var memberValue = GetNestedInstancePropertyValue(instance, fullName, ignoreCase: ignoreCase);
            if (memberValue == null)
                return null;
            return new PropertyValue<T>(memberValue.Property, memberValue.Value);
        }

        /// <summary>
        /// Generates a JSON-like string representation of an object's properties.
        /// </summary>
        /// <param name="value">A value</param>
        /// <returns>A JSON-like string representation of an object</returns>
        public static string DumpToString(object value)
        {
            if (value == null)
                return TextConstants.Null;
            var type = value.GetType();
            var props = type.GetProperties();
            var strb = new StringBuilder("{ ");
            for (int i = 0; i < props.Length; i++)
            {
                strb.AppendIf(i > 0, ", ")
                    .Append(props[i].Name)
                    .Append(": ")
                    .Append(ValueUtil.Display(props[i].GetValue(value, null)));
            }
            strb.Append(" }");
            return strb.ToString();
        }

        /// <summary>
        /// Creates a dictionary from an object's properties.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>A dictionary</returns>
        public static Dictionary<string, object> DumpToDictionary(object obj)
        {
            var dict = new Dictionary<string, object>();

            if (obj == null)
                return dict;

            foreach (var property in obj.GetType().GetProperties())
            {
                dict.Add(property.Name, property.GetValue(obj, null));
            }
            return dict;
        }

        /// <summary>
        /// Creates a dictionary from an object's properties.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>A string dictionary</returns>
        public static Dictionary<string, string> DumpToStringDictionary(object obj)
        {
            var dict = new Dictionary<string, string>();

            if (obj == null)
                return dict;

            foreach (var property in obj.GetType().GetProperties())
            {
                dict.Add(property.Name, property.GetValue(obj, null)?.ToString());
            }
            return dict;
        }
    }
}
