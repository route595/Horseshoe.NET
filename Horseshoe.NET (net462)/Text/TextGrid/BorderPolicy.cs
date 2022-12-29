using System;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// Whether to render borders and which borders to render on the <c>TextGrid</c>.
    /// </summary>
    [Flags]
    public enum BorderPolicy
    {
        /// <summary>
        /// No borders (default)
        /// </summary>
        None = 0,

        /// <summary>
        /// Renders outside border
        /// </summary>
        Outer = 1,

        /// <summary>
        /// Renders vertical inner borders
        /// </summary>
        InnerVertical = 2,

        /// <summary>
        /// Renders horizontal inner borders
        /// </summary>
        InnerHorizontal = 4,

        /// <summary>
        /// Renders all inner borders
        /// Renders all inner borders
        /// </summary>
        Inner = 6, // e.g. InnerVertical and InnerHorizontal

        /// <summary>
        /// Renders all borders
        /// </summary>
        All = 7    // e.g. Outer, InnerVertical and InnerHorizontal
    }
}
