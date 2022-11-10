using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Odbc
{
    public class OdbcConnectionInfo : ConnectionInfo
    {
        /* Other members, see base class...
         * 
         * public string ConnectionString { get; set; }
         * public string ConnectionStringName { get; set; }
         * public bool IsEncryptedPassword { get; set; }
         * public string DataSource { get; set; }
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionAttributes  { get; set; }
         * public DbPlatform? Platform  { get; set; }
         */

        public OdbcConnectionInfo() { }

        public OdbcConnectionInfo(ConnectionInfo connectionInfo)
        {
            ObjectUtil.MapProperties(connectionInfo, this);
        }

        public override string BuildConnectionString(CryptoOptions cryptoOptions = null)
        {
            return OdbcUtil.BuildConnectionString(DataSource, Credentials, AdditionalConnectionAttributes, cryptoOptions);
        }
    }
}
