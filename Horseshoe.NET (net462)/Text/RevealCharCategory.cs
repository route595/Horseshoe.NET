using System;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Indicates which <c>char</c> categories to reveal.
    /// </summary>
    [Flags]
    public enum RevealCharCategory
    {
        /// <summary>
        /// Does not reveal <c>char</c>s of any category.
        /// </summary>
        None = 0,

        /// <summary>
        /// Reveals ASCII whitespaces i.e. space, non-breaking space, tab and new lines (for further whitespace granularity use <c>WhitespacePolicy</c>).
        /// </summary>
        Whitespaces = 1,

        /// <summary>
        /// Reveals printable ASCII <c>char</c> codes (not including extended ASCII).
        /// </summary>
        ASCIIChars = 2,

        /// <summary>
        /// Reveals ASCII control <c>char</c>s (includes extended ASCII controls but does not include tab or new lines which Horseshoe.NET categorizes as whitespaces).
        /// </summary>
        ControlChars = 4,

        /// <summary>
        /// Reveals all other <c>char</c> categories i.e. extended printable ASCII and Unicode (characters, symbols, non-printables, etc.).
        /// </summary>
        Others = 8,

        /// <summary>
        /// Reveals all <c>char</c> categories i.e. whitespaces, printable ASCII <c>char</c> codes, ASCII control <c>char</c>s, extended printable ASCII and Unicode (characters, symbols, non-printables, etc.).
        /// </summary>
        All = 15
    }
}
