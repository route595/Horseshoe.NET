using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A simple equality comparer for strings that uses a case-insensitive string comparison.
    /// </summary>
    public class CaseInsensitiveStringEqualityComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Compares two strings for equality using a case-insensitive string comparison.
        /// </summary>
        /// <param name="x">A <c>string</c></param>
        /// <param name="y">Another <c>string</c></param>
        /// <returns><c>true</c> if equal (regardless of case), otherwise <c>false</c></returns>
        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the hash code for a string using a case-insensitive string comparison.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetHashCode(string str)
        {
            return str.ToLowerInvariant().GetHashCode();
        }

        /// <summary>
        /// A singleton instance of the <c>CaseInsensitiveStringEqualityComparer</c> class.
        /// </summary>
        public static IEqualityComparer<string> Default { get; } = new CaseInsensitiveStringEqualityComparer();
    }
}
