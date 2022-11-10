using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Text
{
    // Unicode ref https://en.wikipedia.org/wiki/List_of_Unicode_characters#Basic_Latin
    // Punctuation ref https://en.wikipedia.org/wiki/Punctuation  (stopped right before Armenian)
    public static class CharLib
    {
        /// <summary>
        /// A subset of whitespace characters
        /// </summary>
        public static char[] ASCIIWhitespaces { get; } = new[]
        {//   9    10    13    32
            '\t', '\n', '\r', ' '
        };

        /// <summary>
        /// A subset of whitespace characters
        /// </summary>
        public static char[] SubsetASCIIWhitespacesExceptNewLines => ASCIIWhitespaces
            .Where(c => !c.In(10, 13))
            .ToArray();

        /// <summary>
        /// The subset of ASCII whitespace characters comprising only carriage return (<c>\r</c>) and line feed (<c>\n</c>)
        /// </summary>
        public static char[] SubsetNewLines => ASCIIWhitespaces
            .Where(c => c.In(10, 13))
            .ToArray();

        public static char[] ASCIIAlpha { get; } = new[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        public static char[] SubsetUCaseASCIIAlpha => ASCIIAlpha
            .Where(c => char.IsUpper(c))
            .ToArray();

        public static char[] SubsetLCaseASCIIAlpha => ASCIIAlpha
            .Where(c => char.IsLower(c))
            .ToArray();

        public static char[] ASCIINumeric { get; } = new[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static char[] AllASCIIAlphaNumeric =>
            ArrayUtil.Combine(ASCIIAlpha, ASCIINumeric);

        public static char[] AllUCaseASCIIAlphaNumeric =>
            ArrayUtil.Combine(SubsetUCaseASCIIAlpha, ASCIINumeric);

        public static char[] AllLCaseASCIIAlphaNumeric =>
            ArrayUtil.Combine(SubsetLCaseASCIIAlpha, ASCIINumeric);

        public static char[] ASCIISymbols { get; } = new[]
        {
            // punctuation
            '.', ',', ':', ';', '<', '!', '?', '"', '\'', '[', ']', '(', ')', '{', '}',
            // symbols
            '#', '$', '%', '*', '+', '&', '/', '/', '|', '\\', '^', '_', '`', '~'
        };

        public static char[] AllASCIIPrintables =>
            ArrayUtil.Combine(AllASCIIAlphaNumeric, ASCIISymbols);

        public static char[] AllASCIIPrintablesAndSpace =>
            AllASCIIPrintables.Append(' ');

        public static char[] AllASCIIPrintablesAndWhitespaces =>
            ArrayUtil.Combine(ASCIIWhitespaces, AllASCIIPrintables);

        public static char[] AllASCIIPrintablesAndWhitespacesExceptNewLines =>
            ArrayUtil.Combine(SubsetASCIIWhitespacesExceptNewLines, AllASCIIPrintables);

        public static char[] ASCIINonprintables { get; } = new[]
        {   
            // Controls (in 0 - 31 range, excluding whitespaces)                                       \t (9)    \n (10)                       \r (13)
            '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', '\u0008',                     '\u000B', '\u000C',           '\u000E', '\u000F',
            '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', '\u001E', '\u001F', 
            
            // Control (127)
            '\u007f'
        };

        /// <summary>
        /// A subset of whitespace characters
        /// </summary>
        public static char[] ExtendedASCIIWhitespaces { get; } = new[]
        {
            // Char (160)
            '\u00A0' /* nbsp */
        };

        /// <summary>
        /// A complete set of whitespace characters
        /// </summary>
        public static char[] AllWhitespaces =>
            ArrayUtil.Combine(ASCIIWhitespaces, ExtendedASCIIWhitespaces);

        /// <summary>
        /// A subset of whitespace characters
        /// </summary>
        public static char[] AllWhitespacesExceptNewLines =>
            ArrayUtil.Combine(SubsetASCIIWhitespacesExceptNewLines, ExtendedASCIIWhitespaces);

        public static char[] ExtendedASCIIAlpha { get; } = new[]
        {
            'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 
            'Ç', 
            'Ð',
            'È', 'É', 'Ê', 'Ë',
            'ƒ',
            'Ì', 'Í', 'Î', 'Ï', 
            'Ñ', 
            'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø', 'Œ', 
            'Š', 'ß',
            'Þ', 'þ', 
            'Ù', 'Ú', 'Û', 'Ü',
            'Ý', 'Ÿ',
            'Ž',
            'à', 'á', 'â', 'ã', 'ä', 'å', 'æ',
            'ç', 
            'ð', 
            'è', 'é', 'ê', 'ë',
            'ì', 'í', 'î', 'ï',
            'ñ',
            'ò', 'ó', 'ô', 'õ', 'ö', 'ø', 'œ', 
            'š',
            'ù', 'ú', 'û', 'ü', 
            'ý', 'ÿ', 
            'ž'
        };

        public static char[] SubsetUCaseExtendedASCIIAlpha => ExtendedASCIIAlpha
            .Where(c => char.IsUpper(c))
            .ToArray();

        public static char[] SubsetLCaseExtendedASCIIAlpha => ExtendedASCIIAlpha
            .Where(c => char.IsLower(c))
            .ToArray();

        public static char[] AllExtendedASCIIAlpha =>
            ArrayUtil.Combine(ASCIIAlpha, ExtendedASCIIAlpha);

        public static char[] AllExtendedASCIIAlphaNumeric =>
            ArrayUtil.Combine(AllASCIIAlphaNumeric, ExtendedASCIIAlpha);

        public static char[] AllUCaseExtendedASCIIAlphaNumeric =>
            ArrayUtil.Combine(AllUCaseASCIIAlphaNumeric, SubsetUCaseExtendedASCIIAlpha);

        public static char[] AllLCaseExtendedASCIIAlphaNumeric =>
            ArrayUtil.Combine(AllLCaseASCIIAlphaNumeric, SubsetLCaseExtendedASCIIAlpha);

        public static char[] ExtendedASCIISymbols { get; } = new[]
        {
            // punctuation
            '‚', '„', '…', '¨', '‹', '‘', '’', '“', '”', '–', '—', '›', '¡', '«', '­', '»', '¿', '·',

            // symbols
            '€', '†', 'ˆ', '•', '˜', '™', '¢', '£', '¤', '¥', '¦', '§', '©', '¬', '®', '¯', '°',
            '±', '´', 'µ', '¶', '¸', 

            // math and programming
            '×', '÷', '¼', '½', '¾', '‰',

            // superscripts and subscripts
            'ª', '²', '³', '¹', 'º'
        };

        public static char[] AllExtendedASCIIPrintables =>
            ArrayUtil.Combine(AllASCIIPrintables, ExtendedASCIIAlpha, ExtendedASCIISymbols);

        public static char[] AllExtendedASCIIPrintablesAndSpace =>
            AllExtendedASCIIPrintables.Append(' ');

        public static char[] AllExtendedASCIIPrintablesAndWhitespaces =>
            ArrayUtil.Combine(AllExtendedASCIIPrintables, AllWhitespaces);

        public static char[] AllExtendedASCIIPrintablesAndWhitespacesExceptNewLines =>
            ArrayUtil.Combine(AllExtendedASCIIPrintables, AllWhitespacesExceptNewLines);

        public static char[] ExtendedASCIINonprintables { get; } = new[]
        {
            // Controls (in 128 - 159 range)
            '\u0080', '\u0081', '\u0082', '\u0083', '\u0084', '\u0085', '\u0086', '\u0087', '\u0088', '\u0089', '\u008A', '\u008B', '\u008C', '\u008D', '\u008E', '\u008F',
            '\u0090', '\u0091', '\u0092', '\u0093', '\u0094', '\u0095', '\u0096', '\u0097', '\u0098', '\u0099', '\u009A', '\u009B', '\u009C', '\u009D', '\u009E', '\u009F'
        };

        public static char[] AllExtendedASCIINonprintables =>
            ArrayUtil.Combine(ASCIINonprintables, ExtendedASCIINonprintables);

        public static char[] OtherNonprintables { get; } = new[]
        {
            /* ByteOrderMark (65279)    UnicodeReplacementChar � (65533) */
            '\uFEFF',                   '\uFFFD'
        };

        public static char[] AllNonprintables =>
            ArrayUtil.Combine(AllExtendedASCIINonprintables, OtherNonprintables);

        public static IDictionary<char, char[]> UnicodeLatinToASCIIAlphaConversions { get; } = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å'  } },
            { 'C', new[] { 'Ç' } },
            { 'E', new[] { 'È', 'É', 'Ê', 'Ë' } },
            { 'I', new[] { 'Ì', 'Í', 'Î', 'Ï' } },
            { 'N', new[] { 'Ñ' } },
            { 'O', new[] { 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø' } },
            { 'U', new[] { 'Ù', 'Ú', 'Û', 'Ü' } },
            { 'Y', new[] { 'Ý', 'Ÿ' } },
            { 'a', new[] { 'à', 'á', 'â', 'ã', 'ä', 'å' } },
            { 'c', new[] { 'ç' } },
            { 'e', new[] { 'è', 'é', 'ê', 'ë' } },
            { 'i', new[] { 'ì', 'í', 'î', 'ï' } },
            { 'n', new[] { 'ñ' } },
            { 'o', new[] { 'ò', 'ó', 'ô', 'õ', 'ö', 'ø' } },
            { 'u', new[] { 'ù', 'ú', 'û', 'ü' } },
            { 'y', new[] { 'ý', 'ÿ' } },
        }.AsImmutable();

        public static IDictionary<string, char[]> UnicodeLatinToASCIIAlphaComplexConversions { get; } = new Dictionary<string, char[]>
        {
            { "AE", new[] { 'Æ' } },
            { "C", new[] { 'Ⅽ' } },
            { "D", new[] { 'Ⅾ' } },
            { "I", new[] { 'Ⅰ' } },
            { "II", new[] { 'Ⅱ' } },
            { "III", new[] { 'Ⅲ' } },
            { "IV", new[] { 'Ⅳ' } },
            { "IX", new[] { 'Ⅸ' } },
            { "L", new[] { 'Ⅼ' } },
            { "M", new[] { 'Ⅿ' } },
            { "V", new[] { 'Ⅴ' } },
            { "VI", new[] { 'Ⅵ' } },
            { "VII", new[] { 'Ⅶ' } },
            { "VIII", new[] { 'Ⅷ' } },
            { "X", new[] { 'Ⅹ' } },
            { "XI", new[] { 'Ⅺ' } },
            { "XII", new[] { 'Ⅻ' } },
            { "YR", new[] { 'Ʀ' }},
            { "ae", new[] { 'æ' } },
            { "c", new[] { 'ⅽ' } },
            { "d", new[] { 'ⅾ' } },
            { "ff", new[] { 'ﬀ' } },
            { "fi", new[] { 'ﬁ' } },
            { "fl", new[] { 'ﬂ' } },
            { "ffi", new[] { 'ﬃ' } },
            { "ffl", new[] { 'ﬄ' } },
            { "ft", new[] { 'ﬅ' } },
            { "hv", new[] { 'ƕ' } },
            { "i", new[] { 'ⅰ' } },
            { "ii", new[] { 'ⅱ' } },
            { "iii", new[] { 'ⅲ' } },
            { "iv", new[] { 'ⅳ' } },
            { "ix", new[] { 'ⅸ' } },
            { "m", new[] { 'ⅿ' } },
            { "st", new[] { 'ﬆ' } },
            { "v", new[] { 'ⅴ' } },
            { "vi", new[] { 'ⅵ' } },
            { "vii", new[] { 'ⅶ' } },
            { "viii", new[] { 'ⅷ' } },
            { "x", new[] { 'ⅹ' } },
            { "xi", new[] { 'ⅺ' } },
            { "xii", new[] { 'ⅻ' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> UnicodeExtendedLatinToASCIIAlphaConversions { get; } = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'Ā', 'Ă', 'Ą', 'Ǎ', 'Ǟ', 'Ǡ', 'Ǻ', 'Ȁ', 'Ȃ', 'Ȧ', 'Ⱥ' } },
            { 'B', new[] { 'Ɓ', 'Ƃ', 'Ƀ', 'Ḃ', 'ß' } },
            { 'C', new[] { 'Ć', 'Ĉ', 'Ċ', 'Č', 'Ƈ', 'Ȼ' } },
            { 'D', new[] { 'Ď', 'Đ', 'Ɖ', 'Ɗ', 'Ƌ', 'Ḋ', 'Ð' } },
            { 'E', new[] { 'Ē', 'Ĕ', 'Ė', 'Ę', 'Ě', 'Ǝ', 'Ə', 'Ɛ', 'Ʃ', 'Ȅ', 'Ȇ', 'Ȩ', 'Ɇ' } },
            { 'F', new[] { 'Ƒ', 'Ḟ' } },
            { 'G', new[] { 'Ĝ', 'Ğ', 'Ġ', 'Ģ', 'Ɠ', 'Ǥ', 'Ǧ', 'Ǵ' } },
            { 'H', new[] { 'Ĥ', 'Ħ', 'Ȟ' } },
            { 'I', new[] { 'Ĩ', 'Ī', 'Ĭ', 'Į', 'İ', 'Ɩ', 'Ɨ', 'Ǐ', 'Ȉ', 'Ȋ' } },
            { 'J', new[] { 'Ĵ', 'Ɉ' } },
            { 'K', new[] { 'Ķ', 'Ƙ', 'Ǩ' } },
            { 'L', new[] { 'Ĺ', 'Ļ', 'Ľ', 'Ŀ', 'Ł', 'Ƚ' } },
            { 'M', new[] { 'Ɯ', 'Ṁ' } },
            { 'N', new[] { 'Ń', 'Ņ', 'Ň', 'Ŋ', 'Ɲ', 'Ǹ', 'Ƞ' } },
            { 'O', new[] { 'Ō', 'Ŏ', 'Ő', 'Ɔ', 'Ɵ', 'Ơ', 'Ǒ', 'Ǫ', 'Ǭ', 'Ǿ', 'Ȍ', 'Ȏ', 'Ȫ', 'Ȭ', 'Ȯ', 'Ȱ' } },
            { 'P', new[] { 'Ƥ', 'Ṗ', 'Þ' } },
            { 'Q', new[] { 'Ɋ' } },
            { 'R', new[] { 'Ŕ', 'Ŗ', 'Ř', 'Ȑ', 'Ȓ', 'Ɍ' } },
            { 'S', new[] { 'Ś', 'Ŝ', 'Ş', 'Š', 'Ƨ', 'Ș', 'Ṡ' } },
            { 'T', new[] { 'Ţ', 'Ť', 'Ŧ', 'Ƭ', 'Ʈ', 'Ț', 'Ⱦ', 'Ṫ' } },
            { 'U', new[] { 'Ũ', 'Ū', 'Ŭ', 'Ů', 'Ű', 'Ų', 'Ư', 'Ʊ', 'Ǔ', 'Ǖ', 'Ǘ', 'Ǚ', 'Ǜ', 'Ȕ', 'Ȗ', 'Ʉ' } },
            { 'V', new[] { 'Ʋ', 'Ʌ' } },
            { 'W', new[] { 'Ŵ', 'Ẁ', 'Ẃ', 'Ẅ' } },
            { 'Y', new[] { 'Ŷ', 'Ɣ', 'Ƴ', 'Ȳ', 'Ɏ', 'Ỳ' } },
            { 'Z', new[] { 'Ź', 'Ż', 'Ž', 'Ƶ', 'Ȥ' } },
            { 'a', new[] { 'ā', 'ă', 'ą', 'ǎ', 'ǟ', 'ǡ', 'ǻ', 'ȁ', 'ȃ', 'ȧ' } },
            { 'b', new[] { 'ƀ', 'ƃ', 'ḃ' } },
            { 'c', new[] { 'ć', 'ĉ', 'ċ', 'č', 'ƈ', 'ȼ' } },
            { 'd', new[] { 'ď', 'đ', 'ƌ', 'ȡ', 'ḋ', 'ð' } },
            { 'e', new[] { 'ē', 'ĕ', 'ė', 'ę', 'ě', 'ǝ', 'ȅ', 'ȇ', 'ȩ', 'ɇ', 'ə' } },
            { 'f', new[] { 'ƒ', 'ḟ' } },
            { 'g', new[] { 'ĝ', 'ğ', 'ġ', 'ģ', 'ǥ', 'ǧ', 'ǵ' } },
            { 'h', new[] { 'ĥ', 'ħ', 'ȟ' } },
            { 'i', new[] { 'ĩ', 'ī', 'ĭ', 'į', 'ı', 'ǐ', 'ȉ', 'ȋ' } },
            { 'j', new[] { 'ĵ', 'ȷ', 'ɉ' } },
            { 'k', new[] { 'ķ', 'ĸ', 'ƙ', 'ǩ' } },
            { 'l', new[] { 'ĺ', 'ļ', 'ľ', 'ŀ', 'ł', 'ƚ', 'ȴ' } },
            { 'm', new[] { 'ṁ' } },
            { 'n', new[] { 'ń', 'ņ', 'ň', 'ŉ', 'ŋ', 'ƞ', 'ǹ', 'ȵ' } },
            { 'o', new[] { 'ō', 'ŏ', 'ő', 'ơ', 'ǒ', 'ǫ', 'ǭ', 'ǿ', 'ȍ', 'ȏ', 'ȫ', 'ȭ', 'ȯ', 'ȱ' } },
            { 'p', new[] { 'ƥ', 'ṗ', 'þ', } },
            { 'q', new[] { 'ɋ' } },
            { 'r', new[] { 'ŕ', 'ŗ', 'ř', 'ȑ', 'ȓ', 'ɍ', 'ɼ' } },
            { 's', new[] { 'ś', 'ŝ', 'ş', 'š', 'ſ', 'ƨ', 'ș', 'ȿ', 'ṡ', 'ẛ' } },
            { 't', new[] { 'ţ', 'ť', 'ŧ', 'ƫ', 'ƭ', 'ț', 'ȶ', 'ṫ' } },
            { 'u', new[] { 'ũ', 'ū', 'ŭ', 'ů', 'ű', 'ų', 'ư', 'ǔ', 'ǖ', 'ǘ', 'ǚ', 'ǜ', 'ȕ', 'ȗ' } },
            { 'w', new[] { 'ŵ', 'ẁ', 'ẃ', 'ẅ' } },
            { 'y', new[] { 'ŷ', 'ƴ', 'ȳ', 'ɏ', 'ỳ' } },
            { 'z', new[] { 'ź', 'ż', 'ž', 'ƶ', 'ȥ', 'ɀ' } },
        }.AsImmutable();

        public static IDictionary<string, char[]> UnicodeExtendedLatinToASCIIAlphaComplexConversions { get; } = new Dictionary<string, char[]>
        {
            { "AE", new[] { 'Ǣ', 'Ǽ' } },
            { "DZ", new[] { 'Ǆ', 'Ǳ' } },
            { "Dz", new[] { 'ǅ', 'ǲ' } },
            { "IJ", new[] { 'Ĳ' } },
            { "LJ", new[] { 'Ǉ' } },
            { "Lj", new[] { 'ǈ' } },
            { "NJ", new[] { 'Ǌ' } },
            { "Nj", new[] { 'ǋ' } },
            { "OE", new[] { 'Œ' } },
            { "OI", new[] { 'Ƣ' } },
            { "OU", new[] { 'Ȣ' } },
            { "ae", new[] { 'ǣ', 'ǽ' } },
            { "db", new[] { 'ȸ' } },
            { "dz", new[] { 'ǆ', 'ǳ' } },
            { "ij", new[] { 'ĳ' } },
            { "lj", new[] { 'ǉ' } },
            { "nj", new[] { 'ǌ' } },
            { "oe", new[] { 'œ' } },
            { "oi", new[] { 'ƣ' } },
            { "ou", new[] { 'ȣ' } },
            { "qp", new[] { 'ȹ' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> UnicodeGreekToASCIIAlphaConversions { get; } = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'Ά', 'Α' } },
            { 'B', new[] { 'Β', 'ϐ' } },
            { 'C', new[] { 'Ϲ', 'Ͻ', 'Ͼ', 'Ͽ' } },
            { 'E', new[] { 'Έ', 'Ε', 'Σ' } },
            { 'F', new[] { 'Ϝ' } },
            { 'H', new[] { 'Ή', 'Η' } },
            { 'I', new[] { 'Ί', 'Ι', 'Ϊ' } },
            { 'J', new[] { 'Ϳ' } },
            { 'K', new[] { 'Κ', 'Ϗ' } },
            { 'M', new[] { 'Μ', 'Ϻ' } },
            { 'N', new[] { 'Ͷ', 'Ν' } },
            { 'O', new[] { 'Ό', 'Θ', 'Ο', 'Φ', 'ϴ' } },
            { 'P', new[] { 'Ρ' } },
            { 'Q', new[] { 'Ϙ' } },
            { 'T', new[] { 'Ͳ', 'Τ' } },
            { 'X', new[] { 'Χ' } },
            { 'Y', new[] { 'Ύ', 'Υ', 'Ϋ', 'ϒ', 'ϓ', 'ϔ' } },
            { 'Z', new[] { 'Ζ' } },
            { 'a', new[] { 'ά', 'α' } },
            { 'b', new[] { 'β' } },
            { 'c', new[] { 'ς' } },
            { 'e', new[] { 'έ', 'ε', 'ϵ', '϶' } },
            { 'f', new[] { 'ϝ' } },
            { 'i', new[] { 'ΐ', 'ί', 'ι', 'ϊ' } },
            { 'k', new[] { 'κ' } },
            { 'm', new[] { 'ϻ' } },
            { 'n', new[] { 'ͷ', 'ή', 'η' } },
            { 'o', new[] { 'θ', 'ο', 'σ', 'ό' } },
            { 'p', new[] { 'ρ' } },
            { 'q', new[] { 'ϙ' } },
            { 't', new[] { 'ͳ', 'τ' } },
            { 'u', new[] { 'ΰ', 'υ', 'ϋ', 'ύ' } },
            { 'v', new[] { 'ν' } },
            { 'w', new[] { 'ω', 'ώ' } },
            { 'x', new[] { 'χ' } },
            { 'y', new[] { 'γ' } },
            { 'z', new[] { 'ζ' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> UnicodeExtendedGreekToASCIIAlphaConversions { get; } = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'Ἀ', 'Ἁ', 'Ἂ', 'Ἃ', 'Ἄ', 'Ἅ', 'Ἆ', 'Ἇ', 'ᾈ', 'ᾉ', 'ᾊ', 'ᾋ', 'ᾌ', 'ᾍ', 'ᾎ', 'ᾏ', 'Ᾰ', 'Ᾱ', 'Ὰ', 'Ά', 'ᾼ' } },
            { 'E', new[] { 'Ἐ', 'Ἑ', 'Ἒ', 'Ἓ', 'Ἔ', 'Ἕ', 'Ὲ', 'Έ' } },
            { 'H', new[] { 'Ἠ', 'Ἡ', 'Ἢ', 'Ἣ', 'Ἤ', 'Ἥ', 'Ἦ', 'Ἧ', 'ᾘ', 'ᾙ', 'ᾚ', 'ᾛ', 'ᾜ', 'ᾝ', 'ᾞ', 'ᾟ', 'Ὴ', 'Ή', 'ῌ' } },
            { 'I', new[] { 'Ἰ', 'Ἱ', 'Ἲ', 'Ἳ', 'Ἴ', 'Ἵ', 'Ἶ', 'Ἷ', 'Ῐ', 'Ῑ', 'Ὶ', 'Ί' } },
            { 'O', new[] { 'Ὀ', 'Ὁ', 'Ὂ', 'Ὃ', 'Ὄ', 'Ὅ', 'Ὸ', 'Ό' } },
            { 'P', new[] { 'Ῥ' } },
            { 'Y', new[] { 'Ὑ', 'Ὓ', 'Ὕ', 'Ὗ', 'Ῠ', 'Ῡ', 'Ὺ', 'Ύ' } },
            { 'a', new[] { 'ἀ', 'ἁ', 'ἂ', 'ἃ', 'ἄ', 'ἅ', 'ἆ', 'ἇ', 'ὰ', 'ά', 'ᾀ', 'ᾁ', 'ᾂ', 'ᾃ', 'ᾄ', 'ᾅ', 'ᾆ', 'ᾇ', 'ᾰ', 'ᾱ', 'ᾲ', 'ᾳ', 'ᾴ', 'ᾶ', 'ᾷ' } },
            { 'e', new[] { 'ἐ', 'ἑ', 'ἒ', 'ἓ', 'ἔ', 'ἕ', 'ὲ', 'έ' } },
            { 'i', new[] { 'ἰ', 'ἱ', 'ἲ', 'ἳ', 'ἴ', 'ἵ', 'ἶ', 'ἷ', 'ὶ', 'ί', 'ῐ', 'ῑ', 'ῒ', 'ΐ', 'ῖ', 'ῗ' } },
            { 'n', new[] { 'ἠ', 'ἡ', 'ἢ', 'ἣ', 'ἤ', 'ἥ', 'ἦ', 'ἧ', 'ὴ', 'ή', 'ᾐ', 'ᾑ', 'ᾒ', 'ᾓ', 'ᾔ', 'ᾕ', 'ᾖ', 'ᾗ', 'ῂ', 'ῃ', 'ῄ', 'ῆ', 'ῇ' } },
            { 'o', new[] { 'ὀ', 'ὁ', 'ὂ', 'ὃ', 'ὄ', 'ὅ', 'ὸ', 'ό' } },
            { 'p', new[] { 'ῤ', 'ῥ' } },
            { 'u', new[] { 'ὐ', 'ὑ', 'ὒ', 'ὓ', 'ὔ', 'ὕ', 'ὖ', 'ὗ', 'ὺ', 'ύ', 'ῠ', 'ῡ', 'ῢ', 'ΰ', 'ῦ', 'ῧ' } },
            { 'w', new[] { 'ὠ', 'ὡ', 'ὢ', 'ὣ', 'ὤ', 'ὥ', 'ὦ', 'ὧ', 'ὼ', 'ώ', 'ᾠ', 'ᾡ', 'ᾢ', 'ᾣ', 'ᾤ', 'ᾥ', 'ᾦ', 'ᾧ', 'ῲ', 'ῳ', 'ῴ', 'ῶ', 'ῷ' } },
        }.AsImmutable();

        public static IDictionary<string, char[]> UnicodeGreekToASCIIAlphaComplexConversions { get; } = new Dictionary<string, char[]>
        {
            { "PI", new[] { 'Π' } },
            { "pi", new[] { 'π' } },
            { "mu", new[] { 'μ' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> UnicodeCyrillicToASCIIAlphaConversions { get; } = new Dictionary<char, char[]>
        {
            { 'A', new[] { 'А', 'Ѧ' } },
            { 'B', new[] { 'Б', 'В' } },
            { 'C', new[] { 'С', 'Ҫ' } },
            { 'E', new[] { 'Ѐ', 'Ё', 'Є', 'Е', 'Э', 'Ҽ', 'Ҿ', 'Ӭ' } },
            { 'H', new[] { 'Н', 'Ң', 'Ҥ', 'Һ' } },
            { 'I', new[] { 'І', 'Ї', 'Ӏ' } },
            { 'J', new[] { 'Ј' } },
            { 'K', new[] { 'Ќ', 'К', 'Қ', 'Ҝ', 'Ҟ', 'Ҡ' } },
            { 'M', new[] { 'М' } },
            { 'N', new[] { 'Ѝ', 'И', 'Й', 'Ҋ', 'Ӣ', 'Ӥ' } },
            { 'O', new[] { 'О', 'Ф', 'Ѳ', 'Ѻ', 'Ӧ', 'Ө', 'Ӫ' } },
            { 'P', new[] { 'Р', 'Ҏ' } },
            { 'R', new[] { 'Я' } },
            { 'S', new[] { 'Ѕ' } },
            { 'T', new[] { 'Т', 'Ҭ' } },
            { 'V', new[] { 'Ѵ', 'Ѷ' } },
            { 'W', new[] { 'Ш', 'Щ', 'Ѡ', 'Ѽ', 'Ѿ' } },
            { 'X', new[] { 'Х', 'Ҳ', 'Җ', 'Ӂ', 'Ӽ', 'Ӿ' } },
            { 'Y', new[] { 'У', 'Ү', 'Ұ', 'Ӯ', 'Ӱ', 'Ӳ' } },
            { 'a', new[] { 'а', 'ѧ' } },
            { 'b', new[] { 'б', 'в' } },
            { 'c', new[] { 'с', 'ҫ' } },
            { 'e', new[] { 'е', 'э', 'ѐ', 'ё', 'є', 'ҽ', 'ҿ', 'ӭ' } },
            { 'h', new[] { 'н', 'ң', 'ҥ', 'һ' } },
            { 'i', new[] { 'і', 'ї' } },
            { 'j', new[] { 'ј' } },
            { 'k', new[] { 'к', 'ќ', 'қ', 'ҝ', 'ҟ', 'ҡ' } },
            { 'm', new[] { 'м' } },
            { 'n', new[] { 'и', 'й', 'ѝ', 'ҋ', 'ӣ', 'ӥ' } },
            { 'o', new[] { 'о', 'ѳ', 'ѻ', 'ӧ', 'ө', 'ӫ' } },
            { 'p', new[] { 'р', 'ҏ' } },
            { 'r', new[] { 'я' } },
            { 's', new[] { 'ѕ' } },
            { 't', new[] { 'т', 'ҭ' } },
            { 'v', new[] { 'ѵ', 'ѷ' } },
            { 'w', new[] { 'ш', 'щ', 'ѡ', 'ѽ', 'ѿ' } },
            { 'x', new[] { 'х', 'ҳ', 'җ', 'ӂ', 'ӽ', 'ӿ' } },
            { 'y', new[] { 'Ў', 'у', 'ў', 'ү', 'ұ', 'ӯ', 'ӱ', 'ӳ' } },
            { '3', new[] { 'З', 'з', 'Ҙ', 'ҙ', 'Ӟ', 'ӟ', 'Ӡ', 'ӡ'  } },
        }.AsImmutable();

        public static IDictionary<string, char[]> UnicodeCyrillicToASCIIAlphaComplexConversions { get; } = new Dictionary<string, char[]>
        {
            { "Bl", new[] { 'Ӹ' } },
            { "IA", new[] { 'Ѩ' } },
            { "Oy", new[] { 'Ѹ' } },
            { "bl", new[] { 'ӹ' } },
            { "ia", new[] { 'ѩ' } },
            { "oy", new[] { 'ѹ' } },
        }.AsImmutable();

        public static IDictionary<char, char[]> UnicodeSymbolsToASCIIConversions { get; } = new Dictionary<char, char[]>
        {
            // letter-like (incl. superscripts, subscripts)
            { 'A', new[] { 'Ⓐ', 'Å', '∀' } },
            { 'B', new[] { 'Ⓑ', 'ℬ', '₿' } },
            { 'C', new[] { 'Ⓒ', 'ℂ', 'ℭ', '₡', '∁' } },
            { 'D', new[] { 'Ⓓ', 'ⅅ' } },
            { 'E', new[] { 'Ⓔ', 'ℰ', 'ℇ', '∃', '∄', '∈', '∉', '∊', '∋', '∌', '∍' } },
            { 'F', new[] { 'Ⓕ', 'ℱ', 'Ⅎ', 'ⅎ', '₣' } },
            { 'G', new[] { 'Ⓖ', '⅁' } },
            { 'H', new[] { 'Ⓗ', 'ℋ', 'ℌ', 'ℍ' } },
            { 'I', new[] { 'Ⓘ', 'ℐ', 'ℑ' } },
            { 'J', new[] { 'Ⓙ' } },
            { 'K', new[] { 'Ⓚ', 'K', '₭' } },
            { 'L', new[] { 'Ⓛ', 'ℒ', '⅃', '⅂', '₤' } },
            { 'M', new[] { 'Ⓜ', 'ℳ' } },
            { 'N', new[] { 'Ⓝ', 'ℕ', '₦' } },
            { 'O', new[] { 'Ⓞ' } },
            { 'P', new[] { 'Ⓟ', '♇', 'ℙ', '℘', '℗', '₽' } },
            { 'Q', new[] { 'Ⓠ', '℺', 'ℚ' } },
            { 'R', new[] { 'Ⓡ', '℟', 'ℝ', 'ℜ', 'ℛ', 'ʶ' } },
            { 'S', new[] { 'Ⓢ', '₷', '⸈' } },
            { 'T', new[] { 'Ⓣ', '₸', '₮', '⸀', '⸁', '⸆', '⸇' } },
            { 'U', new[] { 'Ⓤ' } },
            { 'V', new[] { 'Ⓥ', '℣' } },
            { 'W', new[] { 'Ⓦ', '₩' } },
            { 'X', new[] { 'Ⓧ', '☒', '☓' } },
            { 'Y', new[] { 'Ⓨ', '⅄' } },
            { 'Z', new[] { 'Ⓩ', 'ℤ' } },
            { 'a', new[] { 'ⓐ', '⍺', '⍶', 'ª', 'ₐ' } },
            { 'b', new[] { 'ⓑ', '♭' } },
            { 'c', new[] { 'ⓒ', '¢', '₵', '⸿' } },
            { 'd', new[] { 'ⓓ', 'ⅆ', '₫' } },
            { 'e', new[] { 'ⓔ', '℮', 'ℯ', 'ⅇ', 'ₑ', 'ₔ' } },
            { 'f', new[] { 'ⓕ' } },
            { 'g', new[] { 'ⓖ', 'ℊ' } },
            { 'h', new[] { 'ⓗ', 'ℎ', 'ℏ', 'ℎ', 'ℏ', 'ʰ', 'ʱ', 'ₕ' } },
            { 'i', new[] { 'ⓘ', '℩', 'ℹ', 'ⅈ', '⍳', '⍸', 'ⁱ' } },
            { 'j', new[] { 'ⓙ', 'ⅉ', 'ʲ' } },
            { 'k', new[] { 'ⓚ', 'ₖ' } },
            { 'l', new[] { 'ⓛ', 'ℓ', 'ˡ', 'ˡ', 'ₗ' } },
            { 'm', new[] { 'ⓜ', '₥', 'ⁿ', 'ₘ' } },
            { 'n', new[] { 'ⓝ', 'ⁿ', 'ₙ' } },
            { 'o', new[] { 'ⓞ', 'ℴ', '∘' /* U+2218 Ring operator */, 'º', 'ₒ', '᛫' } },
            { 'p', new[] { 'ⓟ', '⍴', 'ₚ' } },
            { 'q', new[] { 'ⓠ' } },
            { 'r', new[] { 'ⓡ', 'ʳ', 'ʴ', 'ʵ' } },
            { 's', new[] { 'ⓢ', 'ˢ', 'ˢ', 'ₛ' } },
            { 't', new[] { 'ⓣ', '†', 'ₜ' } },
            { 'u', new[] { 'ⓤ' } },
            { 'v', new[] { 'ⓥ', '˅', '˯' } },
            { 'w', new[] { 'ⓦ', '⍵', '⍹', 'ʷ' } },
            { 'x', new[] { 'ⓧ', '˟', 'ˣ', '×' /* U+00D7 Multiplication sign */, '˟', 'ˣ', 'ₓ', '⸼' } },
            { 'y', new[] { 'ⓨ', 'ˠ', 'ʸ' } },
            { 'z', new[] { 'ⓩ' } },
            // number-like (incl. superscripts, subscripts)
            { '0', new[] { '⓪', '⓿', '∅', '⁰', '₀' } },
            { '1', new[] { '①', '⓵', '¹', '₁' } },
            { '2', new[] { '②', '⓶', '↊', '²', '₂' } },
            { '3', new[] { '③', '⓷', '↋', '³', '₃' } },
            { '4', new[] { '④', '⓸', '⁴', '₄' } },
            { '5', new[] { '⑤', '⓹', '⁵', '₅' } },
            { '6', new[] { '⑥', '⓺', '⁶', '₆' } },
            { '7', new[] { '⑦', '⓻', '⁷', '₇', '⁊' } },
            { '8', new[] { '⑧', '⓼', '⁸', '₈' } },
            { '9', new[] { '⑨', '⓽', '⁹', '₉' } },
            // punctuation
            { '!', new[] { '¡', '︕', '﹗', '！' } },
            { '?', new[] { '¿', '؟', '⸮', '︖', '﹖', '？' } },
            { '.', new[] { '·', '⋅' /* U+22C5 Dot operator */, '·', '․', '‧', '⸱', '⸳', '。', '・', '︒', '﹒', '．', '｡', '･' } },
            { ',', new[] { '،', '᠂', '⸒', '⸲', '⸴', '⹁', '、', '︐', '︑', '﹐', '﹑', '，', '､' } },
            { ';', new[] { '⁏', '⍮', ';', '؛', '⸵', '︔', '﹔', '；' } },
            { ':', new[] { 'ː', '˸', '᛬', '⁚', '⁝', '︓', '︰', '﹕', '：' } },
            { '\'', new[] { '‘', '’', '‚', '‛', '′', '‵', '⹄', '＇' } },
            { '"', new[] { 'ˮ', '“', '”', '„', '‟', '«', '»', '⹂', '〝', '〞', '〟', '᳓', '〃', '＂' } },
            { '-', new[] { '˗', '–', '—', '―', '‾', '­', '⁃', '⹃', '﹉', '﹊', '﹋', '﹌' } },
            { '<', new[] { '˂', '˱', '‹', '〈', '❬', '❮', '❰', '⟨', '⦉', '⦑', '⧼', '〈' } },
            { '>', new[] { '˃', '˲', '›', '〉', '❭', '❯', '❱', '⟩', '⦊', '⦒', '⧽', '〉', '‣' } },
            { '[', new[] { '⁅', '⌈', '⌊', '⟦', '⟬', '⦋', '⦍', '⦏', '⸢', '⸤', '「', '『', '〚', '［', '｢' } },
            { ']', new[] { '⁆', '⌉', '⌋', '⟧', '⟭', '⦌', '⦎', '⦐', '⸣', '⸥', '」', '』', '〛', '］', '｣' } },
            { '(', new[] { '⁽', '₍', '❨', '❪', '❲', '⟮', '⦇', '⦗', '⸦', '【', '〔', '〖', '〘', '﹙', '﹝', '（' } },
            { ')', new[] { '⁾', '₎', '❩', '❫', '❳', '⟯', '⦈', '⦘', '⸧', '】', '〕', '〗', '〙', '﹚', '﹞', '）', '﴿' } },
            { '{', new[] { '❴', '⟅', '⦃', '﹛', '｛' } },
            { '}', new[] { '❵', '⟆', '⦄', '﹜', '｝' } },
            // symbols
            { '*', new[] { '⁎', '•', '⁕', '⸎', '⸰', '﹡', '＊' } },
            { '#', new[] { '♯', '﹟', '＃' } },
            { '&', new[] { '⅋', '﹠', '＆' } },
            { '@', new[] { '﹫', '＠' } },
            { '|', new[] { '¦', '⁞', '⧘', '⧙', '︳', '︴', '।', '⁞', '⸽', '⸾' } },
            { '/', new[] { '⁄', '⁄', '∕', '√', '／' } },  // not including '÷'
            { '\\', new[] { '∖', '﹨', '＼' } },
            { '^', new[] { 'ˆ', '˄', '˰', '‸', '⁁' } },
            { '%', new[] { '⸓', '﹪', '％' } },
            { '´', new[] { 'ˊ', 'ˏ' } },
            { '`', new[] { 'ˋ', 'ˎ', '˴', '﹅', '﹆' } },
            { '_', new[] { 'ˍ', '‗', '﹍', '﹎', '﹏', '＿', '⸏', '⸐', '⸑' } },
            { '~', new[] { '˜', '˷', '∼', '∽', '∿', '⁓' } },
            // math and programming
            { '+', new[] { '∔', '᛭', '⸭' } },
            { '=', new[] { '˭' } },

        }.AsImmutable();

        public static IDictionary<string, char[]> UnicodeSymbolsToASCIIComplexConversions { get; } = new Dictionary<string, char[]>
        {
            // letter-like and currencies
            { "A/S", new[] { '⅍' } },
            { "CL", new[] { '℄' } },
            { "Cr", new[] { '₢' } },
            { "EUR", new[] { '€' } },
            { "FAX", new[] { '℻' } },
            { "GBP", new[] { '£' } },
            { "JPY", new[] { '¥' } },
            { "No", new[] { '№' } },
            { "PL", new[] { '⅊' } },
            { "Pts", new[] { '₧' } },
            { "PX", new[] { '☧' } },
            { "Rs", new[] { '₨' } },
            { "Rx", new[] { '℞' } },
            { "SM", new[] { '℠' } },
            { "TEL", new[] { '℡' } },
            { "TM", new[] { '™' } },
            { "a/c", new[] { '℀' } },
            { "a/s", new[] { '℁' } },
            { "c/o", new[] { '℅' } },
            { "c/u", new[] { '℆' } },
            { "lb", new[] { '℔' } },
            { "(a)", new[] { '⒜' } },
            { "(b)", new[] { '⒝' } },
            { "(c)", new[] { '⒞' } },
            { "(d)", new[] { '⒟' } },
            { "(e)", new[] { '⒠' } },
            { "(f)", new[] { '⒡' } },
            { "(g)", new[] { '⒢' } },
            { "(h)", new[] { '⒣' } },
            { "(i)", new[] { '⒤' } },
            { "(j)", new[] { '⒥' } },
            { "(k)", new[] { '⒦' } },
            { "(l)", new[] { '⒧' } },
            { "(m)", new[] { '⒨' } },
            { "(n)", new[] { '⒩' } },
            { "(o)", new[] { '⒪' } },
            { "(p)", new[] { '⒫' } },
            { "(q)", new[] { '⒬' } },
            { "(r)", new[] { '⒭' } },
            { "(s)", new[] { '⒮' } },
            { "(t)", new[] { '⒯' } },
            { "(u)", new[] { '⒰' } },
            { "(v)", new[] { '⒱' } },
            { "(w)", new[] { '⒲' } },
            { "(x)", new[] { '⒳' } },
            { "(y)", new[] { '⒴' } },
            { "(z)", new[] { '⒵' } },
            { "°C", new[] { '℃' } },
            { "°F", new[] { '℉' } },
            // number-like
            { "(1)", new[] { '⑴' } },
            { "(2)", new[] { '⑵' } },
            { "(3)", new[] { '⑶' } },
            { "(4)", new[] { '⑷' } },
            { "(5)", new[] { '⑸' } },
            { "(6)", new[] { '⑹' } },
            { "(7)", new[] { '⑺' } },
            { "(8)", new[] { '⑻' } },
            { "(9)", new[] { '⑼' } },
            { "(10)", new[] { '⑽' } },
            { "(11)", new[] { '⑾' } },
            { "(12)", new[] { '⑿' } },
            { "(13)", new[] { '⒀' } },
            { "(14)", new[] { '⒁' } },
            { "(15)", new[] { '⒂' } },
            { "(16)", new[] { '⒃' } },
            { "(17)", new[] { '⒄' } },
            { "(18)", new[] { '⒅' } },
            { "(19)", new[] { '⒆' } },
            { "(20)", new[] { '⒇' } },
            { "1.", new[] { '⒈' } },
            { "2.", new[] { '⒉' } },
            { "3.", new[] { '⒊' } },
            { "4.", new[] { '⒋' } },
            { "5.", new[] { '⒌' } },
            { "6.", new[] { '⒍' } },
            { "7.", new[] { '⒎' } },
            { "8.", new[] { '⒏' } },
            { "9.", new[] { '⒐' } },
            { "10.", new[] { '⒑' } },
            { "11.", new[] { '⒒' } },
            { "12.", new[] { '⒓' } },
            { "13.", new[] { '⒔' } },
            { "14.", new[] { '⒕' } },
            { "15.", new[] { '⒖' } },
            { "16.", new[] { '⒗' } },
            { "17.", new[] { '⒘' } },
            { "18.", new[] { '⒙' } },
            { "19.", new[] { '⒚' } },
            { "20.", new[] { '⒛' } },
            { "10", new[] { '⓾' } },
            // math and programming
            { "0/00", new[] { '‰' } },
            { "0/000", new[] { '‱' } },
            { "0/3", new[] { '↉' } },
            { "1/", new[] { '⅟' } },
            { "1/2", new[] { '½' } },
            { "1/3", new[] { '⅓' } },
            { "1/4", new[] { '¼' } },
            { "1/5", new[] { '⅕' } },
            { "1/6", new[] { '⅙' } },
            { "1/7", new[] { '⅐' } },
            { "1/8", new[] { '⅛' } },
            { "1/9", new[] { '⅑' } },
            { "1/10", new[] { '⅒' } },
            { "2/3", new[] { '⅔' } },
            { "2/5", new[] { '⅖' } },
            { "3/", new[] { '¾' } },
            { "3/4", new[] { '∛' } },
            { "3/5", new[] { '⅗' } },
            { "3/8", new[] { '⅜' } },
            { "4/", new[] { '∜' } },
            { "4/5", new[] { '⅘' } },
            { "5/6", new[] { '⅚' } },
            { "5/8", new[] { '⅝' } },
            { "7/8", new[] { '⅞' } },
            { "+/-", new[] { '∓' } },
            // basic emoji
            { ":(", new[] { '☹' } },
            { ":)", new[] { '☺', '☻' } },
            // punctuation
            { "..", new[] { '¨', '᠃', '‥' } },
            { "...", new[] { '…', '⋰', '⋱', '⸪', '⸫', '︙' } },
            { "....", new[] { '᠅' } },
            { "!!", new[] { '‼' } },
            { "??", new[] { '⁇' } },
            { "?!", new[] { '⁈', '‽', '⸘' } },
            { "!?", new[] { '⁉' } },
            { ":", new[] { '∴', '∵', '∶' } },
            { "::", new[] { '∷', '⸬' } },
            { ":.", new[] { '჻' } },
            { ".:", new[] { '⁖' } },
            { ".:.", new[] { '⁘', '⁛' } },
            { ":.:", new[] { '⁙' } },
            { ":+:", new[] { '⁜' } },
            { "''", new[] { '″', '‶' } },
            { "'''", new[] { '‴', '‷' } },
            { "''''", new[] { '⁗' } },
            { "((", new[] { '⸨', '⦅', '｟' } },
            { "))", new[] { '⸩', '⦆', '｠' } },
            { "<<", new[] { '⟪', '《' } },
            { ">>", new[] { '⟫', '》' } },
            { "<(", new[] { '⦓' } },
            { ")>", new[] { '⦔' } },
            { ">((", new[] { '⦕' } },
            { "))<", new[] { '⦖' } },
            { ">:", new[] { '⸖' } },
            // symbols
            { "´´", new[] { '˶' } },
            { "``", new[] { '˵' } },
            { "||", new[] { '‖', '⧚', '⧛', '॥' } },
            { "**", new[] { '⁑' } },
            { "***", new[] { '⁂' } },
            { "~o", new[] { '⸛' } },
            { "~.", new[] { '⸞', '⸟' } },
            // meta
            { "[dagger]", new[] { '†', '‡', '⸶', '⸷', '⸸' } },
            { "[palm-branch]", new[] { '⸙' } },
            { "[paragraph]", new[] { '¶', '⁋' } },
            { "[reference]", new[] { '※' } },
            { "[section]", new[] { '§', '⸹' } },
            { "[square]", new[] { '⸋' } },
        }.AsImmutable();

        public static IDictionary<string, IDictionary<char, char[]>> AllUnicodeToASCIIConversions => new Dictionary<string, IDictionary<char, char[]>>
        {
            { "latin", CharLib.UnicodeLatinToASCIIAlphaConversions },
            { "extended latin", CharLib.UnicodeExtendedLatinToASCIIAlphaConversions },
            { "greek", CharLib.UnicodeGreekToASCIIAlphaConversions },
            { "extended greek", CharLib.UnicodeExtendedGreekToASCIIAlphaConversions },
            { "cyrillic", CharLib.UnicodeCyrillicToASCIIAlphaConversions },
            { "symbols", CharLib.UnicodeSymbolsToASCIIConversions }
        };

        public static IDictionary<string, IDictionary<string, char[]>> AllUnicodeToASCIIComplexConversions => new Dictionary<string, IDictionary<string, char[]>>
        {
            { "complex latin", CharLib.UnicodeLatinToASCIIAlphaComplexConversions },
            { "complex extended latin", CharLib.UnicodeExtendedLatinToASCIIAlphaComplexConversions },
            { "complex greek", CharLib.UnicodeGreekToASCIIAlphaComplexConversions },
            { "complex cyrillic", CharLib.UnicodeCyrillicToASCIIAlphaComplexConversions },
            { "complex symbols", CharLib.UnicodeSymbolsToASCIIComplexConversions }
        };
    }
}
