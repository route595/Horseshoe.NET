using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.ActiveDirectory
{
    public static class FilterFactory
    {
        public static string BuildAllUsersFilter()
        {
            return "(objectClass=" + ObjectClass.user + ")";
        }

        public static string BuildAllPersonUsersFilter()
        {
            return "(&(objectCategory=" + ObjectCategory.person + ")(objectClass=" + ObjectClass.user + "))";
        }

        public static string BuildUserLookupFilter(string searchString, UserProperty property = UserProperty.sAMAccountName)
        {
            return "(&(objectClass=" + ObjectClass.user + ")(" + property + "=" + searchString + "))";
        }

        public static string BuildUserLookupFilter(string searchString, IEnumerable<UserProperty> properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            switch (properties.Count())
            {
                case 0:
                    throw new InvalidOperationException("Caller must supply at least one user property to perform a search");
                case 1:
                    return BuildUserLookupFilter(searchString, properties.Single());
                default:
                    var orParts = properties.Select(prop => "(" + prop + "=" + searchString + ")");
                    var orPartsFlat = string.Join("", orParts);
                    return "(&(objectClass=" + ObjectClass.user + ")(|" + orPartsFlat + "))";
            }
        }

        public static string BuildPersonUserLookupFilter(string searchString, UserProperty property = UserProperty.sAMAccountName)
        {
            return "(&(objectCategory=" + ObjectCategory.person + ")(objectClass=" + ObjectClass.user + ")(" + property + "=" + searchString + "))";
        }

        public static string BuildPersonUserLookupFilter(string searchString, IEnumerable<UserProperty> properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            switch (properties.Count())
            {
                case 0:
                    throw new InvalidOperationException("Caller must supply at least one user property to perform a search");
                case 1:
                    return BuildPersonUserLookupFilter(searchString, properties.Single());
                default:
                    var orParts = properties.Select(prop => "(" + prop + "=" + searchString + ")");
                    var orPartsFlat = string.Join("", orParts);
                    return "(&(objectCategory=" + ObjectCategory.person + ")(objectClass=" + ObjectClass.user + ")(|" + orPartsFlat + "))";
            }
        }

        public static string BuildUserSearchFilter(string searchString, UserProperty property = UserProperty.sAMAccountName)
        {
            return "(&(objectClass=" + ObjectClass.user + ")(" + property + "=*" + searchString + "*))";
        }

        public static string BuildUserSearchFilter(string searchString, IEnumerable<UserProperty> properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            switch (properties.Count())
            {
                case 0:
                    throw new InvalidOperationException("Caller must supply at least one user property to perform a search");
                case 1:
                    return BuildUserSearchFilter(searchString, properties.Single());
                default:
                    var orParts = properties.Select(prop => "(" + prop + "=*" + searchString + "*)");
                    var orPartsFlat = string.Join("", orParts);
                    return "(&(objectClass=" + ObjectClass.user + ")(|" + orPartsFlat + "))";
            }
        }

        public static string BuildPersonUserSearchFilter(string searchString, UserProperty property = UserProperty.sAMAccountName)
        {
            return "(&(objectCategory=" + ObjectCategory.person + ")(objectClass=" + ObjectClass.user + ")(" + property + "=*" + searchString + "*))";
        }

        public static string BuildPersonUserSearchFilter(string searchString, IEnumerable<UserProperty> properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            switch (properties.Count())
            {
                case 0:
                    throw new InvalidOperationException("Caller must supply at least one user property to perform a search");
                case 1:
                    return BuildPersonUserSearchFilter(searchString, properties.Single());
                default:
                    var orParts = properties.Select(prop => "(" + prop + "=*" + searchString + "*)");
                    var orPartsFlat = string.Join("", orParts);
                    return "(&(objectCategory=" + ObjectCategory.person + ")(objectClass=" + ObjectClass.user + ")(|" + orPartsFlat + "))";
            }
        }

        public static string BuildAllOUsFilter()
        {
            return "(objectClass=" + ObjectClass.organizationalUnit + ")";
        }
    }
}
