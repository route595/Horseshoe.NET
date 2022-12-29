using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.DataImport
{
    // A collectin of utiltiy methods for data import
    internal static class ImportUtil
    {
        internal static bool IsBlankRow(IEnumerable<string> values)
        {
            if (values.Count() == 0)
                return true;
            if (values.Count() == 1)
                return string.IsNullOrWhiteSpace(values.Single());
            return false;
        }
    }
}
