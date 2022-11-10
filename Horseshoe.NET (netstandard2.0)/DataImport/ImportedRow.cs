using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.DataImport
{
    public class ImportedRow
    {
        public int SourceLineNumber { get; }
        public string[] Values { get; private set; }
        public bool IsEmpty => Values == null;

        public ImportedRow (int sourceLineNumber, IEnumerable<string> values)
        {
            SourceLineNumber = sourceLineNumber;
            Values = values?.ToArray();
        }

        public void UpdateValues(string[] values)
        {
            Values = values;
        }
    }
}
