using System;
using System.Globalization;
using System.Text;
using Horseshoe.NET.Format;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Data
{
    /// <summary>
    /// Represents column data and metadata and is responsible for value parsing (on import) and formatting (on export)
    /// </summary>
    public abstract class ColumnBase
    {
        /// <summary>
        /// The column name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of data in this column
        /// </summary>
        public Type DataType { get; set; } = typeof(object);

        /// <summary>
        /// Responsible for formatting the values associated with this column.
        /// </summary>
        public Formatter Formatter { get; set; }

        /// <summary>
        /// Whether this column should be hidden in the output.  May not be honored by all implementations and the default is <c>false</c>.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// <c>ColumnBase</c> constructor
        /// </summary>
        public ColumnBase() { }

        /// <summary>
        /// <c>ColumnBase</c> constructor - copies properties from the argument <c>ColumnBase</c> instance
        /// </summary>
        /// <param name="columnBase"></param>
        public ColumnBase(ColumnBase columnBase)
        {
            Name = columnBase.Name;
            DataType = columnBase.DataType;
            Formatter = columnBase.Formatter;
            Hidden = columnBase.Hidden;
        }

        /// <summary>
        /// Formats a value (from the column, by convention) based on the column formatting rules
        /// </summary>
        /// <param name="value">An object</param>
        /// <returns>The formatted object</returns>
        public string Format(object value) =>
            Formatter?.Format(value) ?? value?.ToString() ?? TextConstants.Null;

        /// <summary>
        /// Displays a brief description of this column including name and data type
        /// </summary>
        /// <returns>Column description</returns>
        public override string ToString()
        {
            return "{ Name = " + ValueUtil.Display(Name) + "; DataType = " + ValueUtil.Display(DataType) + " }"; 
        }
    }
}
