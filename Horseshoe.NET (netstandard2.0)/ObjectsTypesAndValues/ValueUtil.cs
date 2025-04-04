﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for value type (struct) values.
    /// </summary>
    public static class ValueUtil
    {
        /// <summary>
        /// An easy-to-use value sniffing method for nullable structs.  Built for numeric types.
        /// </summary>
        /// <param name="inValue">The nullable value.</param>
        /// <param name="value">The non-nullable value, if any.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool TryHasValue<T>(T? inValue, out T value) where T : struct
        {
            if (inValue.HasValue)
            {
                value = inValue.Value;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Formats a value based on runtime type
        /// </summary>
        /// <param name="value">A value</param>
        /// <returns>A formatted object</returns>
        public static string Display(object value)
        {
            if (value == null)
                return TextConstants.Null;
            var formatter = TypeFormatters.GetFormatter(value.GetType());
            if (formatter != null)
                return formatter.Invoke(value);
            if (value is DBNull)
                return "[db-null]";
            if (value is char @char)
                return "'" + @char + "'";
            if (value is DateTime dateTime)
                value = dateTime.ToShortDateString() +
                (
                    dateTime.HasTime()
                    ? " " + dateTime.ToShortTimeString()
                    : ""
                );
            if (value is Type _type)
                value = _type.FullName;
            if (value is string @string)
            {
                if (@string.Length == 0)
                    return TextConstants.Empty;
                if (@string.Trim().Length == 0)
                    return TextConstants.Whitespace;
                return "\"" + @string.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"") + "\"";
            }
            if (value is IDictionary dict)  // incl. StringValues, string[], List<string>, etc.
            {
                if (dict.Keys.Count == 0)
                    return "[]";

                var list = new List<string>();
                foreach (var key in dict.Keys)
                {
                    list.Add(Display(key) + ": " + Display(dict[key]));
                }
                return "[ " + string.Join(", ", list) + " ]";
            }
            if (value is IEnumerable enumerable)
            {
                var objs = enumerable.Cast<object>();
                if (!objs.Any())
                    return "[]";
                return "[ " + string.Join(", ", objs.Select(o => Display(o))) + " ]";
            }

            var type = value.GetType();
            if (type.IsEnum)
                return type.Name + "." + value;

            return value.ToString();
        }

        /// <summary>
        /// Returns <c>true</c> if any of the supplied values is non-null
        /// </summary>
        /// <param name="values">One or more values</param>
        /// <returns><c>bool</c></returns>
        public static bool Any(params object[] values)
        {
            if (values == null || values.Length == 0)
                return false;
            foreach (var value in values)
                if (value != null)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if all of the supplied values are non-null
        /// </summary>
        /// <param name="values">One or more values</param>
        /// <returns><c>bool</c></returns>
        public static bool All(params object[] values)
        {
            if (values == null || values.Length == 0)
                return false;
            foreach (var value in values)
                if (value == null)
                    return false;
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if none of the supplied values is non-null
        /// </summary>
        /// <param name="values">One or more values</param>
        /// <returns><c>bool</c></returns>
        public static bool None(params object[] values)
        {
            if (values == null || values.Length == 0)
                return false;
            foreach (var value in values)
                if (value != null)
                    return false;
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if all of the supplied values are either null or non-null
        /// </summary>
        /// <param name="values">One or more values</param>
        /// <returns><c>bool</c></returns>
        public static bool AllOrNone(params object[] values)
        {
            if (values == null || values.Length == 0)
                return false;
            bool isNull = (values[0] == null);
            for (int i = 1; i < values.Length; i++)
                if (isNull != (values[i] == null))
                    return false;
            return true;
        }
    }
}
