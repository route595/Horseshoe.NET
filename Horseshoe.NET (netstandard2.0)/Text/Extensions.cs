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
        /// Tests to see if <c>text</c> contains at least one of a group of <c>string</c>s.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, params string[] contentsToSearchFor)
        {
            return ContainsAny(text, contentsToSearchFor, ignoreCase: false);
        }

        /// <summary>
        /// Tests to see if <c>text</c> contains at least one of a group of <c>string</c>s, not case-sensitive if <c>ignoreCase == true</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <param name="ignoreCase">If <c>true</c> the search is not case-senstive, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, IEnumerable<string> contentsToSearchFor, bool ignoreCase = false)
        {
            if (contentsToSearchFor != null)
            {
                if (ignoreCase)
                {
                    var upper = text.ToUpper();
                    foreach (var content in contentsToSearchFor)
                    {
                        if (upper.Contains(content.ToUpper()))
                            return true;
                    }
                }
                else
                {
                    foreach (var content in contentsToSearchFor)
                    {
                        if (text.Contains(content))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if character is carriage return (\r) or line feed (\n)
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool IsNewLine(this char chr)
        {
            return NET.Extensions.In(chr, 10, 13);
        }

        //internal static void IncrementCleanedWhitespaces(this TraceJournal journal)
        //{
        //    if (journal.Data.ContainsKey("text.clean.whitespaces.count"))
        //    {
        //        journal.Data["text.clean.whitespaces.count"] = (int)journal.Data["text.clean.whitespaces.count"] + 1;
        //    }
        //    else
        //    {
        //        journal.Data.Add("text.clean.whitespaces.count", 1);
        //    }
        //}

        //internal static void IncrementCleanedNonprintables(this TraceJournal journal)
        //{
        //    if (journal.Data.ContainsKey("text.clean.nonprintables.count"))
        //    {
        //        journal.Data["text.clean.nonprintables.count"] = (int)journal.Data["text.clean.nonprintables.count"] + 1;
        //    }
        //    else
        //    {
        //        journal.Data.Add("text.clean.nonprintables.count", 1);
        //    }
        //}

        //internal static void IncrementCleanedUnicode(this TraceJournal journal)
        //{
        //    if (journal.Data.ContainsKey("text.clean.unicode.count"))
        //    {
        //        journal.Data["text.clean.unicode.count"] = (int)journal.Data["text.clean.unicode.count"] + 1;
        //    }
        //    else
        //    {
        //        journal.Data.Add("text.clean.unicode.count", 1);
        //    }
        //}

        //internal static void IncrementCleanedOther(this TraceJournal journal)
        //{
        //    if (journal.Data.ContainsKey("text.clean.other.count"))
        //    {
        //        journal.Data["text.clean.other.count"] = (int)journal.Data["text.clean.other.count"] + 1;
        //    }
        //    else
        //    {
        //        journal.Data.Add("text.clean.other.count", 1);
        //    }
        //}

        ///// <summary>
        ///// Gets the count of whitespace <c>char</c>s that were eliminated or converted during a 'clean' operation.
        ///// </summary>
        ///// <param name="journal"></param>
        ///// <returns>Affected whitespace <c>char</c> count</returns>
        //public static int GetCleanedWhitespaceCharacters(this TraceJournal journal)
        //{
        //    if (journal.Data.TryGetValue("text.clean.whitespaces.count", out object count))
        //        return (int)count;
        //    return 0;
        //}

        ///// <summary>
        ///// Gets the count of nonprintable <c>char</c>s that were eliminated or converted during a 'clean' operation.
        ///// </summary>
        ///// <param name="journal"></param>
        ///// <returns>Affected nonprintable <c>char</c> count</returns>
        //public static int GetCleanedNonprintableCharacters(this TraceJournal journal)
        //{
        //    if (journal.Data.TryGetValue("text.clean.nonprintables.count", out object count))
        //        return (int)count;
        //    return 0;
        //}

        ///// <summary>
        ///// Gets the count of Unicode <c>char</c>s that were eliminated or converted during a 'clean' operation.
        ///// </summary>
        ///// <param name="journal"></param>
        ///// <returns>Affected Unicode <c>char</c> count</returns>
        //public static int GetCleanedUnicodeCharacters(this TraceJournal journal)
        //{
        //    if (journal.Data.TryGetValue("text.clean.unicode.count", out object count))
        //        return (int)count;
        //    return 0;
        //}

        ///// <summary>
        ///// Gets the count of other <c>char</c>s that were eliminated or converted during a 'clean' operation.
        ///// </summary>
        ///// <param name="journal"></param>
        ///// <returns>Other affected <c>char</c> count</returns>
        //public static int GetCleanedOtherCharacters(this TraceJournal journal)
        //{
        //    if (journal.Data.TryGetValue("text.clean.other.count", out object count))
        //        return (int)count;
        //    return 0;
        //}

        ///// <summary>
        ///// Gets the total count of <c>char</c>s that were eliminated or converted during a 'clean' operation.
        ///// </summary>
        ///// <param name="journal"></param>
        ///// <returns></returns>
        //public static int GetTotalCleanedCharacters(this TraceJournal journal)
        //{
        //    var total = 0;
        //    if (journal.Data.TryGetValue("text.clean.whitespaces.count", out object whitespacesCount))
        //        total += (int)whitespacesCount;
        //    if (journal.Data.TryGetValue("text.clean.nonprintables.count", out object nonprintablesCount))
        //        total += (int)nonprintablesCount;
        //    if (journal.Data.TryGetValue("text.clean.unicode.count", out object unicodeCount))
        //        total += (int)unicodeCount;
        //    if (journal.Data.TryGetValue("text.clean.other.count", out object otherCount))
        //        total += (int)otherCount;
        //    return total;
        //}
    }
}
