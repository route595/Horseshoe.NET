using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

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
        
        public static SearchResult GetUser(string userIdNameOrEmail, string propertiesToLoad = ADConstants.UserProperties.Default, string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault, Action<string> peekFilter = null)
        {
            var filter = //"(&(objectCategory=person)(objectClass=user)(|(sAMAccountName=user12345)(mail=user12345)(userPrincipalName=user12345)))"
                LdapFilter.And
                (
                    LdapFilter.ObjectCategory("person"),
                    LdapFilter.ObjectClass("user"),
                    LdapFilter.Or
                    (
                        propertiesToSearch.Split('|', ',')
                            .Select(prop => LdapFilter.Equals(prop, userIdNameOrEmail))
                            .ToArray()
                    )
                );
            return Get(filter, propertiesToLoad: propertiesToLoad, peekFilter: peekFilter);
        }

        public static SearchResultCollection SearchUsers(string partialUserIdNameOrEmail, string propertiesToLoad = ADConstants.UserProperties.Default, string propertiesToSearch = ADConstants.UserSearchProperties.SearchDefault, Action<string> peekFilter = null)
        {
            var filter = //"(&(objectCategory=person)(objectClass=user)(|(sAMAccountName=*chris*)(mail=*chris*)(userPrincipalName=*chris*)(sn=*chris*)(givenName=*chris*)))"
                LdapFilter.And
                (
                    LdapFilter.ObjectCategory("person"),
                    LdapFilter.ObjectClass("user"),
                    LdapFilter.Or
                    (
                        propertiesToSearch.Split('|', ',')
                            .Select(prop => LdapFilter.Contains(prop, partialUserIdNameOrEmail))
                            .ToArray()
                    )
                );
            return Search(filter, propertiesToLoad: propertiesToLoad, peekFilter: peekFilter);
        }

        public static SearchResult GetGroup(string groupName, string propertiesToLoad = ADConstants.GroupProperties.GetDefault, Action<string> peekFilter = null)
        {
            var filter = //"(&(objectCategory=group)(cn=rock stars))";
                LdapFilter.And
                (
                    LdapFilter.ObjectCategory("group"),
                    LdapFilter.Cn(groupName)
                );
            return Get(filter, propertiesToLoad: propertiesToLoad, peekFilter: peekFilter);
        }
    
        public static SearchResultCollection SearchGroups(string partialGroupName, string propertiesToLoad = ADConstants.GroupProperties.SearchDefault, Action<string> peekFilter = null)
        {
            var filter = //"(&(objectCategory=group)(cn=*stars*))";
                LdapFilter.And
                (
                    LdapFilter.ObjectCategory("group"),
                    LdapFilter.CnContains(partialGroupName)
                );
            return Search(filter, propertiesToLoad: propertiesToLoad, peekFilter: peekFilter);
        }

        public static SearchResultCollection ListOUs(Action<string> peekFilter = null)
        {
            var filter =
                LdapFilter.ObjectClass("organizationalUnit");
            return Search(filter, peekFilter: peekFilter);
        }

        public static SearchResultCollection SearchOUs(string partialOUName, Action<string> peekFilter = null)
        {
            var filter =
                LdapFilter.And
                (
                    LdapFilter.ObjectClass("organizationalUnit"),
                    LdapFilter.CnContains(partialOUName)
                );
            return Search(filter, peekFilter: peekFilter);
        }

        public static SearchResult GetOU(string ouName, Action<string> peekFilter = null)
        {
            var filter =
                LdapFilter.And
                (
                    LdapFilter.ObjectClass("organizationalUnit"),
                    LdapFilter.Cn(ouName)
                );
            return Get(filter, peekFilter: peekFilter);
        }

        public static SearchResult Get(ILdapFilter filter, string propertiesToLoad = null, Action<string> peekFilter = null)
        {
            using (var search = BuildDirectorySearcher(_root, filter, propertiesToLoad, peekFilter))
            {
                var result = search.FindOne();
                if (result == null)
                {
                    throw new ADSearchException("cannot find " + ADUtil.BuildDescription(filter));
                }
                return result;
            }
        }
        
        public static SearchResultCollection Search(ILdapFilter filter, string propertiesToLoad = null, Action<string> peekFilter = null)
        {
            using (var search = BuildDirectorySearcher(_root, filter, propertiesToLoad, peekFilter))
            {
                return search.FindAll();
            }
        }
        
        private static DirectorySearcher BuildDirectorySearcher(DirectoryEntry root, ILdapFilter filter, string propertiesToLoad, Action<string> peekFilter)
        {
            var filterText = filter.ToString();
            peekFilter?.Invoke(filterText);
            return propertiesToLoad == null || string.Equals(propertiesToLoad, "*")
                ? new DirectorySearcher(root, filterText)
                : new DirectorySearcher(root, filterText, propertiesToLoad.Split('|', ','));
        }
    }
}