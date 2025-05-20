using System;
using System.Collections.Generic;
using System.Globalization;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Utilities for displaying, grouping and searching <c>char</c>s
    /// </summary>
    public static class CharUtil
    {
        /// <summary>
        /// Gets all the <c>char</c>s (range depends on <c>maxCharIndex</c>) that meet the supplied criteria.
        /// </summary>
        /// <param name="charCategory">Optional <c>CharCategory</c> criteria, e.g. <c>CharCategory.Alpha | CharCategory.Numeric</c></param>
        /// <param name="unicodeCategory">Optional <c>UnicodeCategory</c> criteria, e.g. <c>UnicodeCategory.ConnectorPunctuation</c></param>
        /// <param name="isASCII">Optional i.e. <c>true</c>, <c>false</c> or <c>null</c> (<c>null</c> means not filtered by this attribute)</param>
        /// <param name="script">Optional script or Unicode block name including partial names, i.e. "latin"</param>
        /// <param name="maxCharIndex">Max number of char indices to search, default is <c>5,000</c>, actual max is <c>65,535</c></param>
        /// <returns>A list of matching <c>char</c>s</returns>
        public static IEnumerable<char> FilterCharsBy(CharCategory? charCategory = null, UnicodeCategory? unicodeCategory = null, bool? isASCII = null, string script = null, int maxCharIndex = 5000)
        {
            if (maxCharIndex < 0)
                throw new ValidationException("maxCharIndex may not be negative: " + maxCharIndex);

            var list = new List<char>();
            char c;
            for (int i = 0, max = Math.Min(maxCharIndex, 65535); i <= max; i++)
            {
                c = (char)i;
                LookupDetails(c, out CharCategory _category, out UnicodeCategory _unicodeCategory, out bool _isASCII, out _, out string _script);
                if (charCategory != null && (charCategory & _category) != _category)
                    continue;
                if (unicodeCategory != null && unicodeCategory != _unicodeCategory)
                    continue;
                if (isASCII != null && isASCII != _isASCII)
                    continue;
                if (script != null && _script != null && _script.IndexOf(script, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;
                list.Add(c);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets all the <c>char</c> indices (range depends on <c>maxCharIndex</c>) that meet the supplied criteria.
        /// </summary>
        /// <param name="charCategory">Optional <c>CharCategory</c> criteria, e.g. <c>CharCategory.Alpha | CharCategory.Numeric</c></param>
        /// <param name="unicodeCategory">Optional <c>UnicodeCategory</c> criteria, e.g. <c>UnicodeCategory.ConnectorPunctuation</c></param>
        /// <param name="isASCII">Optional i.e. <c>true</c>, <c>false</c> or <c>null</c> (<c>null</c> means not filtered by this attribute)</param>
        /// <param name="script">Optional script or Unicode block name including partial names, i.e. "latin"</param>
        /// <param name="maxCharIndex">Max number of char indices to search, default is <c>5,000</c>, actual max is <c>65,535</c></param>
        /// <returns>A list of matching <c>char</c> indices</returns>
        public static IEnumerable<int> FilterCharsIndicesBy(CharCategory? charCategory = null, UnicodeCategory? unicodeCategory = null, bool? isASCII = null, string script = null, int maxCharIndex = 5000)
        {
            if (maxCharIndex < 0)
                throw new ValidationException("maxCharIndex may not be negative: " + maxCharIndex);

            var list = new List<int>();
            for (int i = 0, max = Math.Min(maxCharIndex, 65535); i <= max; i++)
            {
                LookupDetails((char)i, out CharCategory _category, out UnicodeCategory _unicodeCategory, out bool _isASCII, out _, out string _script);
                if (charCategory != null && (charCategory & _category) != _category)
                    continue;
                if (unicodeCategory != null && unicodeCategory != _unicodeCategory)
                    continue;
                if (isASCII != null && isASCII != _isASCII)
                    continue;
                if (script != null && _script != null && _script.IndexOf(script, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;
                list.Add(i);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets all the chars (range depends on <c>maxCharIndex</c>) that meet the supplied criteria.
        /// </summary>
        /// <param name="charCategory">Optional <c>CharCategory</c> criteria, e.g. <c>CharCategory.Alpha | CharCategory.Numeric</c></param>
        /// <param name="unicodeCategory">Optional <c>UnicodeCategory</c> criteria, e.g. <c>UnicodeCategory.ConnectorPunctuation</c></param>
        /// <param name="isASCII">Optional i.e. <c>true</c>, <c>false</c> or <c>null</c> (<c>null</c> means not filtered by this attribute)</param>
        /// <param name="script">Optional script or Unicode block name including partial names, i.e. "latin"</param>
        /// <param name="maxCharIndex">Max number of char indices to search, default is <c>5,000</c>, actual max is <c>65,535</c></param>
        /// <returns>A list of matching <c>CharInfo</c>s</returns>
        public static IEnumerable<CharInfo> FilterCharInfosBy(CharCategory? charCategory = null, UnicodeCategory? unicodeCategory = null, bool? isASCII = null, string script = null, int maxCharIndex = 5000)
        {
            if (maxCharIndex < 0)
                throw new ValidationException("maxCharIndex may not be negative: " + maxCharIndex);

            var list = new List<CharInfo>();
            char c;
            for (int i = 0, max = Math.Min(maxCharIndex, 65535); i <= max; i++)
            {
                c = (char)i;
                LookupDetails(c, out CharCategory _category, out UnicodeCategory _unicodeCategory, out bool _isASCII, out string _revealedValue, out string _script);
                if (charCategory != null && (charCategory & _category) != _category)
                    continue;
                if (unicodeCategory != null && unicodeCategory != _unicodeCategory)
                    continue;
                if (isASCII != null && isASCII != _isASCII)
                    continue;
                if (script != null && _script != null && _script.IndexOf(script, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;
                list.Add(new CharInfo(c, _category, _unicodeCategory, _revealedValue, _script));
            }
            return list.ToArray();
        }

        private static bool IsAscii(char c)
        {
            return c <= 127;
        }

        /// <summary>
        /// Looks up character info such as Unicode attributes based on 
        /// </summary>
        /// <param name="c">A <c>char</c></param>
        /// <param name="category">The basic category of which this <c>char</c> is a member</param>
        /// <param name="unicodeCategory">The .NET Unicode category assigned to this <c>char</c>.</param>
        /// <param name="isASCII">Whether this <c>char</c> falls in the lower 128 original ASCII values</param>
        /// <param name="displayValue">The human readable display value of this <c>char</c>.</param>
        /// <param name="script">The Unicode block or script to which this <c>char</c> belongs e.g. Latin-1 Supplement, Greek, etc.</param>
        public static void LookupDetails(char c, out CharCategory category, out UnicodeCategory unicodeCategory, out bool isASCII, out string displayValue, out string script)
        {
            unicodeCategory = char.GetUnicodeCategory(c);
            displayValue = "'" + c + "'[" + (int)c + "]";
            script = null;
            
            if (IsAscii(c))
            {
                isASCII = true;

                // whitespace incl. ASCII (space [32], tab [9], carriage return [13], line feed [10]) and Unicode (non-breaking space [160])
                switch (c)
                {
                    case ' ':   // 32 - space
                        category = CharCategory.WhitespacesSansNewLines;
                        displayValue = TextConstants.Space;
                        return;
                    case '\t':  // 9 - tab
                        category = CharCategory.WhitespacesSansNewLines;
                        displayValue = TextConstants.HorizontalTab;
                        return;
                    case '\r':  // 13 - carriage return
                        category = CharCategory.NewLines;
                        displayValue = TextConstants.CarriageReturn;
                        return;
                    case '\n':  // 10 - line feed
                        category = CharCategory.NewLines;
                        displayValue = TextConstants.LineFeed;
                        return;
                    case '\u00A0':  // 160 - non-breaking space
                        category = CharCategory.WhitespacesSansNewLines;
                        displayValue = TextConstants.NonBreakingSpace;
                        return;
                }

                // ASCII controls
                category = CharCategory.Nonprintable;
                switch (c)
                {
                    case '\x0000':  // 0
                        displayValue = "[NUL]";
                        return;
                    case '\x0001':  // 1
                        displayValue = "[SOH]";
                        return;
                    case '\x0002':  // 2
                        displayValue = "[STX]";
                        return;
                    case '\x0003':  // 3
                        displayValue = "[ETX]";
                        return;
                    case '\x0004':  // 4
                        displayValue = "[EOT]";
                        return;
                    case '\x0005':  // 5
                        displayValue = "[ENQ]";
                        return;
                    case '\x0006':  // 6
                        displayValue = "[ACK]";
                        return;
                    case '\x0007':  // 7
                        displayValue = "[BEL]";
                        return;
                    case '\x0008':  // 8
                        displayValue = "[BS]";
                        return;
                    //case '\x0009':  // 9 - whitespace - tab
                    //case '\x000A':  // 10 - whitespace - line feed
                    case '\x000B':  // 11
                        displayValue = "[VT]";
                        return;
                    case '\x000C':  // 12
                        displayValue = "[FF]";
                        return;
                    //case '\x000D':  // 13 - whitespace - carriage return
                    case '\x000E':  // 14
                        displayValue = "[SO]";
                        return;
                    case '\x000F':  // 15
                        displayValue = "[SI]";
                        return;
                    case '\x0010':  // 16
                        displayValue = "[DLE]";
                        return;
                    case '\x0011':  // 17
                        displayValue = "[DC1]";
                        return;
                    case '\x0012':  // 18
                        displayValue = "[DC2]";
                        return;
                    case '\x0013':  // 19
                        displayValue = "[DC3]";
                        return;
                    case '\x0014':  // 20
                        displayValue = "[DC4]";
                        return;
                    case '\x0015':  // 21
                        displayValue = "[NAK]";
                        return;
                    case '\x0016':  // 22
                        displayValue = "[SYN]";
                        return;
                    case '\x0017':  // 23
                        displayValue = "[EDB]";
                        return;
                    case '\x0018':  // 24
                        displayValue = "[CAN]";
                        return;
                    case '\x0019':  // 25
                        displayValue = "[EM]";
                        return;
                    case '\x001A':  // 26
                        displayValue = "[SUB]";
                        return;
                    case '\x001B':  // 27
                        displayValue = "[ESC]";
                        return;
                    case '\x001C':  // 28
                        displayValue = "[FS]";
                        return;
                    case '\x001D':  // 29
                        displayValue = "[GS]";
                        return;
                    case '\x001E':  // 30
                        displayValue = "[RS]";
                        return;
                    case '\x001F':  // 31
                        displayValue = "[US]";
                        return;
                    case '\x007F':  // 127
                        displayValue = "[DEL]";
                        return;
                }

                script = "Latin-Basic";

                // ASCII numbers, letters and symbols
                //  0 - 9
                if (c >= 48 && c <= 57)
                {
                    category = CharCategory.Numeric;
                    return;
                }

                //   A - Z                   a - z
                if ((c >= 65 && c <= 90) || (c >= 97 && c <= 122))
                {
                    category = CharCategory.Alpha;
                    return;
                }

                category = CharCategory.Symbol;
                if (c == '\'')
                    displayValue = "'\\\''[" + (int)c + "]";
                return;
            }

            isASCII = false;

            // Unicode controls and known non-printables
            category = CharCategory.Nonprintable;
            switch (c)
            {
                case '\x0080':  // 128
                    displayValue = "[PAD]";
                    return;
                case '\x0081':  // 129
                    displayValue = "[HOP]";
                    return;
                case '\x0082':  // 130
                    displayValue = "[BPH]";
                    return;
                case '\x0083':  // 131
                    displayValue = "[NBH]";
                    return;
                case '\x0084':  // 132
                    displayValue = "[IND]";
                    return;
                case '\x0085':  // 133
                    displayValue = "[NEL]";
                    return;
                case '\x0086':  // 134
                    displayValue = "[SSA]";
                    return;
                case '\x0087':  // 135
                    displayValue = "[ESA]";
                    return;
                case '\x0088':  // 136
                    displayValue = "[HTS]";
                    return;
                case '\x0089':  // 137
                    displayValue = "[HTJ]";
                    return;
                case '\x008A':  // 138
                    displayValue = "[VTS]";
                    return;
                case '\x008B':  // 139
                    displayValue = "[PLD]";
                    return;
                case '\x008C':  // 140
                    displayValue = "[PLU]";
                    return;
                case '\x008D':  // 141
                    displayValue = "[RI]";
                    return;
                case '\x008E':  // 142
                    displayValue = "[SS22]";
                    return;
                case '\x008F':  // 143
                    displayValue = "[SS3]";
                    return;
                case '\x0090':  // 144
                    displayValue = "[DCS]";
                    return;
                case '\x0091':  // 145
                    displayValue = "[PU1]";
                    return;
                case '\x0092':  // 146
                    displayValue = "[PU2]";
                    return;
                case '\x0093':  // 147
                    displayValue = "[STS]";
                    return;
                case '\x0094':  // 148
                    displayValue = "[CCH]";
                    return;
                case '\x0095':  // 149
                    displayValue = "[MW]";
                    return;
                case '\x0096':  // 150
                    displayValue = "[SPA]";
                    return;
                case '\x0097':  // 151
                    displayValue = "[EPA]";
                    return;
                case '\x0098':  // 152
                    displayValue = "[SOS]";
                    return;
                case '\x0099':  // 153
                    displayValue = "[SGCI]";
                    return;
                case '\x009A':  // 154
                    displayValue = "[SCI]";
                    return;
                case '\x009B':  // 155
                    displayValue = "[CSI]";
                    return;
                case '\x009C':  // 156
                    displayValue = "[ST]";
                    return;
                case '\x009D':  // 157
                    displayValue = "[OSC]";
                    return;
                case '\x009E':  // 158
                    displayValue = "[PM]";
                    return;
                case '\x009F':  // 159
                    displayValue = "[APC]";
                    return;
                case '\x061C':  // 1564
                    displayValue = "[ALM]"; // arabic letter mark
                    return;
                //case '\x00A0':  // 160 - whitespace - non-breaking space
                case '\xFEFF':  // 65279
                    displayValue = "[byte-order-mark]";
                    return;
                case '\xFFFD':  // 65533
                    displayValue = "[?]";
                    return;
            }

            switch (unicodeCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                    category = CharCategory.Alpha;
                    break;
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.OtherNumber:
                    category = CharCategory.Numeric;
                    break;
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.Format:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.OtherNotAssigned:
                case UnicodeCategory.PrivateUse:
                case UnicodeCategory.Surrogate:
                    category = CharCategory.Nonprintable;
                    switch (unicodeCategory)
                    {
                        case UnicodeCategory.EnclosingMark:
                            displayValue = "[enclosing-mark]";
                            break;
                        case UnicodeCategory.Format:
                            displayValue = "[format]";
                            break;
                        case UnicodeCategory.NonSpacingMark:
                            displayValue = "[nonspacing-mark]";
                            break;
                        case UnicodeCategory.OtherNotAssigned:
                            displayValue = "[not-assigned]";
                            break;
                        case UnicodeCategory.PrivateUse:
                            displayValue = "[private-use]";
                            break;
                        case UnicodeCategory.Surrogate:
                            displayValue = "[surrogate]";
                            break;
                    }
                    break;
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.SpacingCombiningMark:
                    category = CharCategory.WhitespacesSansNewLines;
                    switch (unicodeCategory)
                    {
                        case UnicodeCategory.ParagraphSeparator:
                            displayValue = "[paragraph-separator]";
                            break;
                        case UnicodeCategory.SpaceSeparator:
                            displayValue = "[space-separator]";
                            break;
                        case UnicodeCategory.SpacingCombiningMark:
                            displayValue = "[spacing-combining-mark]";
                            break;
                    }
                    break;
                default:  // includes symbols, punctuation, etc.
                    category = CharCategory.Symbol;
                    break;
            }

            // Latin-1 Supplement
            if (c <= 255)
            {
                script = "Latin-1 Supplement";
                if (c == '\u00AD')
                    displayValue = "[soft-hyphen]";
                return;
            }

            // Latin Extended-A
            if (c <= 383)
            {
                script = "Latin Extended-A";
                if (c == 329)
                    script += "; Deprecated";
                else 
                    script += "; European Latin";
                return;
            }

            // Latin Extended-B
            if (c <= 591)
            {
                script = "Latin Extended-B";
                if (c <= 447)
                    script += "; Non-European & historic Latin";
                else if (c == 451)
                {
                    category = CharCategory.Symbol;
                    script += "; African Clicks";
                }
                else if (c <= 460)
                    script += "; Croatian";
                else if (c <= 476)
                    script += "; Pinyin";
                else if (c <= 511)
                    script += "; Phonetic & historic letters";
                else if (c <= 535)
                    script += "; Slovenian & Croatian";
                else if (c <= 540)
                    script += "; Romanian";
                else if (c <= 553)
                    script += "; Miscellaneous";
                else if (c <= 563)
                    script += "; Livonian";
                else if (c <= 566)
                    script += "; Sinology";
                else
                    script += "; Miscellaneous";
                return;
            }

            // Phonetic; Latin; IPA Extensions
            if (c <= 687)  // U+0250 - U+02AF
            {
                script = "Phonetic; Latin; IPA Extensions";
                return;
            }

            // Phonetic; Spacing Modifier Letters
            if (c <= 767)  // U+02B0 - U+02FF
            {
                script = "Phonetic; Spacing Modifier Letters";
                return;
            }

            // Phonetic Extensions; Combining Marks
            if (c <= 879)  // U+0300 - U+036F
            {
                script = "Phonetic Extensions; Combining Marks";
                return;
            }

            // Greek and Coptic
            if (c <= 1023)  // U+0370 - U+03FF
            {
                script = "Greek and Coptic";
                return;
            }

            // Cyrillic
            if (c <= 1279)  // U+0400 - U+04FF
            {
                script = "Cyrillic";
                return;
            }

            // Armenian
            if (c > 1328 && c <= 1423)  // U+0530 - U+058F
            {
                script = "Armenian";
                return;
            }

            // Hebrew (Semitic)
            if (c > 1424 && c <= 1535)  // U+0590 - U+05FF
            {
                script = "Hebrew";
                return;
            }

            // Arabic (Semitic)
            if (c > 1536 && c <= 1791)  // U+0600 - U+06FF
            {
                if ((c >= 1632 && c <= 1641) || (c >= 1776 && c <= 1785))
                    category = CharCategory.Numeric;
                else if (c <= 1567 || c == 1600 || (c >= 1611 && c <= 1645) || c == 1748 || (c >= 1750 && c <= 1773) || c.In(1789, 1790))
                    category = CharCategory.Symbol;
                else
                    category = CharCategory.Alpha;
                script = "Arabic";
                return;
            }

            // Latin Extended Additional
            if (c >= 7680 && c <= 7935)  // U+1E00 - U+1EFF
            {
                script = "Latin Extended Additional";
                return;
            }

            // Greek Extended
            if (c >= 7936 && c <= 8191)
            {
                script = "Greek Extended";
                return;
            }
        }

        /// <summary>
        /// Tries to find the closest matching ASCII <c>char</c>(s) by shape or appearance
        /// </summary>
        /// <param name="c">A <c>char</c></param>
        /// <param name="replacement">An ASCII representation of <c>char</c>, if applicable.</param>
        /// <param name="script">The script / Unicode block containing the replaced <c>char</c></param>
        /// <returns>An ASCII representation of a Unicode <c>char</c></returns>
        public static bool TryFindASCIIReplacement(char c, out string replacement, out string script)
        {
            if (c <= 127)
            {
                replacement = null;
                script = "ASCII";
                return false;
            }

            // iterate the complex conversion tables (see CharLib)
            foreach (var dictKey in CharLib.UnicodeToASCIIConversions.Keys)
            {
                foreach (char _c in CharLib.UnicodeToASCIIConversions[dictKey])
                {
                    if (c == _c)
                    {
                        replacement = dictKey;
                        script = CharInfo.Get(c).Script;
                        return true;
                    }
                }
            }

            replacement = null;
            script = null;
            return false;
        }
    }
}
