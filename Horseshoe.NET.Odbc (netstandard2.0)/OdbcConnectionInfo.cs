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
    }
}
