﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Comparison;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for types.
    /// </summary>
    public static class TypeUtil
    {
        /// <summary>
        /// Gets a text description of the kind of the supplied type (e.g. "[interface]", "[enum]", etc.).
        /// </summary>
        /// <param name="type">A type.</param>
        /// <returns>The description of the type.</returns>
        public static string GetKindOfType(Type type)
        {
            if (type == null)
                return TextConstants.Null;
            if (type.IsInterface)
                return "[interface]";
            if (type.IsEnum)
                return "[enum]";
            if (type.IsValueType)
                return "[value-type]";
            if (!type.IsClass)
                return "[non-class]";
            if (type.IsArray)
                return "[array]";
            var strb = new StringBuilder("class")
                .InsertIf(0, type.IsSealed, "sealed-")
                .InsertIf(0, type.IsAbstract, "abstract-")
                .InsertIf(0, type.IsGenericType, "generic-")
                .InsertIf(0, type.IsImport, "COM-");
            return ("[" + strb + "]").Replace("abstract-sealed", "static");  // ref: https://stackoverflow.com/questions/1175888/determine-if-a-type-is-static
        }

        /// <summary>
        /// Asserts that the supplied type is a reference type.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <param name="errorMessage">An optional error message.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIsReferenceType(Type type, string errorMessage = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), nameof(type) + " cannot be null");
            if (!type.IsClass)
                throw new AssertionFailedException(errorMessage ?? (type.FullName + " is not a reference type: " + GetKindOfType(type)));
        }

        /// <summary>
        /// Asserts that <c>T</c> is a reference type
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="errorMessage">An optional error message</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIsReferenceType<T>(string errorMessage = null) =>
            AssertIsReferenceType(typeof(T), errorMessage: errorMessage);

        /// <summary>
        /// Asserts that the supplied type is a value type
        /// </summary>
        /// <param name="type">A type</param>
        /// <param name="errorMessage">An optional error message</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIsValueType(Type type, string errorMessage = null)
        {
            if (type == null || !type.IsValueType)
                throw new AssertionFailedException(errorMessage ?? (type.FullName + " is not a value type: " + GetKindOfType(type)));
        }

        /// <summary>
        /// Asserts that <c>T</c> is a value type
        /// </summary>
        /// <typeparam name="T">A value type</typeparam>
        /// <param name="errorMessage">An optional error message</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIsValueType<T>(string errorMessage = null) =>
            AssertIsValueType(typeof(T), errorMessage: errorMessage);

        /// <summary>
        /// Finds the runtime type respresented by the fully qualified type name
        /// </summary>
        /// <param name="typeName">The fully qualified type name.</param>
        /// <param name="assemblyName">An optional assembly name.</param>
        /// <param name="inheritedType">An optional type to which objects of this type can be assigned (e.g. parent type).</param>
        /// <param name="ignoreCase">If <c>true</c>, allows searching assemblies/types that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <param name="strict">If <c>true</c>, an exception is thrown when the specified type is not loaded, default is <c>false</c> (and <c>null</c> is returned).</param>
        /// <exception cref="TypeNotFoundException"/>
        /// <exception cref="TypeException"/>
        /// <returns>The runtime type indicated by <c>typeName</c></returns>
        public static Type GetType(string typeName, string assemblyName = null, Type inheritedType = null, bool ignoreCase = false, bool strict = false)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            Exception lastException = null;

            // step 1 - direct type loading (seldom works)
            Type type = Type.GetType(typeName, throwOnError: false, ignoreCase: ignoreCase);
            if (type == null)
            {
                // step 2 - load type from loaded assemblies (e.g. System.Core, Horseshoe.NET, etc.)
                try
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = assembly.GetType(typeName);
                        if (type != null)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }

                if (type == null && assemblyName != null)
                {
                    // step 3 - load assembly and type
                    try
                    {
                        var assembly = Assembly.Load(assemblyName);
                        if (assembly != null)
                        {
                            type = assembly.GetType(typeName);
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }
            }

            if (type == null)
            {
                if (strict)
                    throw new TypeNotFoundException("Type \"" + typeName + "\" could not be loaded" + (lastException != null ? ": " + lastException.Message : "") + ".  Please ensure you are using the fully qualified class name and that the proper assembly is accessible at runtime, if applicable (e.g. is present in the executable's path or installed in the GAC)", lastException)
                    {
                        IsStrictSensitive = true
                    };
                return null;
            }
            if (inheritedType != null && !inheritedType.IsAssignableFrom(type))
            {
                throw new TypeException("\"" + inheritedType.FullName + "\" is not assignable from " + "\"" + type.FullName + "\"");
            }
            return type;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *                 GET PROPERTIES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the public instance properties of <c>type</c>.
        /// </summary>
        /// <param name="type">A reference type.</param>
        /// <returns>A property array.</returns>
        /// <exception cref="AssertionFailedException">If <c>type</c> is not a reference type.</exception>
        public static PropertyInfo[] GetInstanceProperties(Type type)
        {
            AssertIsReferenceType(type);
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Gets the public instance properties of <c>T</c>
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <returns>A property array</returns>
        public static PropertyInfo[] GetInstanceProperties<T>() where T : class =>
            typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Gets the public instance property of <c>type</c> whose name matches <c>propertyName</c>.
        /// <para>
        /// If exactly one property with the supplied name is not found then an exception is raised.
        /// </para>
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property</returns>
        /// <exception cref="ObjectMemberException"></exception>
        public static PropertyInfo GetInstanceProperty(Type type, string propertyName, bool ignoreCase = false)
        {
            var props = GetInstanceProperties(type)
                .NamedLike(propertyName, LikeMode.Equals, ignoreCase: ignoreCase)
                .ToArray();

            switch (props.Length)
            {
                case 0:
                    throw new ObjectMemberException("No property appears to match this name: " + propertyName) 
                    { 
                        IsStrictSensitive = true 
                    };
                case 1:
                    return props[0];
                default:
                    throw new ObjectMemberException(props.Length + " properties appear to match this name: " + propertyName);
            }
        }

        /// <summary>
        /// Gets the public instance property of <c>T</c> whose name matches <c>propertyName</c>
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property</returns>
        /// <exception cref="ObjectMemberException"></exception>
        public static PropertyInfo GetInstanceProperty<T>(string propertyName, bool ignoreCase = false) where T : class
        {
            var props = GetInstanceProperties<T>()
                .NamedLike(propertyName, LikeMode.Equals, ignoreCase: ignoreCase)
                .ToArray();

            switch (props.Length)
            {
                case 0:
                    throw new ObjectMemberException("No properties appear to match this name: " + propertyName);
                case 1:
                    return props[0];
                default:
                    throw new ObjectMemberException(props.Length + " properties appear to match this name: " + propertyName);
            }
        }

        /// <summary>
        /// Gets the public static properties of <c>type</c>.
        /// </summary>
        /// <param name="type">A reference type.</param>
        /// <returns>A property array.</returns>
        /// <exception cref="AssertionFailedException">If <c>type</c> is not a reference type</exception>
        public static PropertyInfo[] GetStaticProperties(Type type)
        {
            AssertIsReferenceType(type);
            return type.GetProperties(BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Gets the public static properties of <c>T</c>.
        /// </summary>
        /// <typeparam name="T">A reference type.</typeparam>
        /// <returns>A property array.</returns>
        public static PropertyInfo[] GetStaticProperties<T>() where T : class
        {
            return typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Gets the public static property of <c>type</c> whose name matches <c>propertyName</c>
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property</returns>
        /// <exception cref="ObjectMemberException"></exception>
        public static PropertyInfo GetStaticProperty(Type type, string propertyName, bool ignoreCase = false)
        {
            var props = GetStaticProperties(type)
                .NamedLike(propertyName, LikeMode.Equals, ignoreCase: ignoreCase)
                .ToArray();

            switch (props.Length)
            {
                case 0:
                    throw new ObjectMemberException("No properties appear to match this name: " + propertyName);
                case 1:
                    return props[0];
                default:
                    throw new ObjectMemberException(props.Length + " properties appear to match this name: " + propertyName);
            }
        }

        /// <summary>
        /// Gets the public static property of <c>T</c> whose name matches <c>propertyName</c>
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property</returns>
        /// <exception cref="ObjectMemberException"></exception>
        public static PropertyInfo GetStaticProperty<T>(string propertyName, bool ignoreCase = false) where T : class
        {
            var props = GetStaticProperties<T>()
                .NamedLike(propertyName, LikeMode.Equals, ignoreCase: ignoreCase)
                .ToArray();

            switch (props.Length)
            {
                case 0:
                    throw new ObjectMemberException("No properties appear to match this name: " + propertyName);
                case 1:
                    return props[0];
                default:
                    throw new ObjectMemberException(props.Length + " properties appear to match this name: " + propertyName);
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *         GET / SET PROPERTY VALUES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the public static properties of <c>type</c>, values included.
        /// </summary>
        /// <param name="type">A reference type</param>
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
        /// <returns>A property value array.</returns>
        public static PropertyValue[] GetStaticPropertyValues(Type type, Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, LikeMode propertyNameCompareMode = default, string propertyNameCriteria = null, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null)
        {
            if (type == null)
                return null;
            return GetStaticProperties(type)
                .OfPropertyType(propertyTypeFilter, strictType: strictPropertyTypeFilter)
                .NamedLike(propertyNameCriteria, propertyNameCompareMode, ignoreCase: propertyNameIgnoreCase)
                .Where(p => propertyFilter?.Invoke(p) ?? true)
                .Select(prop => new PropertyValue(prop, prop.GetValue(null)))
                .ToArray();
        }

        /// <summary>
        /// Gets the public static properties of <c>type</c>, values included.
        /// </summary>
        /// <typeparam name="T">A referemce type</typeparam>
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
        /// <returns>A property value array.</returns>
        public static PropertyValue[] GetStaticPropertyValues<T>(Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, LikeMode propertyNameCompareMode = default, StringValues propertyNameCriteria = default, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null) where T : class
        {
            return GetStaticProperties<T>()
                .OfPropertyType(propertyTypeFilter, strictType: strictPropertyTypeFilter)
                .NamedLike(propertyNameCriteria, propertyNameCompareMode, ignoreCase: propertyNameIgnoreCase)
                .Where(p => propertyFilter?.Invoke(p) ?? true)
                .Select(prop => new PropertyValue(prop, prop.GetValue(null)))
                .ToArray();
        }

        /// <summary>
        /// Gets the public static property of <c>type</c> whose name matches <c>propertyName</c>, value included
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A property value</returns>
        public static PropertyValue GetStaticPropertyValue(Type type, string propertyName, bool ignoreCase = false)
        {
            var prop = GetStaticProperty(type, propertyName, ignoreCase: ignoreCase);
            return new PropertyValue(prop, prop.GetValue(null));
        }

        /// <summary>
        /// Sets the value of the indicated static property
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="newValue">An <c>object</c></param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        public static void SetStaticPropertyValue(Type type, string propertyName, object newValue, bool ignoreCase = false)
        {
            PropertyInfo prop = GetStaticProperty(type, propertyName, ignoreCase: ignoreCase);
            prop.SetValue(null, newValue);
        }

        /// <summary>
        /// Sets the value of the indicated static property
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="newValue">An <c>object</c></param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        public static void SetStaticPropertyValue<T>(string propertyName, object newValue, bool ignoreCase = false) where T : class
        {
            PropertyInfo prop = GetStaticProperty<T>(propertyName, ignoreCase: ignoreCase);
            prop.SetValue(null, newValue);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *                  INSTANCES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Dynamically creates an instance of the supplied reference type
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(Type type, bool nonPublic = false)
        {
            AssertIsReferenceType(type);
            return Activator.CreateInstance(type, nonPublic: nonPublic);
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied reference type with optional constructor arguments
        /// </summary>
        /// <typeparam name="T">A generic runtime reference type</typeparam>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A dynamically created instance of the supplied reference type</returns>
        public static T GetInstance<T>(bool nonPublic = false) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), nonPublic);
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied reference type with optional constructor arguments
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="args">Constructor args, optional for types with a no-arg constructor</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(Type type, params object[] args)
        {
            AssertIsReferenceType(type);
            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied reference type with optional constructor arguments
        /// </summary>
        /// <typeparam name="T">A generic runtime reference type</typeparam>
        /// <param name="args">Constructor args, optional for types with a no-arg constructor</param>
        /// <returns>A dynamically created instance of the supplied reference type</returns>
        public static T GetInstance<T>(params object[] args) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// Dynamically creates an instance of the class represented by the supplied class name.
        /// </summary>
        /// <param name="className">A fully qualified class name to instantiate</param>
        /// <param name="assemblyName">An assembly name from which to draw types</param>
        /// <param name="args">Constructor args</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <param name="strict">If a <c>Type</c> matching the supplied name cannot be found then <c>strict == true</c> causes an exception to be thrown, default is <c>false</c>.</param>
        /// <param name="ignoreCase">If <c>true</c>, allows searching assemblies/types that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        /// <exception cref="TypeNotFoundException"></exception>
        public static object GetInstance(string className, string assemblyName = null, object[] args = null, bool nonPublic = false, bool strict = false, bool ignoreCase = false)
        {
            try
            {
                var type = GetType(className, assemblyName: assemblyName, ignoreCase: ignoreCase);
                return args != null && args.Any()
                    ? GetInstance(type, args: args)
                    : GetInstance(type, nonPublic: nonPublic);
            }
            catch (TypeNotFoundException tnfex)
            {
                if (tnfex.IsStrictSensitive && strict)
                    throw;
            }
            return null;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *                  DEFAULTS
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Gets the default value of the supplied type, e.g. <c>null</c> for reference types.  
        /// <para>
        /// Similar to <c>default(T)</c>.
        /// </para>
        /// </summary>
        /// <typeparam name="T">A type.</typeparam>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A default value such as <c>null</c></returns>
        public static T GetDefaultValue<T>(bool nonPublic = false) =>
            (T)GetDefaultValue(typeof(T), nonPublic: nonPublic);

        /// <summary>
        /// Gets the default value of the supplied type, e.g. <c>null</c> for reference and nullable types.  
        /// <para>
        /// Similar to <c>default(T)</c>.
        /// </para>
        /// </summary>
        /// <param name="type">A runtime type.</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A default value such as <c>null</c>.</returns>
        public static object GetDefaultValue(Type type, bool nonPublic = false)
        {
            if (type.IsClass || Nullable.GetUnderlyingType(type) != null)  // e.g. MyClass || int?
                return null;

            if (type.IsValueType)
                return Activator.CreateInstance(type, nonPublic: nonPublic);

            throw new TypeException("Type is not associated with a default value: " + type.FullName);
        }
    }
}
