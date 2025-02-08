using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Horseshoe.NET.ActiveDirectory
{
    public static class ADEngine
    {
        private static DomainInfo _domain;
        private static DirectoryEntry _root;
        private static PrincipalContext _domainCtx;

        public static void SetRoot(DomainInfo domain)
        {
            _domain = domain;
            _root = BuildRootEntry();
            _domainCtx = domain.Path == null ? new PrincipalContext(ContextType.Domain) : new PrincipalContext(ContextType.Domain, domain.Name);
        }

        static ADEngine()
        {
            SetRoot(DomainInfo.Default);
        }
        
        public static DirectoryEntry BuildRootEntry() => _domain.Path == null ? new DirectoryEntry() : new DirectoryEntry(_domain.Path);

        public static PrincipalContext GetDomainContext() => _domainCtx; // _domain.Path == null ? new PrincipalContext(ContextType.Domain) : new PrincipalContext(ContextType.Domain, _domain.Name);

        /// <summary>
        /// Creates a filter to search LDAP for a specific user and performs the search.
        /// </summary>
        /// <param name="userId">A unique user identifier including account ID, display name or email (options depend on <c>propertiesToSearch</c>)</param>
        /// <param name="propertiesToLoad">Optional optimization, specify which properties LDAP will return.</param>
        /// <param name="propertiesToSearch">Optional optimization, specify which LDAP properties may be searched (e.g. "sAMAccountName", "mail|userPrincipalName", etc.)</param>
        /// <param name="peekFilter">Allow client access to generated filters prior to querying LDAP.</param>
        /// <returns>A search result containing user info</returns>
        /// <exception cref="ADSearchException"></exception>
        public static SearchResult GetUser(string userId, string propertiesToLoad = ADConstants.UserProperties.Default, string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault, Action<string> peekFilter = null)
        {
            // e.g. "(&(objectCategory=person)(objectClass=user)(|(sAMAccountName=user12345)(mail=user12345)(userPrincipalName=user12345)))"
            var filter = LdapFiltersForAD.GetUser(userId, propertiesToSearch: propertiesToSearch);
            peekFilter?.Invoke(filter.ToString());
            return Get(filter, propertiesToLoad: propertiesToLoad);
        }

        public static SearchResultCollection SearchUsers(string partialUserId, string propertiesToLoad = ADConstants.UserProperties.Default, string propertiesToSearch = ADConstants.UserSearchProperties.SearchDefault, LdapSearchMode searchMode = default, Action<string> peekFilter = null)
        {
            //"(&(objectCategory=person)(objectClass=user)(|(sAMAccountName=*chris*)(mail=*chris*)(userPrincipalName=*chris*)(sn=*chris*)(givenName=*chris*)))"
            var filter = LdapFiltersForAD.SearchUsers(partialUserId, propertiesToSearch: propertiesToSearch, searchMode: searchMode);
            peekFilter?.Invoke(filter.ToString());
            return Search(filter, propertiesToLoad: propertiesToLoad);
        }

        public static SearchResult GetGroup(string groupName, string propertiesToLoad = ADConstants.GroupProperties.GetDefault, Action<string> peekFilter = null)
        {
            //"(&(objectCategory=group)(cn=rock stars))";
            var filter = LdapFiltersForAD.GetGroup(groupName);
            peekFilter?.Invoke(filter.ToString());
            return Get(filter, propertiesToLoad: propertiesToLoad);
        }
    
        public static SearchResultCollection SearchGroups(string partialGroupName, string propertiesToLoad = ADConstants.GroupProperties.SearchDefault, LdapSearchMode searchMode = default, Action<string> peekFilter = null)
        {
            //"(&(objectCategory=group)(cn=*stars*))";
            var filter = LdapFiltersForAD.SearchGroups(partialGroupName, searchMode: searchMode);
            peekFilter?.Invoke(filter.ToString());
            return Search(filter, propertiesToLoad: propertiesToLoad);
        }

        public static SearchResultCollection ListOUs(Action<string> peekFilter = null)
        {
            var filter = LdapFilter.ObjectClass(ObjectClass.organizationalUnit);
            peekFilter?.Invoke(filter.ToString());
            return Search(filter);
        }

        public static SearchResult GetOU(string ouName, Action<string> peekFilter = null)
        {
            var filter = LdapFiltersForAD.GetOU(ouName);
            peekFilter?.Invoke(filter.ToString());
            return Get(filter);
        }

        public static SearchResultCollection SearchOUs(string partialOUName, LdapSearchMode searchMode = default, Action<string> peekFilter = null)
        {
            var filter = LdapFiltersForAD.SearchOUs(partialOUName, searchMode: searchMode);
            peekFilter?.Invoke(filter.ToString());
            return Search(filter);
        }

        /// <summary>
        /// Searches AD based on a sytem or user generated filter
        /// </summary>
        /// <param name="filter">A filter which, when converted to text, can find objects or groups in LDAP.</param>
        /// <param name="propertiesToLoad">Optional optimization, specify which properties LDAP will return.</param>
        /// <returns></returns>
        /// <exception cref="ADSearchException"></exception>
        public static SearchResult Get(ILdapFilter filter, string propertiesToLoad = null)
        {
            using (var search = BuildDirectorySearcher(_root, filter, propertiesToLoad))
            {
                var result = search.FindOne();
                return result ?? throw new ADSearchException("cannot find " + ADUtil.BuildDescription(filter));
            }
        }
        
        public static SearchResultCollection Search(ILdapFilter filter, string propertiesToLoad = null)
        {
            using (var search = BuildDirectorySearcher(_root, filter, propertiesToLoad))
            {
                return search.FindAll();
            }
        }
        
        private static DirectorySearcher BuildDirectorySearcher(DirectoryEntry root, ILdapFilter filter, string propertiesToLoad)
        {
            var filterText = filter.ToString();
            return propertiesToLoad == null || string.Equals(propertiesToLoad, "*")
                ? new DirectorySearcher(root, filterText)
                : new DirectorySearcher(root, filterText, propertiesToLoad.Split('|', ','));
        }
    }
}