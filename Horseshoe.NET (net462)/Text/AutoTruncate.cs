using System;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Instructs certain processes how to handle <c>string</c> data.
    /// </summary>
    [Flags]
    public enum AutoTruncate
    {
        /// <summary>
        /// No action required.
        /// </summary>
        None = 0,

        /// <summary>
        /// Strings must have whitespaces trimmed off the beginning and end.
        /// </summary>
        Trim = 1,

        /// <summary>
        /// Strings must have whitespaces trimmed off the beginning and end; also, zero-length string must be converted to <c>null</c>.
        /// </summary>
        Zap = 2,

        /// <summary>
        /// Causes <c>Zap</c> to ignore non-empty whitespace strings. 
        /// </summary>
        EmptyStringsOnly = 4
    }
}
