using Horseshoe.NET.Comparison;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A <c>DirectoryFilter</c> implementation based on directory name
    /// </summary>
    public class DirectoryNameFilter : DirectoryFilter
    {
        /// <summary>
        /// Everything needed to perform a standard comparison 
        /// </summary>
        public ICriterinator<string> Criterinator { get; }

        /// <summary>
        /// Creates a new <c>DirectoryNameFilter</c>.
        /// </summary>
        /// <param name="criterinator">
        /// <para>
        /// Internal mechanism for performing the comparison search.
        /// </para>
        /// <para>
        /// Examples of <c>ICriterinator</c> usage in directory filter or search
        /// <code>
        /// var criterinator = Compare.Equals("Documents");
        ///                  = Compare.EqualsAny("bin", "obj");
        ///                  = Compare.ContainsIgnoreCase("Preferences");
        ///                  = Compare.EndsWith("_bak");
        ///                  = Compare.Between("aaa", "gzz");
        /// var filteredDirs = dirs.Where(d => criterinator.IsMatch(d.Name));
        /// </code>
        /// </para>
        /// <para>
        /// Note. While operating systems like Windows are not case-sensitive, others are.  So are <c>string</c>s in practically every programming
        /// language.  As such, Horseshoe.NET requires opt-in for case-insensitivity, i.e. using 'IgnoreCase' criterinators.
        /// </para>
        /// </param>
        /// <param name="filterMode">Optional, dictates which items to include based on criteria matching.</param>
        public DirectoryNameFilter(ICriterinator<string> criterinator, FilterMode filterMode = default)
        {
            Criterinator = criterinator;
            FilterMode = filterMode;
        }

        /// <summary>
        /// Indicates whether the supplied directory name constitutes a criteria match.
        /// </summary>
        /// <param name="dir">a directory path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(DirectoryPath dir)
        {
            return Criterinator.IsMatch(dir.Name);
        }
    }
}
