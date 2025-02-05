using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A collection of value based assertions
    /// </summary>
    public static class AssertValue
    {
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
        /// Ensures, without erroring, that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <typeparam name="T">A runtime value type.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The lower end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="max">The upper end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="message">The assert error message, if applicable.</param>
        public static bool TryInRange<T>(T value, T? min, T? max, out string message) where T : struct, IComparable<T>
        {
            if (min != null && value.CompareTo(min.Value) < 0)
            {
                message = value + " is outside the allowed range: " + min + " to " + (max.HasValue ? max.ToString() : "[no max]");
                return false;
            }

            if (max != null && value.CompareTo(max.Value) > 0)
            {
                message = value + " is outside the allowed range: " + (min.HasValue ? min.ToString() : "[no min]") + " to " + max;
                return false;
            }
            message = null;
            return true;
        }

        /// <summary>
        /// Ensures that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The lower end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="max">The upper end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="ignoreCase">Whether to ignore case in the string min/max comparison, default is <c>false</c>.</param>
        /// <param name="firstCharCompare">Whether to match the first <c>char</c> only when min/max are single <c>chars</c>, default is <c>false</c>.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void InRange(string value, string min, string max, bool ignoreCase = false, bool firstCharCompare = false)
        {
            if (value == null)
                return;

            // handle 'zipper' is in range 'X' to 'Z' by elongating range 'X' to 'ZZZ'
            if (firstCharCompare && value.Length > 1 && min != null && min.Length == 1 && max != null && max.Length == 1)
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

        /// <summary>
        /// Ensures that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <typeparam name="T">A runtime value type.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="possibleValues">Each of the possible values of <c>value</c>.</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static void InSet<T>(T value, IEnumerable<T> possibleValues) where T : struct, IEquatable<T>
        {
            if (possibleValues == null)
                throw new ValidationException(nameof(possibleValues) + " cannot be null");
            if (!possibleValues.Contains(value))
                throw new AssertionFailedException(value + " is outside the set of valid values: " + TextUtil.Crop(string.Join(", ", possibleValues), 50, position: HorizontalPosition.Center, truncateMarker: TruncateMarker.LongEllipsis));
        }

        /// <summary>
        /// Ensures that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <typeparam name="T">A runtime value type.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="possibleValues">Each of the possible values of <c>value</c>.</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static void InSet<T>(T value, params T[] possibleValues) where T : struct, IEquatable<T>
        {
            InSet(value, possibleValues as IEnumerable<T>);
        }
    }
}
