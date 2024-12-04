using System;
using System.Data;
using System.Text.RegularExpressions;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A collection of common, provider agnostic factory methods for Horseshoe.NET DB operations.
    /// </summary>
    public static class DbUtilAbstractions
    {
        private static Regex SimpleColumnNamePattern { get; } = new Regex("^[A-Z0-9_]+$", RegexOptions.IgnoreCase);

        /// <summary>
        /// When generating the SQL for parameters or in other situations it may be 
        /// necessary to 'fix' a column name in C# to be valid in SQL, e.g. adding quotes or square brackets 
        /// around the column name especially if it contains spaces or other non-word characters.
        /// </summary>
        /// <param name="columnName">a column name</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns></returns>
        public static string RenderColumnName(string columnName, DbProvider provider = default)
        {
            if (SimpleColumnNamePattern.IsMatch(columnName))
                return columnName;
            switch (provider)
            {
                case DbProvider.SqlServer:
                    return "[" + columnName + "]";
                case DbProvider.Oracle:
                    return "\"" + columnName + "\"";
                default:
                    return columnName;
            }
        }

        /// <summary>
        /// Prepare an object for insertion into a SQL statement.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns></returns>
        public static string Sqlize(object obj, DbProvider provider = default)
        {
            if (obj == null || obj is DBNull) return "NULL";
            if (obj is bool boolValue) obj = boolValue ? 1 : 0;
            if (obj is byte || obj is short || obj is int || obj is long || obj is float || obj is double || obj is decimal) return obj.ToString();
            if (obj is DateTime dateTimeValue)
            {
                string dateTimeFormat;
                switch (provider)
                {
                    case DbProvider.SqlServer:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";  // format inspired by Microsoft SQL Server Managament Studio (SSMS)
                        break;
                    case DbProvider.Oracle:
                        dateTimeFormat = "MM/dd/yyyy hh:mm:ss tt";   // format inspired by dbForge for Oracle
                        var oracleDateFormat = "mm/dd/yyyy hh:mi:ss am";
                        return "TO_DATE('" + dateTimeValue.ToString(dateTimeFormat) + "', '" + oracleDateFormat + "')";  // example: TO_DATE('02/02/2014 3:35:57 PM', 'mm/dd/yyyy hh:mi:ss am')
                    default:
                        dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                        break;
                }
                return "'" + dateTimeValue.ToString(dateTimeFormat) + "'";
            }
            if (obj is SqlLiteral sqlLiteral)
            {
                return sqlLiteral.Render();
            }
            var returnValue = "'" + obj.ToString().Replace("'", "''") + "'";
            if (provider == DbProvider.SqlServer && !TextUtilAbstractions.IsAsciiPrintable(returnValue))
            {
                return "N" + returnValue;
            }
            return returnValue;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *           MISCELLANEOUS             *
         * * * * * * * * * * * * * * * * * * * */
        internal static object WrangleParameterValue(object value)
        {
            if (value.GetType().IsEnum)
                return value.ToString();
            return value;
        }

        internal static DbType CalculateDbType(object value)
        {
            if (value is string) return DbType.String;
            if (value is byte) return DbType.Byte;
            if (value is short) return DbType.Int16;
            if (value is int) return DbType.Int32;
            if (value is long) return DbType.Int64;
            if (value is decimal) return DbType.Decimal;
            if (value is double) return DbType.Double;
            if (value is bool) return DbType.Boolean;
            if (value is DateTime) return DbType.DateTime;
            if (value is Guid) return DbType.Guid;
            return DbType.Object;
        }
    }
}
