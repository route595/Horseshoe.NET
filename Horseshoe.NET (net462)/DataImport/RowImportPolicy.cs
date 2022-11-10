using System;

namespace Horseshoe.NET.DataImport
{
    [Flags]
    public enum RowImportPolicy
    {
        FreeRange = 0,
        Fill = 1,
        EnforceMaxLength = 2
    }
}
