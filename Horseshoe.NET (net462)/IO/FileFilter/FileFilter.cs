using System.Linq;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Comparison;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// The class inherited by all the file filters in <c>Horseshoe.NET.IO.FileFilter</c>
    /// </summary>
    public abstract class FileFilter
    {
        /// <summary>
        /// Dictates which files to include based on criteria matching.
        /// </summary>
        public FilterMode FilterMode { get; protected set; }

        /// <summary>
        /// Indicates whether the supplied file constitutes a critea match.
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public abstract bool IsMatch(FilePath file);

        /// <summary>
        /// Indicates whether the supplied file path is included by this filter (e.g. if it matches the criteria and <c>FilterMode == Include</c>, etc.).
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool IsIncluded(FilePath file) 
        {
            switch (FilterMode)
            {
                case FilterMode.Include:
                default:
                    return IsMatch(file);
                case FilterMode.IncludeAllExcept:
                    return !IsMatch(file);
            }
        }

        /// <summary>
        /// Creates a directory filter "or" group with the supplied filters and <c>FileFilterMode = FileFilterMode.Include</c>.
        /// At least one filter must match in order for the group filter to match.
        /// </summary>
        /// <param name="filters">A param array list of one or more directory filters.</param>
        /// <returns>A group file filter</returns>
        public static FileFilter Or(params FileFilter[] filters)
        {
            return new FileFilterGroup(filters, GroupFilterMode.Or);  // note: FileFilterMode = Include (default)
        }

        /// <summary>
        /// Creates a file filter "and" group with the supplied filters and <c>FileFilterMode = FileFilterMode.Include</c>.  
        /// All filters must match in order for the group filter to match.
        /// </summary>
        /// <param name="filters">A param array list of one or more file filters.</param>
        /// <returns>A group file filter</returns>
        public static FileFilter And(params FileFilter[] filters)
        {
            return new FileFilterGroup(filters, GroupFilterMode.And);  // note: FileFilterMode = Include (default)
        }

        /// <summary>
        /// Creates a file name filter based on file extension search criteria.
        /// </summary>
        /// <param name="fileExtensions">e.g. <c>".rtf"</c>, <c>new[] { ".zip", ".tar.gz" }</c>, etc.</param>
        /// <param name="ignoreCase">
        /// <para>
        /// Set to <c>true</c> (recommended) to ignore the letter case of the file names being compared by this filter, default is <c>false</c>.
        /// </para>
        /// <para>
        /// While operating systems like Windows are not case-sensitive, others are.  So are <c>string</c>s in practically every programming
        /// language.  As such, Horseshoe.NET requires opt-in for case-insensitivity, i.e. setting this parameter to <c>true</c>.
        /// </para>
        /// </param>
        /// <param name="filterMode">Optional, dictates which items to include based on criteria matching.</param>
        /// <returns>A group file filter</returns>
        public static FileFilter CreateFileExtensionFilter(StringValues fileExtensions, FilterMode filterMode = default, bool ignoreCase = false)
        {
            return new FileNameFilter(fileExtensions.Select(ext => ".*" + ext.Replace(".", "\\.")), filterMode, ignoreCase: ignoreCase);
        }
    }
}
