namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// The filter mode dictates which items to include based on criteria matching.
    /// </summary>
    public enum FilterMode
    {
        /// <summary>
        /// Items match if criteria is met.
        /// </summary>
        Include,

        /// <summary>
        /// Exclude items that match the criteria, include all othere.
        /// </summary>
        IncludeAllExcept
    }
}
