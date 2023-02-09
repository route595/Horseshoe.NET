using System;

using Horseshoe.NET.Compare;
using Horseshoe.NET.ObjectsAndTypes;
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
            bool isNumeric = typeof(T).IsNumeric();

            if (min != null && value.CompareTo(min.Value) < 0)
                throw new AssertionFailedException("The value '" + value + "' is outside the allowed range: " + min + " to " + (max.HasValue ? max.ToString() : "[no max]"));

            if (max != null && value.CompareTo(max.Value) > 0)
                throw new AssertionFailedException("The value '" + value + "' is outside the allowed range: " + (min.HasValue ? min.ToString() : "[no min]") + " to " + max);
        }

        /// <summary>
        /// Ensures that <c>value</c> falls within the specified range which can be open ended by supplying <c>null</c> to <c>min</c> or <c>max</c>.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="min">The lower end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <param name="max">The upper end of the allowed range, <c>null</c> signifies open ended.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void InRange(string value, string min, string max)
        {
            if (value == null)
                return;

            if (min != null && value.CompareTo(min) < 0)
                throw new AssertionFailedException("The value '" + value + "' is outside the allowed range: " + min + " to " + (max ?? "[no max]"));

            if (max != null && value.CompareTo(max) > 0)
                throw new AssertionFailedException("The value '" + value + "' is outside the allowed range: " + (min ?? "[no min]") + " to " + max);
        }
    }
}
