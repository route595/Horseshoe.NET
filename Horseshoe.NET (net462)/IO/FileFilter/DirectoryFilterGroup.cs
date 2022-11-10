using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.IO.FileFilter
{
    public class DirectoryFilterGroup : IDirectoryFilter
    {
        private List<IDirectoryFilter> _dirFilters = new List<IDirectoryFilter>();

        public FilterMode FilterMode { get; }

        public bool CaseSensitive { get; }

        public DirectoryFilterGroup(IEnumerable<IDirectoryFilter> dirFilters, FilterMode filterMode = default, bool caseSensitive = false)
        {
            if (dirFilters == null || !dirFilters.Any())
            {
                switch (filterMode)
                {
                    case FilterMode.FilterInAll:
                    case FilterMode.FilterOutAll:
                    default:
                        throw new ValidationException("Invalid \"and\" group filter: must supply one or more directory filters");
                    case FilterMode.FilterInAny:
                    case FilterMode.FilterOutAny:
                        throw new ValidationException("Invalid \"or\" group filter: must supply one or more directory filters");
                }
            }
            _dirFilters.AddRange(dirFilters);
            FilterMode = filterMode;
            CaseSensitive = caseSensitive;
        }

        public bool IsMatch(DirectoryPath dir)
        {
            switch (FilterMode)
            {
                case FilterMode.FilterInAll:
                default:
                    return _dirFilters.All(f => f.IsMatch(dir));
                case FilterMode.FilterInAny:
                    return _dirFilters.Any(f => f.IsMatch(dir));
                case FilterMode.FilterOutAll:
                    return !_dirFilters.All(f => f.IsMatch(dir));
                case FilterMode.FilterOutAny:
                    return !_dirFilters.Any(f => f.IsMatch(dir));
            }
        }
    }
}
