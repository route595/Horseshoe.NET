namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Specifies how items should match the text search value to be included in the results.
    /// </summary>
    public enum TextMatch
    {
        /// <summary>
        /// The item must match the text search value in content and length
        /// </summary>
        Equals, 

        /// <summary>
        /// The item must match the text search value or contain it
        /// </summary>
        Contains,

        /// <summary>
        /// The item must match the text search value or start with it
        /// </summary>
        StartsWith,

        /// <summary>
        /// The item must match the text search value or end with it
        /// </summary>
        EndsWith,

        /// <summary>
        /// The item must alphabetically follow the text search value
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The item must equal or alphabetically follow the text search value
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        /// The item must alphabetically precede the text search value
        /// </summary>
        LessThan,

        /// <summary>
        /// The item must equal or alphabetically precede the text search value
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        /// The item must equal or alphabetically follow the first text search value or equal or alphabetically precede the second text search value
        /// </summary>
        Between,

        /// <summary>
        /// The item must alphabetically follow the first text search value or alphabetically precede the second text search value
        /// </summary>
        BetweenExclusive,

        /// <summary>
        /// The item must equal one of the search values
        /// </summary>
        In,

        /// <summary>
        /// The item must be null
        /// </summary>
        IsNull,

        /// <summary>
        /// The item must be null or contain only whitespaces
        /// </summary>
        IsNullOrWhitespace,

        /// <summary>
        /// The item must match the search value regular expression
        /// </summary>
        Regex
    }
}
