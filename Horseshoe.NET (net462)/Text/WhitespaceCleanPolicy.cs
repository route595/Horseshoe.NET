namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Indicates how to handle non-printable <c>char</c>s when converting to ASCII.
    /// </summary>
    public enum WhitespaceCleanPolicy
    {
        /// <summary>
        /// Replace Unicode spaces (e.g. non-breaking spaces)
        /// </summary>
        Basic,

        /// <summary>
        /// Remove all whitespaces from the result
        /// </summary>
        Drop,

        /// <summary>
        /// Replace any whitespaces with a space
        /// </summary>
        Replace,

        /// <summary>
        /// Replace any whitespaces with a space and combine multiple spaces into one
        /// </summary>
        Combine,

        /// <summary>
        /// Replace with <c>CharInfo.ToString()</c>
        /// </summary>
        Reveal
    }
}
