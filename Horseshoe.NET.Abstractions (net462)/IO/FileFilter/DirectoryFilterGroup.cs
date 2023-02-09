using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A specialized filter that checks a directory against a list of directory filters.  Runs either in <c>And</c> or <c>Or</c> mode.  
    /// </summary>
    public class DirectoryFilterGroup : DirectoryFilter
    {
        private List<DirectoryFilter> _dirFilters = new List<DirectoryFilter>();

        /// <summary>
        /// <c>And</c> or <c>Or</c>
        /// </summary>
        public GroupFilterMode? Mode { get; }

        /// <summary>
        /// Creates a new directory filter group.
        /// </summary>
        /// <param name="dirFilters">A collection of one or more directory filters.</param>
        /// <param name="groupFilterMode"><c>And</c> or <c>Or</c></param>
        /// <param name="filterMode">Optional, dictates which directories to include based on criteria matching.</param>
        /// <exception cref="ValidationException">If no directory filters or no group filter mode was supplied.</exception>
        public DirectoryFilterGroup(IEnumerable<DirectoryFilter> dirFilters, GroupFilterMode? groupFilterMode, FilterMode filterMode = default)
        {
            // validation
            if (dirFilters == null || !dirFilters.Any())
                throw new ValidationException("Invalid filter: must supply one or more directory filters");
            if (!groupFilterMode.HasValue)
                throw new ValidationException("Invalid filter: must supply a value for groupFilterMode");

            _dirFilters.AddRange(dirFilters);
            FilterMode = filterMode;
            Mode = groupFilterMode.Value;
        }

        /// <summary>
        /// Indicates whether the supplied directory constitutes a critea match.
        /// </summary>
        /// <param name="dir">a directory path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(DirectoryPath dir)
        {
            switch (Mode)
            {
                case GroupFilterMode.Or:
                default:
                    foreach (var dirFilter in _dirFilters)
                    {
                        if (dirFilter.IsMatch(dir))
                            return true;
                    }
                    return false;
                case GroupFilterMode.And:
                    foreach (var dirFilter in _dirFilters)
                    {
                        if (!dirFilter.IsMatch(dir))
                            return false;
                    }
                    return true;
            }
        }
    }
}
