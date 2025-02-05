using System.Collections.Generic;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Base class for all DB platform specific connection info
    /// </summary>
    public abstract class ConnectionInfo
    {
        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Connection string name, to lookup connection string from config 'connection strings' section
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// This lets the system know to decrypt the password in the connection string
        /// </summary>
        public bool IsEncryptedPassword { get; set; }

        /// <summary>
        /// The server or alias (data source) for building a connection string
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Connection credentials for building a connection string
        /// </summary>
        public Credential? Credentials { get; set; }

        /// <summary>
        /// Gets or sets optional cryptographic properties used to decrypt database passwords
        /// </summary>
        public CryptoOptions CryptoOptions { get; set; }

        /// <summary>
        /// If <c>true</c>, the <c>Credentials</c> user name and password will be copied (overwritten) into the final generated connection string, default is provider specific.
        /// </summary>
        public virtual bool MergeCredentialsIntoFinalConnectionString { get; set; } = DbSettings.DefaultMergeCredentialsIntoFinalConnectionString;

        /// <summary>
        /// Additional connection attributes for building a connection string
        /// </summary>
        public IDictionary<string, string> AdditionalConnectionAttributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The time to wait while trying to establish a connection before terminating the attempt and generating an error.
        /// </summary>
        public int? ConnectionTimeout { get; set; }

        /// <summary>
        /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
        /// </summary>
        public virtual DbProvider? Provider { get; set; }

        /// <summary>
        /// Adds or replaces a connection string attribute
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        public void AddOrReplaceConnectionAttribute(string key, string value)
        {
            DictionaryUtil.AddOrReplace(AdditionalConnectionAttributes, key, value);
        }

        /// <summary>
        /// Adds or replaces multiple connection string attributes
        /// </summary>
        /// <param name="attrs">An <c>IDictionary</c>.</param>
        public void AddOrReplaceConnectionAttributes(IDictionary<string, string> attrs)
        {
            foreach (var key in attrs.Keys)
            {
                DictionaryUtil.AddOrReplace(AdditionalConnectionAttributes, key, attrs[key]);
            }
        }

        /// <summary>
        /// Builds a provider-specific connection string, subclasses must override and take into account
        /// the value of <see cref="MergeCredentialsIntoFinalConnectionString" />
        /// </summary>
        /// <returns>A connection string build from this <c>ConnectionInfo</c> instance.</returns>
        public abstract string BuildFinalConnectionString();
    }
}
