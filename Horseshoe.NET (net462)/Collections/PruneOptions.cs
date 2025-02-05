using System;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Pruning options for <c>Horseshoe.NET.Collections.[Xyz]Util.Zap()</c> 
    /// </summary>
    [Flags]
    public enum PruneOptions
    {
        /// <summary>
        /// No collection pruning (default)
        /// </summary>
        None = 0,

        /// <summary>
        /// Remove leading blanks and <c>null</c>s
        /// </summary>
        Leading = 1,

        /// <summary>
        /// Remove trailing blanks and <c>null</c>s
        /// </summary>
        Trailing = 2,

        /// <summary>
        /// Remove leading and trailing blanks and <c>null</c>s
        /// </summary>
        LeadingAndTrailing = 3,

        /// <summary>
        /// Remove inner blanks and <c>null</c>s
        /// </summary>
        Inner = 4,

        /// <summary>
        /// Remove leading, trailing and inner blanks and <c>null</c>s
        /// </summary>
        All = 7
    }
}
