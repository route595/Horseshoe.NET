namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Which 
    /// </summary>
    public enum ListSelectionMode
    {
        /// <summary>
        /// Single result, optional.  Default value.
        /// </summary>
        ZeroOrOne,

        /// <summary>
        /// Single result, input required.
        /// </summary>
        ExactlyOne,

        /// <summary>
        /// Multiple results, optional.  Input shorthand is enabled e.g. "none", "1-15,19", "all", "all except 16-18"
        /// </summary>
        ZeroOrMore,

        /// <summary>
        /// Multiple results, input required.  Input shorthand is enabled e.g. "none", "1-15,19", "all", "all except 16-18"
        /// </summary>
        OneOrMore
    }
}
