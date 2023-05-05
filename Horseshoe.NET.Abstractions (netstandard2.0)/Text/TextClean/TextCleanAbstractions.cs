using System;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.Text.TextClean
{
    /// <summary>
    /// A set of factory methods for removing or replacing non-ASCII characters in string.
    /// </summary>
    public static class TextCleanAbstractions
    {
        /// <summary>
        /// Returns a copy of the supplied <c>string</c> without the indicated <c>chars</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A set of <c>char</c>s to remove from <c>text</c>.</param>
        /// <returns>A copy of the supplied <c>string</c> without the indicated <c>chars</c>.</returns>
        public static string Remove(string text, params char[] chars)
        {
            if (text == null)
                return string.Empty;
            if (chars == null || !chars.Any())
                return text;
            var strb = new StringBuilder(text.Length);
            foreach (var c in text.AsSpan())
            {
                if (!c.In(chars))
                {
                    strb.Append(c);
                }
            }
            return strb.ToString();
        }

        /// <summary>
        /// Returns a copy of the supplied <c>string</c> without whitespaces (including new lines).
        /// </summary>
        /// <param name="text">A <c>string</c>.</param>
        /// <returns>A copy of <c>text</c> without whitespaces (including new lines).</returns>
        public static string RemoveAllWhitespace(string text)
        {
            return Remove(text, ' ', '\t', '\r', '\n', '\u00A0');
        }

        /// <summary>
        /// Returns a copy of the supplied <c>string</c> without new lines.
        /// </summary>
        /// <param name="text">A <c>string</c>.</param>
        /// <returns>A copy of the supplied <c>string</c> without new lines.</returns>
        public static string RemoveNewlines(string text)
        {
            return Remove(text, '\r', '\n');
        }
    }
}
