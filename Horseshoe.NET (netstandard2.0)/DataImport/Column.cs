﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Represents data column metadata, responsible for text-to-object parsing as well as object-to-text formatting
    /// </summary>
    public class Column : List<object>
    {
        /// <summary>
        /// The column name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The column's data type
        /// </summary>
        public Type DataType { get; set; } = typeof(object);

        /// <summary>
        /// The column width (only applies to fixed-width data imports)
        /// </summary>
        public int FixedWidth { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column are parsed from text.
        /// Applies to text file imports.
        /// </summary>
        public Func<string, object> Parser { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column are converted from another <c>object</c>. 
        /// Applies to object-direct imports such as from Excel (see <c>Horseshoe.NET.Excel</c> project / NuGet package).
        /// </summary>
        public Func<object, object> Converter { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column should be formtted
        /// </summary>
        public Func<object, string> Formatter { get; set; }

        /// <summary>
        /// Indicates whether <c>Column</c> adds a raw datum to the imported row
        /// </summary>
        public bool NotMapped { get; set; }

        /// <summary>
        /// How to diplay <c>null</c> values
        /// </summary>
        public string DisplayNullAs { get; set; } = "[null]";

        /// <summary>
        /// Obsolete.  All the data errors contained in this column.
        /// </summary>
        [Obsolete("Not used", false)]
        public IEnumerable<DataError> DataErrors => this
            .Where(o => o is DataError)
            .Select(o => o as DataError)
            .ToList();

        /// <summary>
        /// Creates a new <c>Column</c>
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="DataImportException"></exception>
        public Column(string name)
        {
            Name = Zap.String(name) ?? throw new DataImportException("columns must have a name");
        }

        /// <summary>
        /// Parses a raw datum into its <c>object</c> value according the column metadata
        /// </summary>
        /// <param name="raw">a raw datum</param>
        /// <param name="col">1-based column number</param>
        /// <param name="srcRow">1-based row number (calculated source row number)</param>
        /// <param name="errorHandling">how to handle data errors</param>
        /// <returns></returns>
        /// <exception cref="InvalidDatumException"></exception>
        /// <exception cref="DataImportException"></exception>
        public object Parse(string raw, int col, int srcRow, DataErrorHandlingPolicy errorHandling = default)
        {
            if (raw == null)
                return null;
            if (Parser != null)
            {
                try
                {
                    return Parser.Invoke(raw);
                }
                catch (Exception ex)
                {
                    switch (errorHandling)
                    {
                        case DataErrorHandlingPolicy.Throw:
                        default:
                            throw new InvalidDatumException(ex.RenderMessage(), raw, fixedWidth: 0, position: "col: " + col + ", src row: " + srcRow);
                        case DataErrorHandlingPolicy.Embed:
                            return new DataError(ex.Message, col, srcRow);
                        case DataErrorHandlingPolicy.IgnoreAndUseDefaultValue:
                            return TypeUtil.GetDefaultValue(DataType);
                    }
                    throw;
                }
            }
            else if (DataType != typeof(string) && DataType != typeof(object))
            {
                throw new DataImportException("column \"" + Name + "\" does not contain a parser for " + DataType);
            }
            return raw;
        }

        /// <summary>
        /// Format a datum as text
        /// </summary>
        /// <param name="obj">a datum</param>
        /// <returns></returns>
        public string Format(object obj)
        {
            if (obj == null)
                return DisplayNullAs ?? "";
            if (Formatter == null || obj is DataError)
                return obj.ToString();
            return Formatter.Invoke(obj);
        }

        /// <summary>
        /// Creates a string representation of this <c>Column</c> for troubleshooting / informational purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DataType.FullName +
                "(\"" + Name + "\"" +
                (FixedWidth != 0 ? ", " + FixedWidth.ToString() : "") +
                ")";
        }

        /// <summary>
        /// Creates a basic <c>object</c> type <c>Column</c>
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column Object(string name, int fixedWidth = 0) => new Column(name) { FixedWidth = fixedWidth };

        /// <summary>
        /// Creates a basic <c>string</c> type <c>Column</c>
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column String(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(string), FixedWidth = fixedWidth };

        /// <summary>
        /// Creates a basic <c>int</c> type <c>Column</c>
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column Int(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(int), FixedWidth = fixedWidth, Parser = (str) => Zap.Int(str) };

        /// <summary>
        /// Creates a basic <c>decimal</c> type <c>Column</c>
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column Decimal(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(decimal), FixedWidth = fixedWidth, Parser = (str) => Zap.Decimal(str) };

        /// <summary>
        /// Creates a basic currency type <c>Column</c> (<c>decimal</c> type)
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column Currency(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(decimal), FixedWidth = fixedWidth, Parser = (str) => Zap.Decimal(str), Formatter = (obj) => string.Format("{0:C}", obj) };

        /// <summary>
        /// Creates a basic bool <c>Column</c>
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column Bool(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(bool), FixedWidth = fixedWidth, Parser = (str) => Zap.Boolean(str) };

        /// <summary>
        /// Creates a basic <c>DateTime</c> <c>Column</c> for dates
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <param name="sourceLocale">the locale of the dates to be imported (e.g. "en-US")</param>
        /// <param name="displayFormat">custom date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayLocale">a locale (e.g. "en-US")</param>
        /// <returns></returns>
        public static Column Date(string name, int fixedWidth = 0, string sourceLocale = null, string displayFormat = null, string displayLocale = null) => new Column(name)
        {
            FixedWidth = fixedWidth,
            DataType = typeof(DateTime),
            Parser = (str) => Zap.DateTime(str, provider: string.IsNullOrEmpty(sourceLocale) ? null : CultureInfo.GetCultureInfo(sourceLocale)),
            Formatter = (o) => DateFormat(o, format: displayFormat, locale: displayLocale)
        };

        /// <summary>
        /// Creates a basic <c>DateTime</c> <c>Column</c> for dates with a builtin parser that reads YYYYmmdd formatted dates
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="displayFormat">custom date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayLocale">a locale (e.g. "en-US")</param>
        /// <returns></returns>
        public static Column Flat8Date(string name, string displayFormat = null, string displayLocale = null) => new Column(name)
        {
            FixedWidth = 8,
            DataType = typeof(DateTime),
            Parser = (str) => str == "00000000" ? DateTime.MinValue : new DateTime(int.Parse(str.Substring(0, 4)), int.Parse(str.Substring(4, 2)), int.Parse(str.Substring(6))),
            Formatter = (o) => DateFormat(o, format: displayFormat, locale: displayLocale)
        };

        /// <summary>
        /// Creates a basic <c>DateTime</c> <c>Column</c> for times
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <param name="sourceLocale">the locale of the dates to be imported (e.g. "en-US")</param>
        /// <param name="displayFormat">custom time format (e.g. "T", "HH:mm:ss", etc.)</param>
        /// <param name="displayLocale">a locale (e.g. "en-US")</param>
        /// <returns></returns>
        public static Column Time(string name, int fixedWidth = 0, string sourceLocale = null, string displayFormat = null, string displayLocale = null) => new Column(name)
        {
            FixedWidth = fixedWidth,
            DataType = typeof(DateTime),
            Parser = (obj) => TimeConverter(obj, locale: sourceLocale) ?? DateTime.MinValue,
            Formatter = (o) => TimeFormat(o, format: displayFormat, locale: displayLocale)
        };

        /// <summary>
        /// Creates a column that has no data mapping
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column NoMap(string name, int fixedWidth = 0) => new Column(name) { FixedWidth = fixedWidth, NotMapped = true };

        /// <summary>
        /// Attempt to auto-create a <c>Column</c> based on a data type
        /// </summary>
        /// <param name="type">a type</param>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <returns></returns>
        public static Column ByType(Type type, string name, int fixedWidth = 0)
        {
            if (type == typeof(string))
                return String(name, fixedWidth: fixedWidth);
            if (type == typeof(int))
                return Int(name, fixedWidth: fixedWidth);
            if (type == typeof(decimal))
                return Decimal(name, fixedWidth: fixedWidth);
            if (type == typeof(bool))
                return Bool(name, fixedWidth: fixedWidth);
            if (type == typeof(DateTime))
                return Date(name, fixedWidth: fixedWidth);
            return Object(name, fixedWidth: fixedWidth);
        }

        /// <summary>
        /// A robust date formatter accepting custom formats and locales, default is "flex" (intelligently truncates zeros)
        /// </summary>
        /// <param name="obj">A date/time object</param>
        /// <param name="format">custom date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="locale">a locale (e.g. "en-US")</param>
        /// <returns></returns>
        public static string DateFormat(object obj, string format = null, string locale = null)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (!string.IsNullOrEmpty(format))
                {
                    if (!string.IsNullOrEmpty(locale))
                        return dateTimeValue.ToString(format, CultureInfo.GetCultureInfo(locale));
                    return dateTimeValue.ToString(format);
                }
                else if (!string.IsNullOrEmpty(locale))
                    return dateTimeValue.ToFlexDateString(provider: CultureInfo.GetCultureInfo(locale));
                return dateTimeValue.ToFlexDateString();
            }
            return obj.ToString();
        }

        /// <summary>
        /// A robust time formatter accepting custom formats and locales, default is "flex" (intelligently truncates zeros)
        /// </summary>
        /// <param name="obj">A date/time object</param>
        /// <param name="format">custom time format (e.g. "T", "HH:mm:ss", etc.)</param>
        /// <param name="locale">locale (e.g. "en-US")</param>
        /// <returns></returns>
        public static string TimeFormat(object obj, string format = null, string locale = null)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (!string.IsNullOrEmpty(format))
                {
                    if (!string.IsNullOrEmpty(locale))
                        return dateTimeValue.ToString(format, CultureInfo.GetCultureInfo(locale));
                    return dateTimeValue.ToString(format);
                }
                else if (!string.IsNullOrEmpty(locale))
                    return dateTimeValue.ToFlexTimeString(provider: CultureInfo.GetCultureInfo(locale));
                return dateTimeValue.ToFlexTimeString();
            }
            return obj?.ToString() ?? "";
        }

        static object TimeConverter(object obj, string locale = null)
        {
            if (obj == null) 
                return null;
            var timeValue = DateTime.MinValue;
            if (obj is string stringValue)
            {
                obj = Zap.DateTime(stringValue.Trim(), provider: string.IsNullOrEmpty(locale) ? null : CultureInfo.GetCultureInfo(locale));
            }
            if (obj is DateTime dateTimeValue)
            {
                timeValue = timeValue.AddHours(dateTimeValue.Hour);
                timeValue = timeValue.AddMinutes(dateTimeValue.Minute);
                timeValue = timeValue.AddSeconds(dateTimeValue.Second);
                timeValue = timeValue.AddMilliseconds(dateTimeValue.Millisecond);
                return timeValue;
            }
            return obj;
        }
    }
}
