namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A nonexhaustive collection of constant <c>string</c> truncation indicators.
    /// </summary>
    public static class TruncateMarker
    {
        /// <summary>
        /// No indicator (equivalent to <c>string.Empty</c>).
        /// </summary>
        public const string None = "";

        /// <summary>
        /// A triple dot.
        /// </summary>
        public const string Ellipsis = "…";

        /// <summary>
        /// A sequence of three dots.
        /// </summary>
        public const string LongEllipsis = "...";
    }
}
