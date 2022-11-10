using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.IO.FileFilter
{
    public class DirectoryNameFilter : IDirectoryFilter
    {
        public Regex DirNameRegex { get; }

        public FilterMode FilterMode { get; }

        public bool CaseSensitive => (DirNameRegex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;

        public DirectoryNameFilter(Regex dirNameRegex, FilterMode filterMode = default)
        {
            DirNameRegex = dirNameRegex;
            FilterMode = filterMode;
        }

        public DirectoryNameFilter(string dirNamePattern, bool caseSensitive = false, FilterMode filterMode = default) : this(new[] { dirNamePattern }, caseSensitive: caseSensitive, filterMode: filterMode)
        {
        }

        public DirectoryNameFilter(IEnumerable<string> dirNamePatterns, bool caseSensitive = false, FilterMode filterMode = default)
        {
            if (dirNamePatterns == null)
                throw new ValidationException("Invalid filter: must supply one or more directory name patterns");
            if (dirNamePatterns.Any(e => string.IsNullOrWhiteSpace(e)))
                throw new ValidationException("Invalid filter: directory name patterns cannot be null or blank");
            switch (dirNamePatterns.Count())
            {
                case 0:
                    throw new ValidationException("Invalid filter: must supply one or more directory name patterns");
                case 1:
                    DirNameRegex = caseSensitive
                        ? new Regex("^" + dirNamePatterns.Single() + "$")
                        : new Regex("^" + dirNamePatterns.Single() + "$", RegexOptions.IgnoreCase);
                    break;
                default:
                    DirNameRegex = caseSensitive
                        ? new Regex("^(" + string.Join("|", dirNamePatterns.Select(s => "(" + s + ")")) + ")$")
                        : new Regex("^(" + string.Join("|", dirNamePatterns.Select(s => "(" + s + ")")) + ")$", RegexOptions.IgnoreCase);
                    break;
            }
            FilterMode = filterMode;
        }

        public bool IsMatch(DirectoryPath dir)
        {
            switch (FilterMode)
            {
                case FilterMode.FilterInAll:
                case FilterMode.FilterInAny:
                default:
                    return DirNameRegex.IsMatch(dir.Name);
                case FilterMode.FilterOutAll:
                case FilterMode.FilterOutAny:
                    return !DirNameRegex.IsMatch(dir.Name);
            }
        }            
    }
}
