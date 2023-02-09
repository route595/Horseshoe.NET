using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// Dictates how to treat <c>null</c> items when zapping a collection.
    /// </summary>
    [Flags]
    public enum PrunePolicy
    {
        /// <summary>
        /// Do not remove <c>null</c> items.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only remove <c>null</c> items from the beginning of a collection.
        /// </summary>
        Leading = 1,

        /// <summary>
        /// Only remove <c>null</c> items from the end of a collection.
        /// </summary>
        Trailing = 2,

        /// <summary>
        /// Only remove <c>null</c> items from the beginning and end of a collection.
        /// </summary>
        LeadingAndTrailing = 3,

        /// <summary>
        /// Remove all <c>null</c> items.
        /// </summary>
        All = 4
    }
}
