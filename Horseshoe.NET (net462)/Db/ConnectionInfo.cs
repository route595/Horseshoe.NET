using System.Collections.Generic;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.Db
{
    public abstract class ConnectionInfo
    {
        public string Source { get; internal set; }

        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }

        public bool IsEncryptedPassword { get; set; }

        public string DataSource { get; set; }

        public Credential? Credentials { get; set; }

        public IDictionary<string, string> AdditionalConnectionAttributes { get; set; } = new Dictionary<string, string>();

        public virtual DbPlatform? Platform { get; set; }

        public void AddOrReplaceConnectionAttribute(string key, string value)
        {
            AdditionalConnectionAttributes.AddOrReplace(key, value);
        }

        public void AddOrReplaceConnectionAttributes(IDictionary<string, string> attrs)
        {
            foreach (var key in attrs.Keys)
            {
                AdditionalConnectionAttributes.AddOrReplace(key, attrs[key]);
            }
        }

        public abstract string BuildConnectionString(CryptoOptions cryptoOptions = null);

        public ConnectionInfo Clone() => (ConnectionInfo)MemberwiseClone();
    }
}
