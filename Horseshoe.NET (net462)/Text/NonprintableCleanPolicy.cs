namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Indicates how to handle non-printable <c>char</c>s when converting to ASCII.
    /// </summary>
    public enum NonprintableCleanPolicy
    {
        /// <summary>
        /// Remove from the result
        /// </summary>
        Drop,

        /// <summary>
        /// Substitute with <c>CharInfo.ToString()</c>
        /// </summary>
        Reveal
    }
}
