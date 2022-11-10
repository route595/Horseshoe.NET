using System;
using System.Linq;

namespace Horseshoe.NET.Text
{
    public class SearchCriteria
    {
        public string SearchTerm { get; }

        public string[] SearchTerms { get; }

        public bool IgnoreCase { get; }

        public Comparison Comparison { get; } = Comparison.Equals;

        private SearchCriteria(Comparison comparison, bool ignoreCase = false)
        {
            Comparison = comparison;
            IgnoreCase = ignoreCase;
        }

        public SearchCriteria(string searchTerm, Comparison comparison, bool ignoreCase = false)
        {
            SearchTerm = searchTerm;
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
                    if (SearchTerm == null) throw new ArgumentException("When performing a(n) '" + Comparison + "' comparison 'SearchTerm' may NOT be null ");
                    break;
                case Comparison.Between:
                case Comparison.NotBetween:
                case Comparison.BetweenExclusive:
                case Comparison.NotBetweenInclusive:
                case Comparison.BetweenExclusiveLowerBound:
                case Comparison.NotBetweenExclusiveLowerBound:
                case Comparison.BetweenExclusiveUpperBound:
                case Comparison.NotBetweenExclusiveUpperBound:
                    if (SearchTerms?.Length != 2) throw new ArgumentException("When performing a 'Between' comparison 'SearchTerms' MUST contain exactly two items: " + (SearchTerms?.Length.ToString() ?? "[null]"));
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
                    if (SearchTerms?.Length == 0) throw new ArgumentException("When performing an 'In' comparison 'SearchTerms' MUST contain at least one item: " + (SearchTerms?.Length.ToString() ?? "[null]"));
                    break;
                default:
                    break;
            }

            switch (Comparison)
            {
                case Comparison.Equals:
                    return string.Equals(SearchTerm, stringToEvaluate, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                case Comparison.NotEquals:
                    return !string.Equals(SearchTerm, stringToEvaluate, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                case Comparison.Contains:
                    return IgnoreCase
                        ? stringToEvaluate.ToUpper().Contains(SearchTerm.ToUpper())
                        : stringToEvaluate.Contains(SearchTerm);
                case Comparison.NotContains:
                    return IgnoreCase
                        ? !stringToEvaluate.ToUpper().Contains(SearchTerm.ToUpper())
                        : !stringToEvaluate.Contains(SearchTerm);
                case Comparison.StartsWith:
                    return IgnoreCase
                        ? stringToEvaluate.ToUpper().StartsWith(SearchTerm.ToUpper())
                        : stringToEvaluate.StartsWith(SearchTerm);
                case Comparison.NotStartsWith:
                    return IgnoreCase
                        ? !stringToEvaluate.ToUpper().StartsWith(SearchTerm.ToUpper())
                        : !stringToEvaluate.StartsWith(SearchTerm);
                case Comparison.EndsWith:
                    return IgnoreCase
                        ? stringToEvaluate.ToUpper().EndsWith(SearchTerm.ToUpper())
                        : stringToEvaluate.EndsWith(SearchTerm);
                case Comparison.NotEndsWith:
                    return IgnoreCase
                        ? !stringToEvaluate.ToUpper().EndsWith(SearchTerm.ToUpper())
                        : !stringToEvaluate.EndsWith(SearchTerm);
                case Comparison.LessThan:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerm.ToUpper()) < 0
                        : string.Compare(stringToEvaluate, SearchTerm) < 0;
                case Comparison.LessThanOrEqual:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerm.ToUpper()) <= 0
                        : string.Compare(stringToEvaluate, SearchTerm) <= 0;
                case Comparison.GreaterThan:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerm.ToUpper()) > 0
                        : string.Compare(stringToEvaluate, SearchTerm) > 0;
                case Comparison.GreaterThanOrEqual:
                    return IgnoreCase
                        ? string.Compare(stringToEvaluate.ToUpper(), SearchTerm.ToUpper()) >= 0
                        : string.Compare(stringToEvaluate, SearchTerm) >= 0;
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
