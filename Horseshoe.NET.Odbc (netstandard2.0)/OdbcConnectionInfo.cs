using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Odbc
{
    public class OdbcConnectionInfo : ConnectionInfo
    {
        public OdbcConnectionInfo() { }

        public OdbcConnectionInfo(ConnectionInfo connectionInfo)
        {
            ObjectUtil.MapProperties(connectionInfo, this);
        }

        public override string BuildFinalConnectionString()
        {
            return OdbcUtil.BuildConnectionString(DataSource, credentials: Credentials, additionalConnectionAttributes: AdditionalConnectionAttributes, connectionTimeout: ConnectionTimeout);
        }

        /// <summary>
        /// Implicitly converts connection strings to <c>OdbcConnectionInfo</c>
        /// </summary>
        /// <param name="connectionString"></param>
        public static implicit operator OdbcConnectionInfo(string connectionString) => new OdbcConnectionInfo { ConnectionString = connectionString };
    }
}
