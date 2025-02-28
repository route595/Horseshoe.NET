using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// A collection of utiltiy methods for data import
    /// </summary>
    public static class ImportUtil
    {
        internal static bool IsBlankRow(StringValues values)
        {
            switch (values.Count)
            {
                case 0:
                    return true;
                case 1:
                    return string.IsNullOrWhiteSpace(values.Single());
                default:
                    return false;
            }
        }
    }
}
