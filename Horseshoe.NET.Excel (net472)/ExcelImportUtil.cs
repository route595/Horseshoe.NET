using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DataImport;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Excel
{
    public static class ExcelImportUtil
    {
        internal static bool IsBlankRow(IEnumerable<object> values)
        {
            if (values.Count() == 0)
                return true;
            if (values.Count() == 1)
                if (values.Single() is string stringValue)
                    return Zap.String(stringValue) == null;
            return false;
        }

        public static DateTime ConvertToDateTime(object obj, bool suppressErrors = false)
        {
            return ConvertToNDateTime(obj, suppressErrors: suppressErrors) ?? default;
        }

        public static DateTime? ConvertToNDateTime(object obj, bool suppressErrors = false)
        {
            if (obj == null) return null;
            if (obj is int dateSerial)
            {
                return new DateTime(1900, 1, 1)
                    .AddDays(dateSerial - (dateSerial <= 60 ? 1 : 2));
            }
            if (obj is double dateTimeSerial)
            {
                var datePart = (int)Math.Floor(dateTimeSerial);
                double timePart = dateTimeSerial - datePart;
                return new DateTime(1900, 1, 1)
                    .AddDays(datePart - (datePart <= 60 ? 1 : 2))
                    .AddSeconds(timePart * 86400D);               // 86400 = 24 * 60 * 60 a.k.a.seconds per day 

            }
            if (obj is string stringValue)
            {
                if (stringValue.Length == 0) return null;
                if (DateTime.TryParse(stringValue, out DateTime dateTime))
                {
                    return dateTime;
                }
                if (suppressErrors) return null;
                throw new UtilityException(stringValue + " cannot be converted to date/time");
            }
            throw new UtilityException(obj.GetType().FullName + " cannot be converted to date/time");
        }

        /// <summary>
        /// </summary>
        /// <param name="columnIndex">Column index (1-based)</param>
        /// <returns></returns>
        public static string GetDisplayColumn(int columnIndex)
        {
            var chars = new List<char>();
            if (columnIndex > 0)
            {
                chars.Add('A');
                while (columnIndex > 26)
                {
                    if (chars.Count == 1)
                    {
                        chars.Add('A');
                    }
                    else if (chars[chars.Count - 2] < 'Z')
                    {
                        chars[chars.Count - 2]++;
                    }
                    else if (chars.Count == 2)
                    {
                        chars[0] = 'A';
                        chars.Add('A');
                    }
                    else if (chars[chars.Count - 3] < 'Z')
                    {
                        chars[chars.Count - 3]++;
                        chars[1] = 'A';
                    }
                    else
                    {
                        throw new UtilityException("This column would exceed 3 characters");
                    }
                    columnIndex -= 26;
                }
                chars[chars.Count - 1] = (char)(chars[chars.Count - 1] + columnIndex - 1);
            }
            else
            {
                chars.Add('0');
            }
            return new string(chars.ToArray());
        }

        /// <summary>
        /// </summary>
        /// <param name="columnIndex">Column index (1-based)</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns></returns>
        public static string GetDisplayCellAddress(int columnIndex, int rowIndex)
        {
            string displayColumn = GetDisplayColumn(columnIndex);
            return displayColumn + rowIndex;
        }

        public static object ProcessDatum(object value, Column column, string dataReference, DataErrorHandlingPolicy dataErrorHandling)
        {
            DataImportException diex = null;
            if (column?.SourceParser != null)
            {
                try
                {
                    value = column.SourceParser.Invoke(value?.ToString());
                }
                catch (Exception ex)
                {
                    diex = new InvalidDatumException(ex, value, columnName: column?.Name, width: column?.Width ?? 0, position: dataReference);
                }
            }
            if (value is string stringValue && column?.Width > 0 && stringValue.Length > column.Width)
            {
                diex = new InvalidDatumException("Value length (" + stringValue.Length + ") exceeds max length (" + column.Width + ")", stringValue, columnName: column?.Name, width: column?.Width ?? 0, position: dataReference);
            }
            if (diex != null)
            {
                switch (dataErrorHandling)
                {
                    case DataErrorHandlingPolicy.Throw:
                    default:
                        throw diex;
                    case DataErrorHandlingPolicy.Embed:
                        value = diex;
                        break;
                    case DataErrorHandlingPolicy.Ignore:
                        value = TypeUtil.GetDefaultValue(column.DataType);
                        break;
                }
            }
            return value;
        }
    }
}
