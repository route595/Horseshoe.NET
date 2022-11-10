namespace Horseshoe.NET.IO.FileFilter
{
    public enum FilterMode
    {
        /// <summary>
        /// Matches item if all criteria is met
        /// </summary>
        FilterInAll,

        /// <summary>
        /// Matches item if any criteria is met
        /// </summary>
        FilterInAny,

        /// <summary>
        /// Matches item if not all criteria is met
        /// </summary>
        FilterOutAll,

        /// <summary>
        /// Matches item if not any criteria is met
        /// </summary>
        FilterOutAny
    }
}
