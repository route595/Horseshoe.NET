using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.OracleDb.Meta
{
    internal static class OracleTypes
    {
        private static IDictionary<string, OraObjectType> _object_types = new Dictionary<string, OraObjectType>
        {
            { "TABLE", OraObjectType.Table },
            { "VIEW", OraObjectType.View },
            { "PROCEDURE", OraObjectType.Procedure },
            { "FUNCTION", OraObjectType.Function },
            { "MATERIALIZED VIEW", OraObjectType.MaterializedView },
            { "PACKAGE", OraObjectType.Package },
            { "PACKAGE BODY", OraObjectType.PackageBody }
        };

        internal static OraObjectType LookupObjectType(string type)
        {
            if (type != null)
            {
                type = type.ToUpper();
                if (_object_types.ContainsKey(type))
                {
                    return _object_types[type];
                }
            }
            throw new UtilityException("Unknown Oracle object type: " + type);
        }

        internal static string ResolveObjectType(OraObjectType objectType)
        {
            if (_object_types.Any(kvp => kvp.Value == objectType))
            {
                return _object_types.Single(kvp => kvp.Value == objectType).Key;
            }
            throw new UtilityException("Unmapped Oracle object type: " + objectType);
        }

        internal static OraColumnType LookupColumnType(string type)
        {
            return Zap.NEnum<OraColumnType>(type, ignoreCase: true, suppressErrors: true) 
                ?? OraColumnType.NON_STANDARD_COLUMN_TYPE;
        }

        public static bool IsText(OraColumnType columnType)
        {
            return new[]
            {
                OraColumnType.CHAR,
                OraColumnType.NCHAR,
                OraColumnType.NVARCHAR2,
                OraColumnType.VARCHAR,
                OraColumnType.VARCHAR2,
                OraColumnType.XMLTYPE
            }.Contains(columnType);
        }

        public static bool IsNumeric(OraColumnType columnType)
        {
            return new[]
            {
                OraColumnType.DECIMAL,
                OraColumnType.FLOAT,
                OraColumnType.INTEGER,
                OraColumnType.LONG,
                OraColumnType.NUMBER,
                OraColumnType.ROWID,
                OraColumnType.UROWID
            }.Contains(columnType);
        }

        public static bool IsDate(OraColumnType columnType)
        {
            return new[]
            {
                OraColumnType.DATE,
                OraColumnType.TIMESTAMP
            }.Contains(columnType);
        }
    }
}
