using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.OracleDb.Meta;

namespace Horseshoe.NET.OracleDb
{
    public class OracleDbConnectionInfo : ConnectionInfo
    {
        /* Defined in base class...
         * 
         * public string ConnectionString { get; set; }     
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionStringAttributes  { get; set; }
         * public DbPlatform? Platform  { get; set; }  // overridden here
         */

        private OraServer _server;

        public OraServer Server
        {
            get { return _server; }
            set
            {
                _server = value;
                DataSource = _server?.DataSource;
            }
        }

        private OracleCredentialInterface _orclCredentials;
        public OracleCredentialInterface OracleCredentials
        {
            get => _orclCredentials ?? Credentials;
            set => _orclCredentials = value;
        }

        public bool AutoClearConnectionPool { get; set; }

        public override DbPlatform? Platform => DbPlatform.Oracle;

        public OracleDbConnectionInfo() { }

        public OracleDbConnectionInfo(ConnectionInfo connectionInfo)
        {
            ObjectUtil.MapProperties(connectionInfo, this);
        }

        public OracleDbConnectionInfo(OraServer server, OracleDbConnectionInfo connectionInfo) : this(connectionInfo)
        {
            Server = server;
        }

        public override string BuildConnectionString()   // Uses EZ Connect style connection string, not including user id and password
        {
            return OracleDbUtil.BuildConnectionString(DataSource, server: Server, additionalConnectionAttributes: AdditionalConnectionAttributes, connectionTimeout: ConnectionTimeout);
        }
    }
}