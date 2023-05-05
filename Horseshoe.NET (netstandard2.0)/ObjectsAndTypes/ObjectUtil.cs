using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;

namespace Horseshoe.NET.ObjectsAndTypes
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

        /// <inheritdoc cref="ObjectUtilAbstractions.MapProperties"/>
        public static void MapProperties(object src, object dest, bool tryMapAll = false, bool preventNullOverwrite = false, bool ignoreCase = false, bool ignoreErrors = false)
        {
            ObjectUtilAbstractions.MapProperties(src, dest, tryMapAll: tryMapAll, preventNullOverwrite: preventNullOverwrite, ignoreCase: ignoreCase, ignoreErrors: ignoreErrors);
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
            var dest = TypeUtil.GetInstance(src.GetType(), args: args, nonPublic: nonPublic);
            MapProperties(src, dest, ignoreErrors: ignoreErrors);
            return dest;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * 
         *         GET / SET PROPERTY VALUES
         * * * * * * * * * * * * * * * * * * * * * * * */

        /// <inheritdoc cref="ObjectUtilAbstractions.GetInstancePropertyValues(object, Type, bool, CompareMode, StringValues, bool, Func{PropertyInfo, bool})"/>
        public static PropertyValue[] GetInstancePropertyValues(object instance, Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, CompareMode propertyNameCompareMode = default, StringValues propertyNameCriteria = default, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null)
        {
            return ObjectUtilAbstractions.GetInstancePropertyValues(instance, propertyTypeFilter: propertyTypeFilter, strictPropertyTypeFilter: strictPropertyTypeFilter, propertyNameCompareMode: propertyNameCompareMode, propertyNameCriteria: propertyNameCriteria, propertyNameIgnoreCase: propertyNameIgnoreCase, propertyFilter: propertyFilter);
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
    }
}
