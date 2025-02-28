using System;
using System.Globalization;

namespace Horseshoe.NET.DataImport
{
    public interface IColumn
    {
        /// <summary>
        /// The column name
        /// </summary>
        string Name { get; }

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
        /// var dataImport = new DataImport("fixed-width.txt")
        ///     .ImportFixedWidth
        ///     (
        ///         new Column&lt;string&gt;   { Name = "Name",           StartPosition =  0, Width = 20 },
        ///         new Column&lt;DateTime&gt; { Name = "Date of Birth",  StartPosition = 20, Width =  8 },
        ///         new Column&lt;bool&gt;     { Name = "Married",        StartPosition = 28, Width =  1 },
        ///         new Column&lt;int&gt;      { Name = "Number of Kids", StartPosition = 29, Width =  2 },
        ///         new Column&lt;int&gt;      { Name = "Number of Pets", StartPosition = 31, Width =  2 }
        ///     )
        ///     .Render(autoTrunc = AutoTruncate.Zap);
        /// </code>
        /// </summary>
        int StartPosition { get; }

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
        /// var dataImport = new DataImport("fixed-width.txt")
        ///     .ImportFixedWidth
        ///     (
        ///         new Column&lt;string&gt;   { Name = "Name",           StartPosition =  0, Width = 20 },
        ///         new Column&lt;DateTime&gt; { Name = "Date of Birth",  StartPosition = 20, Width =  8 },
        ///         new Column&lt;bool&gt;     { Name = "Married",        StartPosition = 28, Width =  1 },
        ///         new Column&lt;int&gt;      { Name = "Number of Kids", StartPosition = 29, Width =  2 },
        ///         new Column&lt;int&gt;      { Name = "Number of Pets", StartPosition = 31, Width =  2 }
        ///     )
        ///     .Render(autoTrunc = AutoTruncate.Zap);
        /// </code>
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The type of data in this column
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// How <c>object</c>s associated with this column should be parsed
        /// </summary>
        Func<string, object> Parser { get; }

        /// <summary>
        /// How <c>object</c>s associated with this column should be formtted
        /// </summary>
        Func<object, string> Formatter { get; }

        /// <summary>
        /// If supplied, indicates the expected number format.
        /// </summary>
        NumberStyles? ParseNumberStyle { get; }

        /// <summary>
        /// An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.
        /// </summary>
        IFormatProvider ParseFormatProvider { get; }

        /// <summary>
        /// If supplied, indicates the expected date/time style.
        /// </summary>
        DateTimeStyles? ParseDateTimeStyle { get; }

        /// <summary>
        /// If supplied, indicates the exact format from which the date/time will be parsed.
        /// </summary>
        string ParseDateFormat { get; }

        /// <summary>
        /// An optional locale (e.g. "en-US") used to infer the format provider (if not supplied).
        /// </summary>
        string ParseLocale { get; }

        /// <summary>
        /// A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>, default is <c>"y|yes|t|true|1"</c>.
        /// </summary>
        string ParseTrueValues { get; }

        /// <summary>
        /// A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>, default is <c>"n|no|f|false|0"</c>.
        /// </summary>
        string ParseFalseValues { get; }

        /// <summary>
        /// If <c>true</c>, the letter case of <c>bool</c> and <c>enum</c> values is ignored during parsing, default is <c>false</c>.
        /// </summary>
        bool IgnoreCase { get; }

        /// <summary>
        /// If <c>true</c>, throws an exception if numerical value does not fall within the min/max values of the target type.  Default is <c>false</c>.
        /// </summary>
        bool Strict { get; }

        /// <summary>
        /// How to diplay <c>null</c> values
        /// </summary>
        string DisplayNullAs { get; }
    }
}
