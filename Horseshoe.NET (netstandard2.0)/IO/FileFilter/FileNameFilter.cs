using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.IO.FileFilter
{
    public class FileNameFilter : IFileFilter
    {
        public Regex FileNameRegex { get; }

        public FilterMode FilterMode { get; }

        public bool CaseSensitive => (FileNameRegex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;

        public FileNameFilter(Regex fileNameRegex, FilterMode filterMode = default)
        {
            FileNameRegex = fileNameRegex;
            FilterMode = filterMode;
        }

        public FileNameFilter(string fileNamePattern, FilterMode filterMode = default, bool caseSensitive = false) : this(new[] { fileNamePattern }, filterMode: filterMode, caseSensitive: caseSensitive)
        {
        }

        public FileNameFilter(IEnumerable<string> fileNamePatterns, FilterMode filterMode = default, bool caseSensitive = false)
        {
            if (fileNamePatterns == null)
                throw new ValidationException("Invalid filter: must supply one or more file name patterns");
            if (fileNamePatterns.Any(e => string.IsNullOrWhiteSpace(e)))
                throw new ValidationException("Invalid filter: file name patterns cannot be null or blank");
            switch (fileNamePatterns.Count())
            {
                case 0:
                    throw new ValidationException("Invalid filter: must supply one or more file name patterns");
                case 1:
                    FileNameRegex = caseSensitive
                        ? new Regex("^" + fileNamePatterns.Single() + "$")
                        : new Regex("^" + fileNamePatterns.Single() + "$", RegexOptions.IgnoreCase);
                    break;
                default:
                    FileNameRegex = caseSensitive
                        ? new Regex("^(" + string.Join("|", fileNamePatterns.Select(s => "(" + s + ")")) + ")$")
                        : new Regex("^(" + string.Join("|", fileNamePatterns.Select(s => "(" + s + ")")) + ")$", RegexOptions.IgnoreCase);
                    break;
            }
            FilterMode = filterMode;
        }

        public bool IsMatch(FilePath file)
        {
            switch (FilterMode)
            {
                case FilterMode.FilterInAll:
                case FilterMode.FilterInAny:
                default:
                    return FileNameRegex.IsMatch(file.Name);
                case FilterMode.FilterOutAll:
                case FilterMode.FilterOutAny:
                    return !FileNameRegex.IsMatch(file.Name);
            }
        }
    }
}
