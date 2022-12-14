using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// Generates a long string revealing the constituent <c>char</c>s in <c>text</c> 
        /// (e.g. alphanumeric, controls, new lines, etc.).
        /// </summary>
        /// <param name="text">The text to reveal.</param>
        /// <param name="options">Customizations for revealing <c>char</c>s and <c>string</c>s.</param>
        /// <returns>Revealed text e.g. chars, controls, new lines, etc.</returns>
        public static string Reveal(string text, RevealOptions options = null)
        {
            options = options ?? new RevealOptions();

            if (text == null)
                return options.ValueIfNull;
            if (text.Length == 0)
                return options.ValueIfEmpty;
            var whiteSpaces = CharLib.AllWhitespaces;
            if (text.All(c => whiteSpaces.Contains(c)))
                return options.ValueIfWhitespace;

            var sb = new StringBuilder();
            ReadOnlySpan<char> span = text.AsSpan();

            // bulk reveal chars
            foreach (var c in span)
            {
                sb.Append(Reveal(c, options));
            }

            // handle new-lines
            if ((options.CharsToReveal & RevealCharCategory.Whitespaces) == RevealCharCategory.Whitespaces && (options.WhitespacesToReveal & WhitespacePolicy.IncludeNewLines) == WhitespacePolicy.IncludeNewLines)
            {
                sb.Replace(options.ValueIfCr + options.ValueIfLf, options.ValueIfCrLf);
                if (options.PreserveNewLines)
                {
                    sb.Replace(options.ValueIfLf, options.ValueIfLf + "\n");
                    sb.Replace(options.ValueIfCrLf, options.ValueIfCrLf + "\r\n");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a string with <c>char</c> details depending on type e.g. alphanumeric, control, whitespace, etc.
        /// </summary>
        /// <param name="c">A <c>char</c> to reveal.</param>
        /// <param name="options">Customizations for revealing <c>char</c>s and <c>string</c>s.</param>
        /// <returns>Revealed <c>char</c> details.</returns>
        public static string Reveal(char c, RevealOptions options = null)
        {
            int i = c;
            options = options ?? new RevealOptions();
            var all = options.CharsToReveal == RevealCharCategory.All;

            // group 1 - whitespaces and newlines
            if (IsWhitespace(c, extendedASCII: true))
            {
                if ((options.CharsToReveal & RevealCharCategory.Whitespaces) == RevealCharCategory.Whitespaces)
                {
                    switch (c)
                    {
                        case ' ':
                            if (all || (options.WhitespacesToReveal & WhitespacePolicy.IncludeASCIISpace) == WhitespacePolicy.IncludeASCIISpace)
                                return options.ValueIfSpace;
                            return " ";
                        case '\u00A0':  // non-breaking space
                            if (all || (options.WhitespacesToReveal & WhitespacePolicy.IncludeNonbreakingSpace) == WhitespacePolicy.IncludeNonbreakingSpace)
                                return options.ValueIfNbSpace;
                            return "\u00A0";
                        case '\t':
                            if (all || (options.WhitespacesToReveal & WhitespacePolicy.IncludeTab) == WhitespacePolicy.IncludeTab)
                                return options.ValueIfTab;
                            return "\t";
                        case '\r':
                            if (all || (options.WhitespacesToReveal & WhitespacePolicy.IncludeNewLines) == WhitespacePolicy.IncludeNewLines)
                                return options.ValueIfCr;
                            return "\r";
                        case '\n':
                            if (all || (options.WhitespacesToReveal & WhitespacePolicy.IncludeNewLines) == WhitespacePolicy.IncludeNewLines)
                                return options.ValueIfLf;
                            return "\n";
                    }
                }
                return new string(c, 1);
            }

            // group 2 - ASCII printables
            if (IsASCIIPrintable(c))
            {
                if ((options.CharsToReveal & RevealCharCategory.ASCIIChars) == RevealCharCategory.ASCIIChars)
                {
                    // special case - apostrophe(')
                    if (c == '\'')
                        return "['\\\''-" + i + "]";

                    return "['" + c + "'-" + i + "]";
                }
                return new string(c, 1);
            }

            // group 3 - control chars (except whitespaces)
            if (char.IsControl(c) && !c.In(9, 10, 13))
            {
                if ((options.CharsToReveal & RevealCharCategory.ControlChars) == RevealCharCategory.ControlChars)
                {
                    switch (i)
                    {
                        case 0:
                            return "[NUL]";
                        case 1:
                            return "[SOH]";
                        case 2:
                            return "[STX]";
                        case 3:
                            return "[ETX]";
                        case 4:
                            return "[EOT]";
                        case 5:
                            return "[ENQ]";
                        case 6:
                            return "[ACK]";
                        case 7:
                            return "[BEL]";
                        case 8:
                            return "[BS]";
                        case 11:
                            return "[VT]";
                        case 12:
                            return "[FF]";
                        case 14:
                            return "[SO]";
                        case 15:
                            return "[SI]";
                        case 16:
                            return "[DLE]";
                        case 17:
                            return "[DC1]";
                        case 18:
                            return "[DC2]";
                        case 19:
                            return "[DC3]";
                        case 20:
                            return "[DC4]";
                        case 21:
                            return "[NAK]";
                        case 22:
                            return "[SYN]";
                        case 23:
                            return "[EDB]";
                        case 24:
                            return "[CAN]";
                        case 25:
                            return "[EM]";
                        case 26:
                            return "[SUB]";
                        case 27:
                            return "[ESC]";
                        case 28:
                            return "[FS]";
                        case 29:
                            return "[GS]";
                        case 30:
                            return "[RS]";
                        case 31:
                            return "[US]";
                        case 127:
                            return "[DEL]";
                        case 128:
                            return "[PAD]";
                        case 129:
                            return "[HOP]";
                        case 130:
                            return "[BPH]";
                        case 131:
                            return "[NBH]";
                        case 132:
                            return "[IND]";
                        case 133:
                            return "[NEL]";
                        case 134:
                            return "[SSA]";
                        case 135:
                            return "[ESA]";
                        case 136:
                            return "[HTS]";
                        case 137:
                            return "[HTJ]";
                        case 138:
                            return "[VTS]";
                        case 139:
                            return "[PLD]";
                        case 140:
                            return "[PLU]";
                        case 141:
                            return "[RI]";
                        case 142:
                            return "[SS2]";
                        case 143:
                            return "[SS3]";
                        case 144:
                            return "[DCS]";
                        case 145:
                            return "[PU1]";
                        case 146:
                            return "[PU2]";
                        case 147:
                            return "[STS]";
                        case 148:
                            return "[CCH]";
                        case 149:
                            return "[MW]";
                        case 150:
                            return "[SPA]";
                        case 151:
                            return "[EPA]";
                        case 152:
                            return "[SOS]";
                        case 153:
                            return "[SGCI]";
                        case 154:
                            return "[SCI]";
                        case 155:
                            return "[CSI]";
                        case 156:
                            return "[ST]";
                        case 157:
                            return "[OSC]";
                        case 158:
                            return "[PM]";
                        case 159:
                            return "[APC]";
                        default:
                            return "[ctrl-" + i + "]"; // this should never happen
                    }
                }
                return new string(c, 1);
            }

            // group 4 - all other chars (e.g. extended ASCII, Unicode including nonprintables)
            if ((options.CharsToReveal & RevealCharCategory.Others) == RevealCharCategory.Others)
            {
                return "['" + c + "'-" + i + "]";
            }
            return new string(c, 1);
        }

        /// <summary>
        /// Same as <c>(obj?.ToString() ?? "null")</c>.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>A text representation of <c>object</c>.</returns>
        public static string Reveal(object obj)
        {
            if (obj is string objString)
                return Reveal(objString);
            return Reveal(obj?.ToString());
        }


        //public static string AppendIf(bool condition, string text, string textToAppend)
        //{
        //    return condition
        //        ? text + textToAppend
        //        : text;
        //}

        //public static string TrimLeading(string text, params char[] chars)
        //{
        //    if (chars == null) 
        //        return text;
        //    while (chars.Contains(text.First()))
        //    {
        //        text = text.Substring(1);
        //    }
        //    return text;
        //}

        //public static string TrimTrailing(string text, params char[] chars)
        //{
        //    if (chars == null) 
        //        return text;
        //    while (chars.Contains(text.Last()))
        //    {
        //        text = text.Substring(1);
        //    }
        //    return text;
        //}

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

        /// <summary>
        /// Creates a <c>SecureString</c> instance from text.
        /// </summary>
        /// <param name="unsecureString">A text <c>string</c>.</param>
        /// <returns>A <c>SecureString</c>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SecureString ConvertToSecureString(string unsecureString)
        {
            if (unsecureString == null) throw new ArgumentNullException(nameof(unsecureString));

            var secureString = new SecureString();
            foreach (char c in unsecureString)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Restores a <c>string</c> from a <c>SecureString</c>.
        /// </summary>
        /// <param name="secureString">A <c>SecureString</c>.</param>
        /// <returns>A <c>string</c>.</returns>
        /// <remarks>
        /// ref: https://blogs.msdn.microsoft.com/fpintos/2009/06/12/how-to-properly-convert-securestring-to-string/
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null) throw new ArgumentNullException(nameof(secureString));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
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

        /// <summary>
        /// Returns <c>true</c> if <c>text</c> contains only printable chars
        /// </summary>
        /// <param name="text">a text string</param>
        /// <param name="spacesAreConsideredPrintable"><c>true</c> to consider spaces as printable, <c>false</c> is the default indicating that Horseshoe.NET sees whitespaces as a separate category</param>
        /// <param name="tabsAreConsideredPrintable"><c>true</c> to consider tabs as printable, <c>false</c> is the default indicating that Horseshoe.NET sees whitespaces as a separate category</param>
        /// <param name="newLinesAreConsideredPrintable"><c>true</c> to consider new lines as printable, <c>false</c> is the default indicating that Horseshoe.NET sees whitespaces as a separate category</param>
        /// <param name="extendedASCII"><c>true</c> if spaces should include non-breaking spaces</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsASCIIPrintable(string text, bool spacesAreConsideredPrintable = false, bool tabsAreConsideredPrintable = false, bool newLinesAreConsideredPrintable = false, bool extendedASCII = false)
        {
            return text.All(c => IsASCIIPrintable(c, spacesAreConsideredPrintable: spacesAreConsideredPrintable, tabsAreConsideredPrintable: tabsAreConsideredPrintable, newLinesAreConsideredPrintable: newLinesAreConsideredPrintable, extendedASCII: extendedASCII));
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a printable char
        /// </summary>
        /// <param name="c">a char</param>
        /// <param name="spacesAreConsideredPrintable"><c>true</c> to consider spaces as printable, <c>false</c> is the default indicating that Horseshoe.NET sees whitespaces as a separate category</param>
        /// <param name="tabsAreConsideredPrintable"><c>true</c> to consider tabs as printable, <c>false</c> is the default indicating that Horseshoe.NET sees whitespaces as a separate category</param>
        /// <param name="newLinesAreConsideredPrintable"><c>true</c> to consider new lines as printable, <c>false</c> is the default indicating that Horseshoe.NET sees whitespaces as a separate category</param>
        /// <param name="extendedASCII"><c>true</c> if spaces should include non-breaking spaces</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsASCIIPrintable(char c, bool spacesAreConsideredPrintable = false, bool tabsAreConsideredPrintable = false, bool newLinesAreConsideredPrintable = false, bool extendedASCII = false)
        {
            if (c == 9)
                return tabsAreConsideredPrintable;
            if (c.In(10, 13))
                return newLinesAreConsideredPrintable;
            if (c == 32 || (extendedASCII && c == 160))
                return spacesAreConsideredPrintable;
            return (c >= 33 && c <= 126) || (extendedASCII && c >= 161 && c <= 255);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a control
        /// </summary>
        /// <param name="c">a char</param>
        /// <param name="spacesAreConsideredControls"><c>true</c> matches <c>char.IsControl()</c> behavior, <c>false</c> is the default Horseshoe.NET behavior</param>
        /// <param name="tabsAreConsideredControls"><c>true</c> matches <c>char.IsControl()</c> behavior, <c>false</c> is the default Horseshoe.NET behavior</param>
        /// <param name="newLinesAreConsideredControls"><c>true</c> matches <c>char.IsControl()</c> behavior, <c>false</c> is the default Horseshoe.NET behavior</param>
        /// <param name="extendedASCII"><c>true</c> if extended ASCII controls should be included</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsASCIIControl(char c, bool spacesAreConsideredControls = false, bool tabsAreConsideredControls = false, bool newLinesAreConsideredControls = false, bool extendedASCII = false)
        {
            if (c == 9)
                return tabsAreConsideredControls;
            if (c.In(10, 13))
                return newLinesAreConsideredControls;
            if (c == 32)
                return spacesAreConsideredControls;
            return (c >= 0 && c <= 31) || c == 127 || (extendedASCII && c >= 128 && c <= 159);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a space, new line or tab (non-breaking space included if <c>extendedASCII == true</c>)
        /// </summary>
        /// <param name="c">a char</param>
        /// <param name="extendedASCII"><c>true</c> if non-breaking space should be included</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsWhitespace(char c, bool extendedASCII = false)
        {
            return c.In(9, 10, 13, 32) || (extendedASCII && c == 160);
        }

        internal static IFormatProvider GetProvider(IFormatProvider provider, string locale)
        {
            if (provider != null)
                return provider;
            if (locale == null)
                return null;
            return CultureInfo.GetCultureInfo(locale);
        }

        internal static string DumpDatum(object o)
        {
            if (o == null || o is DBNull) return "[null]";
            if (o is string str) return "\"" + str + "\"";
            return o.ToString();
        }
    }
}
