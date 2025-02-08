using System.DirectoryServices;
using System;
using System.Linq;

namespace Horseshoe.NET.ActiveDirectory
{
    /// <summary>
    /// A variety of LDAP object filter blueprints
    /// </summary>
    public static class LdapFiltersForAD
    {
        /// <summary>
        /// Creates a filter to search LDAP for a single user matching the search criteria.
        /// </summary>
        /// <param name="userId">A unique user identifier including account ID, display name or email (options depend on <c>propertiesToSearch</c>)</param>
        /// <param name="propertiesToSearch">Optional optimization, specify which LDAP properties may be searched (e.g. "sAMAccountName", "mail|userPrincipalName", etc.)</param>
        /// <returns>A search result containing user info</returns>
        /// <exception cref="ADSearchException"></exception>
        public static ILdapFilter GetUser(string userId, string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(nameof(userId) + " cannot be blank or null", nameof(userId));

            userId = userId.Trim();

            // e.g. "(&(objectCategory=person)(objectClass=user)(|(sAMAccountName=user12345)(mail=user12345)(userPrincipalName=user12345)))"
            return LdapFilter.And  
            (
                LdapFilter.ObjectCategory(ObjectCategory.person),
                LdapFilter.ObjectClass(ObjectClass.user),
                LdapFilter.Or
                (
                    propertiesToSearch.Split('|', ',')
                        .Select(prop => LdapFilter.Equals(prop, userId))
                        .ToArray()
                )
            );
        }

        /// <summary>
        /// Creates a filter to search LDAP for users matching the search criteria.
        /// </summary>
        /// <param name="partialUserId">A partial unique user identifier including account ID, display name or email (options depend on <c>propertiesToSearch</c>)</param>
        /// <param name="propertiesToSearch">Optional optimization, specify which LDAP properties may be searched (e.g. "sAMAccountName", "mail|userPrincipalName", etc.)</param>
        /// <param name="searchMode">How to search property values in LDAP</param>
        /// <returns>A search result containing user info</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ILdapFilter SearchUsers(string partialUserId, string propertiesToSearch = ADConstants.UserSearchProperties.SearchDefault, LdapSearchMode searchMode = default)
        {
            if (string.IsNullOrWhiteSpace(partialUserId))
                throw new ArgumentException(nameof(partialUserId) + " cannot be blank or null", nameof(partialUserId));

            partialUserId = partialUserId.Trim().Replace('%', '*');

            // e.g. "(&(objectCategory=person)(objectClass=user)(|(sAMAccountName=*chris*)(mail=*chris*)(userPrincipalName=*chris*)(sn=*chris*)(givenName=*chris*)))"
            return LdapFilter.And
            (
                LdapFilter.ObjectCategory(ObjectCategory.person),
                LdapFilter.ObjectClass(ObjectClass.user),
                LdapFilter.Or
                (
                    propertiesToSearch.Split('|', ',')
                        .Select(prop => LdapFilter.Search(prop, partialUserId, searchMode))
                        .ToArray()
                )
            );
        }

        public static ILdapFilter GetGroup(string groupName)
        {
            // e.g. "(&(objectCategory=group)(cn=rock stars))";
            return LdapFilter.And
            (
                LdapFilter.ObjectCategory("group"),
                LdapFilter.CnEquals(groupName)
            );
        }

        public static ILdapFilter SearchGroups(string partialGroupName, LdapSearchMode searchMode = default)
        {
            if (string.IsNullOrWhiteSpace(partialGroupName))
                throw new ArgumentException(nameof(partialGroupName) + " cannot be blank or null", nameof(partialGroupName));

            partialGroupName = partialGroupName.Trim().Replace('%', '*');

            // e.g. "(&(objectCategory=group)(cn=*rock))" or "(&(objectCategory=group)(cn=*stars))" etc.
            return LdapFilter.And
            (
                LdapFilter.ObjectCategory(ObjectCategory.group),
                LdapFilter.CnSearch(partialGroupName, searchMode)
            );
        }

        public static ILdapFilter GetOU(string ouName)
        {
            return LdapFilter.And
            (
                LdapFilter.ObjectClass(ObjectClass.organizationalUnit),
                LdapFilter.CnEquals(ouName)
            );
        }

        public static ILdapFilter SearchOUs(string partialOUName, LdapSearchMode searchMode = default)
        {
            if (string.IsNullOrWhiteSpace(partialOUName))
                throw new ArgumentException(nameof(partialOUName) + " cannot be blank or null", nameof(partialOUName));

            partialOUName = partialOUName.Trim().Replace('%', '*');

            return LdapFilter.And
            (
                LdapFilter.ObjectClass(ObjectClass.organizationalUnit),
                LdapFilter.CnSearch(partialOUName, searchMode: searchMode)
            );
        }
    }
}
