namespace Horseshoe.NET.Text.TextClean
{
    /// <summary>
    /// Indicates how to handle non-printable <c>char</c>s when converting to ASCII.
    /// </summary>
    public enum NonprintablesPolicy
    {
        /// <summary>
        /// Remove from the result
        /// </summary>
        Drop,

        /// <summary>
        /// Sunstitute with an ASCII char (e.g. '?')
        /// </summary>
        Substitute
    }
}
