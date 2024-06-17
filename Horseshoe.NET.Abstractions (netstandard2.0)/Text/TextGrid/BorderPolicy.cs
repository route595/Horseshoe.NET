using System;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// Which borders to render in the <c>TextGrid</c>.
    /// </summary>
    [Flags]
    public enum BorderPolicy
    {
        /// <summary>
        /// No borders (default)
        /// </summary>
        None = 0,

        /// <summary>
        /// Renders vertical inner borders
        /// </summary>
        InnerVertical = 1,

        /// <summary>
        /// Renders horizontal inner borders
        /// </summary>
        InnerHorizontal = 2,

        /// <summary>
        /// Renders all inner borders
        /// Renders all inner borders
        /// </summary>
        Inner = 3,

        /// <summary>
        /// Renders outer top border
        /// </summary>
        OuterTop = 4,

        /// <summary>
        /// Renders outer bottom border
        /// </summary>
        OuterBottom = 8,

        /// <summary>
        /// Renders outer horizontal (top, bottom) borders
        /// </summary>
        OuterHorizontal = 12,

        /// <summary>
        /// Renders outer left border
        /// </summary>
        OuterLeft = 16,

        /// <summary>
        /// Renders outer right border
        /// </summary>
        OuterRight = 32,

        /// <summary>
        /// Renders outer vertical (left, right) borders
        /// </summary>
        OuterVertical = 48,

        /// <summary>
        /// Renders outer borders
        /// </summary>
        Outer = 60,

        /// <summary>
        /// Renders all borders
        /// </summary>
        All = 63
    }
}
