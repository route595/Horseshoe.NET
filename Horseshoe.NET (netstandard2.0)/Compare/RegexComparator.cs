using System.Text.RegularExpressions;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Everything needed to perform a standard regular expression comparison bundled into a single class.
    /// </summary>
    public class RegexComparator : IComparator<string>
    {
        /// <summary>
        /// The compare mode, e.g. Equals, Contains, Between, etc.
        /// </summary>
        public CompareMode Mode => CompareMode.Regex;

        /// <summary>
        /// The resultant regular expression.
        /// </summary>
        public Regex Regex { get; }

        /// <summary>
        /// Creates a new <c>RegexComparator</c>;
        /// </summary>
        /// <param name="expression">A regular expression.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the regular expression, default is <c>false</c>.</param>
        public RegexComparator(string expression, bool ignoreCase = false)
        {
            Regex = ignoreCase
                ? new Regex(expression, RegexOptions.IgnoreCase)
                : new Regex(expression);
        }

        /// <summary>
        /// Indicates whether the input item is a criteria match.
        /// </summary>
        /// <param name="input"></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool IsMatch(string input)
        {
            return Regex.IsMatch(input);
        }
    }
}
