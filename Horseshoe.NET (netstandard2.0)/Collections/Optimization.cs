using System;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Options for merging dictionaries with identical keys
    /// </summary>
    [Flags]
    public enum Optimization
    {
        /// <summary>
        /// No optimization
        /// </summary>
        None = 0,

        /// <summary>
        /// Optimize by reusing lists and arrays when possible
        /// </summary>
        ReuseCollection = 1,

        /// <summary>
        /// Optimize by reducing a collection to its distinct values before performing other operations
        /// </summary>
        Distinct = 2
    }
}
