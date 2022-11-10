using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ActiveDirectory
{
    public static class ADUtil
    {
        static Regex ParseCNRegex { get; } = new Regex(@"(?<=CN\=)[^,]+");
        static Regex ParseOURegex { get; } = new Regex(@"(?<=OU\=)[^,]+");

        /// <summary>
        /// Builds a PrincipalContext of type 'Domain' for AD query operations
        /// </summary>
        public static PrincipalContext GetDomainContext(string domain = null)
        {
            var _domain = domain ?? ADSettings.DefaultDomain;
            return _domain == null
                ? new PrincipalContext(ContextType.Domain)
                : new PrincipalContext(ContextType.Domain, _domain);
        }

        /// <summary>
        /// Validates user credentials and, if successful, returns the associated user information
        /// </summary>
        public static UserInfo Authenticate(string samAccountName, string plainTextPassword, string domain = null)
        {
            if (string.IsNullOrEmpty(samAccountName))
            {
                throw new AuthenticationException("Please supply a user name");
            }
            try
            {
                using (var context = GetDomainContext(domain: domain))
                {
                    if (context.ValidateCredentials(samAccountName, plainTextPassword, ContextOptions.Negotiate))
                    {
                        var userInfo = LookupUser(samAccountName, userProperty: UserProperty.sAMAccountName);
                        return userInfo;
                    }
                }
                throw new AuthenticationException("Login failed");
            }
            catch (ADException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ADException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Validates user credentials (other than SAMAccountName) and, if successful, returns the associated user information
        /// </summary>
        public static UserInfo LookupAndAuthenticate(string userSearchTerm, string plainTextPassword, UserProperty? userProperty = null, string domain = null)
        {
            if (string.IsNullOrEmpty(userSearchTerm))
            {
                throw new AuthenticationException("Please supply a search term");
            }
            var userInfo = LookupUser(userSearchTerm, userProperty: userProperty, domain: domain);
            return Authenticate(userInfo.SAMAccountName, plainTextPassword, domain: domain);
        }

        /* * * * * * * * * * * * * * * * * * * 
         *  USER LOOKUP FUNCTIONS (full-text)
         * * * * * * * * * * * * * * * * * * */

        public static UserInfo LookupUser(string userSearchTerm, UserProperty? userProperty = null, string domain = null)
        {
            var userInfo = BuildUser
            (
                RawLookupUser(userSearchTerm, userProperty: userProperty, domain: domain)
            );
            return userInfo;
        }

        private static DirectoryEntry RawLookupUser(string userSearchTerm, UserProperty? userProperty = null, string domain = null)
        {
            var _domain = domain ?? ADSettings.DefaultDomain;
            var entry = _domain == null
                ? new DirectoryEntry()
                : new DirectoryEntry(ToLdapUrl(dc: _domain));
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = userProperty.HasValue
                    ? FilterFactory.BuildUserLookupFilter(userSearchTerm, userProperty.Value)
                    : FilterFactory.BuildUserLookupFilter(userSearchTerm,
                        new UserProperty[]
                        {
                            UserProperty.sAMAccountName,
                            UserProperty.name,
                            UserProperty.displayName,
                            UserProperty.mail,
                            UserProperty.cn,
                            UserProperty.distinguishedName,
                            UserProperty.userPrincipalName
                        }
                    )
            };
            var results = mySearcher.FindAll();
            if (results == null || results.Count == 0)
            {
                throw new ADException("User info not found: " + userSearchTerm);
            }
            var directoryEntries = results.Cast<SearchResult>()
                .Select(sr => sr.GetDirectoryEntry())
                .ToList();
            switch (directoryEntries.Count())
            {
                case 0:
                    throw new ADException("User info not found: " + userSearchTerm);
                case 1:
                    return directoryEntries.Single();
                default:
                    var names = directoryEntries.Select(de => de.Name).ToList();
                    var summary = TextUtil.Crop(string.Join("; ", names), 50, truncateMarker: TruncateMarker.Ellipsis);
                    throw new ADException("Search for users matching \"" + userSearchTerm + "\" returned " + names.Count + " results: " + summary);
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * 
         *  PERSON USER LOOKUP FUNCTIONS (full-text)
         * * * * * * * * * * * * * * * * * * * * * * */

        public static UserInfo LookupPersonUser(string searchTerm, UserProperty? userProperty = null, string domain = null)
        {
            var userInfo = BuildUser
            (
                RawLookupPersonUser(searchTerm, userProperty: userProperty, domain: domain)
            );
            return userInfo;
        }

        private static DirectoryEntry RawLookupPersonUser(string userSearchTerm, UserProperty? userProperty = null, string domain = null)
        {
            var _domain = domain ?? ADSettings.DefaultDomain;
            var entry = _domain == null
                ? new DirectoryEntry()
                : new DirectoryEntry(ToLdapUrl(dc: _domain));
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = userProperty.HasValue
                    ? FilterFactory.BuildPersonUserLookupFilter(userSearchTerm, userProperty.Value)
                    : FilterFactory.BuildPersonUserLookupFilter(userSearchTerm,
                        new UserProperty[]
                        {
                            UserProperty.sAMAccountName,
                            UserProperty.name,
                            UserProperty.displayName,
                            UserProperty.mail,
                            UserProperty.cn,
                            UserProperty.distinguishedName,
                            UserProperty.userPrincipalName
                        }
                    )
            };
            var results = mySearcher.FindAll();
            if (results == null || results.Count == 0)
            {
                throw new ADException("User info not found: " + userSearchTerm);
            }
            var directoryEntries = results.Cast<SearchResult>()
                .Select(sr => sr.GetDirectoryEntry())
                .ToList();
            switch (directoryEntries.Count())
            {
                case 0:
                    throw new ADException("User info not found: " + userSearchTerm);
                case 1:
                    return directoryEntries.Single();
                default:
                    var names = directoryEntries.Select(de => de.Name).ToList();
                    var summary = TextUtil.Crop(string.Join("; ", names), 50, truncateMarker: TruncateMarker.Ellipsis);
                    throw new ADException("Search for users matching \"" + userSearchTerm + "\" returned " + names.Count + " results: " + summary);
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * 
         *  USER SEARCH FUNCTIONS (partial-text)
         * * * * * * * * * * * * * * * * * * * * */

        public static IEnumerable<DirectoryEntry> RawSearchUsers(string userSearchTerm, UserProperty? userProperty = null, string domain = null)
        {
            var _domain = domain ?? ADSettings.DefaultDomain;
            var entry = _domain == null
                ? new DirectoryEntry()
                : new DirectoryEntry(ToLdapUrl(dc: _domain));
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = userProperty.HasValue
                    ? FilterFactory.BuildUserSearchFilter(userSearchTerm, userProperty.Value)
                    : FilterFactory.BuildUserSearchFilter(userSearchTerm,
                        new UserProperty[]
                        {
                            UserProperty.sAMAccountName,
                            UserProperty.name,
                            UserProperty.displayName,
                            UserProperty.mail,
                            UserProperty.cn,
                            UserProperty.distinguishedName,
                            UserProperty.userPrincipalName
                        }
                    )
            };
            var results = mySearcher.FindAll();
            if (results == null || results.Count == 0)
            {
                throw new ADException("User info not found: " + userSearchTerm);
            }
            return results.Cast<SearchResult>()
                .Select(sr => sr.GetDirectoryEntry())
                .ToList();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * 
         *  PERSON USER SEARCH FUNCTIONS (partial-text)
         * * * * * * * * * * * * * * * * * * * * * * * * */

        public static IEnumerable<DirectoryEntry> RawSearchPersonUsers(string userSearchTerm, UserProperty? userProperty = null, string domain = null)
        {
            var _domain = domain ?? ADSettings.DefaultDomain;
            var entry = _domain == null
                ? new DirectoryEntry()
                : new DirectoryEntry(ToLdapUrl(dc: _domain));
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = userProperty.HasValue
                    ? FilterFactory.BuildPersonUserSearchFilter(userSearchTerm, userProperty.Value)
                    : FilterFactory.BuildPersonUserSearchFilter(userSearchTerm,
                        new UserProperty[]
                        {
                            UserProperty.sAMAccountName,
                            UserProperty.name,
                            UserProperty.displayName,
                            UserProperty.mail,
                            UserProperty.cn,
                            UserProperty.distinguishedName,
                            UserProperty.userPrincipalName
                        }
                    )
            };
            var results = mySearcher.FindAll();
            if (results == null || results.Count == 0)
            {
                throw new ADException("User info not found: " + userSearchTerm);
            }
            return results.Cast<SearchResult>()
                .Select(sr => sr.GetDirectoryEntry())
                .ToList();
        }

        /* * * * * * * * * * * * * 
         *  OU LISTING FUNCTIONS
         * * * * * * * * * * * * */

        public static IEnumerable<OUInfo> ListOUs(bool recursive = false, string domain = null)
        {
            var _domain = domain ?? ADSettings.DefaultDomain;
            var entry = _domain == null
                ? new DirectoryEntry()
                : new DirectoryEntry(ToLdapUrl(dc: _domain));
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = FilterFactory.BuildAllOUsFilter()
            };
            var results = mySearcher.FindAll();
            return results
                .Cast<SearchResult>()
                .Select(sr => BuildOU(sr))
                .Where(ou => recursive || ou.PseudoPath.Length == 1)
                .OrderBy(ou => ou.DisplayPseudoPath)
                .ToList();
        }

        public static IEnumerable<OUInfo> ListOUs(string path, bool recursive = false)
        {
            var ouInfo = BuildOU(path);
            return ListOUs(ouInfo, recursive);
        }

        public static IEnumerable<OUInfo> ListOUs(string[] pseudoPath, bool recursive = false)
        {
            var ouInfo = BuildOU(pseudoPath);
            return ListOUs(ouInfo, recursive);
        }

        public static IEnumerable<OUInfo> ListOUs(OUInfo ouInfo, bool recursive = false)
        {
            var entry = new DirectoryEntry(ouInfo.Path);
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = FilterFactory.BuildAllOUsFilter()
            };
            var results = mySearcher.FindAll();
            return results
                .Cast<SearchResult>()
                .Select(sr => BuildOU(sr))
                .Where(ou => recursive || ou.PseudoPath.Length == ouInfo.PseudoPath.Length + 1)
                .OrderBy(ou => ou.DisplayPseudoPath)
                .ToList();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * 
         *  LDAP USER LISTING FUNCTIONS
         * * * * * * * * * * * * * * * * * * * * * * * * */

        public static UserInfo[] ListUsersByGroup(string groupName, Predicate<UserInfo> filter = null) // e.g. "Domain Users"
        {
            var list = new List<UserInfo>();
            using (var ctx = new PrincipalContext(ContextType.Domain))
            {
                using (var grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, groupName))
                {
                    if (grp != null)
                    {
                        foreach (Principal p in grp.GetMembers())
                        {
                            if (p is UserPrincipal)
                            {
                                list.Add(BuildUser(p as UserPrincipal));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Group '" + groupName + "' not found: server = " + ctx.ConnectedServer);
                    }
                }
            }
            if (filter != null)
            {
                list = list
                    .Where(adUser => filter(adUser))
                    .ToList();
            }
            return list
                .ToArray();
        }

        public static IEnumerable<UserInfo> ListUsersByOU(string ouPath, Predicate<UserInfo> userNameFilter = null, bool recursive = false)
        {
            return ListUsersByOU(BuildOU(ouPath), userNameFilter, recursive);
        }

        public static IEnumerable<UserInfo> ListUsersByOU(OUInfo ou, Predicate<UserInfo> userNameFilter = null, bool recursive = false)
        {
            var list = new List<UserInfo>();
            var entry = new DirectoryEntry(ou.Path);
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = FilterFactory.BuildAllUsersFilter()
            };
            var results = mySearcher.FindAll();
            var users = results
                .Cast<SearchResult>()
                .Select(sr => BuildUser(sr.GetDirectoryEntry()))
                .Where(user => recursive || CollectionUtil.IsIdentical(user.OU.PseudoPath, ou.PseudoPath))
                .OrderBy(user => user.DisplayName)
                .ToList();
            if(userNameFilter != null)
            {
                users = users
                    .Where(u => userNameFilter(u))
                    .ToList();
            }
            return users;
        }

        private static UserInfo BuildUser(UserPrincipal userPrincipal)
        {
            var entry = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
            return BuildUser(entry);
        }

        private static UserInfo BuildUser(DirectoryEntry directoryEntry)
        {
            if (!directoryEntry.SchemaClassName.Equals(ObjectClass.user.ToString()))
            {
                throw new ADException("Invalid object class: " + directoryEntry.SchemaClassName);
            }
            var userInfo = new UserInfo
            {
                SAMAccountName = ParseProperty(directoryEntry.Properties["sAMAccountName"]),
                Name = ParseProperty(directoryEntry.Properties["name"]),
                DisplayName = ParseProperty(directoryEntry.Properties["displayName"]),
                FirstName = ParseProperty(directoryEntry.Properties["givenName"]),
                LastName = ParseProperty(directoryEntry.Properties["sn"]),
                Title = ParseProperty(directoryEntry.Properties["title"]),
                Description = ParseProperty(directoryEntry.Properties["description"]),
                EmailAddress = ParseProperty(directoryEntry.Properties["mail"]),
                TelephoneNumber = ParseProperty(directoryEntry.Properties["telephoneNumber"]),
                Department = ParseProperty(directoryEntry.Properties["department"]),
                PhysicalOfficeName = ParseProperty(directoryEntry.Properties["physicalDeliveryOfficeName"]),
                StreetAddress = ParseProperty(directoryEntry.Properties["streetAddress"]),
                City = ParseProperty(directoryEntry.Properties["l"]),
                State = ParseProperty(directoryEntry.Properties["st"]),
                PostalCode = ParseProperty(directoryEntry.Properties["postalCode"]),
                Country = ParseProperty(directoryEntry.Properties["c"]),
                DateCreated = ParseDateProperty(directoryEntry.Properties["whenCreated"]),
                LastLogon = ParseDateProperty(directoryEntry.Properties["lastLogon"], tryParseComObject: true),
                AccountExpires = ParseDateProperty(directoryEntry.Properties["accountExpires"], tryParseComObject: true),
                UserPrincipalName = ParseProperty(directoryEntry.Properties["userPrincipalName"]),
                Manager = ParseProperty(directoryEntry.Properties["manager"], tryParse: "CN"),
                ExtensionAttribute1 = ParseProperty(directoryEntry.Properties["extensionAttribute1"]),
                ParentPath = directoryEntry.Parent.Path,
                OU = GetOU(directoryEntry),
                GroupNames = ParseProperties(directoryEntry.Properties["memberOf"], tryParse: "CN", alphabetize: true)
            };
            return userInfo;
        }

        private static OUInfo GetOU(DirectoryEntry entry)
        {
            var parent = entry.Parent;
            while (!parent.SchemaClassName.Equals(ObjectClass.organizationalUnit.ToString()))
            {
                parent = parent.Parent;
            }
            if (parent.SchemaClassName.Equals(ObjectClass.organizationalUnit.ToString()))
            {
                return BuildOU(parent.Path);
            }
            return null;
        }

        private static OUInfo BuildOU(SearchResult searchResult)
        {
            var info = new OUInfo
            {
                Name = _ParseProperty(searchResult.GetDirectoryEntry().Name, tryParse: "OU"),
                Path = searchResult.Path,
                PseudoPath = ParseOUs(searchResult.Path)
            };
            return info;
        }

        public static OUInfo BuildOU(string path)
        {
            var info = new OUInfo
            {
                Path = path,
                PseudoPath = ParseOUs(path)
            };
            info.Name = info.PseudoPath.Last();
            return info;
        }

        public static OUInfo BuildOU(string[] pseudoPath) // [Corporate][Users]
        {
            if (pseudoPath.Length == 0)
            {
                throw new ADException("Please supply at least one item in pseudoPath");
            }
            var sb = new StringBuilder(GetADRootPath());          // e.g. LDAP://DC=domain,DC=local
            var pathOUParts = pseudoPath
                .Reverse()
                .Select(ou => "OU=" + ou)
                .ToList();                                      // e.g. [OU=Users][OU=Corporate]
            sb.Insert(7, string.Join(",", pathOUParts) + ",");  // e.g. LDAP://OU=Users,OU=Corporate,DC=domain,DC=local
            return new OUInfo
            {
                Name = pseudoPath.Last(),
                Path = sb.ToString(),
                PseudoPath = pseudoPath
            };
        }

        public static string GetADRootPath()
        {
            var sb = new StringBuilder();
            sb.Append("LDAP://");
            using (var ctx = new PrincipalContext(ContextType.Domain))
            {
                var index = ctx.ConnectedServer.IndexOf(".");                           // e.g. SERVERNAME.domain.local
                if (index > 0 && index < ctx.ConnectedServer.Length)
                {
                    var dcParts = ctx.ConnectedServer.Substring(index + 1).Split('.');  // e.g. [domain][local]
                    sb.Append(string.Join(",", dcParts.Select(dc => "DC=" + dc)));      // e.g. DC=domain,DC=local
                }
            }
            return sb.ToString();                                                       // e.g. LDAP://DC=domain,DC=local
        }

        public static string ParseProperty(PropertyValueCollection collection, string tryParse = null)
        {
            if (collection == null) return null;
            var property = collection.Value?.ToString();
            return _ParseProperty(property, tryParse: tryParse);
        }

        public static DateTime? ParseDateProperty(PropertyValueCollection collection, bool tryParseComObject = false)
        {
            if (collection == null) return null;
            var obj = collection.Value;
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            else if (obj != null)
            {
                var type = obj.GetType();
                if (tryParseComObject)   //  && type.FullName.Equals("System.__ComObject")
                {
                    try
                    {
                        long datePart = (int)type.InvokeMember("HighPart", BindingFlags.GetProperty, null, obj, null);
                        long timePart = (int)type.InvokeMember("LowPart", BindingFlags.GetProperty, null, obj, null);
                        if (datePart == 0 || timePart == -1)
                        {
                            return null;
                        }
                        datePart <<= 32;
                        var dateTime = DateTime.FromFileTime(datePart + timePart);
                        return dateTime;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public static string[] ParseProperties(PropertyValueCollection collection, string tryParse = null, bool alphabetize = false)
        {
            if (collection == null) return null;
            var collectionAsStrings = collection
                .Cast<object>()
                .Select(o => _ParseProperty(o?.ToString(), tryParse: tryParse))
                .ToList();
            if (alphabetize)
            {
                collectionAsStrings.Sort();
            }
            return collectionAsStrings.ToArray();
        }

        private static string _ParseProperty(string property, string tryParse = null)
        {
            if (property != null)
            {
                switch(tryParse)
                {
                    case "CN":
                        var cnMatch = ParseCNRegex.Match(property);
                        if (cnMatch.Success)
                        {
                            property = cnMatch.Value;
                        }
                        break;
                    case "OU":
                        var ouMatch = ParseOURegex.Match(property);
                        if (ouMatch.Success)
                        {
                            property = ouMatch.Value;
                        }
                        break;
                }
            }
            return property;
        }

        private static string[] ParseOUs(string property)
        {
            if (property == null) return null;
            var matches = ParseOURegex.Matches(property);
            if (matches.Count > 0)
            {
                var returnArray = matches
                    .Cast<Match>()
                    .Reverse()
                    .Select(m => m.Value)
                    .ToArray();
                return returnArray;
            }
            return new string[] { };
        }

        public static DomainController DetectDomainController(string domain = null)
        {
            using (var context = GetDomainContext(domain: domain))
            {
                return new DomainController { Name = context.ConnectedServer };
            }
        }

        public static string ToLdapUrl(string dc = null, string ou = null, string[] ous = null, string cn = null)
        {
            var sb = new StringBuilder("LDAP://");
            var prependComma = false;
            if (dc != null)
            {
                sb.Append(string.Join(",", dc.Split('.').Select(part => "DC=" + part)));
                prependComma = true;
            }
            if (ou != null)
            {
                if (prependComma)
                {
                    sb.Append(",");
                }
                sb.Append("OU=" + ou);
                prependComma = true;
            }
            if (ous != null)
            {
                if (prependComma)
                {
                    sb.Append(",");
                }
                sb.Append(string.Join(",", ous.Select(_ou => "OU=" + _ou)));
                prependComma = true;
            }
            if (cn != null)
            {
                if (prependComma)
                {
                    sb.Append(",");
                }
                sb.Append("CN=" + cn);
            }
            return sb.ToString();
        }
    }
}
