using System;
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
        /// Searches a string for certain <c>char</c>s and returns a copy of the original string without those <c>char</c>s.
        /// </summary>
        /// <param name="text">A text <c>string</c> to evaluate.</param>
        /// <param name="chars">A set of <c>char</c>s to remove from <c>text</c>.</param>
        /// <returns>A copy of the original string without the unwanted <c>char</c>s.</returns>
        public static string Remove(string text, params char[] chars)
        {
            if (text == null)
                return string.Empty;
            if (chars == null || !chars.Any())
                return text;
            var strb = new StringBuilder(text.Length);
            ReadOnlySpan<char> roSpan = text.AsSpan();
            foreach (var c in roSpan)
            {
                if (!c.In(chars))
                {
                    strb.Append(c);
                }
            }
            return strb.ToString();
        }

        /// <summary>
        /// Searches a string for whitespace <c>char</c>s and returns a copy of the original string without those <c>char</c>s.
        /// </summary>
        /// <param name="text">A text <c>string</c> to evaluate.</param>
        /// <param name="preserveNewlines">Indicates whether to leave in new lines <c>char</c>s, default is <c>false</c>.</param>
        /// <returns>A copy of the original string without the unwanted <c>char</c>s.</returns>
        public static string RemoveWhitespace(string text, bool preserveNewlines = false)
        {
            return Remove(text, preserveNewlines ? CharLib.SubsetWhitespacesExceptNewLines : CharLib.AllWhitespaces);
        }

        /// <summary>
        /// Searches a string for new line <c>char</c>s and returns a copy of the original string without those <c>char</c>s.
        /// </summary>
        /// <param name="text">A text <c>string</c> to evaluate.</param>
        /// <returns>A copy of the original string without the unwanted <c>char</c>s.</returns>
        public static string RemoveNewlines(string text)
        {
            return Remove(text, CharLib.SubsetNewLines);
        }

        /// <summary>
        /// Searches a string for whitespace <c>char</c>s and returns a copy of the original string combining multiple whitespace <c>char</c>s into one.
        /// </summary>
        /// <param name="text">A text <c>string</c> to evaluate.</param>
        /// <param name="preserveNewlines">Indicates whether to leave in new lines <c>char</c>s, default is <c>false</c>.</param>
        /// <returns></returns>
        public static string CombineWhitespace(string text, bool preserveNewlines = false)
        {
            if (text == null)
                return string.Empty;
            var lastCharWasWhiteSpace = false;
            var wsChars = preserveNewlines ? CharLib.SubsetWhitespacesExceptNewLines : CharLib.AllWhitespaces;
            var strb = new StringBuilder(text.Length);
            ReadOnlySpan<char> roSpan = text.AsSpan();
            foreach (var c in roSpan)
            {
                if (c.In(wsChars))
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
        /// Convert essentially any text to its closest ASCII representation
        /// </summary>
        /// <param name="text">The source text to convert</param>
        /// <param name="journal">A trace journal to which each step of the process is logged.</param>
        /// <param name="whitespacePolicy">Whitespace handling instructions</param>
        /// <param name="nonprintablesPolicy">Nonprintables display instructions</param>
        /// <param name="substitute">How to display non-printables (if <c>NonprintablesPolicy.Substitute</c>) and unknown chars</param>
        /// <param name="extendedASCII"><c>true</c> to allow extended ASCII chars in the output</param>
        /// <returns>An ASCII string</returns>
        public static string ToASCIIPrintable(string text, TraceJournal journal = null, WhitespacePolicy whitespacePolicy = default, NonprintablesPolicy nonprintablesPolicy = default, string substitute = "?", bool extendedASCII = false)
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
            bool isWhitespace = false;
            bool lastWasWhitespace;

            // whitespace policies in order of precedence
            bool noWhitespaces = whitespacePolicy == WhitespacePolicy.None;
            bool combineSpaces = (whitespacePolicy & WhitespacePolicy.CombineSpaces) == WhitespacePolicy.CombineSpaces;
            bool normalizeWhitespaces = (whitespacePolicy & WhitespacePolicy.NormalizeWhitespaces) == WhitespacePolicy.NormalizeWhitespaces;
            bool includeASCIISpace = (whitespacePolicy & WhitespacePolicy.IncludeASCIISpace) == WhitespacePolicy.IncludeASCIISpace;
            bool includeNonbreakingSpace = (whitespacePolicy & WhitespacePolicy.IncludeNonbreakingSpace) == WhitespacePolicy.IncludeNonbreakingSpace;
            bool includeTab = (whitespacePolicy & WhitespacePolicy.IncludeTab) == WhitespacePolicy.IncludeTab;
            bool includeNewLines = (whitespacePolicy & WhitespacePolicy.IncludeNewLines) == WhitespacePolicy.IncludeNewLines;

            for (int i = 0; i < roSpan.Length; i++)
            {
                char c = roSpan[i];
                lastWasWhitespace = isWhitespace;
                isWhitespace = false;
                if (c.In(CharLib.AllWhitespaces))
                {
                    if (noWhitespaces)
                    {
                        journal.WriteEntry(i + ", whitespaces - not included: " + TextUtil.Reveal(c, RevealOptions.All));
                        continue;
                    }
                    isWhitespace = true;
                    switch (c)
                    {
                        case ' ':
                            if (includeASCIISpace)
                            {
                                if (combineSpaces && lastWasWhitespace)
                                {
                                    journal.WriteEntry(i + ", *space (combined)");
                                    journal.IncrementCleanedWhitespaces();
                                }
                                else
                                {
                                    strb.Append(c);
                                    journal.WriteEntry(i + ", +space");
                                }
                            }
                            else
                            {
                                journal.WriteEntry(i + ", -space");
                                journal.IncrementCleanedWhitespaces();
                            }
                            continue;
                        case '\t':
                            if (includeTab)
                            {
                                if (combineSpaces && lastWasWhitespace)
                                {
                                    journal.WriteEntry(i + ", *tab (combined)");
                                    journal.IncrementCleanedWhitespaces();
                                }
                                else if (combineSpaces || normalizeWhitespaces)
                                {
                                    strb.Append(' ');
                                    journal.WriteEntry(i + ", *tab (-> space)");
                                    journal.IncrementCleanedWhitespaces();
                                }
                                else
                                {
                                    strb.Append(c);
                                    journal.WriteEntry(i + ", +tab");
                                }
                            }
                            else
                            {
                                journal.WriteEntry(i + ", -tab");
                                journal.IncrementCleanedWhitespaces();
                            }
                            continue;
                        case '\r':
                        case '\n':
                            if (includeNewLines)
                            {
                                if (combineSpaces && lastWasWhitespace)
                                {
                                    journal.WriteEntry(i + ", *newline (combined): " + TextUtil.Reveal(c, RevealOptions.All));
                                    journal.IncrementCleanedWhitespaces();
                                }
                                else if (combineSpaces || normalizeWhitespaces)
                                {
                                    strb.Append(' ');
                                    journal.IncrementCleanedWhitespaces();
                                    journal.WriteEntry(i + ", *newline (-> space): " + TextUtil.Reveal(c, RevealOptions.All));
                                }
                                else
                                {
                                    strb.Append(c);
                                    journal.WriteEntry(i + ", +newline: " + TextUtil.Reveal(c, RevealOptions.All));
                                }
                            }
                            else
                            {
                                journal.WriteEntry(i + ", -newline: " + TextUtil.Reveal(c, RevealOptions.All));
                                journal.IncrementCleanedWhitespaces();
                            }
                            continue;
                        case '\u00A0':  // non-breaking space
                            if (includeNonbreakingSpace)
                            {
                                if (combineSpaces && lastWasWhitespace)
                                {
                                    journal.WriteEntry(i + ", *nb_space (combined)");
                                    journal.IncrementCleanedWhitespaces();
                                }
                                else if (combineSpaces || normalizeWhitespaces)
                                {
                                    strb.Append(' ');
                                    journal.WriteEntry(i + ", *nb_space (-> space)");
                                    journal.IncrementCleanedWhitespaces();
                                }
                                else
                                {
                                    strb.Append(c);
                                    journal.WriteEntry(i + ", +nb_space");
                                }
                            }
                            else
                            {
                                journal.WriteEntry(i + ", -nb_space");
                                journal.IncrementCleanedWhitespaces();
                            }
                            continue;
                    }
                    throw new ThisShouldNeverHappenException("The switch block always continues as long as all whitespaces are accounted for.");
                }
                else if (TextUtil.IsASCIIPrintable(c, extendedASCII: false))  // excluding whitespaces
                {
                    strb.Append(c);
                    journal.WriteEntry(i + ", +ascii_char: " + c);
                }
                else if (extendedASCII && TextUtil.IsASCIIPrintable(c, extendedASCII: true))  // excluding whitespaces
                {
                    strb.Append(c);
                    journal.WriteEntry(i + ", +extended_ascii_char: " + c);
                }
                else if (TextUtil.IsASCIIControl(c, extendedASCII: false))  // excluding whitespaces
                {
                    journal.WriteEntry(i + ", -ascii_control: " + TextUtil.Reveal(c, RevealOptions.All));
                    journal.IncrementCleanedNonprintables();
                }
                else if (TextUtil.IsASCIIControl(c, extendedASCII: true))  // still handles extended ASCII controls even if not requested
                {
                    journal.WriteEntry(i + ", -extended_ascii_control: " + TextUtil.Reveal(c, RevealOptions.All));
                    journal.IncrementCleanedNonprintables();
                }
                else if (CharLib.OtherNonprintables.Contains(c))
                {
                    switch (nonprintablesPolicy)
                    {
                        case NonprintablesPolicy.Drop:
                        default:
                            journal.WriteEntry(i + ", -nonprintable: " + TextUtil.Reveal(c, RevealOptions.All));
                            break;
                        case NonprintablesPolicy.Substitute:
                            strb.Append(substitute);
                            journal.WriteEntry(i + ", *nonprintable: " + TextUtil.Reveal(c, RevealOptions.All) + " > " + substitute);
                            break;
                    }
                    journal.IncrementCleanedNonprintables();
                }
                else if (_TryFindASCIIReplacement(c, out string replacement, out string source, extendedASCII: extendedASCII))
                {
                    strb.Append(replacement);
                    journal.WriteEntry(i + ", +ascii_replacement: " + TextUtil.Reveal(c, RevealOptions.All) + " > " + TextUtil.Reveal(replacement, RevealOptions.All) + " -- source: " + source);
                    journal.IncrementCleanedUnicode();
                }
                else
                {
                    strb.Append(substitute);
                    journal.WriteEntry(i + ", *other: " + TextUtil.Reveal(c, RevealOptions.All) + " > " + substitute);
                    journal.IncrementCleanedOther();
                }
            }

            // finalize
            journal.Level--;
            return strb.ToString();
        }

        private static IDictionary<string, IDictionary<char, char[]>> UnicodeToASCIIConversions { get; } = CharLib.AllUnicodeToASCIIConversions;

        private static IDictionary<string, IDictionary<string, char[]>> UnicodeToASCIIComplexConversions { get; } = CharLib.AllUnicodeToASCIIComplexConversions;

        private static bool _TryFindASCIIReplacement(char c, out string replacement, out string source, bool extendedASCII)
        {
            replacement = "none";
            source = "none";

            if (extendedASCII)
            {
                // preclude replacing extended ASCII printable chars, if applicable
                if (CharLib.ExtendedASCIIAlpha.Contains(c) || CharLib.ExtendedASCIISymbols.Contains(c))
                {
                    replacement = null;
                    source = "extended ASCII";
                    return false;
                }
            }

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
