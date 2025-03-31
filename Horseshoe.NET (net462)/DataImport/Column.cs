using System;
using System.Globalization;

using Horseshoe.NET.Data;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Represents column data and metadata and is responsible for value parsing (on import) and formatting (on export)
    /// </summary>
    public class Column : ColumnBase, IEquatable<Column>
    {
        /// <summary>
        /// <para>
        /// The 0-based column start position (if fixed-width source)
        /// </para>
        /// Example
        /// <code>
        /// [fixed-width.txt]  (width=8)
        ///                     ┌ dob  ┐ ┌┐# kids (width=2)
        /// Smith, Billy Bob    20010519N0000
        /// Weatherton, Michelle19990212N0001
        /// O'Reilly, Stephen H.20010519Y0100
        /// Chapel, Suzanne     20010519Y0202
        /// └  name (width=20) ┘        │  └┘# pets (width=2)
        ///                 married (width=1) 
        ///
        /// // c# code
        /// var dataImport = DataImport.ImportFixedWidthFile
        /// (
        ///     "fixed-width.txt", 
        ///     new[]
        ///     {
        ///         Column.String(   "Name",           startPosition:  0, width: 20),
        ///         Column.DateTime( "Date of Birth",  startPosition: 20, width:  8, dateFormat="yyyyMMdd"),
        ///         Column.Bool(     "Married",        startPosition: 28, width:  1),
        ///         Column.Int(      "Number of Kids", startPosition: 29, width:  2),
        ///         Column.Int(      "Number of Pets", startPosition: 31, width:  2)
        ///     }
        /// );
        /// </code>
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// <para>
        /// The column width in <c>char</c>s (if fixed-width source)
        /// </para>
        /// Example
        /// <code>
        /// [fixed-width.txt]  (width=8)
        ///                     ┌ dob  ┐ ┌┐# kids (width=2)
        /// Smith, Billy Bob    20010519N0000
        /// Weatherton, Michelle19990212N0001
        /// O'Reilly, Stephen H.20010519Y0100
        /// Chapel, Suzanne     20010519Y0202
        /// └  name (width=20) ┘        │  └┘# pets (width=2)
        ///                 married (width=1) 
        ///
        /// // c# code
        /// var dataImport = DataImport.ImportFixedWidthFile
        /// (
        ///     "fixed-width.txt", 
        ///     new[]
        ///     {
        ///         Column.String(   "Name",           startPosition:  0, width: 20),
        ///         Column.DateTime( "Date of Birth",  startPosition: 20, width:  8, dateFormat="yyyyMMdd"),
        ///         Column.Bool(     "Married",        startPosition: 28, width:  1),
        ///         Column.Int(      "Number of Kids", startPosition: 29, width:  2),
        ///         Column.Int(      "Number of Pets", startPosition: 31, width:  2)
        ///     }
        /// );
        /// </code>
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column are parsed even if column import is not text-based e.g. Excel int -> DateTime.
        /// </summary>
        public Func<object, object> SourceParser { get; set; }

        /// <summary>
        /// If supplied, indicates the expected number format.
        /// </summary>
        public NumberStyles? SourceNumberStyle { get; set; }

        /// <summary>
        /// If supplied, indicates the exact format from which the date/time will be parsed.
        /// </summary>
        public string SourceDateFormat { get; set; }

        /// <summary>
        /// If supplied, indicates the expected date/time style.
        /// </summary>
        public DateTimeStyles? SourceDateTimeStyle { get; set; }

        /// <summary>
        /// An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.
        /// </summary>
        public IFormatProvider SourceFormatProvider { get; set; }

        /// <summary>
        /// An optional locale (e.g. "en-US") used to infer the format provider for parsing the value (if not supplied).
        /// </summary>
        public string SourceLocale { get; set; }

        /// <summary>
        /// A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>, default is <c>"y|yes|t|true|1"</c>.
        /// </summary>
        public string SourceTrueValues { get; set; }

        /// <summary>
        /// A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>, default is <c>"n|no|f|false|0"</c>.
        /// </summary>
        public string SourceFalseValues { get; set; }

        /// <summary>
        /// If <c>true</c>, the letter case of <c>bool</c> and <c>enum</c> values is ignored during parsing, default is <c>false</c>.
        /// </summary>
        public bool SourceIgnoreCase { get; set; }

        /// <summary>
        /// If <c>true</c>, throws an exception if numerical value does not fall within the min/max values of the target type.  Default is <c>false</c>.
        /// </summary>
        public bool SourceStrict { get; set; }

        /// <summary>
        /// Displays a brief description of this column including name and data type
        /// </summary>
        /// <returns>Column description</returns>
        public override string ToString()
        {
            return "{ Name = " + ValueUtil.Display(Name) + "; DataType = " + ValueUtil.Display(DataType?.FullName) + " }";
        }

        /// <summary>
        /// Creates a basic <c>object</c> column
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="startPosition">optional fixed-width column start position</param>
        /// <param name="width">optional fixed-width column width</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>An <c>object</c> column</returns>
        public static Column Object
        (
            string name = null, 
            int startPosition = 0, 
            int width = 0,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => new Column 
        { 
            Name = name,
            StartPosition = startPosition,
            Width = width,
            DisplayFormatter = displayFormatter,
            DisplayPad = displayPad,
            DisplayFormat = displayFormat,
            DisplayFormatProvider = displayFormatProvider,
            DisplayLocale = displayLocale
        };

        /// <summary>
        /// Creates a basic <c>string</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <returns>A <c>string</c> column</returns>
        public static Column String
        (
            string name = null, 
            int startPosition = 0, 
            int width = 0,
            Func<object, string> displayFormatter = null,
            int displayPad = 0
        ) => new Column 
        { 
            Name = name, 
            DataType = typeof(string),
            StartPosition = startPosition,
            Width = width,
            DisplayFormatter = displayFormatter,
            DisplayPad = displayPad
        };

        /// <summary>
        /// Creates a configurable <c>int</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceNumberStyle">If supplied, indicates the expected number style.</param>
        /// <param name="sourceFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="sourceLocale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>An <c>int</c> column</returns>
        public static Column Int
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            NumberStyles? sourceNumberStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => new Column
        {
            Name = name,
            DataType = typeof(int),
            StartPosition = startPosition,
            Width = width,
            SourceNumberStyle = sourceNumberStyle,
            SourceFormatProvider = sourceFormatProvider,
            SourceLocale = sourceLocale,
            DisplayFormatter = displayFormatter,
            DisplayPad = displayPad,
            DisplayFormat = displayFormat,
            DisplayFormatProvider = displayFormatProvider,
            DisplayLocale = displayLocale
        };

        /// <summary>
        /// Creates a configurable <c>int</c> column with hex formatted import and configurable formatted output 
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>An <c>int</c> column</returns>
        public static Column IntFromHex
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => Int
        (
            name: name,
            startPosition: startPosition,
            width: width,
            sourceNumberStyle: NumberStyles.HexNumber,
            displayFormatter: displayFormatter,
            displayPad: displayPad,
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale
        );

        /// <summary>
        /// Creates a configurable <c>decimal</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceNumberStyle">If supplied, indicates the expected number style.</param>
        /// <param name="sourceFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="sourceLocale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>A <c>decimal</c> column</returns>
        public static Column Decimal
        (
            string name = null, 
            int startPosition = 0, 
            int width = 0,
            NumberStyles? sourceNumberStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = "C",
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => new Column
        { 
            Name = name, 
            DataType = typeof(decimal),
            StartPosition = startPosition,
            Width = width, 
            SourceNumberStyle = sourceNumberStyle,
            SourceFormatProvider = sourceFormatProvider,
            SourceLocale = sourceLocale,
            DisplayFormatter = displayFormatter,
            DisplayPad = displayPad,
            DisplayFormat = displayFormat,
            DisplayFormatProvider = displayFormatProvider,
            DisplayLocale = displayLocale
        };

        /// <summary>
        /// Creates a configurable <c>decimal</c> column with plain numeric import and currency formatted output
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceNumberStyle">If supplied, indicates the expected number style.</param>
        /// <param name="sourceFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="sourceLocale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>A <c>decimal</c> column</returns>
        public static Column Currency
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            NumberStyles? sourceNumberStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = "C",
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => Decimal
        (
            name: name,
            startPosition: startPosition,
            width: width,
            sourceNumberStyle: sourceNumberStyle,
            sourceFormatProvider: sourceFormatProvider,
            sourceLocale: sourceLocale,
            displayFormatter: displayFormatter,
            displayPad: displayPad,
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale
        );

        /// <summary>
        /// Creates a configurable <c>bool</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceTrueValues">A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="sourceFalseValues">A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <returns>A <c>bool</c> column</returns>
        public static Column Bool
        (
            string name = null, 
            int startPosition = 0, 
            int width = 0,
            string sourceTrueValues = "y|yes|t|true|1",
            string sourceFalseValues = "n|no|f|false|0",
            Func<object, string> displayFormatter = null
        ) => new Column
        { 
            Name = name, 
            DataType = typeof(bool), 
            StartPosition = startPosition,
            Width = width, 
            SourceTrueValues = sourceTrueValues,
            SourceFalseValues = sourceFalseValues,
            DisplayFormatter = displayFormatter
        };

        /// <summary>
        /// Creates a configurable <c>DateTime</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceParser">How <c>object</c>s associated with this column are parsed even if column import is not text-based e.g. Excel int -> DateTime.</param>
        /// <param name="sourceDateFormat">Optional, specific source date format (e.g. "yyyyMMdd", etc.)</param>
        /// <param name="sourceDateTimeStyle">If supplied, indicates the expected date/time style.</param>
        /// <param name="sourceFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="sourceLocale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>A <c>DateTime</c> column</returns>
        public static Column DateTime
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            Func<object, object> sourceParser = null,
            string sourceDateFormat = null,
            DateTimeStyles? sourceDateTimeStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => new Column
        {
            Name = name,
            DataType = typeof(DateTime),
            StartPosition = startPosition,
            Width = width,
            SourceParser = sourceParser,
            SourceDateFormat = sourceDateFormat,
            SourceDateTimeStyle = sourceDateTimeStyle,
            SourceFormatProvider = sourceFormatProvider,
            SourceLocale = sourceLocale,
            DisplayFormatter = displayFormatter,
            DisplayPad = displayPad,
            DisplayFormat = displayFormat,
            DisplayFormatProvider = displayFormatProvider,
            DisplayLocale = displayLocale
        };

        /// <summary>
        /// Creates a configurable <c>DateTime</c> column formatted YYYYMMDD at the source with formatted output
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="displayFormatter">How <c>values</c>s associated with this column should be formatted.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns>A <c>DateTime</c> column</returns>
        public static Column DateTimeYYYYMMDD
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            Func<object, string> displayFormatter = null,
            int displayPad = 0,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => DateTime
        (
            name: name,
            startPosition: startPosition,
            width: width,
            sourceDateFormat: "yyyyMMdd",
            displayFormatter: displayFormatter,
            displayPad: displayPad,
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale
        );

        /// <summary>
        /// Returns <c>true</c> if the argument instance is the same or equivalent to the current instance.
        /// </summary>
        /// <param name="other">A column to compare to the current column</param>
        /// <returns><c>bool</c></returns>
        public bool Equals(Column other)
        {
            if (other == null)
                return false;
            if (this == other)
                return true;
            return Name == other.Name && StartPosition == other.StartPosition && Width == other.Width && DataType == other.DataType &&
                ValueUtil.AllOrNone(SourceParser, other.SourceParser) &&
                SourceNumberStyle == other.SourceNumberStyle &&
                SourceDateFormat == SourceDateFormat && SourceDateTimeStyle == other.SourceDateTimeStyle &&
                SourceFormatProvider == other.SourceFormatProvider && SourceLocale == other.SourceLocale &&
                SourceTrueValues == other.SourceTrueValues && SourceFalseValues == other.SourceFalseValues &&
                SourceIgnoreCase == other.SourceIgnoreCase && SourceStrict == other.SourceStrict &&
                DisplayNullAs == other.DisplayNullAs &&
                ValueUtil.AllOrNone(DisplayFormatter, other.DisplayFormatter) &&
                DisplayFormat == other.DisplayFormat && DisplayPad == other.DisplayPad &&
                DisplayFormatProvider == other.DisplayFormatProvider && DisplayLocale == other.DisplayLocale;
        }
    }
}
