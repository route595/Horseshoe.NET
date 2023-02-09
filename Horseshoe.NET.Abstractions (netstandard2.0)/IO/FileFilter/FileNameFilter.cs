using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A <c>FileFilter</c> implementation based on file name
    /// </summary>
    public class FileNameFilter : FileFilter
    {
        /// <summary>
        /// Everything needed to perform a standard comparison bundled into a single class.
        /// </summary>
        public IComparator<string> Comparator { get; }

        /// <summary>
        /// Creates a new <c>FileNameFilter</c>.
        /// </summary>
        /// <param name="mode">Specifies how file names should match the search value(s) to be included in the results.</param>
        /// <param name="fileNameCriteria">
        /// <para>
        /// File [partial] name(s) upon which to perform the comparison search.
        /// </para>
        /// <para>
        /// Examples of search values (see quotes):
        /// <code>
        /// filter = new FileNameFilter(TextMatch.Equals, "bill calculator.xls");
        /// filter = new FileNameFilter(TextMatch.EndsWith, ".bak");
        /// filter = new FileNameFilter(TextMatch.In, new[] { "readme.md", "readme.txt" });
        /// filter = new FileNameFilter(TextMatch.Between, new[] { "a", "gzz" });
        /// </code>
        /// </para>
        /// </param>
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
        public FileNameFilter(CompareMode mode, StringValues fileNameCriteria, bool ignoreCase = false, FilterMode filterMode = default)
        {
            // validation
            switch (mode)
            {
                case CompareMode.IsNull:
                case CompareMode.IsNullOrWhitespace:
                    throw new ValidationException("This compare mode is not compatible with this filter: " + mode);
            }

            Comparator = new Comparator<string> { Mode = mode, Criteria = ObjectValues.FromStringValues(fileNameCriteria), IgnoreCase = ignoreCase };
            FilterMode = filterMode;
        }

        /// <summary>
        /// Indicates whether the supplied file name constitutes a criteria match.
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(FilePath file)
        {
            return Comparator.IsMatch(file.Name);
        }
    }
}
