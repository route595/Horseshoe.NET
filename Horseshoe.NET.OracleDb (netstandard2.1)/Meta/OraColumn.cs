using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.OracleDb.Meta
{
    public class OraColumn : OraObjectBase, IEquatable<OraColumn>
    {
        public OraColumnType ColumnType { get; }
        public int Length { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public bool IsNullable { get; set; }
        public int ColumnID { get; set; }
        public string DefaultValue { get; set; }

        public OraColumn(string name, OraColumnType columnType) : base(name, OraObjectType.Column) 
        {
            ColumnType = columnType;
        }

        public string ToDeclareString()
        {
            return ToString() + " " + ToDeclareTypeString();
        }

        public string ToTypeString()
        {
            switch (ColumnType)
            {
                case OraColumnType.CHAR:
                case OraColumnType.NCHAR:
                case OraColumnType.NVARCHAR2:
                case OraColumnType.VARCHAR:
                case OraColumnType.VARCHAR2:
                case OraColumnType.UROWID:
                    return ColumnType + "(" + Length + ")";
                case OraColumnType.DECIMAL:
                case OraColumnType.NUMBER:
                    if (Precision.HasValue)
                    {
                        if (Scale.HasValue)
                        {
                            return ColumnType + "(" + Precision + "," + Scale + ")";
                        }
                        return ColumnType + "(" + Precision + ")";
                    }
                    break;
                case OraColumnType.FLOAT:
                    if (Precision.HasValue)
                    {
                        return ColumnType + "(" + Precision + ")";
                    }
                    break;
            }
            return ColumnType.ToString();
        }

        public string ToDeclareTypeString()
        {
            return ToTypeString() + (IsNullable ? "" : " NOT NULL");
        }

        public bool Equals(OraColumn other)
        {
            return this == other;  // see OraObjectBase
        }
    }
}
