using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Excel
{
    public class ImportedExcelRow
    {
        public int SourceRowNumber { get; }
        public object[] Values { get; private set; }
        public bool IsEmpty => Values == null;

        public ImportedExcelRow (int sourceRowNumber, IEnumerable<object> values)
        {
            SourceRowNumber = sourceRowNumber;
            Values = values?.ToArray();
        }

        public void UpdateValues(string[] values)
        {
            Values = values;
        }
    }
}
