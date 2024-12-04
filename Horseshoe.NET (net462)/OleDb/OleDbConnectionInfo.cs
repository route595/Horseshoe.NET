using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.OleDb
{
    /// <summary>
    /// A connection info specially suited to OLE DB
    /// </summary>
    public class OleDbConnectionInfo : ConnectionInfo
    {
        /// <summary>
        /// Creates new <c>OleDbConnectionInfo</c>
        /// </summary>
        public OleDbConnectionInfo() { }

        /// <summary>
        /// Creates new <c>OleDbConnectionInfo</c> from another
        /// </summary>
        /// <param name="connectionInfo">Another <c>OleDbConnectionInfo</c></param>
        public OleDbConnectionInfo(ConnectionInfo connectionInfo)
        {
            connectionInfo.MapProperties(this);
        }

        /// <summary>
        /// Builds a connection string for connecting to an OLE DB data source
        /// </summary>
        /// <returns>A connection string.</returns>
        public override string BuildFinalConnectionString()
        {

            return OleDbUtil.BuildConnectionString(DataSource, credentials: Credentials, additionalConnectionAttributes: AdditionalConnectionAttributes, connectionTimeout: ConnectionTimeout);
        }

        /// <summary>
        /// Implicitly converts connection strings to <c>OleDbConnectionInfo</c>
        /// </summary>
        /// <param name="connectionString">A connection string</param>
        public static implicit operator OleDbConnectionInfo(string connectionString) => new OleDbConnectionInfo { ConnectionString = connectionString };

    }
}
