using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Use this to capture metadata and messages from data calls such as column definitions and Oracle out parameters.
    /// </summary>
    public class DbCapture
    {
        /// <summary>
        /// Column metadata captured from the data call
        /// </summary>
        public DataColumn[] DataColumns { get; set; } = Array.Empty<DataColumn>();

        /// <summary>
        /// Output parameters captured from procedure calls (currently, Oracle only)
        /// </summary>
        public DbParameter[] OutputParameters { get; set; } = Array.Empty<DbParameter>();

        /// <summary>
        /// Gets out parameter value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public virtual object Get(string name, bool suppressErrors = false)
        {
            try
            {
                return DbUtil.NormalizeDbValue
                (
                    OutputParameters
                        .Single(p => string.Equals(name, p.ParameterName, StringComparison.OrdinalIgnoreCase))?
                        .Value
                );
            }
            catch
            {
                if (suppressErrors)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Gets out parameter <c>string</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public string GetString(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is string stringValue)
                return stringValue;
            return obj.ToString();
        }

        /// <summary>
        /// Gets out parameter <c>int?</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public int? GetNInt(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is int intValue)
                return intValue;
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// Gets out parameter <c>int</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public int GetInt(string name, bool suppressErrors = false)
        {
            return GetNInt(name, suppressErrors: suppressErrors) ?? default;
        }

        /// <summary>
        /// Gets out parameter <c>decimal?</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public decimal? GetNDecimal(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is decimal decimalValue)
                return decimalValue;
            return Convert.ToDecimal(obj);
        }

        /// <summary>
        /// Gets out parameter <c>decimal</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public decimal GetDecimal(string name, bool suppressErrors = false)
        {
            return GetNDecimal(name, suppressErrors: suppressErrors) ?? default;
        }

        /// <summary>
        /// Gets out parameter <c>double?</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public double? GetNDouble(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is double doubleValue)
                return doubleValue;
            return Convert.ToDouble(obj);
        }

        /// <summary>
        /// Gets out parameter <c>double</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public double GetDouble(string name, bool suppressErrors = false)
        {
            return GetNDouble(name, suppressErrors: suppressErrors) ?? default;
        }

        /// <summary>
        /// Gets out parameter <c>DateTime?</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public DateTime? GetNDateTime(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is DateTime dateTime)
                return dateTime;
            return Convert.ToDateTime(obj);
        }

        /// <summary>
        /// Gets out parameter <c>DateTime</c> value by name (currently, Oracle only)
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="suppressErrors">returns <c>null</c> if <c>true</c> and param name not found exactly once, default is <c>false</c></param>
        /// <returns></returns>
        public DateTime GetDateTime(string name, bool suppressErrors = false)
        {
            return GetNDateTime(name, suppressErrors: suppressErrors) ?? default;
        }
    }
}
