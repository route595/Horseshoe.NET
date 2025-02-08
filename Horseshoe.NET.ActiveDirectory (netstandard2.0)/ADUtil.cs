using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ActiveDirectory
{
    /// <summary>
    /// Common Active Directory related tasks
    /// </summary>
    public static class ADUtil
    {
        /// <summary>
        /// Authenticates and gets info about a user
        /// </summary>
        /// <param name="userId">A unique user identifier including account ID, display name or email (options depend on <c>propertiesToSearch</c>)</param>
        /// <param name="password">A password that is kept secure under the hood</param>
        /// <param name="propertiesToLoad">Optional optimization, specify which properties LDAP will return.</param>
        /// <param name="propertiesToSearch">Optional optimization, specify which LDAP properties may be searched (e.g. "sAMAccountName", "mail|userPrincipalName", etc.)</param>
        /// <param name="peekFilter">Allow client access to generated filters prior to querying LDAP.</param>
        /// <returns>An AD user</returns>
        /// <exception cref="ADLoginException"></exception>
        public static ADUser Authenticate
        (
            string userId, 
            Password password,
            string propertiesToLoad = ADConstants.UserProperties.Default,
            string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault,
            Action<string> peekFilter = null
        )
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ADLoginException("Please supply a user name");
            }

            var context = ADEngine.GetDomainContext();
            //using (var context = ADEngine.GetDomainContext())
            //{
            if (context.ValidateCredentials(userId, password.ToUnsecurePassword(), ContextOptions.SecureSocketLayer | ContextOptions.SimpleBind))
            {
                return GetUser(userId, propertiesToLoad: propertiesToLoad, propertiesToSearch: propertiesToSearch, peekFilter: peekFilter);
            }
            //}
            throw new ADLoginException("Login failed");
        }

        /// <summary>
        /// Gets info about a user
        /// </summary>
        /// <param name="userId">A unique user identifier including account ID, display name or email (options depend on <c>propertiesToSearch</c>)</param>
        /// <param name="propertiesToLoad">Optional optimization, specify which properties LDAP will return.</param>
        /// <param name="propertiesToSearch">Optional optimization, specify which LDAP properties may be searched (e.g. "sAMAccountName", "mail|userPrincipalName", etc.)</param>
        /// <param name="peekFilter">Allow client access to generated filters prior to querying LDAP.</param>
        /// <returns>An AD user</returns>
        public static ADUser GetUser
        (
            string userId,
            string propertiesToLoad = ADConstants.UserProperties.Default,
            string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault,
            Action<string> peekFilter = null
        )
        {
            var searchResult = ADEngine.GetUser(userId, propertiesToLoad, propertiesToSearch, peekFilter);
            var dict = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in searchResult.Properties)
            {
                var key = entry.Key.ToString();
                switch (searchResult.Properties[key].Count)
                {
                    case 0: // this should never happen
                        break;
                    case 1:
                        dict.Add(key, searchResult.Properties[key][0]);
                        break;
                    default:
                        dict.Add(key, searchResult.Properties[key].Cast<object>().ToArray());
                        break;
                }
            }

            return new ADUser(dict);
        }

        public static DirectoryEntry GetUserEntry(string userId,
            string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault,
            Action<string> peekFilter = null)
        {
            return ADEngine.GetUser(userId, "*", propertiesToSearch, peekFilter).GetDirectoryEntry();
        }

        public static IEnumerable<string> EnumerateUserProperties(string userId, int maxLength = 0,
            string propertiesToSearch = ADConstants.UserSearchProperties.GetDefault,
            Action<string> peekFilter = null)
        {
            var searchResult = ADEngine.GetUser(userId, "*", propertiesToSearch, peekFilter);
            var list = new List<string>();
            foreach (DictionaryEntry entry in searchResult.Properties)
            {
                string item;
                var key = entry.Key.ToString();
                if (entry.Value == null)
                {
                    item = entry.Key + " = [null]";
                }
                else
                {
                    var typeStr = "(" + searchResult.Properties[key][0].GetType().Name.ToLower() + ")";

                    if (searchResult.Properties[key][0] is object[] objects)
                    {
                        item = string.Join(", ", objects);
                    }
                    else
                    {
                        item = searchResult.Properties[key][0].ToString();
                    }

                    if (maxLength > 0 && key.Length + item.Length + typeStr.Length + 4 > maxLength)
                    {
                        int itemHalf = (maxLength - key.Length - typeStr.Length - 4) / 2 - 3;
                        if (itemHalf < 1)
                        {
                            itemHalf = 1;
                        }

                        item = item.Substring(0, itemHalf) + "..." + item.Substring(item.Length - itemHalf);
                    }

                    item = entry.Key + " = " + item + " " + typeStr;
                }

                list.Add(item);
            }

            return list;
        }

        public static void AssignUserToGroup(string userId, string groupName,
            string userPropertiesToSearch = ADConstants.UserSearchProperties.GetDefault,
            string groupPropertiesToLoad = ADConstants.GroupProperties.member,
            Action<string> peekUserFilter = null, Action<string> peekGroupFilter = null,
            Action beforeCommit = null, Action afterCommit = null)
        {
            var user = GetUserEntry(userId, propertiesToSearch: userPropertiesToSearch, peekFilter: peekUserFilter);
            var group = GetGroupEntry(groupName, propertiesToLoad: groupPropertiesToLoad, peekFilter: peekGroupFilter);
            AssignUserToGroup(user, group, beforeCommit: beforeCommit, afterCommit: afterCommit);
        }

        public static void AssignUserToGroup(DirectoryEntry user, DirectoryEntry group, Action beforeCommit = null, Action afterCommit = null)
        {
            var members = group.RawMembers();
            if (group.RawMembers().Contains(user.RawDistinguishedName()))
            {
                throw new Exception("This user is already a member of this AD group: " + user.Name + ": " + group.Name);
            }

            group.RawMembers().Add(user.RawDistinguishedName());
            beforeCommit?.Invoke();
            group.CommitChanges();
            afterCommit?.Invoke();
        }

        public static IEnumerable<ADUser> SearchUsers(string partialUserId,
            string propertiesToLoad = ADConstants.UserProperties.Default,
            string propertiesToSearch = ADConstants.UserSearchProperties.SearchDefault,
            Func<List<ADUser>, List<ADUser>> alterList = null, Action<string> peekFilter = null)
        {
            var searchResultCollection = ADEngine.SearchUsers(partialUserId, propertiesToLoad,
                propertiesToSearch, peekFilter: peekFilter);
            var users = new List<ADUser>();

            foreach (SearchResult searchResult in searchResultCollection)
            {
                var dict = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in searchResult.Properties)
                {
                    var key = entry.Key.ToString();
                    switch (searchResult.Properties[key].Count)
                    {
                        case 0: // this should never happen
                            break;
                        case 1:
                            dict.Add(key, searchResult.Properties[key][0]);
                            break;
                        default:
                            dict.Add(key, searchResult.Properties[key].Cast<object>().ToArray());
                            break;
                    }
                }

                users.Add(new ADUser(dict));
            }

            if (alterList == null)
                alterList = list => list.OrderBy(u => u.DisplayName ?? u.CN).ToList();
            users = alterList.Invoke(users);
            return users;
        }

        public static ADGroup GetGroup(string groupName, string propertiesToLoad = ADConstants.GroupProperties.GetDefault,
            Action<string> peekFilter = null)
        {
            var searchResult = ADEngine.GetGroup(groupName, propertiesToLoad, peekFilter: peekFilter);
            var dict = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in searchResult.Properties)
            {
                var key = entry.Key.ToString();
                switch (searchResult.Properties[key].Count)
                {
                    case 0: // this should never happen
                        break;
                    case 1:
                        dict.Add(key, searchResult.Properties[key][0]);
                        break;
                    default:
                        dict.Add(key, searchResult.Properties[key].Cast<object>().ToArray());
                        break;
                }
            }
            return new ADGroup(dict)
            {
                Name = searchResult.Path
            };
        }

        public static DirectoryEntry GetGroupEntry(string groupName, string propertiesToLoad = ADConstants.GroupProperties.GetDefault, Action<string> peekFilter = null)
        {
            return ADEngine.GetGroup(groupName, propertiesToLoad, peekFilter: peekFilter).GetDirectoryEntry();
        }

        public static IEnumerable<ADGroup> SearchGroups(string partialGroupName,
            string propertiesToLoad = ADConstants.GroupProperties.SearchDefault,
            Func<List<ADGroup>, List<ADGroup>> alterList = null, Action<string> peekFilter = null)
        {
            var searchResultCollection = ADEngine.SearchGroups(partialGroupName, propertiesToLoad,
                peekFilter: peekFilter);
            var groups = new List<ADGroup>();

            foreach (SearchResult searchResult in searchResultCollection)
            {
                var dict = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in searchResult.Properties)
                {
                    var key = entry.Key.ToString();
                    switch (searchResult.Properties[key].Count)
                    {
                        case 0: // this should never happen
                            break;
                        case 1:
                            dict.Add(key, searchResult.Properties[key][0]);
                            break;
                        default:
                            dict.Add(key, searchResult.Properties[key].Cast<object>().ToArray());
                            break;
                    }
                }

                groups.Add(new ADGroup(dict));
            }

            if (alterList == null)
                alterList = list => list.OrderBy(g => g.CN).ToList();
            groups = alterList.Invoke(groups);
            return groups;
        }

        public static IEnumerable<DirectoryEntry> SearchGroupEntries(string partialGroupName, string propertiesToLoad = ADConstants.GroupProperties.SearchDefault, Action<string> peekFilter = null)
        {
            var entries = new List<DirectoryEntry>();
            using (var groupsSearch = ADEngine.SearchGroups(partialGroupName, propertiesToLoad, peekFilter: peekFilter))
            {
                foreach (SearchResult result in groupsSearch)
                {
                    entries.Add(result.GetDirectoryEntry());
                }
            }
            return entries;
        }

        public static ADMember GetOU(string ouName, Action<string> peekFilter = null)
        {
            var searchResult = ADEngine.GetOU(ouName, peekFilter: peekFilter);
            return ADMember.FromAdsPath(searchResult.Path);
        }

        public static IEnumerable<ADMember> ListOUs(Action<string> peekFilter = null)
        {
            var members = new List<ADMember>();
            using (var ouSearch = ADEngine.ListOUs(peekFilter: peekFilter))
            {
                foreach (SearchResult result in ouSearch)
                {
                    members.Add(ADMember.FromAdsPath(result.Path));
                }
            }
            return members;
        }

        public static IEnumerable<ADMember> SearchOUs(string partialOUName, Action<string> peekFilter = null)
        {
            var members = new List<ADMember>();
            using (var ouSearch = ADEngine.SearchOUs(partialOUName, peekFilter: peekFilter))
            {
                foreach (SearchResult result in ouSearch)
                {
                    members.Add(ADMember.FromAdsPath(result.Path));
                }
            }
            return members;
        }

        public static string BuildDescription(ILdapFilter filter)
        {
            return BuildDescription(filter.ToString());
        }

        private static Regex _objectCategoryFilterPattern =
            new Regex(@"(?<=\(objectCategory=)[^)]+", RegexOptions.IgnoreCase);
        private static Regex _objectClassFilterPattern =
            new Regex(@"(?<=\(objectClass=)[^)]+", RegexOptions.IgnoreCase);
        private static Regex _cnFilterPattern =
            new Regex(@"(?<=\(cn=)[^)]+", RegexOptions.IgnoreCase);

        public static string BuildDescription(string filter)
        {
            var builder = new LdapFilterDescriptionBuilder();
            var match = _objectCategoryFilterPattern.Match(filter);
            if (match.Success)
            {
                builder.ObjectCategory = match.Value;
            }
            match = _objectClassFilterPattern.Match(filter);
            if (match.Success)
            {
                builder.ObjectClass = match.Value;
            }
            match = _cnFilterPattern.Match(filter);
            if (match.Success)
            {
                builder.CN = match.Value;
            }
            return builder.ToString();
        }

        private static Regex _ldapPathPartSeparators = new Regex(@"\b[A-Z]{2}=", RegexOptions.IgnoreCase);
        // private static Regex _cnAdsPathSeparatorPattern =
        //     new Regex("CN=", RegexOptions.IgnoreCase);
        // private static Regex _ouAdsPathSeparatorPattern =
        //     new Regex("OU=", RegexOptions.IgnoreCase);
        // private static Regex _dcAdsPathSeparatorPattern =
        //     new Regex("DC=", RegexOptions.IgnoreCase);

        public static void ExtractMemberParts(string adsPath, out string rawCn, out string rawOu, out string rawDc)
        {
            int rawCnStart = -1,
                rawCnEnd = -1,
                rawOuStart = -1,
                rawOuEnd = -1,
                rawDcStart = -1,
                rawDcEnd = -1;
            var _matches = _ldapPathPartSeparators.Matches(adsPath)
                .Cast<Match>()
                .ToArray();
            for (int i = 0, max = _matches.Length; i < max; i++)
            {
                switch (_matches[i].Value.TrimEnd('='))
                {
                    case "CN":
                        if (rawCnStart == -1)
                            rawCnStart = _matches[i].Index;
                        rawCnEnd = i < max - 1
                            ? _matches[i + 1].Index
                            : adsPath.Length;
                        break;
                    case "OU":
                        if (rawOuStart == -1)
                            rawOuStart = _matches[i].Index;
                        rawOuEnd = i < max - 1
                            ? _matches[i + 1].Index
                            : adsPath.Length;
                        break;
                    case "DC":
                        if (rawDcStart == -1)
                            rawDcStart = _matches[i].Index;
                        rawDcEnd = i < max - 1
                            ? _matches[i + 1].Index
                            : adsPath.Length;
                        break;
                }
            }
            rawCn = rawCnStart > -1
                ? adsPath.Substring(rawCnStart, rawCnEnd - rawCnStart).TrimEnd(',')
                : string.Empty;
            rawOu = rawOuStart > -1
                ? adsPath.Substring(rawOuStart, rawOuEnd - rawOuStart).TrimEnd(',')
                : string.Empty;
            rawDc = rawDcStart > -1
                ? adsPath.Substring(rawDcStart, rawDcEnd - rawDcStart).TrimEnd(',')
                : string.Empty;
        }

        public static string ExtractCNFromRaw(string rawCn)
        {
            if (string.IsNullOrEmpty(rawCn))
                return string.Empty;
            var cn = rawCn.Contains(",CN=")
                ? rawCn.Substring(3, rawCn.IndexOf(",CN=") - 3)
                : rawCn.Substring(3);
            if (Settings.Unescape)
                cn = cn.Replace(@"\,", ",");
            return cn;
        }

        public static string ExtractOUFromRaw(string rawOu)
        {
            if (string.IsNullOrEmpty(rawOu))
                return string.Empty;
            var ou = string.Join(" > ", rawOu.Substring(3).Split(new[] { ",OU=" }, 10, StringSplitOptions.None).Reverse());
            if (Settings.Unescape)
                ou = ou.Replace(@"\,", ",");
            return ou;
        }

        public static string ExtractDCFromRaw(string rawDc)
        {
            if (string.IsNullOrEmpty(rawDc))
                return string.Empty;
            var dc = rawDc.Substring(3).Replace(",DC=", ".");
            // if (Settings.UnescapeCommas)
            //     dc = dc.Replace(@"\,", ",");
            return dc;
        }

        public static string ExtractFirstValue(string ldapPath)
        {
            return ExtractValueAtIndex(ldapPath, 0);
        }

        private static void PreExtract(ref string ldapPath, out Match[] matches, DCOption dcOption)
        {
            var _matches = _ldapPathPartSeparators.Matches(ldapPath).Cast<Match>().ToList();

            if (!_matches.Any())
            {
                matches = Array.Empty<Match>();
                return;
            }

            var _lastMatch = _matches.Last();

            // Combine DC
            // Example: CN=My Group,OU=My OU,DC=myexample,DC=com  -->  My Group > My OU > myexample.com
            if (dcOption == DCOption.Combine && _lastMatch.Value.EndsWith("DC=", StringComparison.CurrentCultureIgnoreCase))
            {
                var strb = new StringBuilder(ldapPath.Substring(_lastMatch.Index + _lastMatch.Length));
                int dcFirstIndex = _matches.Count - 1;
                int dcLastIndex = _matches.Count - 1;

                for (int i = _matches.Count - 2; i >= 0; i--)
                {
                    if (_matches[i].Value.EndsWith("DC=", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int pos = _matches[i].Index + _matches[i].Length;
                        strb.Insert(0, '.');
                        strb.Insert(0, ldapPath.Substring(pos, _matches[i + 1].Index - pos));
                        _matches.RemoveAt(_matches.Count - 1);
                        dcFirstIndex = i;
                        continue;
                    }
                    break;
                }

                if (dcFirstIndex < dcLastIndex)
                {
                    strb.Insert(0, ldapPath.Substring(0, _matches[dcFirstIndex].Index + _matches[dcFirstIndex].Length));
                    ldapPath = strb.ToString();
                }
            }

            // Crop DC
            // Example: CN=My Group,OU=My OU,DC=myexample,DC=com  -->  My Group > My OU
            if (dcOption == DCOption.Crop)
            {
                Match lastRemoved = null;
                for (int i = _matches.Count - 1; i >= 0; i--)
                {
                    if (_matches[i].Value.EndsWith("DC=", StringComparison.CurrentCultureIgnoreCase))
                    {
                        lastRemoved = _matches.PopAt(_matches.Count - 1);
                        continue;
                    }
                    break;
                }

                if (lastRemoved != null)
                {
                    ldapPath = ldapPath.Substring(0, lastRemoved.Index);
                }
            }

            matches = _matches.ToArray();
        }

        public static string[] ExtractValues(string ldapPath, DCOption dcOption = default)
        {
            PreExtract(ref ldapPath, out Match[] matches, dcOption);
            //var matches = ldapPathPartSeparators.Matches(ldapPath).Cast<Match>().ToArray();
            var values = new string[matches.Length];

            for (int i = 0, last = matches.Length - 1; i < matches.Length; i++)
            {
                if (i == last)
                {
                    values[i] = Settings.Unescape
                        ? ldapPath.Substring(matches[i].Index + matches[i].Length).Replace(@"\", "")
                        : ldapPath.Substring(matches[i].Index + matches[i].Length);
                }
                else
                {
                    int pos = matches[i].Index + matches[i].Length;
                    values[i] = Settings.Unescape
                        ? ldapPath.Substring(pos, matches[i + 1].Index - pos).Replace(@"\", "")
                        : ldapPath.Substring(pos, matches[i + 1].Index - pos);
                }
            }

            return values;
        }

        public static string ExtractValueAtIndex(string ldapPath, int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), nameof(index) + "cannot be less than zero");

            PreExtract(ref ldapPath, out Match[] matches, default);
            //var matches = ldapPathPartSeparators.Matches(ldapPath).Cast<Match>().ToArray();

            if (index >= matches.Length)
                throw new ArgumentOutOfRangeException(nameof(index), nameof(index) + "cannot exceed size of array minus 1");

            if (index == matches.Length - 1)
            {
                var lastMatch = matches.Last();
                return ldapPath.Substring(lastMatch.Index + lastMatch.Length);
            }

            int pos = matches[index].Index + matches[index].Length;
            return Settings.Unescape
                ? ldapPath.Substring(pos, matches[index + 1].Index - pos).Replace(@"\", "")
                : ldapPath.Substring(pos, matches[index + 1].Index - pos);
        }
    }
}
