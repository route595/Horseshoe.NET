using System;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET
{
    /// <summary>
    /// A collection of Horseshoe.NET assertions.
    /// </summary>
    public static class Assert
    {
        /// <inheritdoc cref="AssertAbstractions.CriteriaIsValid(CompareMode, ObjectValues, Type)"/>
        public static void CriteriaIsValid(CompareMode mode, ObjectValues criteria, Type typeOfInputItem = null)
        {
            AssertAbstractions.CriteriaIsValid(mode, criteria, typeOfInputItem);
        }

        /// <inheritdoc cref="AssertAbstractions.CriteriaIsValid(CompareMode, ObjectValues, out ValidationFlaggedAction, Type)"/>
        public static void CriteriaIsValid(CompareMode mode, ObjectValues criteria, out ValidationFlaggedAction vAction, Type typeOfInputItem = null)
        {
            AssertAbstractions.CriteriaIsValid(mode, criteria, out vAction, typeOfInputItem);
        }

        /// <summary>
        /// Ensures that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <typeparam name="T">A runtime value type.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The lower end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="max">The upper end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void InRange<T>(T value, T? min, T? max) where T : struct, IComparable<T>
        {
            if (min != null && value.CompareTo(min.Value) < 0)
                throw new AssertionFailedException(value + " is outside the allowed range: " + min + " to " + (max.HasValue ? max.ToString() : "[no max]"));

            if (max != null && value.CompareTo(max.Value) > 0)
                throw new AssertionFailedException(value + " is outside the allowed range: " + (min.HasValue ? min.ToString() : "[no min]") + " to " + max);
        }

        /// <summary>
        /// Ensures that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The lower end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="max">The upper end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="ignoreCase">Whether to ignore case in the string min/max comparison, default is <c>false</c>.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void InRange(string value, string min, string max, bool ignoreCase = false)
        {
            if (value == null)
                return;

            // handle 'zipper' is in range 'X' to 'Z' by elongating range 'X' to 'ZZZ'
            if (value.Length > 1 && min != null && min.Length == 1 && max != null && max.Length == 1)
            {
                max += char.IsUpper(max[0]) ? "ZZ" : "zz";
            }

            var _value = ignoreCase ? value.ToUpper() : value;
            var _min = ignoreCase ? min?.ToUpper() : min;
            var _max = ignoreCase ? max?.ToUpper() : max;

            if (_min != null && _value.CompareTo(_min) < 0)
                throw new AssertionFailedException("'" + value + "' is outside the allowed range" + (ignoreCase ? " (case-insensitive)" : "") + ": '" + min + "' to " + (max != null ? "'" + max + "'" : "[no max]"));

            if (_max != null && _value.CompareTo(_max) > 0)
                throw new AssertionFailedException("'" + value + "' is outside the allowed range" + (ignoreCase ? " (case-insensitive)" : "") + ": " + (min != null ? "'" + min + "'" : "[no min]") + " to '" + max + "'");
        }
    }
}
