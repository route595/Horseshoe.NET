namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// The group filter mode indicates whether to match one or all filter criteria ('or' versus 'and').
    /// </summary>
    public enum GroupFilterMode
    {
        /// <summary>
        /// Match at least one filter criteria to match the entire group filter.
        /// </summary>
        Or,

        /// <summary>
        /// Match all the filters' criteria to match the entire group filter.
        /// </summary>
        And
    }
}
