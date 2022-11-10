using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;
using Horseshoe.NET.SqlDb.Meta;

namespace Horseshoe.NET.SqlDb
{
    public class SqlDbConnectionInfo : ConnectionInfo
    {
        /* Defined in base class...
         * 
         * public string ConnectionString { get; set; }
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionAttributes  { get; set; }
         * public DbPlatform? Platform  { get; set; }  // overridden here
         */

        private DbServer _server;

        public DbServer Server
        {
            get { return _server; }
            set
            {
                _server = value;
                DataSource = _server?.DataSource;
            }
        }

        public string InitialCatalog { get; set; }

        private SqlCredentialInterface _sqlCredentials;
        public SqlCredentialInterface SqlCredentials
        {
            get => _sqlCredentials ?? Credentials;
            set => _sqlCredentials = value;
        }

        public override DbPlatform? Platform => DbPlatform.SqlServer;

        public SqlDbConnectionInfo() { }

        public SqlDbConnectionInfo(ConnectionInfo connectionInfo)
        {
            ObjectUtil.MapProperties(connectionInfo, this);
        }

        public SqlDbConnectionInfo(DbServer server, SqlDbConnectionInfo connectionInfo) : this(connectionInfo)
        {
            Server = server;
        }

        public override string BuildConnectionString(CryptoOptions cryptoOptions = null)
        {
            return SqlDbUtil.BuildConnectionString(DataSource, InitialCatalog, Credentials != null, AdditionalConnectionAttributes);
        }
    }
}
