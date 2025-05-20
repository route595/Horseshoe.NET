//using System;
//using System.Text.RegularExpressions;
//using Microsoft.Extensions.Primitives;

//using Horseshoe.NET.Collections;

//namespace Horseshoe.NET.Text
//{
//    /// <summary>
//    /// Factory methods for comparing text strings against specific criteria.
//    /// </summary>
//    public static class TextSearch
//    {
//        /// <summary>
//        /// Validates whether the search criteria is valid.
//        /// </summary>
//        /// <param name="mode">The text match mode, e.g. Equals, Contains, Between, etc.</param>
//        /// <param name="searchValues">The criteria value(s).</param>
//        /// <exception cref="AssertionFailedException"></exception>
//        public static void AssertSearchCriteriaIsValid(TextMatch mode, StringValues searchValues)
//        {
//            AssertSearchCriteriaIsValid(mode, searchValues, null);
//        }

//        private static void AssertSearchCriteriaIsValid(TextMatch mode, StringValues searchValues, Action<StringValues> substituteSearchValues)
//        {
//            switch (mode)
//            {
//                case TextMatch.Equals:
//                case TextMatch.Contains:
//                case TextMatch.StartsWith:
//                case TextMatch.EndsWith:
//                case TextMatch.GreaterThan:
//                case TextMatch.GreaterThanOrEquals:
//                case TextMatch.LessThan:
//                case TextMatch.LessThanOrEquals:
//                    if (searchValues.Count != 1)
//                        throw new AssertionFailedException("This text match mode requires exactly 1 search value: " + mode);
//                    if (searchValues[0] == null && mode != TextMatch.Equals)
//                        throw new AssertionFailedException("This text match mode requires 1 non-null search value: " + mode);
//                    break;
//                case TextMatch.Between:
//                case TextMatch.BetweenExclusive:
//                    if (searchValues.Count != 1)
//                        throw new AssertionFailedException("This text match mode requires exactly 2 search values: " + mode);
//                    if (searchValues[0] == null || searchValues[1] == null)
//                        throw new AssertionFailedException("This text match mode requires 2 non-null search values: " + mode);
//                    // flip hi and lo, if applicable
//                    if (searchValues[0].CompareTo(searchValues[1]) > 0)
//                    {
//                        substituteSearchValues?.Invoke(new[] { searchValues[1], searchValues[0] });
//                    }
//                    break;
//                case TextMatch.In:
//                    break;
//                case TextMatch.IsNull:
//                case TextMatch.IsNullOrWhitespace:
//                    if (searchValues.Count != 0)
//                        throw new AssertionFailedException("This text match mode requires exactly 0 search value: " + mode);
//                    break;
//                case TextMatch.Regex:
//                    if (searchValues.Count != 1)
//                        throw new AssertionFailedException("This text match mode requires exactly 1 search value: Regex");
//                    if (string.IsNullOrWhiteSpace(searchValues[0]))
//                        throw new AssertionFailedException("This text match mode requires 1 non-blank search value: Regex");
//                    break;
//                default:
//                    throw new ThisShouldNeverHappenException("This text match mode has not been programmed yet: " + mode);
//            }
//        }

//        /// <summary>
//        /// Indicates whther the item to search is a criteria match
//        /// </summary>
//        /// <param name="itemToSearch">The text upon which to perform the comparison search</param>
//        /// <param name="mode">The text match mode, e.g. Equals, Contains, Between, etc.</param>
//        /// <param name="searchValues">The criteria value(s).</param>
//        /// <param name="ignoreCase">Whether to ignore the letter case of the search criteria, default is <c>false</c>.</param>
//        /// <returns></returns>
//        /// <exception cref="ValidationException"></exception>
//        public static bool IsMatch(string itemToSearch, TextMatch mode, StringValues searchValues, bool ignoreCase = false)
//        {
//            AssertSearchCriteriaIsValid(mode, searchValues, (_sv) => searchValues = _sv);

//            // special case 1 - item to search is null
//            if (itemToSearch == null)
//            {
//                switch (mode)
//                {
//                    case TextMatch.Equals:
//                        return searchValues[0] == null;
//                    case TextMatch.IsNull:
//                    case TextMatch.IsNullOrWhitespace:
//                        return true;
//                }
//            }

//            // special case 2 - search values is empty
//            if (searchValues.Count == 0)
//            {
//                switch (mode)
//                {
//                    case TextMatch.In:
//                        return false;
//                }
//            }

//            // normal
//            switch (mode)
//            {
//                case TextMatch.Equals:
//                default:
//                    if (searchValues[0] == null)
//                        return false;
//                    return ignoreCase
//                        ? string.Equals(itemToSearch, searchValues[0], StringComparison.OrdinalIgnoreCase)
//                        : string.Equals(itemToSearch, searchValues[0]);
//                case TextMatch.Contains:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().Contains(searchValues[0].ToUpper())
//                        : itemToSearch.Contains(searchValues[0]);
//                case TextMatch.StartsWith:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().StartsWith(searchValues[0].ToUpper())
//                        : itemToSearch.StartsWith(searchValues[0]);
//                case TextMatch.EndsWith:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().EndsWith(searchValues[0].ToUpper())
//                        : itemToSearch.EndsWith(searchValues[0]);
//                case TextMatch.GreaterThan:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().CompareTo(searchValues[0].ToUpper()) > 0
//                        : itemToSearch.CompareTo(searchValues[0]) > 0;
//                case TextMatch.GreaterThanOrEquals:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().CompareTo(searchValues[0].ToUpper()) >= 0
//                        : itemToSearch.CompareTo(searchValues[0]) >= 0;
//                case TextMatch.LessThan:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().CompareTo(searchValues[0].ToUpper()) < 0
//                        : itemToSearch.CompareTo(searchValues[0]) < 0;
//                case TextMatch.LessThanOrEquals:
//                    return ignoreCase
//                        ? itemToSearch.ToUpper().CompareTo(searchValues[0].ToUpper()) <= 0
//                        : itemToSearch.CompareTo(searchValues[0]) <= 0;
//                case TextMatch.Between:
//                    if (ignoreCase)
//                    {
//                        var itemToSearchUpper = itemToSearch.ToUpper();
//                        return itemToSearchUpper.CompareTo(searchValues[0].ToUpper()) >= 0 && itemToSearchUpper.CompareTo(searchValues[1].ToUpper()) <= 0;
//                    }
//                    return itemToSearch.CompareTo(searchValues[0]) >= 0 && itemToSearch.CompareTo(searchValues[1]) <= 0;
//                case TextMatch.BetweenExclusive:
//                    if (ignoreCase)
//                    {
//                        var itemToSearchUpper = itemToSearch.ToUpper();
//                        return itemToSearchUpper.CompareTo(searchValues[0].ToUpper()) > 0 && itemToSearchUpper.CompareTo(searchValues[1].ToUpper()) < 0;
//                    }
//                    return itemToSearch.CompareTo(searchValues[0]) > 0 && itemToSearch.CompareTo(searchValues[1]) < 0;
//                case TextMatch.In:
//                    return ignoreCase
//                        ? itemToSearch.InIgnoreCase(searchValues)
//                        : itemToSearch.In(searchValues);
//                case TextMatch.IsNull:
//                case TextMatch.IsNullOrWhitespace:
//                    return false;
//                case TextMatch.Regex:
//                    return ignoreCase
//                        ? Regex.IsMatch(itemToSearch, searchValues[0], RegexOptions.IgnoreCase)
//                        : Regex.IsMatch(itemToSearch, searchValues[0]);
//            }
//        }
//    }
//}
