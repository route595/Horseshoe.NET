using System;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Dictates which <c>char</c>s to reveal.
    /// </summary>
    [Flags]
    public enum CharRevealPolicy
    {
        /// <summary>
        /// Does not reveal <c>char</c>s of any category.
        /// </summary>
        None = 0,

        /// <summary>
        /// Reveals ASCII spaces.
        /// </summary>
        Spaces = 1,

        /// <summary>
        /// Reveals non-breaking spaces.
        /// </summary>
        NonbreakingSpaces = 2,

        /// <summary>
        /// Reveals ASCII spaces and non-breaking spaces.
        /// </summary>
        AllSpaces = 3,

        /// <summary>
        /// Reveals horizontal tabs i.e. \t (9).
        /// </summary>
        Tabs = 4,

        /// <summary>
        /// Reveals newline <c>char</c>s i.e. \r (13) and \n (10).
        /// </summary>
        Newlines = 8,

        /// <summary>
        /// Reveals ASCII spaces, non-breaking spaces, horizontal tabs and newlines.
        /// </summary>
        AllWhitespaces = 15,

        /// <summary>
        /// Reveals printable ASCII <c>char</c>s (not including whitespaces).
        /// </summary>
        AsciiPrintables = 16,

        /// <summary>
        /// Reveals printable <c>char</c>s outside the base ASCII set (not including non-breaking spaces).
        /// </summary>
        UnicodePrintables = 32,

        /// <summary>
        /// Reveals all printable <c>char</c>s (including whitespaces).
        /// </summary>
        AllPrintablesAndWhitespaces = 63,

        /// <summary>
        /// Reveals ASCII control <c>char</c>s (not including whitespaces).
        /// </summary>
        AsciiNonprintables = 64,

        /// <summary>
        /// Reveals extended ASCII control <c>char</c>s and known non-printable Unicode <c>char</c>s.
        /// </summary>
        UnicodeNonprintables = 128,

        /// <summary>
        /// Reveals all known non-printable <c>char</c>s.
        /// </summary>
        AllNonprintables = 192,

        /// <summary>
        /// Reveals all <c>char</c>s indluding whitespaces and non-printables.
        /// </summary>
        All = 255
    }
}
