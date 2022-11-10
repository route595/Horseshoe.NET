namespace Horseshoe.NET.Text
{
    public enum Comparison
    {
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        NotStartsWith,
        EndsWith,
        NotEndsWith,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Between,
        NotBetween,  // exclusive by nature
        BetweenExclusive,
        NotBetweenInclusive,
        BetweenExclusiveLowerBound,
        NotBetweenExclusiveLowerBound,
        BetweenExclusiveUpperBound,
        NotBetweenExclusiveUpperBound,
        In,
        NotIn,
        IsNull,
        IsNotNull
    }
}
