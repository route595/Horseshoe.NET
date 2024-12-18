﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for object handling, reflection and manipulation.
    /// </summary>
    public static class ObjectUtilAbstractions
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
        public static PropertyValue[] GetInstancePropertyValues(object instance, Type propertyTypeFilter = null, bool strictPropertyTypeFilter = false, CompareMode propertyNameCompareMode = default, StringValues propertyNameCriteria = default, bool propertyNameIgnoreCase = false, Func<PropertyInfo, bool> propertyFilter = null)
        {
            if (instance == null)
                return null;
            return TypeUtilAbstractions.GetInstanceProperties(instance.GetType())
                .OfPropertyType(propertyTypeFilter, strictType: strictPropertyTypeFilter)
                .NamedLike(propertyNameCompareMode, propertyNameCriteria, ignoreCase: propertyNameIgnoreCase)
                .Where(p => propertyFilter?.Invoke(p) ?? true)
                .Select(prop => new PropertyValue(prop, prop.GetValue(instance)))
                .ToArray();
        }
    }
}
