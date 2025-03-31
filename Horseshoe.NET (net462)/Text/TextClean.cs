using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A set of factory methods for removing or replacing non-ASCII characters in string.
    /// </summary>
    public static class TextClean
    {
        private static readonly string MessageRelayGroup = typeof(TextClean).Namespace;

        /// <summary>
        /// Returns a copy of the supplied <c>string</c> without the indicated <c>chars</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="chars">A set of <c>char</c>s to remove from <c>text</c>.</param>
        /// <returns>A copy of the supplied <c>string</c> without the indicated <c>chars</c>.</returns>
        public static string Remove(string text, params char[] chars)
        {
            if (string.IsNullOrEmpty(text))
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
        /// <returns>A copy of <c>text</c> without whitespaces (including new lines and non-breaking spaces).</returns>
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
        /// <param name="charsToDrop">A set of <c>char</c>s to remove from the resulting <c>string</c>.</param>
        /// <param name="charsToIgnore">A set of <c>char</c>s to allow to pass through to the resulting <c>string</c>.</param>
        /// <param name="whitespacePolicy">How to handle whitespaces, default is <c>Basic</c>.</param>
        /// <param name="nonprintablePolicy">How to handle nonprintables, default is <c>Drop</c>.</param>
        /// <returns>An ASCII <c>string</c>.</returns>
        public static string ToAsciiPrintable(string text, IEnumerable<char> charsToDrop = null, IEnumerable<char> charsToIgnore = null, WhitespaceCleanPolicy whitespacePolicy = default, NonprintableCleanPolicy nonprintablePolicy = default)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParam(nameof(text), text, group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParam(nameof(charsToDrop), charsToDrop, group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParam(nameof(charsToIgnore), charsToIgnore, group: MessageRelayGroup);

            if (string.IsNullOrEmpty(text))
            {
                SystemMessageRelay.RelayMethodReturnValue(text, group: MessageRelayGroup);
                return string.Empty;
            }

            // prepare to convert to ASCII
            ReadOnlySpan<char> roSpan = text.AsSpan();
            var strb = new StringBuilder(1 + (int)(text.Length * 1.2));  // slightly inflated initial capacity
            int pos = -1;

            foreach (char c in roSpan)
            {
                pos++;

                if (charsToIgnore != null && charsToIgnore.Contains(c))
                {
                    strb.Append(c);
                    SystemMessageRelay.RelayMessage(() => "[pos=" + pos + "] Manually ignored (see charsToIgnore): " + CharInfo.Get(c), group: MessageRelayGroup);
                    continue;
                }

                if (charsToDrop != null && charsToDrop.Contains(c))
                {
                    SystemMessageRelay.RelayMessage(() => "[pos=" + pos + "] Manually dropped (see charsToDrop): " + CharInfo.Get(c), group: MessageRelayGroup);
                    continue;
                }

                // is ASCII
                if (c <= 127)
                {
                    // ASCII whitespace (not including the space or nonbreaking space
                    if (c.In(9, 10, 13))
                    {
                        switch (whitespacePolicy)
                        {
                            case WhitespaceCleanPolicy.Basic:
                            default:
                                strb.Append(c);
                                continue;
                            case WhitespaceCleanPolicy.Drop:
                                SystemMessageRelay.RelayMessage(() => "[pos=" + pos + "] Whitespace dropped: " + CharInfo.Get(c), group: MessageRelayGroup);
                                continue;
                            case WhitespaceCleanPolicy.Reveal:
                                strb.Append(CharInfo.Get(c));
                                continue;
                            case WhitespaceCleanPolicy.Replace:
                            case WhitespaceCleanPolicy.Combine:
                                SystemMessageRelay.RelayMessage(() => "[pos=" + pos + "] Whitespace: " + CharInfo.Get(c) + " -> " + TextConstants.Space, group: MessageRelayGroup);
                                strb.Append(" ");
                                continue;
                        }
                    }

                    // ASCII controls
                    if (c <= 31 || c == 127)
                    {
                        switch (nonprintablePolicy)
                        {
                            case NonprintableCleanPolicy.Drop:
                            default:
                                continue;
                            case NonprintableCleanPolicy.Reveal:
                                strb.Append(CharInfo.Get(c));
                                continue;
                        }
                    }

                    // ASCII letters, numbers, symbols, punctuation
                    strb.Append(c);
                    continue;
                }

                // Non-ASCII Unicode

                var ci = CharInfo.Get(c);

                switch (ci.Category)
                {
                    case CharCategory.Nonprintable:
                        switch (nonprintablePolicy)
                        {
                            case NonprintableCleanPolicy.Drop:
                            default:
                                SystemMessageRelay.RelayMessage("[pos=" + pos + "] Unicode dropped: " + ci, group: MessageRelayGroup);
                                continue;
                            case NonprintableCleanPolicy.Reveal:
                                strb.Append(ci);
                                continue;
                        }
                    case CharCategory.WhitespacesSansNewLines:
                    case CharCategory.NewLines:
                        switch (whitespacePolicy)
                        {
                            case WhitespaceCleanPolicy.Basic:
                            case WhitespaceCleanPolicy.Replace:
                            case WhitespaceCleanPolicy.Combine:
                            default:
                                strb.Append(" ");
                                SystemMessageRelay.RelayMessage("[pos=" + pos + "] Whitespace: " + ci + " -> " + TextConstants.Space, group: MessageRelayGroup);
                                continue;
                            case WhitespaceCleanPolicy.Drop:
                                continue;
                            case WhitespaceCleanPolicy.Reveal:
                                strb.Append(ci);
                                continue;
                        }
                    default:
                        if (CharUtil.TryFindASCIIReplacement(c, out string replacement, out string script))
                        {
                            strb.Append(replacement);
                            SystemMessageRelay.RelayMessage("[pos=" + pos + "] Unicode -> ASCII: " + ci + " -> " + replacement, group: MessageRelayGroup);
                        }
                        else
                        {
                            strb.Append(c);
                            SystemMessageRelay.RelayMessage("[pos=" + pos + "] No ASCII replacement: " + ci, group: MessageRelayGroup);
                        }
                        break;
                }
            }

            var result = strb.ToString();
            while (whitespacePolicy == WhitespaceCleanPolicy.Combine && result.Contains("  "))
                result = result.Replace("  ", " ");
            SystemMessageRelay.RelayMethodReturnValue(() => TextUtil.Crop(result, 40, position: HorizontalPosition.Center, truncateMarker: TruncateMarker.LongEllipsis), group: MessageRelayGroup);
            return result;
        }
    }
}
