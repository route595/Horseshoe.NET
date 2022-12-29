using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Represents an imported data row including metadata i.e. source line number
    /// </summary>
    public class ImportedRow
    {
        /// <summary>
        /// The calculated line number in the original file where the row data came from
        /// </summary>
        public int SourceLineNumber { get; }

        /// <summary>
        /// The row values read in from the text / file
        /// </summary>
        public string[] Values { get; private set; }

        /// <summary>
        /// Whether this row contains any data
        /// </summary>
        public bool IsEmpty => Values == null;

        /// <summary>
        /// Creates new <c>ImportedRow</c>
        /// </summary>
        /// <param name="sourceLineNumber"></param>
        /// <param name="values"></param>
        public ImportedRow (int sourceLineNumber, IEnumerable<string> values)
        {
            SourceLineNumber = sourceLineNumber;
            Values = values?.ToArray();
        }

        /// <summary>
        /// Replaces <c>Values</c> with the supplied <c>string[]</c>
        /// </summary>
        /// <param name="values"></param>
        public void UpdateValues(string[] values)
        {
            Values = values;
        }
    }
}
