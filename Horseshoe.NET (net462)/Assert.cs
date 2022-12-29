using System;
using System.Linq;

using Horseshoe.NET.Compare;
using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET
{
    /// <summary>
    /// A factory of Horseshoe.NET assertions.
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Validates whether the criteria is valid (type, quantity and content).
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">The criteria value(s) to compare against.</param>
        /// <param name="typeOfInputItem">The type of item, optiona. If supplied then it must match the criteria type.</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public static void CriteriaIsValid(CompareMode mode, ObjectValues criteria, Type typeOfInputItem = null)
        {
            CriteriaIsValid(mode, criteria, out _, typeOfInputItem: typeOfInputItem);
        }

        /// <summary>
        /// Validates whether the criteria is valid (type, quantity and content).
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">The criteria value(s) to compare against.</param>
        /// <param name="typeOfInputItem">The type of item, optiona. If supplied then it must match the criteria type.</param>
        /// <param name="vAction">Alerts client code to perform the action identified by the validator, if any (for example, when the between hi and lo criteria are switched).</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public static void CriteriaIsValid(CompareMode mode, ObjectValues criteria, out ValidationFlaggedAction vAction, Type typeOfInputItem = null)
        {
            vAction = ValidationFlaggedAction.None;
            if (typeOfInputItem != null && criteria.Any(o => !o.GetType().IsAssignableFrom(typeOfInputItem)))
            {
                throw new AssertionFailedException("Type mismatch: input is " + typeOfInputItem + "; criteria are " + string.Join(", ", criteria.Select(o => o.GetType()).Distinct()));
            }

            switch (mode)
            {
                case CompareMode.Equals:
                case CompareMode.Contains:
                case CompareMode.StartsWith:
                case CompareMode.EndsWith:
                case CompareMode.GreaterThan:
                case CompareMode.GreaterThanOrEquals:
                case CompareMode.LessThan:
                case CompareMode.LessThanOrEquals:
                    if (criteria.Count != 1)
                        throw new AssertionFailedException("This compare mode requires exactly 1 criterium: " + mode);
                    if (criteria[0] == null && mode != CompareMode.Equals)
                        throw new AssertionFailedException("This compare mode requires 1 non-null criterium: " + mode);
                    break;
                case CompareMode.Between:
                case CompareMode.BetweenExclusive:
                    if (criteria.Count != 2)
                        throw new AssertionFailedException("This compare mode requires exactly 2 criteria: " + mode);
                    if (criteria[0] == null || criteria[1] == null)
                        throw new AssertionFailedException("This compare mode requires 2 non-null criteria: " + mode);
                    // flip hi and lo, if applicable
                    if (criteria[0] is IComparable loValue && loValue.CompareTo(criteria[1]) > 0)
                    {
                        vAction = ValidationFlaggedAction.SwitchHiAndLoValues;
                    }
                    break;
                case CompareMode.In:
                    break;
                case CompareMode.IsNull:
                case CompareMode.IsNullOrWhitespace:
                    if (criteria.Count != 0)
                        throw new AssertionFailedException("This compare mode requires exactly 0 criteria: " + mode);
                    break;
                case CompareMode.Regex:
                    if (criteria.Count != 1)
                        throw new AssertionFailedException("This compare mode requires exactly 1 criterium: Regex");
                    if (criteria[0] is string stringSearchValue)
                    {
                        if (string.IsNullOrWhiteSpace(stringSearchValue))
                            throw new AssertionFailedException("This compare mode requires a non-blank criterium: Regex");
                    }
                    else throw new AssertionFailedException("This compare mode requires a criterium of type " + typeof(string) + ": Regex");
                    break;
                default:
                    throw new ThisShouldNeverHappenException("This compare mode is invalid: " + mode);
            }
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
                throw new AssertionFailedException("The value " + (isNumeric ? "is less than" : "precedes") + " the miminum: '" + value + "' < '" + min + "'");

            if (max != null && value.CompareTo(max.Value) > 0)
                throw new AssertionFailedException("The value " + (isNumeric ? "is greater than" : "exceeds") + " the maximum: '" + value + "' > '" + max + "'");
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
                throw new AssertionFailedException("The value precedes the miminum: '" + value + "' < '" + min + "'");

            if (max != null && value.CompareTo(max) > 0)
                throw new AssertionFailedException("The value exceeds the maximum: '" + value + "' > '" + max + "'");
        }
    }
}
