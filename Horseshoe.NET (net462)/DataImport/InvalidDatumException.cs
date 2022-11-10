using System;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataImport
{
    public class InvalidDatumException : DataImportException
    {
        public object Datum { get; set; }

        public string ColumnName { get; set; }

        public int FixedWidth { get; set; }

        public string Position { get; set; }

        public InvalidDatumException(string message, object datum, string columnName = null, int fixedWidth = 0, string position = null) : base(ToMessageString(message, datum, columnName, fixedWidth, position))
        {
            Datum = datum;
            ColumnName = columnName;
            FixedWidth = fixedWidth;
            Position = position;
        }

        public InvalidDatumException(Exception innerException, object datum, string columnName = null, int fixedWidth = 0, string position = null) : base(ToMessageString(innerException.Message, datum, columnName, fixedWidth, position), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            FixedWidth = fixedWidth;
            Position = position;
        }

        public InvalidDatumException(string message, Exception innerException, object datum, string columnName = null, int fixedWidth = 0, string position = null) : base(ToMessageString(message ?? innerException.Message, datum, columnName, fixedWidth, position), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            FixedWidth = fixedWidth;
            Position = position;
        }

        static string ToMessageString(string message, object datum, string columnName, int fixedWidth, string position)
        {
            return
                (!string.IsNullOrEmpty(message) ? message + " -- " : "") +
                "{ Value: \"" + TextUtil.Reveal(datum).CropCenter(20, truncateMarker: TruncateMarker.LongEllipsis) + "\"" + 
                (columnName != null ? "; Column: \"" + columnName + "\"" : "") +
                (fixedWidth > 0 ? "; Fixed Width: " + fixedWidth : "") +
                (position != null ? "; Position: \"" + position + "\"": "") +
                " }";
        }
    }
}
