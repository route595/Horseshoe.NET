using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb.Meta
{
    public class DbVersion : IEquatable<DbVersion>
    {
        public static DbVersion SQL2000 { get; } = new DbVersion("2000", 8.0m);
        public static DbVersion SQL2005 { get; } = new DbVersion("2005", 9.0m);
        public static DbVersion SQL2008 { get; } = new DbVersion("2008", 10.0m);
        public static DbVersion SQL2008R2 { get; } = new DbVersion("2008R2", 10.5m);
        public static DbVersion SQL2012 { get; } = new DbVersion("2012", 11.0m);
        public static DbVersion SQL2014 { get; } = new DbVersion("2014", 12.0m);
        public static DbVersion SQL2016 { get; } = new DbVersion("2016", 13.0m);
        public static DbVersion SQL2017 { get; } = new DbVersion("2017", 14.0m);
        public static DbVersion SQL2019 { get; } = new DbVersion("2019", 15.0m);

        public static IEnumerable<DbVersion> LookupAll()
        {
            var dbVersions = ObjectUtil.GetStaticPropertyValuesOfType<DbVersion>(typeof(DbVersion))
                .Select(mv => mv.Value)
                .ToList();
            return dbVersions;
        }

        public static DbVersion Lookup(string versionName, bool ignoreCase = false)
        {
            var searchCriteria = SearchCriteria.Equals(versionName, ignoreCase: ignoreCase);
            try
            {
                return LookupAll()
                    .Single(v => searchCriteria.Evaluate(v.Name) || searchCriteria.Evaluate("SQL" + v.Name));
            }
            catch
            {
                throw new UtilityException("Cannot find a SQL Server version named " + versionName);
            }
        }

        public string Name { get; }
        public decimal Number { get; }

        public DbVersion(string name, decimal number)
        {
            Name = name;
            Number = number;
        }

        public override string ToString()
        {
            return "SQL Server " + Name;
        }

        public override bool Equals(object other)
        {
            if (other is DbVersion version)
            {
                return Equals(version);
            }
            return false;
        }

        public bool Equals(DbVersion other)
        {
            return other != null &&
                   Name == other.Name &&
                   Number == other.Number;
        }

        public override int GetHashCode()
        {
            var hashCode = -1796076155;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DbVersion left, DbVersion right)
        {
            return EqualityComparer<DbVersion>.Default.Equals(left, right);
        }

        public static bool operator !=(DbVersion left, DbVersion right)
        {
            return !(left == right);
        }
    }
}
