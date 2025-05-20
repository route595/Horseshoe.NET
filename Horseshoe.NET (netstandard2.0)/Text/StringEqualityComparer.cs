using System.Collections.Generic;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A simple equality comparer for strings that uses the default string comparison.
    /// </summary>
    public class StringEqualityComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Compares two strings for equality using the default string comparison.
        /// </summary>
        /// <param name="x">A <c>string</c></param>
        /// <param name="y">Another <c>string</c></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public bool Equals(string x, string y)
        {
            return string.Equals(x, y);
        }

        /// <summary>
        /// Returns the hash code for a string using the default string comparison.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetHashCode(string str)
        {
            return str.GetHashCode();
        }


        /// <summary>
        /// A singleton instance of the <c>StringEqualityComparer</c> class.
        /// </summary>
        public static IEqualityComparer<string> Default { get; } = new StringEqualityComparer();
    }
}
