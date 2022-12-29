using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.OracleDb.Meta
{
    public class OraServer : OraObjectBase, IEquatable<OraServer>
    {
        public string DataSource { get; }

        public int? Port { get; }

        public string ServiceName { get; }

        public string InstanceName { get; }

        public OraServer(string dataSource, int? port = null, string serviceName = null, string instanceName = null, string name = null) : base(name ?? dataSource, OraObjectType.Server)
        {
            DataSource = (Zap.String(dataSource) ?? throw new UtilityException("Data source cannot be null or blank")) + (port.HasValue ? ":" + port : "");
            Port = port;
            ServiceName = serviceName;
            InstanceName = instanceName;
        }

        public static IEnumerable<OraServer> LookupAll()
        {
            return OracleDbSettings.ServerList;
        }

        public static OraServer Lookup(string nameOrDataSource, bool suppressErrors = false)
        {
            var list = LookupAll();
            if (list == null || !list.Any())
            {
                if (suppressErrors) return null;
                throw new UtilityException("No Oracle servers have been configured.  Set the list at DataAccess.OracleDb.Settings > ServerList, config file > [key=...DataAccess.Oracle:ServerList] or OrganizationalDefaultSettings > [key=DataAccess.Oracle.ServerList]");
            }
            list = list
                .Where(os => string.Equals(os.Name, nameOrDataSource, StringComparison.CurrentCultureIgnoreCase) || string.Equals(os.DataSource, nameOrDataSource, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            if (list.Count() == 1)
            {
                return list.Single();
            }
            throw new UtilityException(list.Count() + " Oracle servers match this name / data source: " + nameOrDataSource);
        }

        public bool Equals(OraServer other)
        {
            return this == other;  // see OraObjectBase
        }
    }
}
