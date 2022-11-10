using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Novell.Directory.Ldap;

namespace Horseshoe.NET.Ldap
{
    public static class LdapExec
    {
        static void ConnectAndExecute(string userId, string password, string domain = null, bool useSSL = false, Action<LdapConnection> execute = null)
        {
            try
            {
                using (var connection = new LdapConnection { SecureSocketLayer = useSSL })
                {
                    connection.Connect(GetDomain(), LdapConnection.DefaultPort);
                    connection.Bind($"{userId}@{GetDomain()}", password);
                    if (!connection.Bound)
                    {
                        throw new LoginException("login failed");
                    }
                    execute?.Invoke(connection);
                }
            }
            catch (LdapException ex)
            {
                throw new LoginException("login failed", ex);
            }

            string GetDomain() => domain ?? Settings.DefaultDomain ?? throw new LoginException("no domain");
        }

        /// <summary>
        /// Creates a connection and logs on with the supplied credentials.  After using the connection please remember to close it.
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="password">password</param>
        /// <param name="domain">domain</param>
        /// <param name="useSSL">use SSL</param>
        /// <returns></returns>
        public static LdapConnection LaunchConnection(string userId, string password, string domain = null, bool useSSL = false)
        {
            try
            {
                var connection = new LdapConnection { SecureSocketLayer = useSSL };
                connection.Connect(GetDomain(), LdapConnection.DefaultPort);
                connection.Bind($"{userId}@{GetDomain()}", password);
                return connection;
            }
            catch (LdapException ex)
            {
                throw new LoginException("login failed", ex);
            }

            string GetDomain() => domain ?? Settings.DefaultDomain ?? throw new LoginException("no domain");
        }

