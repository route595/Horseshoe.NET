using System;
using System.Globalization;

using Horseshoe.NET.Data;
using Horseshoe.NET.Format;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

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
        /// Creates a basic <c>object</c> column
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="dataType">The type of data in this column</param>
        /// <param name="startPosition">optional fixed-width column start position</param>
        /// <param name="width">optional fixed-width column width</param>
        /// <param name="displayFormatter">
        /// Responsible for formatting the values associated with this column. 
        /// Alternatively, use <c>displayFormat</c> optionally along with <c>displayFormatProvider</c>, <c>displayLocale</c>, <c>displayNullAs</c> and <c>displayPad</c>
        /// </param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">If supplied, this custom formatter trumps the other display related parameters.</param>
        /// <param name="customPostRenderer">
        /// Optionally adds custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// </code>
        /// </para>
        /// </param>
        /// <returns>An <c>object</c> column</returns>
        public static Column Object
        (
            string name = null,
            Type dataType = null,
            int startPosition = 0,
            int width = 0,
            Formatter displayFormatter = null,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        )
        {
            if (displayFormatter == null)
            {
                if (customFormatter != null)
                    displayFormatter = new Formatter(customFormatter, displayNullAs: displayNullAs);
                else if (displayFormat != null)
                    displayFormatter = new Formatter(displayFormat, provider: displayFormatProvider, locale: displayLocale, pad: displayPad, displayNullAs: displayNullAs, customPostRenderer: customPostRenderer);
            }

            return new Column
            {
                Name = name,
                DataType = dataType ?? typeof(object),
                StartPosition = startPosition,
                Width = width,
                Formatter = displayFormatter
            };
        }

        /// <summary>
        /// Creates a basic <c>string</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="displayNullAs">How to display <c>null</c> values.  One option is <c>Horseshoe.NET.Text.TextConstants.Null</c>.  Default is <c>""</c>.</param>
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
            string displayNullAs = null,
            int displayPad = 0
        ) => new Column 
        { 
            Name = name, 
            DataType = typeof(string),
            StartPosition = startPosition,
            Width = width,
            Formatter = new Formatter
            (
                value =>
                {
                    string stringValue = value is string _stringValue
                        ? _stringValue
                        : value.ToString();  // note: this is safe due to value is guaranteed to not be null
                    if (displayPad > 0)
                        return stringValue.PadLeft(displayPad);
                    if (displayPad < 0)
                        return stringValue.PadRight(displayPad * -1);
                    return stringValue;
                }, 
                displayNullAs: displayNullAs
            )
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
        /// <param name="displayFormatter">
        /// Responsible for formatting the values associated with this column. 
        /// Alternatively, use <c>displayFormat</c> optionally along with <c>displayFormatProvider</c>, <c>displayLocale</c>, <c>displayNullAs</c> and <c>displayPad</c>
        /// </param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>Column</c></returns>
        public static Column Int
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            NumberStyles? sourceNumberStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            Formatter displayFormatter = null,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        )
        {
            var column = Object
            (
                name: name,
                dataType: typeof(int),
                startPosition: startPosition,
                width: width,
                displayFormatter: displayFormatter,
                displayFormat: displayFormat,
                displayFormatProvider: displayFormatProvider,
                displayLocale: displayLocale,
                displayNullAs: displayNullAs,
                displayPad: displayPad,
                customFormatter: customFormatter,
                customPostRenderer: customPostRenderer
            );
            column.SourceNumberStyle = sourceNumberStyle;
            column.SourceFormatProvider = sourceFormatProvider;
            column.SourceLocale = sourceLocale;
            return column;
        }

        /// <summary>
        /// Creates a configurable <c>int</c> column with hex formatted input and configurable formatted output 
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="displayFormatter">
        /// Responsible for formatting the values associated with this column. 
        /// Alternatively, use <c>displayFormat</c> optionally along with <c>displayFormatProvider</c>, <c>displayLocale</c>, <c>displayNullAs</c> and <c>displayPad</c>
        /// </param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>Column</c></returns>
        public static Column IntFromHex
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            Formatter displayFormatter = null,
            string displayFormat = "X",
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        ) => Int
        (
            name: name,
            startPosition: startPosition,
            width: width,
            sourceNumberStyle: NumberStyles.HexNumber,
            displayFormatter: displayFormatter,
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale,
            displayNullAs: displayNullAs,
            displayPad: displayPad,
            customFormatter: customFormatter,
            customPostRenderer: customPostRenderer
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
        /// <param name="displayFormatter">
        /// Responsible for formatting the values associated with this column. 
        /// Alternatively, use <c>displayFormat</c> optionally along with <c>displayFormatProvider</c>, <c>displayLocale</c>, <c>displayNullAs</c> and <c>displayPad</c>
        /// </param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>Column</c></returns>
        public static Column Decimal
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            NumberStyles? sourceNumberStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            Formatter displayFormatter = null,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        )
        {
            var column = Object
            (
                name: name,
                dataType: typeof(decimal),
                startPosition: startPosition,
                width: width,
                displayFormatter: displayFormatter,
                displayFormat: displayFormat,
                displayFormatProvider: displayFormatProvider,
                displayLocale: displayLocale,
                displayNullAs: displayNullAs,
                displayPad: displayPad,
                customFormatter: customFormatter,
                customPostRenderer: customPostRenderer
            );
            column.SourceNumberStyle = sourceNumberStyle;
            column.SourceFormatProvider = sourceFormatProvider;
            column.SourceLocale = sourceLocale;
            return column;
        }

        /// <summary>
        /// Creates a configurable currency formatted <c>decimal</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceNumberStyle">If supplied, indicates the expected number style.</param>
        /// <param name="sourceFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="sourceLocale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>Column</c></returns>
        public static Column Currency
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            NumberStyles? sourceNumberStyle = null,
            IFormatProvider sourceFormatProvider = null,
            string sourceLocale = null,
            string displayFormat = "C",
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        ) => 
        Decimal
        (
            name: name,
            startPosition: startPosition,
            width: width,
            sourceNumberStyle: sourceNumberStyle,
            sourceFormatProvider: sourceFormatProvider,
            sourceLocale: sourceLocale,
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale,
            displayNullAs: displayNullAs,
            displayPad: displayPad,
            customFormatter: customFormatter,
            customPostRenderer: customPostRenderer
        );

        /// <summary>
        /// Creates a configurable <c>bool</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceTrueValues">A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="sourceFalseValues">A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="displayTrueValue">A value indicating <c>true</c>, e.g. <c>"Y"</c>, <c>"True"</c>, <c>"1"</c>, etc.</param>
        /// <param name="displayFalseValue">A value indicating <c>false</c>, e.g. <c>"N"</c>, <c>"False"</c>, <c>"0"</c>, etc.</param>
        /// <param name="displayNullAs">How to display <c>null</c> values.  One option is <c>Horseshoe.NET.Text.TextConstants.Null</c>.  Default is <c>""</c>.</param>
        /// <param name="displayPad">
        /// How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc ").  
        /// The purpose is mainly to visually align values in a column.
        /// </param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>Column</c></returns>
        public static Column Bool
        (
            string name = null,
            int startPosition = 0,
            int width = 0,
            string sourceTrueValues = "y|yes|t|true|1",
            string sourceFalseValues = "n|no|f|false|0",
            string displayTrueValue = null,
            string displayFalseValue = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        )
        {
            var column = Object
            (
                name: name,
                dataType: typeof(bool),
                startPosition: startPosition,
                width: width,
                displayNullAs: displayNullAs,
                displayPad: displayPad,
                customFormatter: customFormatter ?? formatBool,
                customPostRenderer: customPostRenderer
            );
            column.SourceTrueValues = sourceTrueValues;
            column.SourceFalseValues = sourceFalseValues;
            return column;

            string formatBool(object value) 
            { 
                if (value is bool boolValue)
                    return (boolValue ? displayTrueValue : displayFalseValue) ?? boolValue.ToString();
                return value?.ToString() ?? TextConstants.Null; 
            }
        }

        /// <summary>
        /// Creates a configurable <c>decimal</c> column
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="sourceParser">How <c>object</c>s associated with this column are parsed even if column import is not text-based e.g. Excel int -> DateTime.</param>
        /// <param name="sourceDateFormat">Optional, specific source date format (e.g. "yyyyMMdd", etc.)</param>
        /// <param name="sourceDateTimeStyle">If supplied, indicates the expected date/time style.</param>
        /// <param name="sourceFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="sourceLocale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="displayFormatter">
        /// Responsible for formatting the values associated with this column. 
        /// Alternatively, use <c>displayFormat</c> optionally along with <c>displayFormatProvider</c>, <c>displayLocale</c>, <c>displayNullAs</c> and <c>displayPad</c>
        /// </param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>Column</c></returns>
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
            Formatter displayFormatter = null,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        )
        {
            var column = Object
            (
                name: name,
                dataType: typeof(DateTime),
                startPosition: startPosition,
                width: width,
                displayFormatter: displayFormatter,
                displayFormat: displayFormat,
                displayFormatProvider: displayFormatProvider,
                displayLocale: displayLocale,
                displayNullAs: displayNullAs,
                displayPad: displayPad,
                customFormatter: customFormatter,
                customPostRenderer: customPostRenderer
            );
            column.SourceParser = sourceParser;
            column.SourceDateFormat = sourceDateFormat;
            column.SourceDateTimeStyle = sourceDateTimeStyle;
            column.SourceFormatProvider = sourceFormatProvider;
            column.SourceLocale = sourceLocale;
            return column;
        }

        /// <summary>
        /// Creates a configurable <c>DateTime</c> column formatted YYYYMMDD at the source with formatted output
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="startPosition">An optional fixed-width column start position</param>
        /// <param name="width">An optional fixed-width column width</param>
        /// <param name="displayFormatter">
        /// Responsible for formatting the values associated with this column. 
        /// Alternatively, use <c>displayFormat</c> optionally along with <c>displayFormatProvider</c>, <c>displayLocale</c>, <c>displayNullAs</c> and <c>displayPad</c>
        /// </param>
        /// <param name="displayFormat">Optional format (e.g. "C", "M/d/yyyy", etc.). Alternatively, use <c>displayFormatter</c> (user-supplied custom formatter).</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <param name="displayNullAs">How to display <c>null</c> values, default is <c>Horseshoe.NET.Text.TextConstants.Null</c> (<c>"[null]"</c>).</param>
        /// <param name="displayPad">How formatted <c>values</c>s should be padded (e.g. 4 -> " abc", -4 -> "abc "). Use padding to visually align values in a column.</param>
        /// <param name="customFormatter">A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <returns>A <c>DateTime</c> column</returns>
        public static Column DateTimeYYYYMMDD
        (
            string name = null,
            int startPosition = 0,
            int width = 8,
            Formatter displayFormatter = null,
            string displayFormat = "yyyyMMdd",
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null,
            string displayNullAs = null,
            int displayPad = 0,
            Func<object, string> customFormatter = null,
            Func<string, object, string> customPostRenderer = null
        ) => DateTime
        (
            name: name,
            startPosition: startPosition,
            width: width,
            sourceDateFormat: "yyyyMMdd",
            displayFormatter: displayFormatter,
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale,
            displayNullAs: displayNullAs,
            displayPad: displayPad,
            customFormatter: customFormatter,
            customPostRenderer: customPostRenderer
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
            return Name == other.Name
                && DataType == other.DataType
                && Equals(Formatter, other.Formatter)
                && Hidden == other.Hidden
                && StartPosition == other.StartPosition
                && Width == other.Width
                && ValueUtil.AllOrNone(SourceParser, other.SourceParser)
                && SourceNumberStyle == other.SourceNumberStyle
                && SourceDateFormat == SourceDateFormat
                && SourceDateTimeStyle == other.SourceDateTimeStyle
                && SourceFormatProvider == other.SourceFormatProvider
                && SourceLocale == other.SourceLocale
                && SourceTrueValues == other.SourceTrueValues
                && SourceFalseValues == other.SourceFalseValues
                && SourceIgnoreCase == other.SourceIgnoreCase
                && SourceStrict == other.SourceStrict;
        }
    }
}
