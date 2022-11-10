using System;

namespace Horseshoe.NET.Text.TextGrid
{
    [Flags]
    public enum BorderPolicy
    {
        None = 0,
        Outer = 1,
        InnerVertical = 2,
        InnerHorizontal = 4,
        Inner = 6, // e.g. InnerVertical and InnerHorizontal
        All = 7    // e.g. Outer, InnerVertical and InnerHorizontal
    }
}
