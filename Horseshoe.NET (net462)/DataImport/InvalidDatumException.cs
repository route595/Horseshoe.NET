using System;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Exception to be thrown when a data value cannot be parsed or is otherwise invalid
    /// </summary>
    public class InvalidDatumException : DataImportException
    {
        /// <summary>
        /// The invalid datum
        /// </summary>
        public object Datum { get; set; }

        /// <summary>
        /// The column name where the invalid datum was encountered
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The size of the column if fixed-width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Position information, free text (e.g. row 4 col 3)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Create a new <c>InvalidDatumException</c>
        /// </summary>
        /// <param name="message">a message</param>
        /// <param name="datum">the invalid datum</param>
        /// <param name="columnName">the column name where the invalid datum was encountered</param>
        /// <param name="width">the size of the column if fixed-width</param>
        /// <param name="position">Position or cell reference, free text (e.g. row 4 col 3)</param>
        public InvalidDatumException(string message, object datum, string columnName = null, int width = 0, string position = null) : base(ToMessageString(message, datum, columnName, width, position))
        {
            Datum = datum;
            ColumnName = columnName;
            Width = width;
            Position = position;
        }

        /// <summary>
        /// Create a new <c>InvalidDatumException</c>
        /// </summary>
        /// <param name="innerException">the root exception</param>
        /// <param name="datum">the invalid datum</param>
        /// <param name="columnName">the column name where the invalid datum was encountered</param>
        /// <param name="width">the size of the column if fixed-width</param>
        /// <param name="position">Position or cell reference, free text (e.g. row 4 col 3)</param>
        public InvalidDatumException(Exception innerException, object datum, string columnName = null, int width = 0, string position = null) : base(ToMessageString(innerException.Message, datum, columnName, width, position), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            Width = width;
            Position = position;
        }

        static string ToMessageString(string message, object datum, string columnName, int width, string position)
        {
            return
                (!string.IsNullOrEmpty(message) ? message + " -- " : "") +
                "{ Value: \"" + (datum?.ToString() ?? TextConstants.Null).Replace("\"", "\\\"") + "\"" + 
                (columnName != null ? "; Column: \"" + columnName + "\"" : "") +
                (width > 0 ? "; Fixed Width: " + width : "") +
                (position != null ? "; Position: \"" + position + "\"": "") +
                " }";
        }
    }
}
