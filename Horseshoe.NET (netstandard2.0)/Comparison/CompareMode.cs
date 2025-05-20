namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Specifies how an item or set of items should match a criterium (a.k.a. search text).
    /// </summary>
    public enum CompareMode
    {
        /// <summary>
        /// Tests if an item equals the criterium (criteria count != 1 -> err).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        Equals,

        /// <summary>
        /// Tests if an item equals at least one criterium from a set.
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        EqualsAny,

        /// <summary>
        /// Tests if an item does not equal the criterium (criteria count != 1 -> err).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        NotEquals,

        /// <summary>
        /// Tests if an item does not equals any criterium from a set.
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        NotEqualsAny,

        /// <summary>
        /// Tests if a <c>string</c> contains the criterium (criteria count != 1 -> err).
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        Contains,

        /// <summary>
        /// Tests if a <c>string</c> contains at least one criterium from a set.
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        ContainsAny,

        /// <summary>
        /// Tests if a <c>string</c> contains all criteria from a set (not commonly used).
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        ContainsAll,

        /// <summary>
        /// Tests if a <c>string</c> starts with the criterium.
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Tests if a <c>string</c> starts with at least one criterium from a set.
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        StartsWithAny,

        /// <summary>
        /// Tests if a <c>string</c> ends with the criterium.
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        EndsWith,

        /// <summary>
        /// Tests if a <c>string</c> ends with at least one criterium from a set.
        /// Case-sensitivity depends on the implementation.
        /// </summary>
        EndsWithAny,

        /// <summary>
        /// Tests if an item is greater than a criterium (or the first of multiple criteria).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Tests if an item is greater than or equal to a criterium (or the first of multiple criteria).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// Alternate implementation: <c>GreaterThan</c> or <c>Equals</c>.
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        /// Tests if an item is less than a criterium (or the first of multiple criteria).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        LessThan,

        /// <summary>
        /// Tests if an item is less than or equal to a criterium (or the first of multiple criteria).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// Alternate implementation: <c>LessThan</c> or <c>Equals</c>.
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        /// Tests if an item is between 2 criteria (or equal to one).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// Alternate implementation: <c>GreaterThan</c> lo and <c>LessThan</c> hi or <c>Equals</c> lo/hi.
        /// </summary>
        Between,

        /// <summary>
        /// Tests if an item is between 2 criteria (but not equal).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// Alternate implementation: <c>GreaterThan</c> lo and <c>LessThan</c> hi.
        /// </summary>
        BetweenExclusive,

        /// <summary>
        /// Tests if an item is outside of 2 criteria (equal to neither).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        NotBetween,

        /// <summary>
        /// Tests if an item is outside of 2 criteria (or equal to one).
        /// Case-sensitivity in <c>string</c>s depends on the implementation.
        /// </summary>
        NotBetweenInclusive,

        /// <summary>
        /// Tests if an item is <c>null</c>.
        /// Note, a <c>struct</c> cannot have a <c>null</c> value, although <see cref="System.Nullable{T}"/> can.
        /// </summary>
        IsNull,

        /// <summary>
        /// Tests if a <c>string</c> is either <c>null</c> or "".
        /// </summary>
        IsNullOrEmpty,

        /// <summary>
        /// Tests if a <c>string</c> either is <c>null</c> or contains only whitespaces.
        /// </summary>
        IsNullOrWhitespace,

        /// <summary>
        /// Tests if a <c>string</c> is a match for a single regular expression criterium.
        /// </summary>
        Regex,

        /// <summary>
        /// Tests if a <c>string</c> is a match for at least one regular expression criterium in a set.
        /// </summary>
        RegexAny,

        /// <summary>
        /// Tests if a <c>string</c> is a match for all regular expression criteria in a set.
        /// </summary>
        RegexAll,

        /// <summary>
        /// Performs no test, rather groups criterinators and tests them as a single unit.  E.g. <c>And</c> or <c>Or</c>.
        /// </summary>
        Group
    }
}
