using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb.Meta;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Horseshoe.NET.OracleDb
{
    public static class OracleDbUtil
    {
        //private static Regex dbServerPattern = new Regex("^[a-z0-9-_]+(\\.[a-z0-9-_]+)*$", RegexOptions.IgnoreCase);
        //private static Regex dbServerPortPattern = new Regex("^[a-z0-9-_]+(\\.[a-z0-9-_]+)*[:][0-9]+$", RegexOptions.IgnoreCase);

        public static string BuildConnectionString(OraServer dataSource = null, bool hasCredentials = false, IDictionary<string, string> additionalConnectionAttributes = null, int? connectionTimeout = null)
        {
            if (dataSource == null)
            {
                return null;
            }

            // data source
            var sb = new StringBuilder("Data Source=");  // e.g. Server.DataSource (includes port)
            if (dataSource.ServiceName == null && dataSource.InstanceName == null)
            {
                // basic style                                    // e.g. Data Source=MY_ORA_SERVER[:1234]
                sb.Append(dataSource.DataSource);                 //      Data Source=MY_TNS_ALIAS
            }
            else 
            {
                // data source continued - EZ connect style       // e.g. Data Source=//MY_ORA_SERVER[:1234]//MY_INSTANCE
                sb.Append("//").Append(dataSource.DataSource);    //      Data Source=//MY_ORA_SERVER[:1234]/MY_SERVICE/MY_INSTANCE

                // service name
                sb.Append('/');
                sb.AppendIf(dataSource.ServiceName != null, dataSource.ServiceName);

                // instance name
                if (dataSource.InstanceName != null)
                {
                    sb.Append('/').Append(dataSource.InstanceName);
                }
            }

            // integrated security
            if (!hasCredentials)
            {
                sb.Append(";Integrated Security=SSPI");
            }

            // additional attributes
            if (additionalConnectionAttributes != null)
            {
                foreach (var kvp in additionalConnectionAttributes)
                {
                    sb.Append(";" + kvp.Key + "=" + kvp.Value);
                }
            }

            // additional attributes
            // ref https://learn.microsoft.com/en-us/dotnet/api/system.data.oracleclient.oracleconnection.connectiontimeout?view=netframework-4.8
            if (connectionTimeout.HasValue)
            {
                sb.Append(";Connection Timeout=" + connectionTimeout);
            }

            return sb.ToString();
        }

        public static string BuildConnectionStringFromConfig()
        {
            return BuildConnectionString(OracleDbSettings.DefaultDataSource, OracleDbSettings.DefaultCredentials.HasValue, additionalConnectionAttributes: OracleDbSettings.DefaultAdditionalConnectionAttributes, connectionTimeout: OracleDbSettings.DefaultConnectionTimeout);
        }

        public static OracleConnection LaunchConnection
        (
            OracleDbConnectionInfo connectionInfo = null,
            TraceJournal journal = null
        )
        {
            connectionInfo = DbUtil.LoadConnectionInfo(connectionInfo, () => BuildConnectionStringFromConfig(), journal);
            var conn = connectionInfo.OracleCredentials != null
                ? new OracleConnection(connectionInfo.ConnectionString, connectionInfo.OracleCredentials)
                : new OracleConnection(connectionInfo.ConnectionString);
            conn.Open();
            if (connectionInfo.AutoClearConnectionPool || OracleDbSettings.AutoClearConnectionPool)
            {
                // ref: https://stackoverflow.com/questions/54373754/oracle-managed-dataaccess-connection-object-is-keeping-the-connection-open
                OracleConnection.ClearPool(conn);   // will clear on close
            }
            return conn;
        }

        public static OracleConnection LaunchConnection
        (
            OracleConnection conn,
            bool autoClearConnectionPool = false
        )
        {
            conn = conn.Credential != null
                ? new OracleConnection(conn.ConnectionString, conn.Credential)
                : new OracleConnection(conn.ConnectionString);
            conn.Open();
            if (autoClearConnectionPool || OracleDbSettings.AutoClearConnectionPool)
            {
                // ref: https://stackoverflow.com/questions/54373754/oracle-managed-dataaccess-connection-object-is-keeping-the-connection-open
                OracleConnection.ClearPool(conn);   // will clear on close
            }
            return conn;
        }

        internal static OracleCommand BuildTextCommand
        (
            OracleConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OracleCommand> alterCommand
        )
        {
            return BuildCommand
            (
                conn,
                CommandType.Text,
                commandText,
                parameters,
                commandTimeout,
                alterCommand
            );
        }

        internal static OracleCommand BuildProcedureCommand
        (
            OracleConnection conn,
            string commandText,
            IEnumerable<DbParameter> parameters,
            int? commandTimeout,
            Action<OracleCommand> alterCommand
        )
        {
            return BuildCommand
            (
                conn,
                CommandType.StoredProcedure,
                commandText,
                parameters,
                commandTimeout,
                alterCommand
            );
        }

        public static OracleCommand BuildCommand
        (
            OracleConnection conn,
            CommandType commandType,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int? commandTimeout = null,
            Action<OracleCommand> modifyCommand = null
        )
        {
            var cmd = new OracleCommand
            {
                Connection = conn,
                CommandType = commandType,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param is OracleParameter oraParam)
                    {
                        cmd.Parameters.Add(oraParam);
                    }
                    else
                    {
                        cmd.Parameters.Add(param.ToOracleParameter());
                    }
                }
            }
            if (commandTimeout.TryHasValue(out int value))
            {
                cmd.CommandTimeout = value;
            }
            modifyCommand?.Invoke(cmd);
            return cmd;
        }

        /* * * * * * * * * * * * * * * * * * * * 
         *  ORACLE SPECIFIC METHODS / OBJECTS  *
         * * * * * * * * * * * * * * * * * * * */

        public static OracleParameter BuildInParam(string name = null, object value = null)
        {
            return new OracleParameter(name, value ?? DBNull.Value) { Direction = ParameterDirection.Input };
        }

        public static OracleParameter BuildVarchar2InParam(string name = null, string value = null)
        {
            return new OracleParameter(name, OracleDbType.Varchar2, value as object ?? DBNull.Value, ParameterDirection.Input);
        }

        public static OracleParameter BuildVarchar2OutParam(string name = null, int length = 100)
        {
            return new OracleParameter(name, OracleDbType.Varchar2, length) { Direction = ParameterDirection.Output };
        }

        public static OracleParameter BuildIntInParam(string name = null, int? value = null)
        {
            return new OracleParameter(name, OracleDbType.Int32, value as object ?? DBNull.Value, ParameterDirection.Input);
        }

        public static OracleParameter BuildIntOutParam(string name = null)
        {
            return new OracleParameter(name, OracleDbType.Int32, ParameterDirection.Output);
        }

        public static OracleParameter BuildDateInParam(string name = null, DateTime? value = null)
        {
            return new OracleParameter(name, OracleDbType.Date, value as object ?? DBNull.Value, ParameterDirection.Input);
        }

        public static OracleParameter BuildDateOutParam(string name = null)
        {
            return new OracleParameter(name, OracleDbType.Date, ParameterDirection.Output);
        }

        public static OracleParameter BuildOutRefCursor(string name = null)
        {
            return new OracleParameter(name, OracleDbType.RefCursor, ParameterDirection.Output);
        }

        public static object NormalizeDbValue(object obj)
        {
            if (obj == null || obj is DBNull)
                return null;
            // string
            if (obj is OracleString oracleString)
                return oracleString.Value;
            if (obj is OracleXmlType oracleXml)
                return oracleXml.Value;
            if (obj is OracleXmlStream oracleXmlStream)
                return oracleXmlStream.Value;
            if (obj is OracleClob oracleClob)
                return oracleClob.Value;
            // date/time/timespan
            if (obj is OracleDate oracleDate)
                return oracleDate.Value;
            if (obj is OracleTimeStamp oracleTimeStamp)
                return oracleTimeStamp.Value;
            if (obj is OracleTimeStampLTZ oracleTimeStampLTZ)
                return oracleTimeStampLTZ.Value;
            if (obj is OracleTimeStampTZ oracleTimeStampTZ)
                return oracleTimeStampTZ.Value;
            if (obj is OracleIntervalDS oracleIntervalDS)
                return oracleIntervalDS.Value;
            // bool
            if (obj is OracleBoolean oracleBool)
                return oracleBool.Value;
            // numeric
            if (obj is OracleDecimal oracleDecimal)
                return oracleDecimal.Value;
            if (obj is OracleIntervalYM oracleIntervalYM)
                return oracleIntervalYM.Value;
            // bytes
            if (obj is OracleBinary oracleBinary)
                return oracleBinary.Value;
            if (obj is OracleBlob oracleBlob)
                return oracleBlob.Value;
            if (obj is OracleBFile oracleBFile)
                return oracleBFile.Value;
            return obj;
        }
    }
}
