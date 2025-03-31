using System.Collections.Generic;
using System.Globalization;

using Horseshoe.NET.Dotnet;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A collection of information about a Unicode <c>char</c> in .NET
    /// </summary>
    public readonly struct CharInfo
    {
        /// <summary>
        /// The <c>System.Char</c> value represented by this <c>CharInfo</c>
        /// </summary>
        public char Char { get; }

        /// <summary>
        /// S <c>char</c>'s basic category e.g. alpha, symbol, etc.
        /// </summary>
        public CharCategory Category { get; }

        /// <inheritdoc cref="UnicodeCategory"/>
        public UnicodeCategory UnicodeCategory { get; }

        private string _displayValue { get; }

        /// <summary>
        /// Which script or Unicode block contains <c>Char</c>
        /// </summary>
        public string Script { get; }

        /// <summary>
        /// A value indicating whether <c>Char</c> is in the ASCII (0 - 127) block of <c>char</c> indices.
        /// </summary>
        public bool IsASCII => Char <= 127;

        /// <summary>
        /// A value indicating whether <c>Char</c> is recognized as a lower case letter.
        /// </summary>
        public bool IsLower => UnicodeCategory == UnicodeCategory.LowercaseLetter;

        /// <summary>
        /// A value indicating whether <c>Char</c> is recognized as an upper case letter.
        /// </summary>
        public bool IsUpper => UnicodeCategory == UnicodeCategory.UppercaseLetter;

        internal CharInfo(char @char, CharCategory category, UnicodeCategory unicodeCategory, string displayValue, string script)
        {
            Char = @char;
            Category = category;
            UnicodeCategory = unicodeCategory;
            _displayValue = displayValue;
            Script = script;
        }

        /// <summary>
        /// Generates a new <c>CharInfo</c> by looking up the basic attributes of the supplied <c>char</c>.  Not cached.
        /// </summary>
        /// <param name="char">A supplied <c>char</c></param>
        public CharInfo(char @char)
        {
            CharUtil.LookupDetails(@char, out CharCategory category, out UnicodeCategory unicodeCategory, out _, out string displayValue, out string script);
            Char = @char;
            Category = category;
            UnicodeCategory = unicodeCategory;
            _displayValue = displayValue;
            Script = script;
        }

        /// <summary>
        /// Pulls the HTML representation from <c>CharLib.HTMLCodes</c>, if applicable, e.g. ( [38-"&amp;"] -&gt; "&amp;amp;")
        /// </summary>
        /// <returns>An HTML representation of <c>Char</c>.</returns>
        public string LookupHTMLNotation()
        {
            if (CharLib.HTMLCodes.TryGetValue(Char, out string htmlCode))
                return htmlCode;
            return new string(Char, 1);
        }

        /// <summary>
        /// Tries to find the closest matching ASCII <c>char</c>(s) by shape or appearance. Applies to char index &gt;= 128.
        /// </summary>
        /// <returns>An ASCII representation of a Unicode <c>char</c>.</returns>
        public string LookupASCIIEquivalent()
        {
            if (CharUtil.TryFindASCIIReplacement(Char, out string replacement, out _))
                return replacement;
            return null;
        }

        /// <summary>
        /// Tries to find the closest matching ASCII <c>char</c>(s) by shape or appearance. Applies to char index &gt;= 128.
        /// </summary>
        /// <param name="script">The script / Unicode block containing the replaced <c>char</c></param>
        /// <returns>An ASCII representation of a Unicode <c>char</c>.</returns>
        public string LookupASCIIEquivalent(out string script)
        {
            if (CharUtil.TryFindASCIIReplacement(Char, out string replacement, out script))
                return replacement;
            return null;
        }

        /// <summary>
        /// Displays this <c>CharInfo</c> object in a <c>string</c> formatted value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _displayValue;
        }

        /// <summary>
        /// Displays <c>Char</c> formatted as source hex code (C# style by default)
        /// </summary>
        /// <param name="language">Format for which available .NET language, default is <c>C#</c>.</param>
        /// <returns></returns>
        public string ToUnicodeHexString(DotnetLanguage language = DotnetLanguage.CSharp)
        {
            var hex = ToHexString();
            switch (language)
            {
                case DotnetLanguage.CSharp:
                default:
                    return "'\\u" + hex + "'";
                case DotnetLanguage.VB:
                    return "\"\\u" + hex + "\"";
            }
        }

        /// <summary>
        /// Displays <c>Char</c> as a 4-length hexadecimal <c>string</c>
        /// </summary>
        /// <returns></returns>
        public string ToHexString()
        {
            return ((int)Char).ToString("X").PadLeft(4, '0');
        }

        private static Dictionary<char, CharInfo> _cache;

        /// <summary>
        /// Generates a new <c>CharInfo</c> by looking up the basic attributes of the supplied <c>char</c>.  
        /// Caches new values and retrieves from cache when possible.
        /// </summary>
        /// <param name="c">A <c>char</c></param>
        /// <returns>A new or caches <c>CharInfo</c></returns>
        public static CharInfo Get(char c)
        {
            if (_cache == null)
                _cache = new Dictionary<char, CharInfo>();
            else if (_cache.TryGetValue(c, out CharInfo _info))
                return _info;
            CharUtil.LookupDetails(c, out CharCategory category, out UnicodeCategory unicodeCategory, out _, out string displayValue, out string script);
            var info = new CharInfo(c, category, unicodeCategory, displayValue, script);
            _cache.Add(c, info);
            return info;
        }
    }
}
