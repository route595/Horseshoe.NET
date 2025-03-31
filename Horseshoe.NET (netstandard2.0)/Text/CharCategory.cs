using System;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Indicates a <c>char</c>'s basic category e.g. alpha, symbol, etc.
    /// For more granular categorization please see <c>System.Globalization.UnicodeCategory</c>.
    /// </summary>
    [Flags]
    public enum CharCategory
    {
        /// <summary>
        /// No <c>char</c> categories.  Default value.
        /// </summary>
        NotDefined = 0,

        /// <summary>
        /// Represents alphabetic <c>char</c>s.  
        /// If <c>IsASCII == true</c> includes Latin ASCII letters.
        /// If <c>IsASCII == false</c> includes letters from other scripts / Unicode blocks e.g. extended Latin, Greek, Cyrillic, etc.
        /// If <c>IsASCII == null</c> includes all of the above.
        /// </summary>
        Alpha = 1,

        /// <summary>
        /// Represents numeric <c>char</c>s.
        /// If <c>IsASCII == true</c> includes ASCII <c>char</c>s <c>0 - 9</c>.
        /// If <c>IsASCII == false</c> includes numeric <c>char</c>s in other scripts / Unicode blocks e.g. Roman, Arabic numerals, etc.
        /// If <c>IsASCII == null</c> includes all of the above.
        /// </summary>
        Numeric = 2,

        /// <summary>
        /// Represents a combination of <c>Alpha</c> and <c>Numeric</c>
        /// </summary>
        AlphaNumeric = 3,  // combination

        /// <summary>
        /// Symbols including operators, punctuation, shapes, etc.
        /// </summary>
        Symbol = 4,

        /// <summary>
        /// Horizontal tab (\t [9]), ASCII space ([32]) and Unicode non-breaking space ([160])
        /// </summary>
        WhitespacesSansNewLines = 8,

        /// <summary>
        /// New lines (\r [13] and \n [10])
        /// </summary>
        NewLines = 16,

        /// <summary>
        /// Represents a combination of all whitespaces i.g. horizontal tab (\t [9]) and new lines (\r [13] and \n [10]), ASCII space ([32]) and Unicode non-breaking space ([160])
        /// </summary>
        AllWhitespaces = 24,

        /// <summary>
        /// Represents a combination of <c>Alpha</c>, <c>Numeric</c>, <c>Symbol</c> and <c>AllWhitespace</c>.
        /// </summary>
        AllPrintables = 31,

        /// <summary>
        /// ASCII and Unicode controls plus byte-order-mark ([65279]) and Unicode replacement char ([65533])
        /// </summary>
        Nonprintable = 32,

        /// <summary>
        /// Represents a combinatino of all <c>char</c> categories.
        /// </summary>
        All = 63,
    }
}
