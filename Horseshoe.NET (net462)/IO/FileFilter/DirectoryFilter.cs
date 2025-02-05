namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// The class inherited by all the directory filters in <c>Horseshoe.NET.IO.FileFilter</c>
    /// </summary>
    public abstract class DirectoryFilter
    {
        /// <summary>
        /// Dictates which directories to include based on criteria matching.
        /// </summary>
        public FilterMode FilterMode { get; protected set; }

        /// <summary>
        /// Indicates whether the supplied directory constitutes a critea match.
        /// </summary>
        /// <param name="dir">a directory path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public abstract bool IsMatch(DirectoryPath dir);

        /// <summary>
        /// Indicates whether the supplied directory path is included by this filter (e.g. if it matches the criteria and <c>FilterMode == Include</c>, etc.).
        /// </summary>
        /// <param name="dir">a directory path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool IsIncluded(DirectoryPath dir) 
        {
            switch (FilterMode)
            {
                case FilterMode.Include:
                default:
                    return IsMatch(dir);
                case FilterMode.IncludeAllExcept:
                    return !IsMatch(dir);
            }
        }

        /// <summary>
        /// Creates a directory filter "or" group with the supplied filters and <c>FileFilterMode = FileFilterMode.Include</c>.
        /// At least one filter must match in order for the group filter to match.
        /// </summary>
        /// <param name="filters">A param array list of one or more directory filters.</param>
        /// <returns></returns>
        public static DirectoryFilter Or(params DirectoryFilter[] filters)
        {
            return new DirectoryFilterGroup(filters, GroupFilterMode.Or);  // note: FileFilterMode = Include (default)
        }

        /// <summary>
        /// Creates a directory filter "and" group with the supplied filters and <c>FileFilterMode = FileFilterMode.Include</c>.  
        /// All filters must match in order for the group filter to match.
        /// </summary>
        /// <param name="filters">A param array list of one or more directory filters.</param>
        /// <returns></returns>
        public static DirectoryFilter And(params DirectoryFilter[] filters)
        {
            return new DirectoryFilterGroup(filters, GroupFilterMode.And);  // note: FileFilterMode = Include (default)
        }
    }
}
