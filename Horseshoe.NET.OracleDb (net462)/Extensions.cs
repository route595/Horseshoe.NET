using System.Data.Common;

using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public static class Extensions
    {
        public static OracleParameter ToOracleParameter(this DbParameter parameter)
        {
            if (parameter is OracleParameter oracleParameter)
                return oracleParameter;
            if (parameter.ParameterName == null)
            {
                oracleParameter = new OracleParameter
                {
                    Value = parameter.Value
                };
            }
            else
            {
                oracleParameter = new OracleParameter(parameter.ParameterName, parameter.Value);
            }
            oracleParameter.DbType = parameter.DbType;
            oracleParameter.Direction = parameter.Direction;
            oracleParameter.IsNullable = parameter.IsNullable;
            oracleParameter.Precision = parameter.Precision;
            oracleParameter.Scale = parameter.Scale;
            oracleParameter.Size = parameter.Size;
            oracleParameter.SourceColumn = parameter.SourceColumn;
            oracleParameter.SourceColumnNullMapping = parameter.SourceColumnNullMapping;
            oracleParameter.SourceVersion = parameter.SourceVersion;
            //oracleParameter.ArrayBindSize
            //oracleParameter.ArrayBindStatus
            //oracleParameter.CollectionType
            //oracleParameter.Offset
            //oracleParameter.OracleDbType
            //oracleParameter.OracleDbTypeEx
            //oracleParameter.SkipConversionToLocalTime
            //oracleParameter.Status
            //oracleParameter.UdtTypeName
            return oracleParameter;
        }
    }
}
