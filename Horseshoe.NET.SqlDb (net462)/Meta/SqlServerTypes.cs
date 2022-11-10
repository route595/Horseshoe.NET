using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.SqlDb.Meta
{
    internal static class SqlServerTypes
    {
        private static IDictionary<string, SqlObjectType> _object_types = new Dictionary<string, SqlObjectType>
        {
            { "U", SqlObjectType.Table },
            { "V", SqlObjectType.View },
            { "P", SqlObjectType.StoredProcedure },
            { "FN", SqlObjectType.Function },
            { "TF", SqlObjectType.TableValuedFunction },
            { "IF", SqlObjectType.InlineTableValuedFunction }
        };

        private static IDictionary<int, SqlDbType> _column_types = new Dictionary<int, SqlDbType>
        {
            { 127, SqlDbType.BigInt },
            { 173, SqlDbType.Binary },
            { 104, SqlDbType.Bit },
            { 175, SqlDbType.Char },
            { 61, SqlDbType.DateTime },
            { 106, SqlDbType.Decimal },
            { 62, SqlDbType.Float },
            { 34, SqlDbType.Image },
            { 56, SqlDbType.Int },
            { 60, SqlDbType.Money },
            { 239, SqlDbType.NChar },
            { 99, SqlDbType.NText },
            { 231, SqlDbType.NVarChar },
            { 256, SqlDbType.NVarChar },  // a.k.a. sysname
            { 59, SqlDbType.Real },
            { 58, SqlDbType.SmallDateTime },
            { 52, SqlDbType.SmallInt },
            { 122, SqlDbType.SmallMoney },
            { 35, SqlDbType.Text },
            { 189, SqlDbType.Timestamp },
            { 48, SqlDbType.TinyInt },
            { 36, SqlDbType.UniqueIdentifier },
            { 165, SqlDbType.VarBinary },
            { 167, SqlDbType.VarChar }
        };

        internal static SqlObjectType LookupObjectType(string type)
        {
            if (type != null)
            {
                type = type.ToUpper();
                if (_object_types.ContainsKey(type))
                {
                    return _object_types[type];
                }
            }
            throw new UtilityException("Unknown SQL Server object type: " + type);
        }

        internal static string ResolveType(SqlObjectType objectType)
        {
            if (_object_types.Any(kvp => kvp.Value == objectType))
            {
                return _object_types.Single(kvp => kvp.Value == objectType).Key;
            }
            throw new UtilityException("Unmapped SQL Server object type: " + objectType);
        }

        internal static SqlDbType LookupColumnType(int type)
        {
            if (_column_types.ContainsKey(type))
            {
                return _column_types[type];
            }
            throw new UtilityException("Unknown type: " + type);
        }

        internal static int ResolveType(SqlDbType columnType)
        {
            if (_column_types.Any(kvp => kvp.Value == columnType))
            {
                if (columnType == SqlDbType.NVarChar) return 231;  // duplicate of sysname
                return _column_types.Single(kvp => kvp.Value == columnType).Key;
            }
            throw new UtilityException("Unmapped SQL Db type (aka column type): " + columnType);
        }

        public static bool IsText(SqlDbType columnType)
        {
            return new[]
            {
                SqlDbType.Char,
                SqlDbType.VarChar,
                SqlDbType.NChar,
                SqlDbType.NVarChar,
                SqlDbType.Text,
                SqlDbType.NText
            }.Contains(columnType);
        }

        public static bool IsNumeric(SqlDbType columnType)
        {
            return new[]
            {
                SqlDbType.Bit,
                SqlDbType.TinyInt,
                SqlDbType.SmallInt,
                SqlDbType.Int,
                SqlDbType.BigInt,
                SqlDbType.Decimal,
                SqlDbType.Float,
                SqlDbType.Real,
                SqlDbType.SmallMoney,
                SqlDbType.Money,
            }.Contains(columnType);
        }

        public static bool IsDate(SqlDbType columnType)
        {
            return new[]
            {
                SqlDbType.Date,
                SqlDbType.DateTime,
                SqlDbType.DateTime2,
                SqlDbType.SmallDateTime,
                SqlDbType.Time
            }.Contains(columnType);
        }
    }
}
