using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.OracleDb.Meta
{
    public class OraServer : OraObjectBase//, IEquatable<OraServer>
    {
        public string DataSource { get; }

        public string ServiceName { get; }

        public string InstanceName { get; }

        public OraServer(string dataSource, int port = 0, string serviceName = null, string instanceName = null, string name = null) : base(name ?? dataSource, OraObjectType.Server)
        {
            DataSource = (Zap.String(dataSource) ?? throw new UtilityException("Data source cannot be null or blank")) + (port > 0 ? ":" + port : "");
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

        //public bool Equals(OraServer other)
        //{
        //    return this == other;  // see OraObjectBase
        //}

        public static IEnumerable<OraServer> ParseList(string rawList)
        {
            try
            {
                if (string.IsNullOrEmpty(rawList))
                    return null;
                string[] rawServers = Zap.Strings(rawList.Split('|'), prunePolicy: PrunePolicy.All);
                var servers = rawServers
                    .Select(raw => Parse(raw))
                    .ToList();
                return servers;
            }
            catch (UtilityException ex)
            {
                throw new UtilityException("Malformed server list.  Must resemble: ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1", ex);
            }
        }

        private static readonly Regex ServerNamePattern = new Regex("(?<=')[^']+(?='.+)");

        public static OraServer Parse(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                throw new ValidationException("Invalid raw server string");
            string name = null;
            var nameMatch = ServerNamePattern.Match(raw);

            if (nameMatch.Success)
            {
                name = nameMatch.Value;
                raw = raw.Replace("'" + name + "'", "");
            }

            string[] parts = Zap.Strings(raw.Split(';'), prunePolicy: PrunePolicy.Trailing);

            if (parts.Length > 0 && parts.Length <= 3)
            {
                string dataSource = parts[0]?.Trim() ?? throw new ValidationException("datasource not parsed");
                string serviceName = null;
                string instanceName = null;

                if (parts.Length == 1 && name == null)
                {
                    var serverFromList = OraServer.Lookup(dataSource, suppressErrors: true);
                    if (serverFromList != null)
                        return serverFromList;
                }

                if (parts.Length > 1)
                {
                    serviceName = Zap.String(parts[1]);
                    if (parts.Length > 2)
                    {
                        instanceName = Zap.String(parts[2]);
                    }
                }
                return new OraServer(dataSource, serviceName: serviceName, instanceName: instanceName, name: name ?? dataSource);
            }
            throw new UtilityException("Malformed server.  Must resemble { ORADBSVR01 or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1 }.");
        }

        //public static string BuildList(IEnumerable<OraServer> list)
        //{
        //    if (list == null || !list.Any())
        //        return null;
        //    var listString = string.Join("|", list.Select(svr => svr.DataSource + (svr.ServiceName != null ? ";" + svr.ServiceName : "") + (svr.InstanceName != null ? (svr.ServiceName == null ? ";" : "") + ";" + svr.InstanceName : "")));
        //    return listString;
        //}

        public static implicit operator OraServer(string raw) => Parse(raw);
    }
}
