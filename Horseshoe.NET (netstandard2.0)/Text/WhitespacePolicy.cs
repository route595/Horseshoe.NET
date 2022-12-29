using System;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Determines which whitespaces to include in an operation and other actions such as normalize and combine.
    /// </summary>
    [Flags]
    public enum WhitespacePolicy
    {
        /// <summary>
        /// Includes no whitespaces or new lines.
        /// </summary>
        None = 0,

        /// <summary>
        /// Include normal spaces.
        /// </summary>
        IncludeASCIISpace = 1,

        /// <summary>
        /// Include non-breaking spaces.
        /// </summary>
        IncludeNonbreakingSpace = 2,

        /// <summary>
        /// Include both normal and non-breaking spaces.
        /// </summary>
        IncludeAllSpaces = 3,

        /// <summary>
        /// Includes tabs.
        /// </summary>
        IncludeTab = 4,

        /// <summary>
        /// Includes new lines.
        /// </summary>
        IncludeNewLines = 8,

        /// <summary>
        /// Includes all whitespaces (except non-breaking spaces) and new lines.
        /// </summary>
        IncludeAllASCIIWhitespaces = 13,

        /// <summary>
        /// Includes all whitespaces and new lines.
        /// </summary>
        IncludeAllWhitespaces = 15,

        /// <summary>
        /// Converts all whitespaces to normal spaces.  Often used in conjunction with <c>CombineSpaces</c>.
        /// </summary>
        NormalizeWhitespaces = 16,

        /// <summary>
        /// Combines multiple contiguous spaces into one.
        /// </summary>
        CombineSpaces = 32
    }
}
