using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.IO.FileFilter
{
    public class FileFilterGroup : IFileFilter
    {
        private List<IFileFilter> _fileFilters = new List<IFileFilter>();

        public FilterMode FilterMode { get; }

        public bool CaseSensitive { get; }

        public FileFilterGroup(IEnumerable<IFileFilter> fileFilters, FilterMode filterMode = default, bool caseSensitive = false)
        {
            if (fileFilters == null || !fileFilters.Any())
            {
                switch (filterMode)
                {
                    case FilterMode.FilterInAll:
                    case FilterMode.FilterOutAll:
                    default:
                        throw new ValidationException("Invalid \"and\" group filter: must supply one or more file filters");
                    case FilterMode.FilterInAny:
                    case FilterMode.FilterOutAny:
                        throw new ValidationException("Invalid \"or\" group filter: must supply one or more file filters");
                }
            }
            _fileFilters.AddRange(fileFilters);
            FilterMode = filterMode;
            CaseSensitive = caseSensitive;
        }

        public bool IsMatch(FilePath file)
        {
            switch (FilterMode)
            {
                case FilterMode.FilterInAll:
                default:
                    return _fileFilters.All(f => f.IsMatch(file));
                case FilterMode.FilterInAny:
                    return _fileFilters.Any(f => f.IsMatch(file));
                case FilterMode.FilterOutAll:
                    return !_fileFilters.All(f => f.IsMatch(file));
                case FilterMode.FilterOutAny:
                    return !_fileFilters.Any(f => f.IsMatch(file));
            }
        }
    }
}
