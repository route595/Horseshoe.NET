using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.SqlDb.Meta;

namespace Horseshoe.NET.SqlDb
{
    internal class StoredProcedure
    {
        internal string Name { get; set; }
        internal IEnumerable<SqlParameter> Parameters { get; set; }

        /// <seealso cref="https://docs.microsoft.com/en-us/sql/relational-databases/databases/database-identifiers" />
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference" />
        private static Regex ProcedureNamePattern { get; } = new Regex(@"^((?<sbrac>\[)?[A-Z_][A-Z0-9_]{0,127}(?(sbrac)\]|)\.)?(?<pbrac>\[)?[A-Z_][A-Z0-9_]{0,127}(?(pbrac)\]|)$", RegexOptions.IgnoreCase);

        internal static void Validate(string procedureName, IEnumerable<SqlParameter> parameters = null, SqlDbConnectionInfo connectionInfo = null)
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo))
            {
                conn.Open();
                _Validate(conn, procedureName, parameters);
            }
        }

        internal static void Validate(SqlConnection conn, string procedureName, IEnumerable<SqlParameter> parameters = null)
        {
            if (!ProcedureNamePattern.IsMatch(procedureName))
            {
                throw new Exception("Invalid procedure name");
            }
            _Validate(conn, procedureName, parameters);
        }

        private static void _Validate(SqlConnection conn, string procedureName, IEnumerable<SqlParameter> parameters = null)
        {
            var statement =
            @"
                SELECT SCHEMA_NAME([schema_id]) AS [schema_name], [name] 
                FROM [sys].[all_objects] 
                WHERE 
                    [type] = 'P' AND
	                '" + procedureName.Replace("[", "").Replace("]", "") + @"' IN ([name], SCHEMA_NAME([schema_id]) + '.' + [name])
            ".Trim();
            var exists = false;
            var reader = Query.SQL.AsDataReader(conn, statement, keepOpen: true);
            if (reader.Read())
            {
                exists = true;
            }
            reader.Close();
            if (!exists)
            {
                throw new Exception("Non-existent procedure");
            }
            var procedure = new StoredProcedure { Name = procedureName, Parameters = GetParameters(procedureName) };
            if (parameters?.Any() ?? false)
            {
                foreach (var xName in parameters.Select(p => p.ParameterName))
                {
                    if (!procedure.Parameters.Any(p => string.Equals(p.ParameterName, xName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        throw new Exception("This stored procedure does not accept a parameter named " + xName);
                    }
                }
                foreach (var name in procedure.Parameters.Select(p => p.ParameterName))
                {
                    if (!parameters.Any(p => string.Equals(p.ParameterName, name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        throw new Exception("This stored procedure expects a parameter named " + name + " which was not supplied");
                    }
                }
            }
        }

        internal static IEnumerable<SqlParameter> GetParameters(string procedureName, SqlDbConnectionInfo connectionInfo = null)
        {
            using (var conn = SqlDbUtil.LaunchConnection(connectionInfo))
            {
                conn.Open();
                return GetParameters(conn, procedureName);
            }
        }

        internal static IEnumerable<SqlParameter> GetParameters(SqlConnection conn, string procedureName)
        {
            var list = new List<SqlParameter>();
            var statement =
            @"
                SELECT 
	                objs.[name] AS [procedure_name], 
	                cols.[name], 
	                cols.prec, 
	                cols.scale, 
	                cols.isnullable, 
	                cols.isoutparam, 
	                cols.[length], 
	                cols.xtype 
                FROM sys.sysobjects objs 
                INNER JOIN sys.syscolumns cols ON objs.id = cols.id 
                WHERE objs.[type] = 'P' and objs.[name] = '" + procedureName + @"'
            ";
            var reader = Query.SQL.AsDataReader(conn, statement.Trim());
            while (reader.Read())
            {
                list.Add(
                    new SqlParameter
                    {
                        ParameterName = (string)reader["name"],
                        SqlDbType = SqlServerTypes.LookupColumnType((byte)reader["xtype"]),
                        Size = (short)reader["length"],
                        Precision = (byte)((short)reader["prec"]),
                        Scale = reader["scale"] is DBNull ? default : (byte)((int)reader["scale"]),
                        Direction = (int)reader["isoutparam"] == 1 ? ParameterDirection.Output : ParameterDirection.Input,
                        IsNullable = (int)reader["isnullable"] == 1
                    }
                );
            }
            reader.Close();
            return list;
        }
    }
}
