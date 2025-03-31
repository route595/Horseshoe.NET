using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Contains the imported data for an individual row
    /// </summary>
    public class ImportedDataRow
    {
        /// <summary>
        /// The 1-based imported row number
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// The original row text if importing from a text-based source
        /// </summary>
        public string RawRow { get; set; }

        /// <summary>
        /// The trimmed row data if importing from a text-based source
        /// </summary>
        public string[] TrimmedRawValues { get; set; }

        /// <summary>
        /// The final parsed / zapped / converted imported values
        /// </summary>
        public object[] Values { get; set; }

        /// <summary>
        /// Returns <c>true</c> if and only if <c>Values == null</c>.
        /// </summary>
        public bool IsBlank => Values == null;

        /// <summary>
        /// Displays a brief description of this imported data row including name and data type
        /// </summary>
        /// <returns>Data row description</returns>
        public override string ToString()
        {
            return "{ Row# = " + RowNumber + "; RawRow = \"" + TextUtil.Crop(RawRow, 25, position: HorizontalPosition.Center, truncateMarker: TruncateMarker.LongEllipsis) + "\" }";
        }
    }
}
