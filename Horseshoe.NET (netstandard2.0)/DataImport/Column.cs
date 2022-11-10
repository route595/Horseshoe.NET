using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.DataImport
{
    public class Column : List<object>
    {
        public string Name { get; }

        public Type DataType { get; set; } = typeof(object);

        public int FixedWidth { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column are parsed from text
        /// </summary>
        public Func<string, object> Parser { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column are converted from another <c>object</c>. 
        /// Applies object-direct imports such as from Excel.
        /// </summary>
        public Func<object, object> Converter { get; set; }

        /// <summary>
        /// How <c>object</c>s associated with this column should be formtted
        /// </summary>
        public Func<object, string> Formatter { get; set; }

        public string DisplayNullAs { get; set; } = "[null]";

        public IEnumerable<DataError> DataErrors => this
            .Where(o => o is DataError)
            .Select(o => o as DataError)
            .ToList();

        public Column (string name)
        {
            Name = Zap.String(name) ?? throw new DataImportException("columns must have a name");
        }

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
                            return ObjectUtil.GetDefault(DataType);
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

        public string Format(object obj)
        {
            if (obj == null)
                return DisplayNullAs ?? "";
            if (Formatter == null || obj is DataError)
                return obj.ToString();
            return Formatter.Invoke(obj);
        }

        public override string ToString()
        {
            return DataType.FullName +
                "(\"" + Name + "\"" +
                (FixedWidth != 0 ? ", " + FixedWidth.ToString() : "") +
                ")";
        }

        public static Column Object(string name, int fixedWidth = 0) => new Column(name) { FixedWidth = fixedWidth };

        public static Column String(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(string), FixedWidth = fixedWidth };

        public static Column Int(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(int), FixedWidth = fixedWidth, Parser = (str) => Zap.Int(str) };

        public static Column Decimal(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(decimal), FixedWidth = fixedWidth, Parser = (str) => Zap.Decimal(str) };

        public static Column Currency(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(decimal), FixedWidth = fixedWidth, Parser = (str) => Zap.Decimal(str), Formatter = (obj) => string.Format("{0:C}", obj) };

        public static Column Bool(string name, int fixedWidth = 0) => new Column(name) { DataType = typeof(bool), FixedWidth = fixedWidth, Parser = (str) => Zap.Bool(str) };

        public static Column Date(string name, int fixedWidth = 0, string customDateFormat = null) => new Column(name)
        {
            FixedWidth = fixedWidth,
            DataType = typeof(DateTime),
            Parser = (str) => Zap.DateTime(str),
            Formatter = (o) => customDateFormat != null && o is DateTime dt
                ? dt.ToString(customDateFormat)
                : DateFormat(o)
        };

        public static Column Flat8Date(string name, string customDateFormat = null) => new Column(name)
        {
            FixedWidth = 8,
            DataType = typeof(DateTime),
            Parser = (str) => str == "00000000" ? DateTime.MinValue : new DateTime(int.Parse(str.Substring(0, 4)), int.Parse(str.Substring(4, 2)), int.Parse(str.Substring(6))),
            Formatter = (o) => customDateFormat != null && o is DateTime dt
                ? dt.ToString(customDateFormat)
                : DateFormat(o)
        };

        public static Column Time(string name, int fixedWidth = 0, string customTimeFormat = null) => new Column(name)
        {
            FixedWidth = fixedWidth,
            DataType = typeof(DateTime),
            Parser = (obj) => TimeConverter(obj) ?? DateTime.MinValue,
            Formatter = (o) => customTimeFormat != null && o is DateTime dt
                ? dt.ToString(customTimeFormat)
                : TimeFormat(o)
        };

        public static Column NoMap(string name, int fixedWidth = 0) => new Column(name) { FixedWidth = fixedWidth };

        public static Column ByType(Type type, string name, int fixedWidth = 0)
        {
            if (type == typeof(string))
                return String(name, fixedWidth: fixedWidth);
            if (type == typeof(int))
                return Int(name, fixedWidth: fixedWidth);
            if (type == typeof(decimal))
                return Decimal(name, fixedWidth: fixedWidth);
            if (type == typeof(decimal?))
                return Bool(name, fixedWidth: fixedWidth);
            if (type == typeof(DateTime))
                return Date(name, fixedWidth: fixedWidth);
            return new Column(name) 
            {
                FixedWidth = fixedWidth 
            };
        }

        static string DateFormat(object obj)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (dateTimeValue.Hour > 0 || dateTimeValue.Minute > 0)
                {
                    if (dateTimeValue.Millisecond > 0)
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm:ss.fff tt");
                    }
                    else if (dateTimeValue.Second > 0)
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm tt");
                    }
                }
                return dateTimeValue.ToString("M/d/yyyy");
            }
            return obj.ToString();
        }

        public static string DateFormatNoMilliseconds(object obj)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (dateTimeValue.Hour > 0 || dateTimeValue.Minute > 0)
                {
                    if (dateTimeValue.Second > 0)
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        return dateTimeValue.ToString("M/d/yyyy hh:mm tt");
                    }
                }
                return dateTimeValue.ToString("M/d/yyyy");
            }
            return obj?.ToString() ?? "";
        }

        static string TimeFormat(object obj)
        {
            if (obj is DateTime dateTimeValue)
            {
                if (dateTimeValue.Millisecond > 0)
                {
                    return dateTimeValue.ToString("hh:mm:ss.fff tt");
                }
                if (dateTimeValue.Second > 0)
                {
                    return dateTimeValue.ToString("hh:mm:ss tt");
                }
                return dateTimeValue.ToString("hh:mm tt");
            }
            return obj?.ToString() ?? "";
        }

        static object TimeConverter(object obj)
        {
            if (obj == null) return null;
            var timeValue = DateTime.MinValue;
            if (obj is DateTime dateTimeValue)
            {
                timeValue = timeValue.AddHours(dateTimeValue.Hour);
                timeValue = timeValue.AddMinutes(dateTimeValue.Minute);
                timeValue = timeValue.AddSeconds(dateTimeValue.Second);
                timeValue = timeValue.AddMilliseconds(dateTimeValue.Millisecond);
                return timeValue;
            }
            if (obj is string stringValue)
            {
                //var origStringValue = stringValue;
                stringValue = stringValue.Trim();
                var isPM = false;
                if (stringValue.ToUpper().EndsWith("AM"))
                {
                    stringValue = stringValue.Substring(0, stringValue.Length - 2).Trim();
                }
                else if (stringValue.ToUpper().EndsWith("PM"))
                {
                    isPM = true;
                    stringValue = stringValue.Substring(0, stringValue.Length - 2).Trim();
                }
                var timeParts = stringValue.Replace(".", ":").Split(':');
                switch (timeParts.Length)
                {
                    default:
                        return obj;
                    case 2:
                    case 3:
                    case 4:
                        timeValue = timeValue.AddHours(int.Parse(timeParts[0]) + (isPM ? 12 : 0));
                        timeValue = timeValue.AddMinutes(int.Parse(timeParts[1]));
                        if (timeParts.Length > 2)
                        {
                            timeValue = timeValue.AddSeconds(int.Parse(timeParts[2]));
                        }
                        if (timeParts.Length > 3)
                        {
                            timeValue = timeValue.AddMilliseconds(int.Parse(timeParts[3]));
                        }
                        break;
                }
                return timeValue;
            }
            return obj;
        }
    }
}
