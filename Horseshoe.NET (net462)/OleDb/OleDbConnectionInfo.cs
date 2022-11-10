using System;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Db;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.OleDb
{
    public class OleDbConnectionInfo : ConnectionInfo
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

        public OleDbConnectionInfo() { }

        public OleDbConnectionInfo(ConnectionInfo connectionInfo)
        {
            ObjectUtil.MapProperties(connectionInfo, this);
        }

        public override string BuildConnectionString(CryptoOptions cryptoOptions = null)
        {
            return OleDbUtil.BuildConnectionString(DataSource, Credentials, AdditionalConnectionAttributes, cryptoOptions);
        }
    }
}