        public static Person Login(string userId, string password, string domain = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Person> results = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    results = SearchPeople(connection, userId, searchWhat: new[] { PersonSearchParam.Equals(PersonAttribute.SamAccountName) }, recursive: true, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return results?.SingleOrDefault();
        }

        public static Person Login_AD(string userId, string password, string domain = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Person> results = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    results = SearchPeople_AD(connection, userId, searchWhat: new[] { PersonSearchParam.Equals(PersonAttribute.SamAccountName) }, recursive: true, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return results?.SingleOrDefault();
        }

        public static IEnumerable<Person> ListPeopleByOu(string ou, string userId, string password, string domain = null, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Person> people = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    people = ListPeopleByOu(connection, ou, recursive: recursive, attrFilter: attrFilter, maxResults: maxResults, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return people;
        }

        public static IEnumerable<Person> ListPeopleByOu(LdapConnection connection, string ou, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            List<Person> people = new List<Person>();

            // ldap search constraints
            var ldapSearchConstraints = new LdapSearchConstraints();
            if (maxResults.HasValue)
            {
                ldapSearchConstraints.MaxResults = maxResults.Value;
            }

            // format and run query
            var ldapFormattedQuery =
                "(objectClass=person)";
            var ldapResults = connection.Search
            (
                ou,
                recursive
                    ? LdapConnection.ScopeSub
                    : LdapConnection.ScopeOne,
                ldapFormattedQuery,
                attrFilter,
                false,
                ldapSearchConstraints
            );

            LdapEntry ldapEntry;
            while (ldapResults.HasMore())
            {
                try
                {
                    ldapEntry = ldapResults.Next();
                }
                catch (LdapException lex)
                {
                    if (!string.Equals(lex.Message, "Referral"))
                        throw;
                    break;
                }

                inspectLdapEntry?.Invoke(ldapEntry);
                var user = Util.ParseUser(ldapEntry, attrFilter);
                if (user != null)
                {
                    people.Add(user);
                }
            }
            people = people
                .OrderBy(p => p.DisplayName)
                .ToList();
            return people;
        }

        public static IEnumerable<Person> ListPeopleByOu_AD(string ou, string userId, string password, string domain = null, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Person> people = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    people = ListPeopleByOu_AD(connection, ou, recursive: recursive, attrFilter: attrFilter, maxResults: maxResults, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return people;
        }

        public static IEnumerable<Person> ListPeopleByOu_AD(LdapConnection connection, string ou, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            List<Person> people = new List<Person>();

            // ldap search constraints
            var ldapSearchConstraints = new LdapSearchConstraints();
            if (maxResults.HasValue)
            {
                ldapSearchConstraints.MaxResults = maxResults.Value;
            }

            // format and run query
            var ldapFormattedQuery =
                "(objectClass=person)";
            var ldapResults = connection.Search
            (
                ou,
                recursive
                    ? LdapConnection.ScopeSub
                    : LdapConnection.ScopeOne,
                ldapFormattedQuery,
                attrFilter,
                false,
                ldapSearchConstraints
            );

            LdapEntry ldapEntry;
            while (ldapResults.HasMore())
            {
                try
                {
                    ldapEntry = ldapResults.Next();
                }
                catch (LdapException lex)
                {
                    if (!string.Equals(lex.Message, "Referral"))
                        throw;
                    break;
                }

                inspectLdapEntry?.Invoke(ldapEntry);
                var user = Util.ParseUser(ldapEntry, attrFilter, isActiveDirectory: true);
                if (user != null)
                {
                    people.Add(user);
                }
            }
            people = people
                .OrderBy(p => p.DisplayName)
                .ToList();
            return people;
        }

        public static IEnumerable<Person> SearchPeople(string searchText, string userId, string password, string domain = null, IEnumerable<PersonSearchParam> searchWhat = null, string ou = null, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Person> people = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    people = SearchPeople(connection, searchText, searchWhat: searchWhat, ou: ou, recursive: recursive, attrFilter: attrFilter, maxResults: maxResults, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return people;
        }

        public static IEnumerable<Person> SearchPeople(LdapConnection connection, string searchText, IEnumerable<PersonSearchParam> searchWhat = null, string ou = null, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            List<Person> people = new List<Person>();

            // person search param(s)
            if (searchWhat == null || !searchWhat.Any())
            {
                searchWhat = new[]
                {
                    PersonSearchParam.Equals(PersonAttribute.SamAccountName),
                    PersonSearchParam.Equals(PersonAttribute.UserPrincipalName),
                    PersonSearchParam.Contains(PersonAttribute.DisplayName),
                };
            }

            // ldap search constraints
            var ldapSearchConstraints = new LdapSearchConstraints();
            if (maxResults.HasValue)
            {
                ldapSearchConstraints.MaxResults = maxResults.Value;
            }

            // format and run query
            var ldapFormattedQuery =
                "(&(objectClass=person)" +
                (
                    searchWhat.Count() == 1
                        ? searchWhat.Single().Render(searchText)
                        : "(|" + string.Join("", searchWhat.Select(psp => psp.Render(searchText))) + ")"
                ) +
                ")";
            var ldapResults = connection.Search
            (
                ou ?? new Dn(connection.Host),
                recursive
                    ? LdapConnection.ScopeSub
                    : LdapConnection.ScopeOne,
                ldapFormattedQuery, //"(&(objectClass=user)(objectClass=person)(|(sAMAccountName=" + searchText + ")(sn=" + searchText + ")(givenName=" + searchText + ")))",
                attrFilter,
                false,
                ldapSearchConstraints
            );

            LdapEntry ldapEntry;
            while (ldapResults.HasMore())
            {
                try
                {
                    ldapEntry = ldapResults.Next();
                }
                catch (LdapException lex)
                {
                    if (!string.Equals(lex.Message, "Referral"))
                        throw;
                    break;
                }

                inspectLdapEntry?.Invoke(ldapEntry);
                var user = Util.ParseUser(ldapEntry, attrFilter);
                if (user != null)
                {
                    people.Add(user);
                }
            }
            people = people
                .OrderBy(p => p.DisplayName)
                .ToList();
            return people;
        }

        public static IEnumerable<Person> SearchPeople_AD(string searchText, string userId, string password, string domain = null, IEnumerable<PersonSearchParam> searchWhat = null, string ou = null, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Person> people = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    people = SearchPeople_AD(connection, searchText, searchWhat: searchWhat, ou: ou, recursive: recursive, attrFilter: attrFilter, maxResults: maxResults, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return people;
        }

        public static IEnumerable<Person> SearchPeople_AD(LdapConnection connection, string searchText, IEnumerable<PersonSearchParam> searchWhat = null, string ou = null, bool recursive = false, string[] attrFilter = null, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            List<Person> people = new List<Person>();

            // person search param(s)
            if (searchWhat == null || !searchWhat.Any())
            {
                searchWhat = new[]
                {
                    PersonSearchParam.Equals(PersonAttribute.SamAccountName),
                    PersonSearchParam.Equals(PersonAttribute.UserPrincipalName),
                    PersonSearchParam.Contains(PersonAttribute.DisplayName),
                };
            }

            // ldap search constraints
            var ldapSearchConstraints = new LdapSearchConstraints();
            if (maxResults.HasValue)
            {
                ldapSearchConstraints.MaxResults = maxResults.Value;
            }

            // format and run query
            var ldapFormattedQuery =
                "(&(objectClass=person)" +
                (
                    searchWhat.Count() == 1
                        ? searchWhat.Single().Render(searchText)
                        : "(|" + string.Join("", searchWhat.Select(psp => psp.Render(searchText))) + ")"
                ) +
                ")";
            var ldapResults = connection.Search
            (
                ou ?? new Dn(connection.Host),
                recursive
                    ? LdapConnection.ScopeSub
                    : LdapConnection.ScopeOne,
                ldapFormattedQuery, //"(&(objectClass=person)(|(sAMAccountName=" + searchText + ")(sn=" + searchText + ")(givenName=" + searchText + ")))",
                attrFilter,
                false,
                ldapSearchConstraints
            );

            LdapEntry ldapEntry;
            while (ldapResults.HasMore())
            {
                try
                {
                    ldapEntry = ldapResults.Next();
                }
                catch (LdapException lex)
                {
                    if (!string.Equals(lex.Message, "Referral"))
                        throw;
                    break;
                }

                inspectLdapEntry?.Invoke(ldapEntry);
                var user = Util.ParseUser(ldapEntry, attrFilter, isActiveDirectory: true);
                if (user != null)
                {
                    people.Add(user);
                }
            }
            people = people
                .OrderBy(p => p.DisplayName)
                .ToList();
            return people;
        }

        public static IEnumerable<Dn> ListOus(string userId, string password, string domain = null, string ou = null, bool recursive = false, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            IEnumerable<Dn> ous = null;
            ConnectAndExecute
            (
                userId,
                password,
                domain: domain,
                execute: (connection) =>
                {
                    ous = ListOus(connection, ou: ou, recursive: recursive, maxResults: maxResults, inspectLdapEntry: inspectLdapEntry);
                }
            );
            return ous;
        }

        public static IEnumerable<Dn> ListOus(LdapConnection connection, string ou = null, bool recursive = false, int? maxResults = null, Action<LdapEntry> inspectLdapEntry = null)
        {
            List<Dn> ous = new List<Dn>();

            // ldap search constraints
            var ldapSearchConstraints = new LdapSearchConstraints();
            if (maxResults.HasValue)
            {
                ldapSearchConstraints.MaxResults = maxResults.Value;
            }

            // format and run query
            var ldapFormattedQuery =
                "(objectClass=organizationalUnit)";
            var ldapResults = connection.Search
            (
                ou ?? new Dn(connection.Host),
                recursive
                    ? LdapConnection.ScopeSub
                    : LdapConnection.ScopeOne,
                ldapFormattedQuery,
                null,
                false,
                ldapSearchConstraints
            );

            LdapEntry ldapEntry;
            while (ldapResults.HasMore())
            {
                try
                {
                    ldapEntry = ldapResults.Next();
                }
                catch (LdapException lex)
                {
                    if (!string.Equals(lex.Message, "Referral"))
                        throw;
                    break;
                }

                inspectLdapEntry?.Invoke(ldapEntry);
                var dn = Dn.Parse(ldapEntry.Dn);
                ous.Add(dn);
            }
            ous = ous
                .OrderBy(dn => dn.ShortDn.ToReversePathString())
                .ToList();
            return ous;
        }
    }
}
