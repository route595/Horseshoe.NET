using System;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Dictates which <c>char</c>s to reveal.
    /// </summary>
    [Flags]
    public enum CharCategory
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// ASCII spaces and non-breaking spaces.
        /// </summary>
        Spaces = 1,

        /// <summary>
        /// Horizontal tabs (\t [9]) and new lines (\r [13] and \n [10]).
        /// </summary>
        OtherWhitespaces = 2,

        /// <summary>
        /// ASCII spaces, non-breaking spaces, horizontal tabs (\t [9]) and new lines (\r [13] and \n [10]).
        /// </summary>
        AllWhitespaces = 3,

        /// <summary>
        /// Numeric ASCII <c>char</c>s (0 - 9).
        /// </summary>
        AsciiNumeric = 4,

        /// <summary>
        /// Alphabetic ASCII <c>char</c>s (A - Z, a - z).
        /// </summary>
        AsciiAlphabetic = 8,

        /// <summary>
        /// Alphanumeric ASCII <c>char</c>s (0 - 9, A - Z, a - z).
        /// </summary>
        AsciiAlphanumeric = 12,

        /// <summary>
        /// Non-alphanumeric ASCII printable <c>char</c>s (not including whitespaces).
        /// </summary>
        AsciiSymbols = 16,

        /// <summary>
        /// All ASCII printable <c>char</c>s (not including whitespaces).
        /// </summary>
        AllAsciiPrintables = 28,

        /// <summary>
        /// Unicode (i.e. Non-ASCII) printable <c>char</c>s (not including non-breaking space).
        /// </summary>
        UnicodePrintables = 32,

        /// <summary>
        /// Printable ASCII and Unicode <c>char</c>s (not including whitespaces).
        /// </summary>
        AllPrintables = 60,

        /// <summary>
        /// Printable ASCII and Unicode <c>char</c>s and all whitespaces.
        /// </summary>
        AllPrintablesAndWhitespaces = 63,

        /// <summary>
        /// ASCII and Unicode control <c>char</c>s (not including whitespaces) and any Unicode <c>char</c>s considered by Horseshoe.NET to be non-printable.
        /// </summary>
        Nonprintables = 64,

        /// <summary>
        /// All <c>char</c> categories including whitespaces and non-printables.
        /// </summary>
        All = 127
    }
}
