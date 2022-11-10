using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Horseshoe.NET.SqlDb.Meta
{
    public class DbColumn : DbObjectBase, IEquatable<DbColumn>
    {
        public SqlDbType ColumnType { get; set; }
        public short Length { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public bool IsNullable { get; set; }
        public short MaxLength { get; set; }
        public int ColumnID { get; set; }
        public IEnumerable<object> CachedValues { get; set; }

        public DbColumn(string name, SqlDbType columnType) : base(name, SqlObjectType.Column) 
        {
            ColumnType = columnType;
        }

        public string ToDeclareString()
        {
            return ToString() + " " + ToDeclareTypeString();
        }

        public string ToTypeString()
        {
            var columnType = ColumnType.ToString().ToLower();
            switch (ColumnType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                    return columnType + "(" + MaxLength + ")";
                case SqlDbType.Decimal:
                    if (Scale < 0 || Precision < 0) return columnType + "(invalid)";
                    if (Scale == 0)
                    {
                        if (Precision != 0) return columnType + "(invalid)";
                        return columnType;
                    }
                    if (Precision == 0) return columnType + "(" + Precision + ")";
                    return columnType + "(" + Precision + ", " + Scale + ")";
            }
            return columnType;
        }

        public string ToDeclareTypeString()
        {
            return ToTypeString() + (IsNullable ? "" : " NOT NULL");
        }

        public IEnumerable<E> GetCachedValues<E>()
        {
            if (CachedValues == null) return null;
            return CachedValues
                .Select(o => o == null || o is DBNull ? default : (E)o)
                .ToList();
        }

        public bool Equals(DbColumn other)
        {
            return this == other;  // see DbObjectBase
        }
    }
}
