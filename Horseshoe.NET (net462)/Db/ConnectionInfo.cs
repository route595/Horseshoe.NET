using System.Collections.Generic;

using Horseshoe.NET.Collections;

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
        /// Additional connection attributes for building a connection string
        /// </summary>
        public IDictionary<string, string> AdditionalConnectionAttributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The time to wait while trying to establish a connection before terminating the attempt and generating an error.
        /// </summary>
        public int? ConnectionTimeout { get; set; }

        /// <summary>
        /// A DB platform may lend hints about how to render column names and parameters.
        /// </summary>
        public virtual DbPlatform? Platform { get; set; }

        /// <summary>
        /// Adds or replaces a connection string attribute
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        public void AddOrReplaceConnectionAttribute(string key, string value)
        {
            AdditionalConnectionAttributes.AddOrReplace(key, value);
        }

        /// <summary>
        /// Adds or replaces multiple connection string attributes
        /// </summary>
        /// <param name="attrs">An <c>IDictionary</c>.</param>
        public void AddOrReplaceConnectionAttributes(IDictionary<string, string> attrs)
        {
            foreach (var key in attrs.Keys)
            {
                AdditionalConnectionAttributes.AddOrReplace(key, attrs[key]);
            }
        }

        /// <summary>
        /// Builds a connection string, platform-specific, must be implemented by all subclasses
        /// </summary>
        /// <returns>A connection string.</returns>
        public abstract string BuildConnectionString();

        /// <summary>
        /// Creates a shallow copy of the current <c>ConnectionInfo</c> instance.
        /// </summary>
        /// <returns>A copy of the current <c>ConnectionInfo</c> instance.</returns>
        public ConnectionInfo Clone() => (ConnectionInfo)MemberwiseClone();
    }
}
