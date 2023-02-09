using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A <c>DirectoryFilter</c> implementation based on directory name
    /// </summary>
    public class DirectoryNameFilter : DirectoryFilter
    {
        /// <summary>
        /// Everything needed to perform a standard comparison bundled into a single class.
        /// </summary>
        public IComparator<string> Comparator { get; }

        /// <summary>
        /// Creates a new <c>DirectoryNameFilter</c>.
        /// </summary>
        /// <param name="mode">Specifies how directory names should match the search value(s) to be included in the results.</param>
        /// <param name="directoryNameCriteria">
        /// <para>
        /// Directory [partial] name(s) upon which to perform the comparison search.
        /// </para>
        /// <para>
        /// Examples of search values (see quotes):
        /// <code>
        /// filter = new DirectoryNameFilter(CompareMode.Equals, "Documents");
        /// filter = new DirectoryNameFilter(CompareMode.EndsWith, "_bak");
        /// filter = new DirectoryNameFilter(CompareMode.In, new[] { "bin", "obj" });
        /// filter = new DirectoryNameFilter(CompareMode.Between, new[] { "a", "gzz" });
        /// </code>
        /// </para>
        /// </param>
        /// <param name="ignoreCase">
        /// <para>
        /// Set to <c>true</c> (recommended) to ignore the letter case of the directory names being compared by this filter, default is <c>false</c>.
        /// </para>
        /// <para>
        /// While operating systems like Windows are not case-sensitive, others are.  So are <c>string</c>s in practically every programming
        /// language.  As such, Horseshoe.NET requires opt-in for case-insensitivity, i.e. setting this parameter to <c>true</c>.
        /// </para>
        /// </param>
        /// <param name="filterMode">Optional, dictates which items to include based on criteria matching.</param>
        public DirectoryNameFilter(CompareMode mode, StringValues directoryNameCriteria, bool ignoreCase = false, FilterMode filterMode = default)
        {
            // validation
            switch (mode)
            {
                case CompareMode.IsNull:
                case CompareMode.IsNullOrWhitespace:
                    throw new ValidationException("This compare mode is not compatible with this filter: " + mode);
            }

            Comparator = new Comparator<string> { Mode = mode, Criteria = ObjectValues.FromStringValues(directoryNameCriteria), IgnoreCase = ignoreCase };
            FilterMode = filterMode;
        }

        /// <summary>
        /// Indicates whether the supplied directory name constitutes a criteria match.
        /// </summary>
        /// <param name="dir">a directory path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(DirectoryPath dir)
        {
            return Comparator.IsMatch(dir.Name);
        }
    }
}
