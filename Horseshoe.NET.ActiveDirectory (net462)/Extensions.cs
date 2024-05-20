using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Horseshoe.NET.ActiveDirectory
{
    public static class Extensions
    {
        private static Regex DirectoryEntryNamePattern { get; } = new Regex("(?<=^[A-Z]+=).*$");

        public static string GetDirectoryEntryName(this SearchResult searchResult)
        {
            var name = searchResult.GetDirectoryEntry().Name;
            var match = DirectoryEntryNamePattern.Match(name);
            return match.Success
                ? match.Value
                : name;
        }

        public static string GetName(this DirectoryEntry directoryEntry)
        {
            var match = DirectoryEntryNamePattern.Match(directoryEntry.Name);
            return match.Success
                ? match.Value
                : directoryEntry.Name;
        }

        public static object RawDistinguishedName(this DirectoryEntry directoryEntry)
        {
            return directoryEntry.Properties["distinguishedName"].Value;
        }

        public static string DistinguishedName(this DirectoryEntry directoryEntry)
        {
            return RawDistinguishedName(directoryEntry)?.ToString() ?? "[null]";
        }

        public static PropertyValueCollection RawMembers(this DirectoryEntry directoryEntry)
        {
            return directoryEntry.Properties["member"];
        }
    }
}
