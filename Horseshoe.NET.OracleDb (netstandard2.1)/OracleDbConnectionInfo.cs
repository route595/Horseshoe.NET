using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.OracleDb.Meta;

namespace Horseshoe.NET.OracleDb
{
    public class OracleDbConnectionInfo : ConnectionInfo
    {
        /* Defined in base class...
         * 
         * public string ConnectionString { get; set; }     
         * public string ConnectionStringName { get; set; }
         * public string DataSource { get; set; }  --  HIDDEN!
         * public bool IsEncryptedPassword { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionStringAttributes  { get; set; }
         * public DbPlatform? Platform  { get; set; }  // overridden here
         */

        private OraServer _dataSource;
        public new OraServer DataSource 
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                base.DataSource = _dataSource?.DataSource;
            }
        }

        private OracleCredentialInterface _orclCredentials;
        public OracleCredentialInterface OracleCredentials
        {
            get => _orclCredentials ?? Credentials;
            set => _orclCredentials = value;
        }

        public bool AutoClearConnectionPool { get; set; }

        public override DbProvider? Provider => DbProvider.Oracle;

        public OracleDbConnectionInfo() { }

        public OracleDbConnectionInfo(ConnectionInfo connectionInfo)
        {
            ObjectUtil.MapProperties(connectionInfo, this);
        }

        public OracleDbConnectionInfo(OraServer dataSource, OracleDbConnectionInfo connectionInfo) : this(connectionInfo)
        {
            DataSource = dataSource;
        }

        public override string BuildFinalConnectionString()   // Uses EZ Connect style connection string, not including user id and password
        {
            return OracleDbUtil.BuildConnectionString(DataSource, hasCredentials: OracleCredentials != null, additionalConnectionAttributes: AdditionalConnectionAttributes, connectionTimeout: ConnectionTimeout);
        }

        /// <summary>
        /// Implicitly converts connection strings to <c>OracleDbConnectionInfo</c>
        /// </summary>
        /// <param name="connectionString"></param>
        public static implicit operator OracleDbConnectionInfo(string connectionString) => new OracleDbConnectionInfo { ConnectionString = connectionString };
    }
}