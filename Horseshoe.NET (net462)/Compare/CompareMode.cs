namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Specifies how input item should compare with criteria value(s) to be included in the results.
    /// </summary>
    public enum CompareMode
    {
        /// <summary>
        /// The input item must equal the criterium (case-sensitivity can be part of the implementation).
        /// </summary>
        Equals,

        /// <summary>
        /// The text item must contain the criterium (case-sensitivity can be part of the implementation).
        /// </summary>
        Contains,

        /// <summary>
        /// The text item must start with the criterium (case-sensitivity can be part of the implementation).
        /// </summary>
        StartsWith,

        /// <summary>
        /// The text item must end with the criterium (case-sensitivity can be part of the implementation).
        /// </summary>
        EndsWith,

        /// <summary>
        /// The input item must follow the criterium using natural ordering (case-sensitivity can be part of the implementation).
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The input item must follow the criterium using natural ordering or be equal to it (case-sensitivity can be part of the implementation).
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        /// The input item must precede the criterium using natural ordering (case-sensitivity can be part of the implementation).
        /// </summary>
        LessThan,

        /// <summary>
        /// The input item must precede the criterium using natural ordering or be equal to it (case-sensitivity can be part of the implementation).
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        /// The input item must be between both criteria using natural ordering or equal either (case-sensitivity can be part of the implementation).
        /// </summary>
        Between,

        /// <summary>
        /// The input item must be between both criteria using natural ordering but equal to beither (case-sensitivity can be part of the implementation).
        /// </summary>
        BetweenExclusive,

        /// <summary>
        /// The input item must equal one of the criteria (case-sensitivity can be part of the implementation).
        /// </summary>
        In,

        /// <summary>
        /// The input item must be null.
        /// </summary>
        IsNull,

        /// <summary>
        /// The text item must be null, blank or contain only whitespaces.
        /// </summary>
        IsNullOrWhitespace,

        /// <summary>
        /// The text item must match the criterium regular expression.
        /// </summary>
        Regex
    }
}
