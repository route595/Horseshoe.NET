using System;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// Which cell borders to pad in the <c>TextGrid</c>.
    /// </summary>
    [Flags]
    public enum CellPaddingPolicy
    {
        /// <summary>
        /// No borders (default)
        /// </summary>
        None = 0,

        /// <summary>
        /// Pads tops of cells.  Combine with 'Inner' or 'Outer' for more effect.
        /// </summary>
        Top = 1,

        /// <summary>
        /// Pads bottoms of cells.  Combine with 'Inner' or 'Outer' for more effect.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// Pads tops and bottoms of cells.  Combine with 'Inner' or 'Outer' for more effect.
        /// </summary>
        Horizontal = 3,

        /// <summary>
        /// Pads left sides of cells.  Combine with 'Inner' or 'Outer' for more effect.
        /// </summary>
        Left = 4,

        /// <summary>
        /// Pads right sides of cells.  Combine with 'Inner' or 'Outer' for more effect.
        /// </summary>
        Right = 8,

        /// <summary>
        /// Pads vertical sides of cells.  Combine with 'Inner' or 'Outer' for more effect.
        /// </summary>
        Vertical = 12,

        /// <summary>
        /// Pads all sides of all cells.
        /// </summary>
        All = 15,

        /// <summary>
        /// Leave padding off the leftmost border
        /// </summary>
        ExceptLeftmost = 16,

        /// <summary>
        /// Leave padding off the rightmost border
        /// </summary>
        ExceptRightmost = 32,

        /// <summary>
        /// Leave padding off the leftmost and rightmost borders
        /// </summary>
        ExceptOutermost = 48
    }
}
