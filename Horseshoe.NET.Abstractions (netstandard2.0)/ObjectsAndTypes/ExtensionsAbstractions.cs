using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.ObjectsAndTypes
{
    /// <summary>
    /// A set of extension methods for Horseshoe.NET.ObjectsAndTypes.
    /// </summary>
    public static class ExtensionsAbstractions
    {
        /// <summary>
        /// Tests whether this type is a nullable value type and, if so, gets the underlying type.
        /// </summary>
        /// <param name="type">A runtime type.</param>
        /// <param name="underlyingType">The non-nullable underlying type, if applicable.</param>
        /// <returns><c>true</c> if a nullable value type, otherwise <c>false</c>.</returns>
        public static bool TryGetUnderlyingType(this Type type, out Type underlyingType)
        {
            underlyingType = null;
            if (type.IsValueType)
            {
                underlyingType = Nullable.GetUnderlyingType(type);
                return underlyingType != null;
            }
            return false;
        }

        /// <summary>
        /// Tests whether this type is a nullable value type.
        /// </summary>
        /// <param name="type">A runtime type.</param>
        /// <returns><c>true</c> if a nullable value type, otherwise <c>false</c>.</returns>
        public static bool IsNullable(this Type type)
        {
            return TryGetUnderlyingType(type, out _);
        }

        /// <summary>
        /// Filters a collection of properties by type
        /// </summary>
        /// <param name="properties">A collection of properties</param>
        /// <param name="type">The type of property to include in the results</param>
        /// <param name="strictType">If <c>true</c>, only exact type matches are included, default is <c>false</c> which also allows inherited types</param>
        /// <returns>A filtered property collection</returns>
        public static IEnumerable<PropertyInfo> OfPropertyType(this IEnumerable<PropertyInfo> properties, Type type, bool strictType = false)
        {
            if (type == null)
                return properties;
            return properties
                .Where
                (
                    p => strictType
                        ? p.PropertyType == type
                        : p.PropertyType.IsAssignableFrom(type)
                );
        }

        /// <summary>
        /// Filters a collection of properties by type
        /// </summary>
        /// <typeparam name="T">A type</typeparam>
        /// <param name="properties">A collection of properties</param>
        /// <param name="strictType">If <c>true</c>, only exact type matches are included, default is <c>false</c> which also allows inherited types</param>
        /// <returns>A filtered property collection</returns>
        public static IEnumerable<PropertyInfo> OfPropertyType<T>(this IEnumerable<PropertyInfo> properties, bool strictType = false)
        {
            return properties
                .Where
                (
                    p => strictType
                        ? p.PropertyType == typeof(T)
                        : p.PropertyType.IsAssignableFrom(typeof(T))
                );
        }

        /// <summary>
        /// Filters a collection of properties by name.
        /// </summary>
        /// <param name="properties">A collection of properties</param>
        /// <param name="mode">The search text matching strategy i.e. <c>Contains</c>, <c>StartsWith</c>, <c>EndsWith</c> or <c>Equals</c></param>
        /// <param name="propertyNameSearchValues">The full or partial property name to search for</param>
        /// <param name="ignoreCase">If <c>true</c>, filter will include properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A filtered property collection</returns>
        public static IEnumerable<PropertyInfo> NamedLike(this IEnumerable<PropertyInfo> properties, CompareMode mode, StringValues propertyNameSearchValues, bool ignoreCase = false)
        {
            if (propertyNameSearchValues.Count > 0)
                return properties
                    .Where(p => Comparator.IsMatch(p.Name, mode, ObjectValues.FromStringValues(propertyNameSearchValues), ignoreCase: ignoreCase));
            return properties;
        }
    }
}
