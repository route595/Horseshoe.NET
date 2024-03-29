﻿using Horseshoe.NET.Db;
using Horseshoe.NET.ObjectsAndTypes;
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
         * public string DataSource { get; set; }  --  HIDDEN!
         * public Credential? Credentials { get; set; }
         * public IDictionary<string, string> AdditionalConnectionAttributes  { get; set; }
         * public DbPlatform? Platform  { get; set; }  // overridden here
         */

        private DbServer _dataSource;

        public new DbServer DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                base.DataSource = _dataSource?.DataSource;
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

        public SqlDbConnectionInfo(DbServer dataSource, SqlDbConnectionInfo connectionInfo) : this(connectionInfo)
        {
            DataSource = dataSource;
        }

        public override string BuildConnectionString()
        {
            return SqlDbUtil.BuildConnectionString(DataSource, initialCatalog: InitialCatalog, hasCredentials: Credentials != null, additionalConnectionAttributes: AdditionalConnectionAttributes, connectionTimeout: ConnectionTimeout);
        }
    }
}
