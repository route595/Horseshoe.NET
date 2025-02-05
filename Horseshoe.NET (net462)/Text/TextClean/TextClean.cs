using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Text.TextClean
{
    /// <summary>
    /// A set of factory methods for removing or replacing non-ASCII characters in string.
    /// </summary>
    public static class TextClean
    {
        private static readonly string MessageRelayGroup = typeof(TextClean).Namespace;

        /// <inheritdoc cref="TextCleanAbstractions.Remove(string, char[])"/>
        public static string Remove(string text, params char[] chars)
        {
            return TextCleanAbstractions.Remove(text, chars);
        }

        /// <inheritdoc cref="TextCleanAbstractions.RemoveAllWhitespace(string)"/>
        public static string RemoveAllWhitespace(string text)
        {
            return TextCleanAbstractions.RemoveAllWhitespace(text);
        }

        /// <inheritdoc cref="TextCleanAbstractions.RemoveNewlines(string)"/>
        public static string RemoveNewlines(string text)
        {
            return TextCleanAbstractions.RemoveNewlines(text);
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
        /// <param name="charsToRemove">A set of <c>char</c>s to remove from <c>text</c>.</param>
        /// <param name="nonprintablesPolicy">Nonprintables display hint.</param>
        /// <param name="substitute">How to display non-printables (if <c>NonprintablesPolicy.Substitute</c>) and unknown <c>chars</c>.</param>
        /// <returns>An ASCII <c>string</c>.</returns>
        public static string ToAsciiPrintable(string text, char[] charsToRemove = null, NonprintablesPolicy nonprintablesPolicy = default, string substitute = "?")
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParam(nameof(text), text, group: MessageRelayGroup);

            text = Remove(text, charsToRemove);

            if (string.IsNullOrEmpty(text))
            {
                SystemMessageRelay.RelayMethodReturnValue(text, group: MessageRelayGroup);
                return string.Empty;
            }

            // prepare to convert to ASCII
            ReadOnlySpan<char> roSpan = text.AsSpan();
            var strb = new StringBuilder((int)(text.Length * 1.1));
            var revealOptions = new RevealOptions { CharCategory = CharCategory.AllWhitespaces | CharCategory.Nonprintables | CharCategory.UnicodePrintables };

            // statistics
            var log = new List<string>();

            for (int i = 0; i < roSpan.Length; i++)
            {
                char c = roSpan[i];
                if (char.IsWhiteSpace(c))
                {
                    switch (c)
                    {
                        case '\u00A0':  // non-breaking space
                            strb.Append(' ');
                            log.Add("[" + i + "] " + revealOptions.ValueIfNbSpace + " -> " + revealOptions.ValueIfSpace);
                            break;
                        default:
                            strb.Append(c);
                            log.Add("[" + i + "] " + TextUtil.Reveal(c));
                            break;
                    }
                }
                else if (TextUtil.IsAsciiPrintable(c))
                {
                    strb.Append(c);
                    log.Add("[" + i + "] ascii: " + c);
                }
                else if (TextUtil.IsPrintable(c))
                {
                    if (_TryFindAsciiReplacement(c, out string replacement, out string source))
                    {
                        strb.Append(replacement);
                        log.Add("[" + i + "] unicode -> ascii: " + TextUtil.Reveal(c) + " -> " + replacement + " (" + source + ")");
                    }
                    else
                    {
                        strb.Append(substitute);
                        log.Add("[" + i + "] unicode (no ASCII replacement): " + TextUtil.Reveal(c) + " -> " + substitute);
                    }
                }
                else
                {
                    switch (nonprintablesPolicy)
                    {
                        case NonprintablesPolicy.Drop:
                        default:
                            log.Add("[" + i + "] nonprintable: " + TextUtil.Reveal(c) + " (dropped)");
                            break;
                        case NonprintablesPolicy.Substitute:
                            strb.Append(substitute);
                            log.Add("[" + i + "] nonprintable: " + TextUtil.Reveal(c) + " -> " + substitute);
                            break;
                    }
                }
            }

            SystemMessageRelay.RelayMessage(() => ValueUtil.Display(log), group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodReturnValue(() => strb.ToString(), group: MessageRelayGroup);
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
