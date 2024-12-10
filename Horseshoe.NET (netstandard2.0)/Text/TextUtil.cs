using System;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A collection of factory methods for <c>string</c> and <c>char</c> interpretation and <c>string</c> manipulation.
    /// </summary>
    public static class TextUtil
    {
        /// <summary>
        /// Trims each line of a multi-line <c>string</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <returns>A line-trimmed version of <c>text</c>.</returns>
        public static string MultilineTrim(string text)
        {
            if (text == null)
                return string.Empty;
            text = text.Trim().Replace("\r\n", "\n");
            var lines = text.Split('\n')
                .Trim()
                .ToArray();
            text = string.Join(Environment.NewLine, lines);
            return text;
        }

        /// <summary>
        /// Detects whitespace <c>chars</c> in a <c>string</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool HasWhitespace(string text)
        {
            if (text == null) return false;
            return text.Any(c => char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by repeating the supplied text.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="allowOverflow">If <c>true</c> and if <c>text</c> length &gt; 1 then result is not truncated to <c>targetLength</c>, default is <c>false</c>.</param>
        /// <param name="rtl">If <c>true</c>, fills right-to-left, default is <c>false</c>.</param>
        /// <returns>The new fixed-length <c>string</c>.</returns>
        public static string Fill(string text, int targetLength, bool allowOverflow = false, bool rtl = false)
        {
            if (text == null || targetLength < 1)
                return string.Empty;
            if (text.Length == 0) text = " ";
            if (text.Length == 1)
                return new string(text[0], targetLength);
            var sb = new StringBuilder();
            while (sb.Length < targetLength)
            {
                sb.Append(text);
            }
            if (allowOverflow)
                return sb.ToString();
            return rtl
                ? Crop(sb.ToString(), targetLength, direction: HorizontalPosition.Left)
                : Crop(sb.ToString(), targetLength);
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by adding <c>char</c>s to one or both ends of <c>text</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="direction">The padding direction.</param>
        /// <param name="padding">The padding text.</param>
        /// <param name="leftPadding">Left padding.</param>
        /// <param name="cannotExceedTargetLength">If <c>true</c>, throws an exception if <c>text</c> is longer than <c>targetLength</c>, default is <c>false</c>.</param>
        /// <returns>The new fixed-length <c>string</c>.</returns>
        /// <exception cref="ValidationException"></exception>
        public static string Pad(string text, int targetLength, HorizontalPosition direction = HorizontalPosition.Right, string padding = null, string leftPadding = null, bool cannotExceedTargetLength = false)
        {
            if (text == null)
                text = string.Empty;
            if (text.Length == targetLength) return text;
            if (text.Length > targetLength)
            {
                if (cannotExceedTargetLength) throw new ValidationException("Text length (" + text.Length + ") cannot exceed target length (" + targetLength + ")");
                return text;
            }
            switch (padding = padding ?? string.Empty)
            {
                case "":
                    padding = " ";
                    break;
            }
            if (padding.Length > targetLength)
                throw new ValidationException("Padding length (" + padding.Length + ") cannot exceed target length (" + targetLength + ")");
            var rtl = leftPadding != null;  // todo: what?
            leftPadding = leftPadding == null
                ? padding
                : (leftPadding.Length == 0 ? " " : leftPadding);

            switch (direction)
            {
                case HorizontalPosition.Left:
                    return Fill(leftPadding, targetLength - text.Length) + text;
                case HorizontalPosition.Center:
                    var sb = new StringBuilder();
                    int temp = (targetLength - text.Length) / 2;  // in case of uneven padding to left and right of text always prefer a smaller left
                    sb.Append(Fill(leftPadding, temp, rtl: rtl));
                    sb.Append(text);
                    temp = targetLength - text.Length - temp;
                    sb.Append(Fill(padding, temp));
                    return sb.ToString();
                case HorizontalPosition.Right:
                default:
                    return text + Fill(padding, targetLength - text.Length);
            }
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by removing <c>char</c>s from one or both ends of <c>text</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="direction">The padding direction.</param>
        /// <param name="truncateMarker">An optional truncation indicator, e.g. <c>"..."</c> or <c>TruncateMarker.Ellipsis</c>.</param>
        /// <returns>The new fixed-length <c>string</c>.</returns>
        /// <exception cref="ValidationException"></exception>
        public static string Crop(string text, int targetLength, HorizontalPosition direction = HorizontalPosition.Right, string truncateMarker = null)
        {
            if (text == null || targetLength <= 0) return string.Empty;
            if (text.Length <= targetLength) return text;
            truncateMarker = truncateMarker ?? TruncateMarker.None;
            if (truncateMarker.Length > targetLength) throw new ValidationException("Truncate marker length (" + truncateMarker.Length + ") cannot exceed target length (" + targetLength + ")");
            if (truncateMarker.Length == targetLength) return truncateMarker;

            switch (direction)
            {
                case HorizontalPosition.Left:
                    return truncateMarker + text.Substring(text.Length - targetLength + truncateMarker.Length);
                case HorizontalPosition.Center:
                    var sb = new StringBuilder();
                    int temp = (targetLength - truncateMarker.Length) / 2;  // in case of uneven characters to left and right of marker always prefer a smaller left
                    if (temp == 0) temp = 1;                        // except when left is 0 and right is 1 in which case switch
                    sb.Append(text.Substring(0, temp));
                    sb.Append(truncateMarker);
                    temp = targetLength - temp - truncateMarker.Length;
                    sb.Append(text.Substring(text.Length - temp));
                    return sb.ToString();
                case HorizontalPosition.Right:
                default:
                    return text.Substring(0, targetLength - truncateMarker.Length) + truncateMarker;
            }
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by either adding or removing <c>char</c>s from one or both ends of <c>text</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="direction">The padding direction.</param>
        /// <param name="padding">The padding text.</param>
        /// <param name="leftPadding">Left padding.</param>
        /// <param name="truncateDirection">The truncate direction.</param>
        /// <param name="truncateMarker">An optional truncation indicator, e.g. <c>"..."</c> or <c>TruncateMarker.Ellipsis</c>.</param>
        /// <returns>The new fixed-length <c>string</c>.</returns>
        public static string Fit(string text, int targetLength, HorizontalPosition direction = HorizontalPosition.Left, string padding = null, string leftPadding = null, HorizontalPosition? truncateDirection = null, string truncateMarker = null)
        {
            if (text == null)
                text = string.Empty;
            if (text.Length == targetLength) return text;
            if (text.Length < targetLength) return Pad(text, targetLength, direction: FitSwitchPadDirection(direction), padding: padding, leftPadding: leftPadding);
            return Crop(text, targetLength, direction: truncateDirection ?? FitSwitchTruncateDirection(direction), truncateMarker: truncateMarker);
        }

        private static HorizontalPosition FitSwitchPadDirection(HorizontalPosition direction)
        {
            switch (direction)
            {
                case HorizontalPosition.Left:
                    return HorizontalPosition.Right;
                case HorizontalPosition.Center:
                    return HorizontalPosition.Center;
                case HorizontalPosition.Right:
                default:
                    return HorizontalPosition.Left;
            }
        }

        private static HorizontalPosition FitSwitchTruncateDirection(HorizontalPosition direction)
        {
            switch (direction)
            {
                case HorizontalPosition.Left:
                case HorizontalPosition.Right:
                default:
                    return HorizontalPosition.Right;
                case HorizontalPosition.Center:
                    return HorizontalPosition.Center;
            }
        }

        /// <summary>
        /// Create a <c>string</c> by repeating <c>text</c> a specific number of time.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="numberOfTimes">A number of times to repeat <c>text</c>.</param>
        /// <returns>A newly constructed <c>string</c>.</returns>
        public static string Repeat(string text, int numberOfTimes)
        {
            if (text == null)
                return string.Empty;
            var sb = new StringBuilder();
            for (int i = 0; i < numberOfTimes; i++)
            {
                sb.Append(text);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Searches <c>text</c> and replaces the last occurance of <c>textToReplace</c> with <c>replacementText</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="textToReplace">Text to replace.</param>
        /// <param name="replacementText">Replacement text.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>textToReplace</c>, default is <c>false</c>.</param>
        /// <returns>A newly constructed <c>string</c>.</returns>
        public static string ReplaceLast(string text, string textToReplace, string replacementText, bool ignoreCase = false)
        {
            if (text == null)
                return string.Empty;
            if (textToReplace == null || replacementText == null || textToReplace == string.Empty)
                return text;

            int pos = ignoreCase
                ? text.ToLower().LastIndexOf(textToReplace.ToLower())
                : text.LastIndexOf(textToReplace);

            if (pos < 0) return text;

            return new StringBuilder(text.Substring(0, pos))
                .Append(replacementText)
                .Append(text.Substring(pos + textToReplace.Length))
                .ToString();
        }

        /// <summary>
        /// Displays a <c>string</c> representation of the supplied <c>object</c>.
        /// </summary>
        /// <param name="obj">The <c>object</c> to reveal.</param>
        /// <param name="options">Options for revealing <c>object</c>s, <c>string</c>s and/or <c>char</c>s.</param>
        /// <returns>A <c>string</c> representation of a <c>obj</c>.  Depending on options, could simply be the original <c>char</c> value.</returns>
        public static string Reveal(object obj, RevealOptions options = null)
        {
            options = options ?? new RevealOptions();

            if (obj == null) return options.ValueIfNull;

            if (obj is char _c) return RevealChar(_c, options);

            if (obj is string text)
            {
                if (text.Length == 0) return options.ValueIfEmpty;
                if (text.Trim().Length == 0) return options.ValueIfWhitespace;

                var sb = new StringBuilder();
                ReadOnlySpan<char> span = text.AsSpan();

                if (options.IsRevealingChars)
                {
                    // bulk reveal chars
                    foreach (var c in span)
                    {
                        sb.Append(RevealChar(c, options));
                    }
                }
                else
                {
                    sb.Append(text);
                }

                // handle new-lines
                if ((options.CharCategory & CharCategory.OtherWhitespaces) == CharCategory.OtherWhitespaces)
                {
                    sb.Replace(options.ValueIfCr + options.ValueIfLf, options.ValueIfCrLf);
                    if (options.PreserveNewLines)
                    {
                        sb.Replace(options.ValueIfLf, options.ValueIfLf + "\n");
                        sb.Replace(options.ValueIfCrLf, options.ValueIfCrLf + "\r\n");
                    }
                }

                // append quotes, if applicable
                if (options.StringQuotationLevel >= 2)
                {
                    sb.Insert(0, '\"');
                    sb.Append('\"');
                }
                else if (options.StringQuotationLevel == 1)
                {
                    sb.Insert(0, '\'');
                    sb.Append('\'');
                }

                return sb.ToString();
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Displays a <c>char</c> in <c>string</c> format.
        /// </summary>
        /// <param name="c">The <c>char</c> to reveal.</param>
        /// <param name="options">Options for revealing <c>char</c>s.</param>
        /// <returns>A <c>string</c> representation of a <c>char</c> including, depending on options, original <c>char</c> value.</returns>
        public static string RevealChar(char c, RevealOptions options = null)
        {
            var category = GetCharCategory(c);
            if (options == null)
            {
                options = new RevealOptions { CharCategory = CharCategory.All };
            }

            if (!(CollectionUtil.Contains(options.CharsToReveal, c) || (options.CharCategory & category) == category))
            {
                return new string(c, 1);
            }

            int i = c;

            switch (category)
            {
                case CharCategory.Spaces:
                    switch (i)
                    {
                        case 32: // ' ' - space
                            return options.ValueIfSpace;
                        case 160:  // '\u00A0' - non-breaking space
                            return options.ValueIfNbSpace;
                        default:
                            throw new ThisShouldNeverHappenException("space char not found");
                    }

                case CharCategory.OtherWhitespaces:
                    switch (i)
                    {
                        case 9: // '\t' - horixontal tab
                            return options.ValueIfTab;
                        case 10: // '\n' - line feed
                            return options.ValueIfLf;
                        case 13: // '\r' - carriage return (usually found before '\n')
                            return options.ValueIfCr;
                        default:
                            throw new ThisShouldNeverHappenException("other whitespace char not found");
                    }

                case CharCategory.Nonprintables:
                    switch (i)
                    {
                        // ASCII
                        case 0: return "[NUL]";
                        case 1: return "[SOH]";
                        case 2: return "[STX]";
                        case 3: return "[ETX]";
                        case 4: return "[EOT]";
                        case 5: return "[ENQ]";
                        case 6: return "[ACK]";
                        case 7: return "[BEL]";
                        case 8: return "[BS]";
                        case 11: return "[VT]";
                        case 12: return "[FF]";
                        case 14: return "[SO]";
                        case 15: return "[SI]";
                        case 16: return "[DLE]";
                        case 17: return "[DC1]";
                        case 18: return "[DC2]";
                        case 19: return "[DC3]";
                        case 20: return "[DC4]";
                        case 21: return "[NAK]";
                        case 22: return "[SYN]";
                        case 23: return "[EDB]";
                        case 24: return "[CAN]";
                        case 25: return "[EM]";
                        case 26: return "[SUB]";
                        case 27: return "[ESC]";
                        case 28: return "[FS]";
                        case 29: return "[GS]";
                        case 30: return "[RS]";
                        case 31: return "[US]";
                        case 127: return "[DEL]";

                        // Unicode
                        case 128: return "[PAD]";
                        case 129: return "[HOP]";
                        case 130: return "[BPH]";
                        case 131: return "[NBH]";
                        case 132: return "[IND]";
                        case 133: return "[NEL]";
                        case 134: return "[SSA]";
                        case 135: return "[ESA]";
                        case 136: return "[HTS]";
                        case 137: return "[HTJ]";
                        case 138: return "[VTS]";
                        case 139: return "[PLD]";
                        case 140: return "[PLU]";
                        case 141: return "[RI]";
                        case 142: return "[SS2]";
                        case 143: return "[SS3]";
                        case 144: return "[DCS]";
                        case 145: return "[PU1]";
                        case 146: return "[PU2]";
                        case 147: return "[STS]";
                        case 148: return "[CCH]";
                        case 149: return "[MW]";
                        case 150: return "[SPA]";
                        case 151: return "[EPA]";
                        case 152: return "[SOS]";
                        case 153: return "[SGCI]";
                        case 154: return "[SCI]";
                        case 155: return "[CSI]";
                        case 156: return "[ST]";
                        case 157: return "[OSC]";
                        case 158: return "[PM]";
                        case 159: return "[APC]";
                        case 65279: return "[byte-order-mark]";
                        case 65533: return "[?]";
                        default: throw new ThisShouldNeverHappenException("nonprintable char not found: [??-" + i + "]");
                    }

                default:
                    // special case - apostrophe(')
                    if (c == '\'')
                        return "['\\\''-" + i + "]";

                    return "['" + c + "'-" + i + "]";
            }
        }

        /// <summary>
        /// Determines a <c>char</c>'s Horseshoe.NET category
        /// </summary>
        /// <param name="c">A <c>char</c></param>
        /// <returns>The <c>char</c>'s <c>CharCategory</c></returns>
        public static CharCategory GetCharCategory(char c)
        {
            return GetCharCategoryByIndex((int)c);
        }

        /// <summary>
        /// Determines a <c>char</c>'s Horseshoe.NET category
        /// </summary>
        /// <param name="index">A <c>char</c> index</param>
        /// <returns>The <c>char</c>'s <c>CharCategory</c></returns>
        public static CharCategory GetCharCategoryByIndex(int index)
        {
            switch (index)
            {
                // set 1 - whitespaces (incl. newlines)

                // set 1a - spaces
                case 32: // ' ' - space
                case 160:  // '\u00A0' - non-breaking space
                    return CharCategory.Spaces;

                // set 1b - other whitespaces
                case 9:  // '\t' - horixontal tab
                case 13: // '\r' - carriage return
                case 10: // '\n' - line feed
                    return CharCategory.OtherWhitespaces;

                // set 2 - nonprintable chars e.g. control chars (except 9, 10 and 13 whitespaces)

                // set 2a - ASCII nonprintables
                case 0: // NUL
                case 1: // SOH
                case 2: // STX
                case 3: // ETX
                case 4: // EOT
                case 5: // ENQ
                case 6: // ACK
                case 7: // BEL
                case 8: // BS
                case 11: // VT
                case 12: // FF
                case 14: // SO
                case 15: // SI
                case 16: // DLE
                case 17: // DC1
                case 18: // DC2
                case 19: // DC3
                case 20: // DC4
                case 21: // NAK
                case 22: // SYN
                case 23: // EDB
                case 24: // CAN
                case 25: // EM
                case 26: // SUB
                case 27: // ESC
                case 28: // FS
                case 29: // GS
                case 30: // RS
                case 31: // US
                case 127: // DEL

                // set 2b - Unicode nonprintables
                case 128: // PAD
                case 129: // HOP
                case 130: // BPH
                case 131: // NBH
                case 132: // IND
                case 133: // NEL
                case 134: // SSA
                case 135: // ESA
                case 136: // HTS
                case 137: // HTJ
                case 138: // VTS
                case 139: // PLD
                case 140: // PLU
                case 141: // RI
                case 142: // SS2
                case 143: // SS3
                case 144: // DCS
                case 145: // PU1
                case 146: // PU2
                case 147: // STS
                case 148: // CCH
                case 149: // MW
                case 150: // SPA
                case 151: // EPA
                case 152: // SOS
                case 153: // SGCI
                case 154: // SCI
                case 155: // CSI
                case 156: // ST
                case 157: // OSC
                case 158: // PM
                case 159: // APC
                case 65279: // byte order mark
                case 65533: // unicode replacement character (�)
                    return CharCategory.Nonprintables;

                // set 3 - printable chars

                default:
                    if (index >= 48 && index <= 57)
                        return CharCategory.AsciiNumeric;
                    if ((index >= 65 && index <= 90) || (index >= 97 && index <= 122))
                        return CharCategory.AsciiAlphabetic;
                    if (index <= 127)
                        return CharCategory.AsciiSymbols;
                    else
                        return CharCategory.UnicodePrintables;
            }
        }

        /// <summary>
        /// Inserts spaces into title case text such as C# object property names.
        /// </summary>
        /// <param name="titleCaseText">A text <c>string</c>.</param>
        /// <returns>The altered text.</returns>
        public static string SpaceOutTitleCase(string titleCaseText)
        {
            if (titleCaseText == null)
                return string.Empty;
            var sb = new StringBuilder();
            bool charIsLetter;
            bool charIsLowerCase;
            bool charIsUpperCase;
            bool charIsDigit;
            //bool charIsWhitespace;
            //bool charIsSpecial;
            bool lastCharWasLetter = false;
            bool lastCharWasLowerCase = false;
            bool lastCharWasUpperCase = false;
            bool lastCharWasDigit = false;
            //bool lastCharWasWhitespace = false;
            //bool lastCharWasSpecial = false;
            for (int i = 0; i < titleCaseText.Length; i++)
            {
                char c = titleCaseText[i];

                charIsLetter = char.IsLetter(c);
                charIsLowerCase = false;
                charIsUpperCase = false;
                charIsDigit = false;
                //charIsWhitespace = false;
                //charIsSpecial = false;

                if (charIsLetter)
                {
                    charIsLowerCase = char.IsLower(c);
                    charIsUpperCase = char.IsUpper(c);
                }
                else if (char.IsDigit(c))
                {
                    charIsDigit = true;
                }
                //else if (char.IsWhiteSpace(c))
                //{
                //    charIsWhitespace = true;
                //}
                //else
                //{
                //    charIsSpecial = true;
                //}

                if (i > 0)
                {
                    //                         v
                    // case #1 -- MyString > My String
                    //
                    if (lastCharWasLowerCase && charIsUpperCase)
                    {
                        sb.Append(" ");
                    }

                    //                                v
                    // case #2 -- MySOAString > My SOA String
                    //                   012345678910
                    else if (lastCharWasUpperCase && charIsUpperCase && titleCaseText.Length >= i + 2 && char.IsLower(titleCaseText[i + 1]))
                    {
                        sb.Append(" ");
                    }

                    //                            v   v
                    // case #3 -- My123String > My 123 String
                    //
                    else if ((lastCharWasDigit && charIsLetter) || (lastCharWasLetter && charIsDigit))
                    {
                        sb.Append(" ");
                    }
                }

                sb.Append(c);
                lastCharWasLowerCase = charIsLowerCase;
                lastCharWasUpperCase = charIsUpperCase;
                lastCharWasLetter = charIsLetter;
                lastCharWasDigit = charIsDigit;
                //lastCharWasWhitespace = charIsWhitespace;
                //lastCharWasSpecial = charIsSpecial;
            }
            return sb.ToString();
        }

        /// <inheritdoc cref="TextUtilAbstractions.ConvertToSecureString"/>
        public static SecureString ConvertToSecureString(string unsecureString)
        {
            return TextUtilAbstractions.ConvertToSecureString(unsecureString);
        }

        /// <inheritdoc cref="TextUtilAbstractions.ConvertToUnsecureString"/>
        public static string ConvertToUnsecureString(SecureString secureString)
        {
            return TextUtilAbstractions.ConvertToUnsecureString(secureString);
        }

        /// <summary>
        /// Encodes a text <c>string</c> as HTML.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <returns>An HTML <c>string</c></returns>
        public static string ConvertToHtml(string text)
        {
            if (text == null)
                return string.Empty;
            var html = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "<br />").Replace("\n", "<br />");
            return html;
        }

        /// <inheritdoc cref="TextUtilAbstractions.IsAsciiPrintable(char, bool, bool, bool)"/>
        public static bool IsAsciiPrintable(char c, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return TextUtilAbstractions.IsAsciiPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines);
        }

        /// <inheritdoc cref="TextUtilAbstractions.IsAsciiPrintable(string, bool, bool, bool)"/>
        public static bool IsAsciiPrintable(string text, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return TextUtilAbstractions.IsAsciiPrintable(text, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines);
        }

        /// <inheritdoc cref="TextUtilAbstractions.IsPrintable(char, bool, bool, bool)"/>
        public static bool IsPrintable(char c, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return TextUtilAbstractions.IsPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines);
        }

        /// <inheritdoc cref="TextUtilAbstractions.IsPrintable(string, bool, bool, bool)"/>
        public static bool IsPrintable(string text, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return TextUtilAbstractions.IsPrintable(text, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a control.
        /// </summary>
        /// <param name="c">a char</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as controls. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as controls. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsAsciiControl(char c, bool excludeTabs = false, bool excludeNewlines = false)
        {
            if (excludeTabs && c == 9)
                return false;
            if (excludeNewlines && (c == 10 || c == 13))
                return false;
            return (c >= 0 && c <= 31) || c == 127;
        }

        /// <inheritdoc cref="TextUtilAbstractions.GetProvider(IFormatProvider, string)"/>
        public static IFormatProvider GetProvider(IFormatProvider provider, string locale)
        {
            return TextUtilAbstractions.GetProvider(provider, locale);
        }

        internal static string DumpDatum(object o)
        {
            if (o == null || o is DBNull) return "[null]";
            if (o is string str) return "\"" + str + "\"";
            return o.ToString();
        }
    }
}
