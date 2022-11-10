using System.Data.Common;
using System.Data.SqlClient;

namespace Horseshoe.NET.SqlDb
{
    public static class Extensions
    {
        public static SqlParameter ToSqlParameter(this DbParameter parameter)
        {
            if (parameter is SqlParameter sqlParameter)
                return sqlParameter;
            if (parameter.ParameterName == null)
            {
                sqlParameter = new SqlParameter
                {
                    Value = parameter.Value
                };
            }
            else
            {
                sqlParameter = new SqlParameter(parameter.ParameterName, parameter.Value);
            }
            sqlParameter.DbType = parameter.DbType;
            sqlParameter.Direction = parameter.Direction;
            sqlParameter.IsNullable = parameter.IsNullable;
            sqlParameter.Precision = parameter.Precision;
            sqlParameter.Scale = parameter.Scale;
            sqlParameter.Size = parameter.Size;
            sqlParameter.SourceColumn = parameter.SourceColumn;
            sqlParameter.SourceColumnNullMapping = parameter.SourceColumnNullMapping;
            sqlParameter.SourceVersion = parameter.SourceVersion;
            //sqlParameter.CompareInfo
            //sqlParameter.LocaleId
            //sqlParameter.Offset
            //sqlParameter.SqlDbType
            //sqlParameter.SqlValue
            //sqlParameter.TypeName
            //sqlParameter.UdtTypeName
            //sqlParameter.XmlSchemaCollectionDatabase
            //sqlParameter.XmlSchemaCollectionName
            //sqlParameter.XmlSchemaCollectionOwningSchema
            return sqlParameter;
        }
    }
}
