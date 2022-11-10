using System;

namespace Horseshoe.NET.Text
{
    [Flags]
    public enum WhitespacePolicy
    {
        Allow = 0,
        Combine = 1,
        CombineExceptNewLines = 2,
        ConvertToSpaces = 4,
        Drop = 8,
        AllowSpaces = 16,
        DropSpaces = 32,
        AllowTabs = 64,
        ConvertTabsToSpaces = 128,
        DropTabs = 256,
        AllowNewLines = 512,
        ConvertNewLinesToSpaces = 1024,
        DropNewLines = 2048,
        AllowNonBreakingSpaces = 4096,
        ConvertNonBreakingSpacesToSpaces = 8192,
        DropNonBreakingSpaces = 16384
    }
}
