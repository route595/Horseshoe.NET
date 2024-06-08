using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Primitives;
using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Everything needed to perform a standard comparison bundled into a single class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Comparator<T> : IComparator<T> where T : IComparable<T>
    {
        /// <summary>
        /// The compare mode, e.g. Equals, Contains, Between, etc.
        /// </summary>
        public CompareMode Mode { get; set; }

        /// <summary>
        /// The criteria value(s) to compare against.
        /// </summary>
        public ObjectValues Criteria { get; set; }

        /// <summary>
        /// Whether to ignore the letter case of the criteria.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Indicates whether the input item is a criteria match.
        /// </summary>
        /// <param name="inputItem"></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool IsMatch(T inputItem)
        {
            Comparator.AssertCriteriaIsValid(Mode, Criteria, typeOfInputItem: typeof(T));

            // special case 1 - item to compare is null
            if (inputItem == null)
            {
                switch (Mode)
                {
                    case CompareMode.Equals:
                        return Criteria[0] == null;
                    case CompareMode.IsNull:
                    case CompareMode.IsNullOrWhitespace:
                        return true;
                }
            }

            // special case 2 - criteria is empty
            if (Criteria.Count == 0)
            {
                switch (Mode)
                {
                    case CompareMode.In:
                        return false;
                }
            }

            // normal case
            switch (Mode)
            {
                case CompareMode.Equals:
                default:
                    if (Criteria[0] == null)
                        return false;
                    if (typeof(T) == typeof(string))
                    {
                        return IgnoreCase
                            ? string.Equals(inputItem as string, Criteria[0] as string, StringComparison.OrdinalIgnoreCase)
                            : string.Equals(inputItem as string, Criteria[0] as string);
                    }
                    return inputItem.CompareTo((T)Criteria[0]) == 0;
                case CompareMode.Contains:
                    if (typeof(T) == typeof(string))
                    {
                        return IgnoreCase
                            ? (inputItem as string).ToUpper().Contains((Criteria[0] as string).ToUpper())
                            : (inputItem as string).Contains(Criteria[0] as string);
                    }
                    else throw new ValidationException("Compare Mode 'Contains' only works when T is " + typeof(string));
                case CompareMode.StartsWith:
                    if (typeof(T) == typeof(string))
                    {
                        return IgnoreCase
                            ? (inputItem as string).ToUpper().StartsWith((Criteria[0] as string).ToUpper())
                            : (inputItem as string).StartsWith(Criteria[0] as string);
                    }
                    else throw new ValidationException("Compare Mode 'StartsWith' only works when T is " + typeof(string));
                case CompareMode.EndsWith:
                    if (typeof(T) == typeof(string))
                    {
                        return IgnoreCase
                            ? (inputItem as string).ToUpper().EndsWith((Criteria[0] as string).ToUpper())
                            : (inputItem as string).EndsWith(Criteria[0] as string);
                    }
                    else throw new ValidationException("Compare Mode 'EndsWith' only works when T is " + typeof(string));
                case CompareMode.GreaterThan:
                    if (typeof(T) == typeof(string) && IgnoreCase)
                        return (inputItem as string).ToUpper().CompareTo((Criteria[0] as string).ToUpper()) > 0;
                    return inputItem.CompareTo((T)Criteria[0]) > 0;
                case CompareMode.GreaterThanOrEquals:
                    if (typeof(T) == typeof(string) && IgnoreCase)
                        return (inputItem as string).ToUpper().CompareTo((Criteria[0] as string).ToUpper()) >= 0;
                    return inputItem.CompareTo((T)Criteria[0]) >= 0;
                case CompareMode.LessThan:
                    if (typeof(T) == typeof(string) && IgnoreCase)
                        return (inputItem as string).ToUpper().CompareTo((Criteria[0] as string).ToUpper()) < 0;
                    return inputItem.CompareTo((T)Criteria[0]) < 0;
                case CompareMode.LessThanOrEquals:
                    if (typeof(T) == typeof(string) && IgnoreCase)
                        return (inputItem as string).ToUpper().CompareTo((Criteria[0] as string).ToUpper()) <= 0;
                    return inputItem.CompareTo((T)Criteria[0]) <= 0;
                case CompareMode.Between:
                    if (typeof(T) == typeof(string) && IgnoreCase)
                    {
                        var inputItemUpper = (inputItem as string).ToUpper();
                        return inputItemUpper.CompareTo((Criteria[0] as string).ToUpper()) >= 0 && inputItemUpper.CompareTo((Criteria[0] as string).ToUpper()) <= 0;
                    }
                    return inputItem.CompareTo((T)Criteria[0]) >= 0 && inputItem.CompareTo((T)Criteria[1]) <= 0;
                case CompareMode.BetweenExclusive:
                    if (typeof(T) == typeof(string) && IgnoreCase)
                    {
                        var inputItemUpper = (inputItem as string).ToUpper();
                        return inputItemUpper.CompareTo((Criteria[0] as string).ToUpper()) > 0 && inputItemUpper.CompareTo((Criteria[0] as string).ToUpper()) < 0;
                    }
                    return inputItem.CompareTo((T)Criteria[0]) > 0 && inputItem.CompareTo((T)Criteria[1]) < 0;
                case CompareMode.In:
                    return (typeof(T) == typeof(string) && IgnoreCase)
                        ? TextUtilAbstractions.In(inputItem as string, true, Criteria.Select(t => t as string).ToArray())
                        : CollectionUtilAbstractions.Contains(Criteria, inputItem);
                case CompareMode.IsNull:
                    return false;
                case CompareMode.IsNullOrWhitespace:
                    if (typeof(T) == typeof(string))
                        return string.IsNullOrWhiteSpace(inputItem as string);
                    return false;
                case CompareMode.Regex:
                    if (typeof(T) == typeof(string))
                    {
                        return IgnoreCase
                            ? Regex.IsMatch(inputItem as string, Criteria[0] as string, RegexOptions.IgnoreCase)
                            : Regex.IsMatch(inputItem as string, Criteria[0] as string);
                    }
                    else throw new ValidationException("Compare mode 'Regex' only works when T is " + typeof(string));
            }
        }
    }

    /// <summary>
    /// Factory methods for validating, building and running <c>Comparator</c> instances.
    /// </summary>
    public static class Comparator
    {
        /// <summary>
        /// Builds a new <c>Comparator&lt;T&gt;</c> with a single criterium.
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparator</c>.</returns>
        public static ICriterinator<T> Build<T>(CompareMode mode, T criterium, bool ignoreCase = false) where T : IComparable<T> =>
            new Comparator<T> { Mode = mode, Criteria = ObjectValues.From(criterium), IgnoreCase = ignoreCase };

        /// <summary>
        /// Builds a new <c>Comparator&lt;T&gt;</c> with a single criterium.
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparator</c>.</returns>
        public static ICriterinator<string> Build(CompareMode mode, StringValues criteria, bool ignoreCase = false) =>
            new Comparator<string> { Mode = mode, Criteria = ObjectValues.FromStringValues(criteria), IgnoreCase = ignoreCase };

        /// <summary>
        /// Builds a new <c>Comparator&lt;T&gt;</c> with more than one critera value.
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparator</c>.</returns>
        public static ICriterinator<T> Build<T>(CompareMode mode, IList<T> criteria, bool ignoreCase = false) where T : IComparable<T> =>
            new Comparator<T> { Mode = mode, Criteria = new ObjectValues(criteria.Cast<object>()), IgnoreCase = ignoreCase };

        /// <summary>
        /// Creates a new 'Equals' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparator</c>.</returns>
        public static ICriterinator<T> Equals<T>(T criterium, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.Equals, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'Contains' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparator</c>.</returns>
        public static ICriterinator<string> Contains(string criterium, bool ignoreCase = false) =>
            Build(CompareMode.Contains, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'StartsWith' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<string> StartsWith(string criterium, bool ignoreCase = false) =>
            Build(CompareMode.StartsWith, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'EndsWith' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<string> EndsWith(string criterium, bool ignoreCase = false) =>
            Build(CompareMode.EndsWith, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'GreaterThan' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> GreaterThan<T>(T criterium, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.GreaterThan, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'GreaterThanOrEquals' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> GreaterThanOrEquals<T>(T criterium, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.GreaterThanOrEquals, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'LessThan' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> LessThan<T>(T criterium, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.LessThan, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'LessThanOrEquals' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> LessThanOrEquals<T>(T criterium, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.LessThanOrEquals, criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'Between' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> Between<T>(IList<T> criteria, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.Between, criteria, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'BetweenExclusive' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> BetweenExclusive<T>(IList<T> criteria, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.BetweenExclusive, criteria, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'In' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">What to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<T> In<T>(IList<T> criteria, bool ignoreCase = false) where T : IComparable<T> =>
            Build(CompareMode.In, criteria, ignoreCase: ignoreCase);

        /// <summary>
        /// Creates a new 'IsNull' comparator based on the supplied criteria.
        /// </summary>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<IComparable<object>> IsNull() =>
            new Comparator<IComparable<object>> { Mode = CompareMode.IsNull, Criteria = ObjectValues.Empty };

        /// <summary>
        /// Creates a new 'IsNullOrWhitespace' comparator based on the supplied criteria.
        /// </summary>
        /// <returns>A <c>Comparitor</c>.</returns>
        public static ICriterinator<IComparable<object>> IsNullOrWhitespace() =>
            new Comparator<IComparable<object>> { Mode = CompareMode.IsNullOrWhitespace, Criteria = ObjectValues.Empty };

        /// <summary>
        /// Creates a new 'Regex' comparator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">The regular expression to compare the input(s) against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the regular expression, default is <c>false</c>.</param>
        /// <returns>A <c>Comparator</c>.</returns>
        public static ICriterinator<string> Regex(string criterium, bool ignoreCase = false) =>
            new RegexComparator(criterium, ignoreCase: ignoreCase);

        /// <summary>
        /// Indicates whether the input item is a criteria match.
        /// </summary>
        /// <param name="inputItem">The item to compare against the criteria.</param>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">The criteria value(s) to compare against.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the criteria, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        /// <exception cref="ValidationException"></exception>
        public static bool IsMatch<T>(T inputItem, CompareMode mode, ObjectValues criteria, bool ignoreCase = false) where T : IComparable<T>
        {
            AssertCriteriaIsValid(mode, criteria, typeOfInputItem: typeof(T));

            // special case 1 - item to compare is null
            if (inputItem == null)
            {
                switch (mode)
                {
                    case CompareMode.Equals:
                        return criteria[0] == null;
                    case CompareMode.IsNull:
                    case CompareMode.IsNullOrWhitespace:
                        return true;
                }
            }

            // special case 2 - criteria is empty
            if (criteria.Count == 0)
            {
                switch (mode)
                {
                    case CompareMode.In:
                        return false;
                }
            }

            // normal case
            switch (mode)
            {
                case CompareMode.Equals:
                default:
                    if (criteria[0] == null)
                        return false;
                    if (typeof(T) == typeof(string))
                    {
                        return ignoreCase
                            ? string.Equals(inputItem as string, criteria[0] as string, StringComparison.OrdinalIgnoreCase)
                            : string.Equals(inputItem as string, criteria[0] as string);
                    }
                    return inputItem.CompareTo((T)criteria[0]) == 0;
                case CompareMode.Contains:
                    if (typeof(T) == typeof(string))
                    {
                        return ignoreCase
                            ? (inputItem as string).ToUpper().Contains((criteria[0] as string).ToUpper())
                            : (inputItem as string).Contains(criteria[0] as string);
                    }
                    else throw new ValidationException("Compare mode 'Contains' only works when T is " + typeof(string));
                case CompareMode.StartsWith:
                    if (typeof(T) == typeof(string))
                    {
                        return ignoreCase
                            ? (inputItem as string).ToUpper().StartsWith((criteria[0] as string).ToUpper())
                            : (inputItem as string).StartsWith(criteria[0] as string);
                    }
                    else throw new ValidationException("Compare mode 'StartsWith' only works when T is " + typeof(string));
                case CompareMode.EndsWith:
                    if (typeof(T) == typeof(string))
                    {
                        return ignoreCase
                            ? (inputItem as string).ToUpper().EndsWith((criteria[0] as string).ToUpper())
                            : (inputItem as string).EndsWith(criteria[0] as string);
                    }
                    else throw new ValidationException("Compare mode 'EndsWith' only works when T is " + typeof(string));
                case CompareMode.GreaterThan:
                    if (typeof(T) == typeof(string) && ignoreCase)
                        return (inputItem as string).ToUpper().CompareTo((criteria[0] as string).ToUpper()) > 0;
                    return inputItem.CompareTo((T)criteria[0]) > 0;
                case CompareMode.GreaterThanOrEquals:
                    if (typeof(T) == typeof(string) && ignoreCase)
                        return (inputItem as string).ToUpper().CompareTo((criteria[0] as string).ToUpper()) >= 0;
                    return inputItem.CompareTo((T)criteria[0]) >= 0;
                case CompareMode.LessThan:
                    if (typeof(T) == typeof(string) && ignoreCase)
                        return (inputItem as string).ToUpper().CompareTo((criteria[0] as string).ToUpper()) < 0;
                    return inputItem.CompareTo((T)criteria[0]) < 0;
                case CompareMode.LessThanOrEquals:
                    if (typeof(T) == typeof(string) && ignoreCase)
                        return (inputItem as string).ToUpper().CompareTo((criteria[0] as string).ToUpper()) <= 0;
                    return inputItem.CompareTo((T)criteria[0]) <= 0;
                case CompareMode.Between:
                    if (typeof(T) == typeof(string) && ignoreCase)
                    {
                        var inputItemUpper = (inputItem as string).ToUpper();
                        return inputItemUpper.CompareTo((criteria[0] as string).ToUpper()) >= 0 && inputItemUpper.CompareTo((criteria[0] as string).ToUpper()) <= 0;
                    }
                    return inputItem.CompareTo((T)criteria[0]) >= 0 && inputItem.CompareTo((T)criteria[1]) <= 0;
                case CompareMode.BetweenExclusive:
                    if (typeof(T) == typeof(string) && ignoreCase)
                    {
                        var inputItemUpper = (inputItem as string).ToUpper();
                        return inputItemUpper.CompareTo((criteria[0] as string).ToUpper()) > 0 && inputItemUpper.CompareTo((criteria[0] as string).ToUpper()) < 0;
                    }
                    return inputItem.CompareTo((T)criteria[0]) > 0 && inputItem.CompareTo((T)criteria[1]) < 0;
                case CompareMode.In:
                    return (typeof(T) == typeof(string) && ignoreCase)
                        ? (inputItem as string).InIgnoreCase(criteria.Select(t => t as string))
                        : inputItem.In(criteria);
                case CompareMode.IsNull:
                    return false;
                case CompareMode.IsNullOrWhitespace:
                    if (typeof(T) == typeof(string))
                        return string.IsNullOrWhiteSpace(inputItem as string);
                    return false;
                case CompareMode.Regex:
                    if (typeof(T) == typeof(string))
                    {
                        return ignoreCase
                            ? System.Text.RegularExpressions.Regex.IsMatch(inputItem as string, criteria[0] as string, RegexOptions.IgnoreCase)
                            : System.Text.RegularExpressions.Regex.IsMatch(inputItem as string, criteria[0] as string);
                    }
                    else throw new ValidationException("Compare mode 'Regex' only works when T is " + typeof(string));
            }
        }

        /// <summary>
        /// Validates whether the criteria is valid (type, quantity and content).
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">The criteria value(s) to compare against.</param>
        /// <param name="typeOfInputItem">The type of item, optiona. If supplied then it must match the criteria type.</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public static void AssertCriteriaIsValid(CompareMode mode, ObjectValues criteria, Type typeOfInputItem = null)
        {
            AssertCriteriaIsValid(mode, criteria, out _, typeOfInputItem: typeOfInputItem);
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
        public static void AssertCriteriaIsValid(CompareMode mode, ObjectValues criteria, out ValidationFlaggedAction vAction, Type typeOfInputItem = null)
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
    }
}
