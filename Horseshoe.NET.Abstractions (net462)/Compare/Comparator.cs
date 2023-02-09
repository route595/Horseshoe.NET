using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Primitives;
using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

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
            AssertAbstractions.CriteriaIsValid(Mode, Criteria, typeOfInputItem: typeof(T));

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
                        : CollectionUtilAbstractions.In(inputItem, Criteria);
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
}
