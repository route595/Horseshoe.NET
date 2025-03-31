using System;
using System.Globalization;
using System.Text;

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
        /// How to diplay <c>null</c> values, default is <c>"[null]"</c>
        /// </summary>
        public string DisplayNullAs { get; set; } = TextConstants.Null;

        /// <summary>
        /// How <c>values</c>s associated with this column should be formatted.
        /// </summary>
        public Func<object, string> DisplayFormatter { get; set; }

        /// <summary>
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </summary>
        public int DisplayPad { get; set; }

        /// <summary>
        /// How <c>values</c>s associated with this column should be formatted.
        /// </summary>
        public string DisplayFormat { get; set; }

        /// <summary>
        /// An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.
        /// </summary>
        public IFormatProvider DisplayFormatProvider { get; set; }

        /// <summary>
        /// An optional locale (e.g. "en-US") used to infer the format provider for displaying the value (if not supplied).
        /// </summary>
        public string DisplayLocale { get; set; }

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
            DisplayNullAs = columnBase.DisplayNullAs;
            DisplayFormatter = columnBase.DisplayFormatter;
            DisplayPad = columnBase.DisplayPad;
            DisplayFormat = columnBase.DisplayFormat;
            DisplayFormatProvider = columnBase.DisplayFormatProvider;
            DisplayLocale = columnBase.DisplayLocale;
        }

        /// <summary>
        /// Formats a value (from the column, by convention) based on the column formatting rules
        /// </summary>
        /// <param name="value">An object</param>
        /// <returns>The formatted object</returns>
        public string Format(object value)
        {
            if (value == null)
                return DisplayNullAs;
            if (DisplayFormatter != null)
                return DisplayFormatter.Invoke(value);
            DisplayFormatProvider = DisplayFormatProvider ?? (!string.IsNullOrWhiteSpace(DisplayLocale) ? CultureInfo.GetCultureInfo(DisplayLocale) : null);
            if (DisplayFormatProvider != null)
                return string.Format(DisplayFormatProvider, BuildFormatString(), value);
            return string.Format(BuildFormatString(), value);
        }

        private StringBuilder strb;

        private string BuildFormatString()  // example "{0,5:N1}°F" -> "  25.7°F"
        {
            if (strb == null)
                strb = new StringBuilder();
            else
                strb.Clear();
            strb.Append("{0")
                .AppendIf(DisplayPad != 0, "," + DisplayPad)
                .AppendIf(!string.IsNullOrWhiteSpace(DisplayFormat), ":" + DisplayFormat)
                .Append("}");
            return strb.ToString();
        }

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
