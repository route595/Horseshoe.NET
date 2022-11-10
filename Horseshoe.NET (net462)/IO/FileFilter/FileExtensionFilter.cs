using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileFilter
{
    public class FileExtensionFilter : FileNameFilter
    {
        public FileExtensionFilter(string fileExtension, FilterMode filterMode = default, bool caseSensitive = false) : this(new[] { fileExtension }, filterMode: filterMode, caseSensitive: caseSensitive)
        {
        }

        public FileExtensionFilter(IEnumerable<string> fileExtensions, FilterMode filterMode = default, bool caseSensitive = false) : base(BuildRegex(fileExtensions, caseSensitive), filterMode: filterMode)
        {
        }

        private static Regex BuildRegex(IEnumerable<string> fileExtensions, bool caseSensitive)
        {
            if (fileExtensions == null)
                throw new ValidationException("Invalid filter: must supply one or more file extensions");
            if (fileExtensions.Any(e => string.IsNullOrWhiteSpace(e)))
                throw new ValidationException("Invalid filter: file extensions cannot be null or blank");
            switch (fileExtensions.Count())
            {
                case 0:
                    throw new ValidationException("Invalid filter: must supply one or more file extensions");
                case 1:
                    return caseSensitive
                        ? new Regex(@"^.*\." + fileExtensions.Single() + "$")
                        : new Regex(@"^.*\." + fileExtensions.Single() + "$", RegexOptions.IgnoreCase);
                default:
                    return caseSensitive
                        ? new Regex(@"^.*\.(" + string.Join("|", fileExtensions.Select(s => "(" + TextUtil.TrimLeading(s, '*', '.') + ")")) + ")$")
                        : new Regex(@"^.*\.(" + string.Join("|", fileExtensions.Select(s => "(" + TextUtil.TrimLeading(s, '*', '.') + ")")) + ")$", RegexOptions.IgnoreCase);
            }
        }
    }
}
