using System;
using System.Collections.Generic;
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
                .TrimAll();
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
                ? Crop(sb.ToString(), targetLength, position: HorizontalPosition.Left)
                : Crop(sb.ToString(), targetLength);
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by adding <c>char</c>s to one or both ends of <c>text</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">
        /// The number of characters in the resulting string, equal to the number
        /// of original characters plus any additional padding characters.
        /// </param>
        /// <param name="position">Where in a <c>string</c> to pad.</param>
        /// <param name="padChar">A Unicode padding character.</param>
        /// <returns>A new fixed-length <c>string</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static string Pad(string text, int targetLength, HorizontalPosition position = default, char? padChar = ' ')
        {
            // validation
            if (targetLength < 0)
                throw new ValidationException("The target length must be a positive integer or zero");

            // handle null and oversized text
            if (text == null)
                text = string.Empty;
            else if (text.Length >= targetLength)
                return text;

            switch (position)
            {
                case HorizontalPosition.Right:
                default:
                    return padChar.HasValue
                        ? text.PadRight(targetLength, padChar.Value)
                        : text.PadRight(targetLength);
                case HorizontalPosition.Left:
                    return padChar.HasValue
                        ? text.PadLeft(targetLength, padChar.Value)
                        : text.PadLeft(targetLength);
                case HorizontalPosition.Center:
                    var leftPad = (targetLength - text.Length) / 2;
                    return padChar.HasValue
                        ? text.PadLeft(text.Length + leftPad, padChar.Value).PadRight(targetLength, padChar.Value)
                        : text.PadLeft(text.Length + leftPad).PadRight(targetLength);
            }
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by removing <c>char</c>s from one or both ends of <c>text</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="position">Where in a <c>string</c> to crop.</param>
        /// <param name="truncateMarker">An optional truncation indicator, e.g. <c>"..."</c> or <c>TruncateMarker.Ellipsis</c>.</param>
        /// <returns>A new fixed-length <c>string</c>.</returns>
        /// <exception cref="ValidationException"></exception>
        public static string Crop(string text, int targetLength, HorizontalPosition position = HorizontalPosition.Right, string truncateMarker = TruncateMarker.None)
        {
            // validation
            if (targetLength < 0)
                throw new ValidationException("The target length must be a positive integer or zero");

            if (truncateMarker == null)
                truncateMarker = TruncateMarker.None;
            else if (truncateMarker.Length > targetLength)
                throw new ValidationException("Truncate marker length exceeds target length: " + truncateMarker.Length + " > " + targetLength);
            else if (truncateMarker.Length == targetLength)
                return truncateMarker;

            // handle null and undersized text
            if (text == null)
                text = string.Empty;
            else if (text.Length <= targetLength)
                return text;

            switch (position)
            {
                case HorizontalPosition.Right:
                default:
                    return text.Substring(0, targetLength - truncateMarker.Length) + truncateMarker;
                case HorizontalPosition.Left:
                    return truncateMarker + text.Substring(text.Length - targetLength + truncateMarker.Length);
                case HorizontalPosition.Center:
                    int leftChunk = (targetLength - truncateMarker.Length) / 2;
                    if (leftChunk == 0)  // prefer smaller left pad when uneven except when left is 0
                        leftChunk = 1;
                    int rightChunk = targetLength - leftChunk - truncateMarker.Length;
                    var strb = new StringBuilder(text.Substring(0, leftChunk))
                        .Append(truncateMarker)
                        .Append(text.Substring(text.Length - rightChunk));
                    return strb.ToString();
            }
        }

        /// <summary>
        /// Creates a fixed-length <c>string</c> by either adding or removing <c>char</c>s from one or both ends of <c>text</c>.
        /// </summary>
        /// <param name="text">A text <c>string</c>.</param>
        /// <param name="targetLength">The target length.</param>
        /// <param name="padPosition">Where in a <c>string</c> to pad.</param>
        /// <param name="cropPosition">Where in a <c>string</c> to crop.</param>
        /// <param name="padChar">A Unicode padding character.</param>
        /// <param name="truncateMarker">An optional truncation indicator, e.g. <c>"..."</c> or <c>TruncateMarker.Ellipsis</c>.</param>
        /// <returns>A new fixed-length <c>string</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static string Fit(string text, int targetLength, HorizontalPosition padPosition = default, char padChar = ' ', HorizontalPosition cropPosition = default, string truncateMarker = null)
        {
            // handle null
            if (text == null)
                text = string.Empty;

            if (text.Length < targetLength)
                return Pad(text, targetLength, position: padPosition, padChar: padChar);
            else
                return Crop(text, targetLength, position: cropPosition, truncateMarker: truncateMarker);
        }

        //private static HorizontalPosition FitSwitchPadDirection(HorizontalPosition direction)
        //{
        //    switch (direction)
        //    {
        //        case HorizontalPosition.Left:
        //            return HorizontalPosition.Right;
        //        case HorizontalPosition.Center:
        //            return HorizontalPosition.Center;
        //        case HorizontalPosition.Right:
        //        default:
        //            return HorizontalPosition.Left;
        //    }
        //}

        //private static HorizontalPosition FitSwitchTruncateDirection(HorizontalPosition direction)
        //{
        //    switch (direction)
        //    {
        //        case HorizontalPosition.Left:
        //        case HorizontalPosition.Right:
        //        default:
        //            return HorizontalPosition.Right;
        //        case HorizontalPosition.Center:
        //            return HorizontalPosition.Center;
        //    }
        //}

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
                if ((options.CharCategory & CharCategory.NewLines) == CharCategory.NewLines)
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
        /// Displays a <c>char</c> in <c>string</c> format including the <c>char</c> index, i.e. <c>'c' => ['c'-99]</c>.
        /// </summary>
        /// <param name="c">The <c>char</c> to reveal.</param>
        /// <param name="options">Options for revealing <c>char</c>s.</param>
        /// <returns>A <c>string</c> representation of a <c>char</c> including, depending on options, original <c>char</c> value.</returns>
        public static string RevealChar(char c, RevealOptions options = null)
        {
            var ci = CharInfo.Get(c);
            
            options = options ?? new RevealOptions { CharCategory = CharCategory.All };

            if (!(CollectionUtil.Contains(options.CharsToReveal, c) || (options.CharCategory & ci.Category) == ci.Category))
            {
                return new string(c, 1);
            }
            
            return ci.ToString();
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
        /// Returns <c>true</c> if <c>c</c> represents a printable ASCII <c>char</c>.
        /// </summary>
        /// <param name="c">A char.</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsAsciiPrintable(char c, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            if (c == 32)
                return !excludeSpaces;
            if (c == 9)
                return !excludeTabs;
            if (c == 10 || c == 13)
                return !excludeNewlines;
            return c <= 127 && !char.IsControl(c);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>text</c> contains only printable ASCII <c>char</c>s.
        /// </summary>
        /// <param name="text">A text string.</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsAsciiPrintable(string text, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return text.All(c => IsAsciiPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines));
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a printable <c>char</c>.
        /// </summary>
        /// <param name="c">a char</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsPrintable(char c, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            if (c <= 127)
                return IsAsciiPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines);
            if (c <= 159)
                return false;
            if (c == 160)
                return !excludeSpaces;
            /* 
             * ByteOrderMark          '\uFEFF'   (65279)    
             * UnicodeReplacementChar '\uFFFD' � (65533) 
             * ------------------------------------
             * Source: CharLib.cs > UnicodeNonprintables
             */
            if (c == '\uFEFF' || c == '\uFFFD')
                return false;
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>text</c> contains only printable <c>char</c>s.
        /// </summary>
        /// <param name="text">A text string.</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsPrintable(string text, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return text.All(c => IsPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines));
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

        /// <summary>
        /// Used internally for determining which <c>IFormatProvider</c> to use in certain conversion operations.
        /// </summary>
        /// <param name="provider">An optional format provider.</param>
        /// <param name="locale">An optional locale from which to derive a format provider if one has was not specified.</param>
        /// <returns>A format provider, or <c>null</c>.</returns>
        public static IFormatProvider GetProvider(IFormatProvider provider, string locale)
        {
            if (provider != null)
                return provider;
            if (locale == null)
                return null;
            return CultureInfo.GetCultureInfo(locale);
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection of strings.
        /// </summary>
        /// <param name="text">The <c>string</c> to search match.</param>
        /// <param name="strings">A <c>string</c> collection to search.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(string text, params string[] strings)
        {
            return InIgnoreCase(text, strings as IEnumerable<string>);
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection of strings.
        /// </summary>
        /// <param name="text">The <c>string</c> to search match.</param>
        /// <param name="strings">A <c>string</c> collection to search.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(string text, IEnumerable<string> strings)
        {
            if (strings == null)
                return false;
            foreach (var s in strings)
            {
                if (string.Equals(s, text, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection of strings.
        /// </summary>
        /// <param name="char">The <c>char</c> to search match.</param>
        /// <param name="chars">A <c>char</c> collection to search.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(char @char, params char[] chars)
        {
            if (chars == null)
                return false;
            string charString = new string(@char, 1);
            foreach (var c in chars)
            {
                if (string.Equals(new string(c, 1), charString, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
