using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A specialized filter that checks a file against a list of file filters.  Runs either in <c>And</c> or <c>Or</c> mode.  
    /// </summary>
    public class FileFilterGroup : FileFilter
    {
        private List<FileFilter> _fileFilters = new List<FileFilter>();

        /// <summary>
        /// <c>And</c> or <c>Or</c>
        /// </summary>
        public GroupFilterMode? Mode { get; }

        /// <summary>
        /// Creates a new file filter group.
        /// </summary>
        /// <param name="fileFilters">A collection of one or more file filters.</param>
        /// <param name="groupFilterMode"><c>And</c> or <c>Or</c></param>
        /// <param name="filterMode">Optional, dictates which files to include based on criteria matching.</param>
        /// <exception cref="ValidationException">If no file filters or no group filter mode was supplied.</exception>
        public FileFilterGroup(IEnumerable<FileFilter> fileFilters, GroupFilterMode? groupFilterMode, FilterMode filterMode = default)
        {
            // validation
            if (fileFilters == null || !fileFilters.Any())
                throw new ValidationException("Invalid filter: must supply one or more file filters");
            if (!groupFilterMode.HasValue)
                throw new ValidationException("Invalid filter: must supply a value for groupFilterMode");
          
            _fileFilters.AddRange(fileFilters);
            FilterMode = filterMode;
            Mode = groupFilterMode.Value;
        }

        /// <summary>
        /// Indicates whether the supplied file constitutes a critea match.
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(FilePath file)
        {
            switch (Mode)
            {
                case GroupFilterMode.Or:
                default:
                    foreach (var fileFilter in _fileFilters)
                    {
                        if (fileFilter.IsMatch(file))
                            return true;
                    }
                    return false;
                case GroupFilterMode.And:
                    foreach (var fileFilter in _fileFilters)
                    {
                        if (!fileFilter.IsMatch(file))
                            return false;
                    }
                    return true;
            }
        }
    }
}
