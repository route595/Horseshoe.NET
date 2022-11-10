using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Text
{
    public class SearchCriteria
    {
        public StringValues SearchTerms { get; }

        public bool IgnoreCase { get; }

        public Comparison Comparison { get; } = Comparison.Equals;

        private SearchCriteria(Comparison comparison, bool ignoreCase = false)
        {
            Comparison = comparison;
            IgnoreCase = ignoreCase;
        }

        public SearchCriteria(string searchTerm, Comparison comparison, bool ignoreCase = false)
        {
            SearchTerms = searchTerm;
            Comparison = comparison;
            IgnoreCase = ignoreCase;
        }

        public SearchCriteria(string[] searchTerms, Comparison comparison, bool ignoreCase = false)
        {
            SearchTerms = searchTerms;
            Comparison = comparison;
            IgnoreCase = ignoreCase;
        }

        public bool Evaluate(string stringToEvaluate)
        {
            if (stringToEvaluate == null) return Comparison == Comparison.IsNull;

            // validation
            switch (Comparison)
            {
                case Comparison.Equals:
                case Comparison.NotEquals:
                case Comparison.Contains:
                case Comparison.NotContains:
                case Comparison.StartsWith:
                case Comparison.NotStartsWith:
                case Comparison.EndsWith:
                case Comparison.NotEndsWith:
                case Comparison.LessThan:
                case Comparison.LessThanOrEqual:
                case Comparison.GreaterThan:
                case Comparison.GreaterThanOrEqual:
                    if (SearchTerms.Count != 1) throw new ArgumentException("When performing a(n) '" + Comparison + "' comparison 'SearchTerms' must contain 1 item: " + SearchTerms.Count);
                    break;
                case Comparison.Between:
                case Comparison.NotBetween:
                case Comparison.BetweenExclusive:
                case Comparison.NotBetweenInclusive:
                case Comparison.BetweenExclusiveLowerBound:
                case Comparison.NotBetweenExclusiveLowerBound:
                case Comparison.BetweenExclusiveUpperBound:
                case Comparison.NotBetweenExclusiveUpperBound:
                    if (SearchTerms.Count != 2) throw new ArgumentException("When performing a 'Between' comparison 'SearchTerms' MUST contain two items: " + SearchTerms.Count);
                    if (string.IsNullOrEmpty(SearchTerms[0]))
                    {
                        if (string.IsNullOrEmpty(SearchTerms[1]))
                        {
                            throw new ArgumentException("When performing a 'Between' comparison 'SearchTerms' MUST contain at least one value that is not blank or null");
                        }

                        // redirect to less than search
                        var redirectedSearchCriteria = new SearchCriteria
                        (
                            SearchTerms[1],
                            Comparison == Comparison.Between || Comparison == Comparison.BetweenExclusiveLowerBound
                                ? Comparison.LessThanOrEqual
                                : Comparison.LessThan
                        );
                        return redirectedSearchCriteria.Evaluate(stringToEvaluate);
                    }
                    else if (string.IsNullOrEmpty(SearchTerms[1]))
                    {
                        // redirect to greater than search
                        var redirectedSearchCriteria = new SearchCriteria
                        (
                            SearchTerms[0],
                            Comparison == Comparison.Between || Comparison == Comparison.BetweenExclusiveUpperBound
                                ? Comparison.GreaterThanOrEqual
                                : Comparison.GreaterThan
                        );
                        return redirectedSearchCriteria.Evaluate(stringToEvaluate);
                    }
                    break;
                case Comparison.In:
                case Comparison.NotIn:
                    if (!SearchTerms.Any()) throw new ArgumentException("When performing an 'In' comparison 'SearchTerms' MUST contain at least one item: 0");
                    break;
                default:
                    break;
            }

            switch (Comparison)
            {
                case Comparison.Equals:
                    return string.Equals(SearchTerms[0], stringToEvaluate, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                case Comparison.NotEquals:
                    return !string.Equals(SearchTerms[0], stringToEvaluate, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                case Comparison.Contains:
                    return IgnoreCase
                        ? stringToEvaluate.ToUpper().Contains(SearchTerms[0].ToUpper())
                        : stringToEvaluate.Contains(SearchTerms[0]);
                case Comparison.NotContains:
                    return IgnoreCase
                        ? !stringToEvaluate.ToUpper().Contains(SearchTerms[0].ToUpper())
                        : !stringToEvaluate.Contains(SearchTerms[0]);
                case Comparison.StartsWith:
                    return IgnoreCase
                        ? stringToEvaluate.ToUpper().StartsWith(SearchTerms[0].ToUpper())
                        : stringToEvaluate.StartsWith(SearchTerms[0]);
                case Comparison.NotStartsWith:
                    return IgnoreCase
                        ? !stringToEvaluate.ToUpper().StartsWith(SearchTerms[0].ToUpper())
                        : !stringToEvaluate.StartsWith(SearchTerms[0]);
                case Comparison.EndsWith:
                    return IgnoreCase
                        ? stringToEvaluate.ToUpper().EndsWith(SearchTerms[0].ToUpper())
                        : stringToEvaluate.EndsWith(SearchTerms[0]);
                case Comparison.NotEndsWith:
                    return IgnoreCase
                        ? !stringToEvaluate.ToUpper().EndsWith(SearchTerms[0].ToUpper())
                        : !stringToEvaluate.EndsWith(SearchTerms[0]);
                case Comparison.LessThan:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerms[0].ToUpper()) < 0
                        : string.Compare(stringToEvaluate, SearchTerms[0]) < 0;
                case Comparison.LessThanOrEqual:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerms[0].ToUpper()) <= 0
                        : string.Compare(stringToEvaluate, SearchTerms[0]) <= 0;
                case Comparison.GreaterThan:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerms[0].ToUpper()) > 0
                        : string.Compare(stringToEvaluate, SearchTerms[0]) > 0;
                case Comparison.GreaterThanOrEqual:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerms[0].ToUpper()) >= 0
                        : string.Compare(stringToEvaluate, SearchTerms[0]) >= 0;
                case Comparison.Between:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) >= 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) <= 0;
                case Comparison.NotBetween:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) < 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) > 0;
                case Comparison.BetweenExclusive:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) > 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) < 0;
                case Comparison.NotBetweenInclusive:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) <= 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) >= 0;
                case Comparison.BetweenExclusiveLowerBound:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) > 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) <= 0;
                case Comparison.NotBetweenExclusiveLowerBound:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) <= 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) > 0;
                case Comparison.BetweenExclusiveUpperBound:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) >= 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) < 0;
                case Comparison.NotBetweenExclusiveUpperBound:
                    return
                        string.Compare(stringToEvaluate, SearchTerms[0], IgnoreCase) < 0 &&
                        string.Compare(stringToEvaluate, SearchTerms[1], IgnoreCase) >= 0;
                case Comparison.In:
                    return IgnoreCase
                        ? SearchTerms.Select(s => s?.ToUpper()).Contains(stringToEvaluate.ToUpper())
                        : SearchTerms.Contains(stringToEvaluate);
                case Comparison.NotIn:
                    return IgnoreCase
                        ? !SearchTerms.Select(s => s?.ToUpper()).Contains(stringToEvaluate.ToUpper())
                        : !SearchTerms.Contains(stringToEvaluate);
                case Comparison.IsNull:
                    return false;
                case Comparison.IsNotNull:
                    return true;
            }
            return false;
        }

        public static SearchCriteria IsNull() => new SearchCriteria(Comparison.IsNull);

        public static SearchCriteria IsNotNull() => new SearchCriteria(Comparison.IsNotNull);

        public static SearchCriteria Equals(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.Equals, ignoreCase: ignoreCase);

        public static SearchCriteria NotEquals(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.NotEquals, ignoreCase: ignoreCase);

        public static SearchCriteria Contains(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.Contains, ignoreCase: ignoreCase);

        public static SearchCriteria NotContains(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.NotContains, ignoreCase: ignoreCase);

        public static SearchCriteria StartsWith(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.StartsWith, ignoreCase: ignoreCase);

        public static SearchCriteria NotStartsWith(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.NotStartsWith, ignoreCase: ignoreCase);

        public static SearchCriteria EndsWith(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.EndsWith, ignoreCase: ignoreCase);

        public static SearchCriteria NotEndsWith(string searchTerm, bool ignoreCase = false) => new SearchCriteria(searchTerm, Comparison.NotEndsWith, ignoreCase: ignoreCase);

        public static SearchCriteria Between(string[] searchTerms, bool ignoreCase = false) => new SearchCriteria(searchTerms, Comparison.Between, ignoreCase: ignoreCase);

        public static SearchCriteria NotBetween(string[] searchTerms, bool ignoreCase = false) => new SearchCriteria(searchTerms, Comparison.NotBetween, ignoreCase: ignoreCase);

        public static SearchCriteria In(string[] searchTerms, bool ignoreCase = false) => new SearchCriteria(searchTerms, Comparison.In, ignoreCase: ignoreCase);

        public static SearchCriteria NotIn(string[] searchTerms, bool ignoreCase = false) => new SearchCriteria(searchTerms, Comparison.NotIn, ignoreCase: ignoreCase);
    }
}
