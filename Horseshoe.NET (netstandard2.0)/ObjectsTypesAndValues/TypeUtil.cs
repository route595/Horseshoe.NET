using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for types.
    /// </summary>
    public static class TypeUtil
    {
        /// <inheritdoc cref="TypeUtilAbstractions.GetKindOfType(Type)"/>
        public static string GetKindOfType(Type type)
        {
            return TypeUtilAbstractions.GetKindOfType(type);
        }

        /// <inheritdoc cref="TypeUtilAbstractions.AssertIsReferenceType(Type, string)"/>
        public static void AssertIsReferenceType(Type type, string errorMessage = null)
        {
            TypeUtilAbstractions.AssertIsReferenceType(type, errorMessage: errorMessage);
        }

        /// <summary>
        /// Asserts that <c>T</c> is a reference type
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="errorMessage">An optional error message</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIsReferenceType<T>(string errorMessage = null)
        {
            AssertIsReferenceType(typeof(T), errorMessage: errorMessage);
        }

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
        public static void AssertIsValueType<T>(string errorMessage = null)
        {
            AssertIsValueType(typeof(T), errorMessage: errorMessage);
        }

        /// <inheritdoc cref="TypeUtilAbstractions.GetType(string, string, bool)"/>
        public static Type GetType(string className, string assemblyName = null, bool ignoreCase = false)
        {
            return TypeUtilAbstractions.GetType(className, assemblyName: assemblyName, ignoreCase: ignoreCase);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *                 GET PROPERTIES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <inheritdoc cref="TypeUtilAbstractions.GetInstanceProperties(Type)"/>
        public static PropertyInfo[] GetInstanceProperties(Type type)
        {
            return TypeUtilAbstractions.GetInstanceProperties(type);
        }

        /// <summary>
        /// Gets the public instance properties of <c>T</c>
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <returns>A property array</returns>
        public static PropertyInfo[] GetInstanceProperties<T>() where T : class
        {
            return typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

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
                .NamedLike(CompareMode.Equals, propertyName, ignoreCase: ignoreCase)
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
                .NamedLike(CompareMode.Equals, propertyName, ignoreCase: ignoreCase)
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

        /// <inheritdoc cref="TypeUtilAbstractions.GetStaticProperties(Type)"/>
        public static PropertyInfo[] GetStaticProperties(Type type)
        {
            return TypeUtilAbstractions.GetStaticProperties(type);
        }

        /// <inheritdoc cref="TypeUtilAbstractions.GetStaticProperties{T}"/>
        public static PropertyInfo[] GetStaticProperties<T>() where T : class
        {
            return TypeUtilAbstractions.GetStaticProperties<T>();
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
                .NamedLike(CompareMode.Equals, propertyName, ignoreCase: ignoreCase)
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
                .NamedLike(CompareMode.Equals, propertyName, ignoreCase: ignoreCase)
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

        /// <inheritdoc cref="TypeUtilAbstractions.GetStaticPropertyValues(Type, Type, bool, CompareMode, string, bool, Func{PropertyInfo, bool})"/>
        public static PropertyValue[] GetStaticPropertyValues(Type type, Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, CompareMode propertyNameCompareMode = default, string propertyNameCriteria = null, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null)
        {
            return TypeUtilAbstractions.GetStaticPropertyValues(type, propertyTypeFilter: propertyTypeFilter, strictPropertyTypeFilter: strictPropertyTypeFilter, propertyNameCompareMode: propertyNameCompareMode, propertyNameCriteria: propertyNameCriteria, propertyNameIgnoreCase: propertyNameIgnoreCase, propertyFilter: propertyFilter);
        }

        /// <inheritdoc cref="TypeUtilAbstractions.GetStaticPropertyValues{T}(Type, bool, CompareMode, StringValues, bool, Func{PropertyInfo, bool})"/>
        public static PropertyValue[] GetStaticPropertyValues<T>(Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, CompareMode propertyNameCompareMode = default, StringValues propertyNameCriteria = default, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null) where T : class
        {
            return TypeUtilAbstractions.GetStaticPropertyValues<T>(propertyTypeFilter: propertyTypeFilter, strictPropertyTypeFilter: strictPropertyTypeFilter, propertyNameCompareMode: propertyNameCompareMode, propertyNameCriteria: propertyNameCriteria, propertyNameIgnoreCase: propertyNameIgnoreCase, propertyFilter: propertyFilter);
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
         *                  DEFAULTS
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Dynamically creates an instance of the supplied type
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="args">Constructor args, optional for types with a no-arg constructor</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(Type type, object[] args = null, bool nonPublic = false)
        {
            AssertIsReferenceType(type);
            return args != null && args.Any()
                ? Activator.CreateInstance(type, args)
                : Activator.CreateInstance(type, nonPublic: nonPublic);
        }

        /// <summary>
        /// Dynamically creates an instance of the class represented by the supplied class name.
        /// </summary>
        /// <param name="className">A fully qualified class name to instantiate</param>
        /// <param name="assemblyName">An assembly name from which to draw types</param>
        /// <param name="args">constructor args</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <param name="strict">If a <c>Type</c> matching the supplied name cannot be found then <c>strict == true</c> causes an exception to be thrown, default is <c>false</c>.</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(string className, string assemblyName = null, object[] args = null, bool nonPublic = false, bool strict = false)
        {
            try
            {
                var type = GetType(className, assemblyName: assemblyName);
                return GetInstance(type, args: args, nonPublic: nonPublic);
            }
            catch (TypeNotFoundException tnfex)
            {
                if (!tnfex.IsStrictSensitive || strict)
                    throw;
            }
            return null;
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type using the default constructor, if one does not exist this method throws an exception
        /// </summary>
        /// <typeparam name="T">A type (the return type)</typeparam>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A dynamically created instance of the supplied type parameter</returns>
        public static T GetDefaultInstance<T>(bool nonPublic = false)
        {
            return (T)Activator.CreateInstance(typeof(T), nonPublic: nonPublic);
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type using the default constructor, if one does not exist this method throws an exception
        /// </summary>
        /// <param name="type">A reference type.</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A dynamically created instance of the supplied type parameter</returns>
        public static object GetDefaultInstance(Type type, bool nonPublic = false)
        {
            return Activator.CreateInstance(type, nonPublic: nonPublic);
        }

        /// <summary>
        /// Gets the default value of the supplied type, e.g. <c>null</c> for reference types.  
        /// <para>
        /// Similar to <c>default(T)</c>.
        /// </para>
        /// </summary>
        /// <typeparam name="T">A type.</typeparam>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>A default value such as <c>null</c></returns>
        public static T GetDefaultValue<T>(bool nonPublic = false)
        {
            return (T)GetDefaultValue(typeof(T), nonPublic: nonPublic);
        }

        /// <inheritdoc cref="TypeUtilAbstractions.GetDefaultValue(Type, bool)"/>
        public static object GetDefaultValue(Type type, bool nonPublic = false)
        {
            return TypeUtilAbstractions.GetDefaultValue(type, nonPublic: nonPublic);
        }
    }
}
