﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    public static class ObjectExtensions
    {
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
        public static void MapProperties(this object src, object dest, bool tryMapAll = false, bool preventNullOverwrite = false, bool ignoreCase = false, bool ignoreErrors = false) =>
            ObjectUtil.MapProperties(src, dest, tryMapAll: tryMapAll, preventNullOverwrite: preventNullOverwrite, ignoreCase: ignoreCase, ignoreErrors: ignoreErrors);

        /// <summary>
        /// Creates a shallow duplicate of an <c>object</c>
        /// </summary>
        /// <param name="src">The source <c>object</c></param>
        /// <param name="ignoreErrors">If <c>true</c>, bypasses mapping errors leaving the values unchanged, default is <c>false</c>.</param>
        /// <returns>A new <c>object</c> identical to <c>src</c></returns>
        public static object Duplicate(this object src, bool ignoreErrors = false) =>
            ObjectUtil.Duplicate(src, ignoreErrors: ignoreErrors);

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
