using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A collection of extension methods for <c>string</c> and <c>char</c> interpretation and <c>string</c> building and manipulation.
    /// </summary>
    public static class Extensions
    {
        /// <inheritdoc cref="TextUtil.HasWhitespace" />
        public static bool HasWhitespace(this string text)
        {
            return TextUtil.HasWhitespace(text);
        }

        /// <inheritdoc cref="TextUtil.MultilineTrim" />
        public static string MultilineTrim(this string text)
        {
            return TextUtil.MultilineTrim(text);
        }

        /// <inheritdoc cref="TextUtil.Repeat" />
        public static string Repeat(this string text, int numberOfTimes)
        {
            return TextUtil.Repeat(text, numberOfTimes);
        }

        /// <inheritdoc cref="TextUtil.ReplaceLast" />
        public static string ReplaceLast(this string textToSearch, string textToReplace, string replacementText)
        {
            return TextUtil.ReplaceLast(textToSearch, textToReplace, replacementText);
        }

        /// <inheritdoc cref="TextUtil.ConvertToSecureString" />
        public static SecureString ToSecureString(this string unsecureString)
        {
            return TextUtil.ConvertToSecureString(unsecureString);
        }

        /// <inheritdoc cref="TextUtil.ConvertToUnsecureString" />
        public static string ToUnsecureString(this SecureString secureString)
        {
            return TextUtil.ConvertToUnsecureString(secureString);
        }

        /// <summary>
        /// Tests if <c>text</c> contains at least one of the <c>char</c>s in <c>chars</c>s.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, params char[] chars)
        {
            return ContainsAny(text, chars as IEnumerable<char>);
        }

        /// <summary>
        /// Tests if <c>text</c> contains at least one of the <c>char</c>s in <c>chars</c>s.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;
            foreach (char c in chars)
            {
                if (text.Contains(c))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if <c>text</c> contains at least one of the <c>char</c>s in <c>chars</c>s, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(this string text, params char[] chars)
        {
            return ContainsAnyIgnoreCase(text, chars as IEnumerable<char>);
        }

        /// <summary>
        /// Tests if <c>text</c> contains at least one of the <c>char</c>s in <c>chars</c>s, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;
            foreach (char c in chars)
            {
                if (text.IndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase) > -1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all of the <c>char</c>s in <c>chars</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll(this string text, params char[] chars) =>
            ContainsAll(text, chars as IEnumerable<char>);

        /// <summary>
        /// Tests if <c>text</c> contains all of the <c>char</c>s in <c>chars</c>s.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;
            foreach (char c in chars)
            {
                if (!text.Contains(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all of the <c>char</c>s in <c>chars</c>s, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(this string text, params char[] chars) =>
            ContainsAllIgnoreCase(text, chars as IEnumerable<char>);

        /// <summary>
        /// Tests if <c>text</c> contains all of the <c>char</c>s in <c>chars</c>s, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;
            foreach (char c in chars)
            {
                if (text.IndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase) == -1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInSequence(this string text, params char[] chars) =>
            ContainsAllInSequence(text, chars as IEnumerable<char>);

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInSequence(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;

            int runningPos = -1;
            int pos;
            foreach (char c in chars)
            {
                pos = runningPos == -1
                    ? text.IndexOf(c)
                    : text.IndexOf(c, runningPos);
                if (pos <= runningPos)
                    return false;
                runningPos = pos;
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInSequenceIgnoreCase(this string text, params char[] chars) =>
            ContainsAllInSequenceIgnoreCase(text, chars as IEnumerable<char>);

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInSequenceIgnoreCase(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;

            int runningPos = -1;
            int pos;
            foreach (char c in chars)
            {
                pos = runningPos == -1
                    ? text.IndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase)
                    : text.IndexOf(new string(c, 1), runningPos, StringComparison.OrdinalIgnoreCase);
                if (pos <= runningPos)
                    return false;
                runningPos = pos;
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s ONLY in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInStrictSequence(this string text, params char[] chars) =>
            ContainsAllInStrictSequence(text, chars as IEnumerable<char>);

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s ONLY in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInStrictSequence(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;

            int runningPos = -1;
            int pos;
            foreach (char c in chars)
            {
                pos = text.IndexOf(c);
                if (pos <= runningPos)
                    return false;
                runningPos = text.LastIndexOf(c);
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s ONLY in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInStrictSequenceIgnoreCase(this string text, params char[] chars) =>
            ContainsAllInStrictSequenceIgnoreCase(text, chars as IEnumerable<char>);

        /// <summary>
        /// Tests if <c>text</c> contains all of the supplied <c>char</c>s ONLY in the supplied order.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A group of <c>char</c>s to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllInStrictSequenceIgnoreCase(this string text, IEnumerable<char> chars)
        {
            if (text == null || chars == null || !chars.Any())
                return false;

            int runningPos = -1;
            int pos;
            foreach (char c in chars)
            {
                pos = text.IndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase);
                if (pos <= runningPos)
                    return false;
                runningPos = text.LastIndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains at least one item in <c>contentsToSearchFor</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, params string[] contentsToSearchFor) =>
            ContainsAny(text, contentsToSearchFor as IEnumerable<string>);

        /// <summary>
        /// Tests if <c>text</c> contains at least one item in <c>contentsToSearchFor</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, IEnumerable<string> contentsToSearchFor)
        {
            if (contentsToSearchFor == null)
                return false;
            foreach (var content in contentsToSearchFor)
            {
                if (text.Contains(content))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if <c>text</c> contains at least one item in <c>contentsToSearchFor</c>, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(this string text, params string[] contentsToSearchFor) =>
            ContainsAnyIgnoreCase(text, contentsToSearchFor as IEnumerable<string>);

        /// <summary>
        /// Tests if <c>text</c> contains at least one item in <c>contentsToSearchFor</c>, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAnyIgnoreCase(this string text, IEnumerable<string> contentsToSearchFor)
        {
            if (contentsToSearchFor == null)
                return false;
            foreach (var content in contentsToSearchFor)
            {
                if (text.IndexOf(content, StringComparison.OrdinalIgnoreCase) > -1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all items in <c>contentsToSearchFor</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll(this string text, params string[] contentsToSearchFor) =>
            ContainsAll(text, contentsToSearchFor as IEnumerable<string>);

        /// <summary>
        /// Tests if <c>text</c> contains all items in <c>contentsToSearchFor</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAll(this string text, IEnumerable<string> contentsToSearchFor)
        {
            if (contentsToSearchFor == null || !contentsToSearchFor.Any())
                return false;
            foreach (var content in contentsToSearchFor)
            {
                if (!text.Contains(content))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if <c>text</c> contains all items in <c>contentsToSearchFor</c>, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(this string text, params string[] contentsToSearchFor) =>
            ContainsAllIgnoreCase(text, contentsToSearchFor as IEnumerable<string>);

        /// <summary>
        /// Tests if <c>text</c> contains all items in <c>contentsToSearchFor</c>, not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAllIgnoreCase(this string text, IEnumerable<string> contentsToSearchFor)
        {
            if (contentsToSearchFor == null || !contentsToSearchFor.Any())
                return false;
            foreach (var content in contentsToSearchFor)
            {
                if (text.IndexOf(content, StringComparison.OrdinalIgnoreCase) == -1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if character is carriage return (\r) or line feed (\n)
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool IsNewLine(this char chr) =>
            NET.Extensions.In(chr, 10, 13);

        /// <summary>
        /// Returns the highest index of the supplied <c>char</c>s contained in <c>texxt</c>, if applicable, otherwise returns <c>-1</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The highest <c>char</c>'s index</returns>
        public static int MaxIndexOf(this string text, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            return charsToSearch
                .Select(c => text.IndexOf(c))
                .Max();
        }

        /// <summary>
        /// Returns the highest index of the supplied <c>char</c>s contained in <c>texxt</c>, if applicable, otherwise returns <c>-1</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="startIndex">Where in <c>text</c> to start searching.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The highest <c>char</c>'s index</returns>
        public static int MaxIndexOf(this string text, int startIndex, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            return charsToSearch
                .Select(c => text.IndexOf(c, startIndex))
                .Max();
        }

        /// <summary>
        /// Returns the highest index of the supplied <c>char</c>s contained in <c>texxt</c>, if applicable, otherwise returns <c>-1</c>.  Not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The highest <c>char</c>'s index</returns>
        public static int MaxIndexOfIgnoreCase(this string text, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            return charsToSearch
                .Select(c => text.IndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase))
                .Max();
        }

        /// <summary>
        /// Returns the highest index of the supplied <c>char</c>s contained in <c>texxt</c>, if applicable, otherwise returns <c>-1</c>.  Not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="startIndex">Where in <c>text</c> to start searching.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The highest <c>char</c>'s index</returns>
        public static int MaxIndexOfIgnoreCase(this string text, int startIndex, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            return charsToSearch
                .Select(c => text.IndexOf(new string(c, 1), startIndex, StringComparison.OrdinalIgnoreCase))
                .Max();
        }

        /// <summary>
        /// Returns the lowest index of the supplied <c>char</c>s if at least one is contained in <c>texxt</c>, otherwise returns <c>-1</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The lowest <c>char</c>'s index</returns>
        public static int MinIndexOf(this string text, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            var nonNegIndices = charsToSearch
                .Select(c => text.IndexOf(c))
                .Where(i => i > -1);
            if (nonNegIndices.Any())
                return nonNegIndices.Min();
            return -1;
        }

        /// <summary>
        /// Returns the lowest index of the supplied <c>char</c>s if at least one is contained in <c>texxt</c>, otherwise returns <c>-1</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="startIndex">Where in <c>text</c> to start searching.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The lowest <c>char</c>'s index</returns>
        public static int MinIndexOf(this string text, int startIndex, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            var nonNegIndices = charsToSearch
                .Select(c => text.IndexOf(c, startIndex))
                .Where(i => i > -1);
            if (nonNegIndices.Any())
                return nonNegIndices.Min();
            return -1;
        }

        /// <summary>
        /// Returns the lowest index of the supplied <c>char</c>s if at least one is contained in <c>texxt</c>, otherwise returns <c>-1</c>.  Not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The lowest <c>char</c>'s index</returns>
        public static int MinIndexOfIgnoreCase(this string text, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            var nonNegIndices = charsToSearch
                .Select(c => text.IndexOf(new string(c, 1), StringComparison.OrdinalIgnoreCase))
                .Where(i => i > -1);
            if (nonNegIndices.Any())
                return nonNegIndices.Min();
            return -1;
        }

        /// <summary>
        /// Returns the lowest index of the supplied <c>char</c>s if at least one is contained in <c>texxt</c>, otherwise returns <c>-1</c>.  Not case-sensitive.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="startIndex">Where in <c>text</c> to start searching.</param>
        /// <param name="charsToSearch">A group of <c>char</c>s to search for.</param>
        /// <returns>The lowest <c>char</c>'s index</returns>
        public static int MinIndexOfIgnoreCase(this string text, int startIndex, params char[] charsToSearch)
        {
            if (charsToSearch == null || !charsToSearch.Any())
                return -1;
            var nonNegIndices = charsToSearch
                .Select(c => text.IndexOf(new string(c, 1), startIndex, StringComparison.OrdinalIgnoreCase))
                .Where(i => i > -1);
            if (nonNegIndices.Any())
                return nonNegIndices.Min();
            return -1;
        }
    }
}
