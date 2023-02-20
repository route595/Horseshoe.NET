using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.SqlDb.Meta
{
    public class DbServer : DbObjectBase, IEquatable<DbServer>
    {
        public string DataSource { get; }

        public DbVersion Version { get; }

        public DbServer(string dataSource, int port = 0, DbVersion version = null, string name = null) : base(name ?? dataSource, SqlObjectType.Server)
        {
            DataSource = (Zap.String(dataSource) ?? throw new UtilityException("Data source cannot be null or blank")) + (port > 0 ? ":" + port : ""); ;
            Version = version;
        }

        public bool Equals(DbServer other)
        {
            return this == other;  // see DbObjectBase
        }

        public static IEnumerable<DbServer> LookupAll()
        {
            return SqlDbSettings.ServerList;
        }

        public static DbServer Lookup(string dataSource, bool suppressErrors = false)
        {
            var list = LookupAll();
            if (list == null || !list.Any())
            {
                if (suppressErrors) return null;
                throw new UtilityException("Zero SQL Servers have been configured.  Set this list via DataAccess.Sql.Settings > ServerList, config file > [key=...DataAccess.SQL:ServerList] or OrganizationalDefaultSettings > [key=DataAccess.SQL.ServerList]");
            }
            var filteredList = list
                .Where(os => string.Equals(os.DataSource, dataSource, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            switch (filteredList.Count)
            {
                case 0:
                    if (!suppressErrors)
                    {
                        throw new UtilityException("Zero configured SQL Servers match this data source: " + dataSource);
                    }
                    return null;
                case 1:
                    return filteredList.Single();
                default:
                    throw new UtilityException(filteredList.Count + " configured SQL Servers match this data source: " + dataSource);
            }
        }

        public static IEnumerable<DbServer> ParseList(string rawList)
        {
            if (string.IsNullOrEmpty(rawList)) return null;
            var rawServers = Zap.Strings(rawList.Split('|'), prunePolicy: PrunePolicy.All);
            try
            {
                var servers = rawServers
                    .Select(raw => Parse(raw))
                    .ToList();
                return servers;
            }
            catch (UtilityException ex)
            {
                throw new UtilityException("Malformed server list string.  Must resemble { DBSVR01|'NAME'11.22.33.44:9999;2012|DBSVR02;2008R2 }", ex);
            }
        }

        static readonly Regex ServerNamePattern = new Regex("(?<=')[^']*(?='.+)");

        public static DbServer Parse(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            string name = null;
            var nameMatch = ServerNamePattern.Match(raw);
            if (nameMatch.Success)
            {
                name = nameMatch.Value;
                raw = raw.Replace("'" + name + "'", "");
            }

            var parts = Zap.Strings(raw.Split(';'), prunePolicy: PrunePolicy.Trailing);

            if (parts.Length > 0 && parts.Length <= 2)
            {
                string dataSource = parts[0].Trim();
                DbVersion version = null;
                if (parts.Length == 1 && name == null)
                {
                    var serverFromList = DbServer.Lookup(dataSource, suppressErrors: true);
                    if (serverFromList != null) return serverFromList;
                }
                if (parts.Length > 1)
                {
                    version = DbVersion.Lookup(parts[1]);
                }
                return new DbServer(dataSource, version: version, name: name ?? dataSource);
            }
            throw new UtilityException("Malformed server string.  Must resemble { DBSVR01 or 'NAME'11.22.33.44:9999;2012 or DBSVR02;2008R2 }.");
        }

        public static string BuildList(IEnumerable<DbServer> list)
        {
            if (list == null || !list.Any()) return null;
            var listString = string.Join("|", list.Select(s => s.DataSource + (s.Version != null ? ";" + s.Version.Name : "")));
            return listString;
        }

        public static implicit operator DbServer(string raw) => Parse(raw);
    }
}
