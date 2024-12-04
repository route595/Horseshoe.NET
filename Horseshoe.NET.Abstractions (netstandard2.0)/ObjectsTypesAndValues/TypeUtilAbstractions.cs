using System;
using System.Linq;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Text;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for types.
    /// </summary>
    public static class TypeUtilAbstractions
    {
        /// <summary>
        /// Finds the runtime type respresented by the fully qualified type name
        /// </summary>
        /// <param name="typeName">The fully qualified type name.</param>
        /// <param name="assemblyName">An optional assembly name.</param>
        /// <param name="ignoreCase">If <c>true</c>, allows searching assemblies/types that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <exception cref="TypeNotFoundException"/>
        /// <returns>The type indicated by <c>name</c></returns>
        public static Type GetType(string typeName, string assemblyName = null, bool ignoreCase = false)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            Exception lastException = null;

            // step 1 - direct type loading (seldom works)
            Type type = Type.GetType(typeName, throwOnError: false, ignoreCase: ignoreCase);
            if (type != null)
                return type;

            // step 2 - load type from loaded assemblies (e.g. System.Core, Horseshoe.NET, etc.)
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(typeName);
                    if (type != null)
                        return type;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            // step 3 - load assembly and type
            try
            {
                if (assemblyName != null)
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        type = assembly.GetType(typeName);
                        if (type != null)
                            return type;
                    }
                }

            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            throw new TypeNotFoundException("Type \"" + typeName + "\" could not be loaded" + (lastException != null ? ": " + lastException.Message : "") + ".  Please ensure you are using the fully qualified class name and that the proper assembly is accessible, if applicable (e.g. is present in the executable's path or installed in the GAC)", lastException) { IsStrictSensitive = true };
        }

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
            if (type.IsValueType && !type.IsNullable())
            {
                return Activator.CreateInstance(type, nonPublic: nonPublic);
            }
            return null;
        }

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
        public static PropertyValue[] GetStaticPropertyValues(Type type, Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, CompareMode propertyNameCompareMode = default, string propertyNameCriteria = null, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null)
        {
            if (type == null)
                return null;
            return GetStaticProperties(type)
                .OfPropertyType(propertyTypeFilter, strictType: strictPropertyTypeFilter)
                .NamedLike(propertyNameCompareMode, propertyNameCriteria, ignoreCase: propertyNameIgnoreCase)
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
        public static PropertyValue[] GetStaticPropertyValues<T>(Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, CompareMode propertyNameCompareMode = default, StringValues propertyNameCriteria = default, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null) where T : class
        {
            return GetStaticProperties<T>()
                .OfPropertyType(propertyTypeFilter, strictType: strictPropertyTypeFilter)
                .NamedLike(propertyNameCompareMode, propertyNameCriteria, ignoreCase: propertyNameIgnoreCase)
                .Where(p => propertyFilter?.Invoke(p) ?? true)
                .Select(prop => new PropertyValue(prop, prop.GetValue(null)))
                .ToArray();
        }

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
            if (type.IsSealed && type.IsAbstract)
                return "[static-class]";
            var strb = new StringBuilder("class");
            if (type.IsSealed)
                strb.Insert(0, "sealed-");
            if (type.IsAbstract)
                strb.Insert(0, "abstract-");
            if (type.IsGenericType)
                strb.Insert(0, "generic-");
            if (type.IsImport)
                strb.Insert(0, "COM-");
            return "[" + strb + "]";
        }

        /// <summary>
        /// Asserts that the supplied type is a reference type.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <param name="errorMessage">An optional error message.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIsReferenceType(Type type, string errorMessage = null)
        {
            if (type == null || !type.IsClass)
                throw new AssertionFailedException(errorMessage ?? (type.FullName + " is not a reference type: " + GetKindOfType(type)));
        }
    }
}
