using System.Collections.Generic;

using Horseshoe.NET.Comparison;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A <c>FileFilter</c> implementation based on file name
    /// </summary>
    public class FileNameFilter : FileFilter
    {
        /// <summary>
        /// Everything needed to perform a standard comparison
        /// </summary>
        public ICriterinator<string> Criterinator { get; }

        /// <summary>
        /// Creates a search filter with 'ContainsAny' criterinator
        /// </summary>
        /// <param name="namesOrPartialsToInclude">File names (or partial names) to include in the search results</param>
        public FileNameFilter(params string[] namesOrPartialsToInclude) : this(namesOrPartialsToInclude as IEnumerable<string>, default)
        {
        }

        /// <summary>
        /// Creates a search filter with 'ContainsAny' (or 'ContainsAnyIgnoreCase') criterinator
        /// </summary>
        /// <param name="namesOrPartialsToInclude">File names (or partial names) to include in the search results</param>
        /// <param name="filterMode">i.e. 'Include' or 'IncludeAllExcept'</param>
        /// <param name="ignoreCase">if <c>true</c>, creates a criterinator that is not case-sensitive.  Default is <c>false</c>.</param>
        public FileNameFilter(IEnumerable<string> namesOrPartialsToInclude, FilterMode filterMode = default, bool ignoreCase = false) : this(ignoreCase? Comparison.Criterinator.ContainsAnyIgnoreCase(namesOrPartialsToInclude) : Comparison.Criterinator.ContainsAny(namesOrPartialsToInclude), filterMode)
        {
        }

        /// <summary>
        /// Creates a new <c>FileNameFilter</c>.
        /// </summary>
        /// <param name="criterinator">
        /// <para>
        /// Internal mechanism for performing the comparison search.
        /// </para>
        /// <para>
        /// Examples of <c>ICriterinator</c> usage in file filter or search
        /// <code>
        /// var criterinator = Compare.Equals("bill calculator.xls");
        ///                  = Compare.EndsWithIgnoreCase(".bak");
        ///                  = Compare.EqualsAnyIgnoreCase("readme.md", "readme.txt");
        /// var filteredFiles = files.Where(f => criterinator.IsMatch(f.Name));
        /// </code>
        /// </para>
        /// <para>
        /// Note. While operating systems like Windows are not case-sensitive, others are.  So are <c>string</c>s in practically every programming
        /// language.  As such, Horseshoe.NET requires opt-in for case-insensitivity, i.e. using 'IgnoreCase' criterinators.
        /// </para>
        /// </param>
        /// <param name="filterMode">Optional, dictates which items to include based on criteria matching.</param>
        public FileNameFilter(ICriterinator<string> criterinator, FilterMode filterMode = default)
        {
            Criterinator = criterinator;
            FilterMode = filterMode;
        }

        /// <summary>
        /// Indicates whether the supplied file name constitutes a criteria match.
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(FilePath file)
        {
            return Criterinator.IsMatch(file.Name);
        }
    }
}
