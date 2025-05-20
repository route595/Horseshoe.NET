namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Literal, Contains, StartsWith or EndsWith
    /// </summary>
    public enum LikeMode
    {
        /// <summary>
        /// As supplied, no interpretation
        /// </summary>
        Literal,

        /// <summary>
        /// Interpreted as <c>&lt;expression&gt; LIKE '%&lt;criterium&gt;%'</c>
        /// </summary>
        Contains,

        /// <summary>
        /// Interpreted as <c>&lt;expression&gt; LIKE '&lt;criterium&gt;%'</c>
        /// </summary>
        StartsWith,

        /// <summary>
        /// Interpreted as <c>&lt;expression&gt; LIKE '%&lt;criterium&gt;'</c>
        /// </summary>
        EndsWith
    }
}
