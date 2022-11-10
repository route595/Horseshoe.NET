using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Horseshoe.NET.Db
{
    public class DbCapture
    {
        public DataColumn[] DataColumns { get; set; } = Array.Empty<DataColumn>();
        public DbParameter[] OutputParameters { get; set; } = Array.Empty<DbParameter>();

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

        public string GetString(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is string stringValue)
                return stringValue;
            return obj.ToString();
        }

        public int? GetNInt(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is int intValue)
                return intValue;
            return Convert.ToInt32(obj);
        }

        public int GetInt(string name, bool suppressErrors = false)
        {
            return GetNInt(name, suppressErrors: suppressErrors) ?? default;
        }

        public decimal? GetNDecimal(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is decimal decimalValue)
                return decimalValue;
            return Convert.ToDecimal(obj);
        }

        public decimal GetDecimal(string name, bool suppressErrors = false)
        {
            return GetNDecimal(name, suppressErrors: suppressErrors) ?? default;
        }

        public double? GetNDouble(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is double doubleValue)
                return doubleValue;
            return Convert.ToDouble(obj);
        }

        public double GetDouble(string name, bool suppressErrors = false)
        {
            return GetNDouble(name, suppressErrors: suppressErrors) ?? default;
        }

        public DateTime? GetNDateTime(string name, bool suppressErrors = false)
        {
            var obj = Get(name, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            if (obj is DateTime dateTime)
                return dateTime;
            return Convert.ToDateTime(obj);
        }

        public DateTime GetDateTime(string name, bool suppressErrors = false)
        {
            return GetNDateTime(name, suppressErrors: suppressErrors) ?? default;
        }
    }
}
