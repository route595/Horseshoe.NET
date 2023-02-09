﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Text.TextClean
{
    /// <summary>
    /// A set of factory methods for removing or replaceing non-ASCII characters in string.
    /// </summary>
    public static class TextClean
    {
        /// <summary>
        /// Searches a <c>string</c> for certain <c>char</c>s and returns a copy of the original <c>string</c> without them.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A set of <c>char</c>s to remove from <c>text</c>.</param>
        /// <returns>A copy of <c>text</c> without the specified <c>char</c>s.</returns>
        public static string Remove(string text, params char[] chars)
        {
            if (text == null)
                return string.Empty;
            if (chars == null || !chars.Any())
                return text;
            var strb = new StringBuilder(text.Length);
            var dirty = false;
            foreach (var c in text.AsSpan())
            {
                if (c.In(chars))
                {
                    dirty = true;
                }
                else
                {
                    strb.Append(c);
                }
            }
            if (dirty)
                return strb.ToString();
            return text;
        }

        /// <summary>
        /// Searches a <c>string</c> for whitespace <c>char</c>s and returns a copy of the original <c>string</c> without them.
        /// </summary>
        /// <param name="text">A <c>string</c> whose whitespaces to remove.</param>
        /// <returns>A copy of <c>text</c> without whitespaces.</returns>
        public static string RemoveAllWhitespace(string text)
        {
            return Remove(text, ' ', '\t', '\r', '\n', '\u00A0');
        }

        /// <summary>
        /// Searches a <c>string</c> for newline <c>char</c>s and returns a copy of the original <c>string</c> without them.
        /// </summary>
        /// <param name="text">A <c>string</c> whose newlines to remove.</param>
        /// <returns>A copy of <c>text</c> without newlines.</returns>
        public static string RemoveNewlines(string text)
        {
            return Remove(text, '\r', '\n');
        }

        /// <summary>
        /// Searches a <c>string</c> for whitespaces and converts them all to ASCII spaces and combines them into one, if applicable.
        /// </summary>
        /// <param name="text">A text <c>string</c> to evaluate.</param>
        /// <param name="exceptNewlines">If <c>true</c> newlines are not combined, default is <c>false</c>.</param>
        /// <returns>A copy of <c>text</c> with all whitespaces converted to ASCII spaces and combined into one, if applicable.</returns>
        public static string CombineWhitespaces(string text, bool exceptNewlines = false)
        {
            if (text == null)
                return string.Empty;
            var lastCharWasWhiteSpace = false;
            var whitespaces = exceptNewlines ? new[] { ' ', '\t', '\u00A0' } : new[] { ' ', '\t', '\r', '\n', '\u00A0' };
            var strb = new StringBuilder(text.Length);
            ReadOnlySpan<char> roSpan = text.AsSpan();
            foreach (var c in roSpan)
            {
                if (c.In(whitespaces))
                {
                    if (!lastCharWasWhiteSpace)
                    {
                        strb.Append(" ");
                    }
                    lastCharWasWhiteSpace = true;
                }
                else
                {
                    strb.Append(c);
                    lastCharWasWhiteSpace = false;
                }
            }
            return strb.ToString();
        }

        /// <summary>
        /// Convert essentially any text to its closest ASCII representation.
        /// </summary>
        /// <param name="text">The source text to convert.</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <param name="nonprintablesPolicy">Nonprintables display hint.</param>
        /// <param name="substitute">How to display non-printables (if <c>NonprintablesPolicy.Substitute</c>) and unknown <c>chars</c>.</param>
        /// <returns>An ASCII <c>string</c>.</returns>
        public static string ToAsciiPrintable(string text, TraceJournal journal = null, NonprintablesPolicy nonprintablesPolicy = default, string substitute = "?")
        {
            // trace journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("ToASCIIPrintable()");
            journal.Level++;

            // special case
            if (text == null || text.Length == 0)
            {
                journal.WriteEntry("-1, " + (text == null ? TextConstants.Null : TextConstants.Empty));
                journal.Level--;
                return string.Empty;
            }

            // prepare to convert to ASCII
            ReadOnlySpan<char> roSpan = text.AsSpan();
            var strb = new StringBuilder((int)(text.Length * 1.1));
            var revealOptions = new RevealOptions { CharsToReveal = CharRevealPolicy.AllWhitespaces | CharRevealPolicy.AllNonprintables | CharRevealPolicy.UnicodePrintables };

            for (int i = 0; i < roSpan.Length; i++)
            {
                char c = roSpan[i];
                if (char.IsWhiteSpace(c))
                {
                    switch (c)
                    {
                        case '\u00A0':  // non-breaking space
                            strb.Append(' ');
                            journal.WriteEntry(i + " " + revealOptions.ValueIfNbSpace + " -> " + revealOptions.ValueIfSpace);
                            journal.IncrementCleanedWhitespaces();
                            break;
                        default:
                            strb.Append(c);
                            journal.WriteEntry(i + " " + TextUtil.Reveal(c, options: revealOptions));
                            break;
                    }
                }
                else if (TextUtil.IsAsciiPrintable(c))
                {
                    strb.Append(c);
                    journal.WriteEntry(i + " ascii: " + c);
                }
                else if (TextUtil.IsPrintable(c))
                {
                    if (_TryFindAsciiReplacement(c, out string replacement, out string source))
                    {
                        strb.Append(replacement);
                        journal.WriteEntry(i + " unicode -> ascii: " + c + " -> " + replacement + " (" + source + ")");
                        journal.IncrementCleanedUnicode();
                    }
                    else
                    {
                        strb.Append(substitute);
                        journal.WriteEntry(i + " unicode (no ASCII replacement): " + TextUtil.Reveal(c, revealOptions));
                        journal.IncrementCleanedOther();
                    }
                }
                else
                {
                    switch (nonprintablesPolicy)
                    {
                        case NonprintablesPolicy.Drop:
                        default:
                            journal.WriteEntry(i + " non-printable (dropped): " + TextUtil.Reveal(c, RevealOptions.All));
                            break;
                        case NonprintablesPolicy.Substitute:
                            strb.Append(substitute);
                            journal.WriteEntry(i + " non-printable -> '" + substitute + "': " + TextUtil.Reveal(c, revealOptions));
                            break;
                    }
                    journal.IncrementCleanedNonprintables();
                }
            }

            // finalize
            journal.Level--;
            return strb.ToString();
        }

        private static IDictionary<string, IDictionary<char, char[]>> UnicodeToASCIIConversions { get; } = CharLib.AllUnicodeToASCIIConversions;

        private static IDictionary<string, IDictionary<string, char[]>> UnicodeToASCIIComplexConversions { get; } = CharLib.AllUnicodeToASCIIComplexConversions;

        private static bool _TryFindAsciiReplacement(char c, out string replacement, out string source)
        {
            replacement = "none";
            source = "none";

            // iterate the conversion tables (see CharLib)
            foreach (var dictKey in UnicodeToASCIIConversions.Keys)
            {
                foreach (var convKvp in UnicodeToASCIIConversions[dictKey])
                {
                    if (convKvp.Value.Contains(c))
                    {
                        replacement = string.Concat(convKvp.Key);
                        source = dictKey;
                        return true;
                    }
                }
            }

            // iterate the complex conversion tables (see CharLib)
            foreach (var dictKey in UnicodeToASCIIComplexConversions.Keys)
            {
                foreach (var convKvp in UnicodeToASCIIComplexConversions[dictKey])
                {
                    if (convKvp.Value.Contains(c))
                    {
                        replacement = convKvp.Key;
                        source = dictKey;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
