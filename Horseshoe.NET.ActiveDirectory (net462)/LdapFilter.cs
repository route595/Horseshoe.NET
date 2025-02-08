using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.ActiveDirectory
{
    public class LdapFilter : ILdapFilter
    {
        internal string _text;

        private LdapFilter(string text)
        {
            _text = text;
        }
        
        public override string ToString() =>
            "(" + _text + ")";

        public static ILdapFilter Equals(string left, string right) =>
           new LdapFilter(left + "=" + right);

        public static ILdapFilter Contains(string left, string right) =>
            new LdapFilter(left + "=*" + right + "*");

        public static ILdapFilter StartsWith(string left, string right) =>
            new LdapFilter(left + "=" + right + "*");

        public static ILdapFilter EndsWith(string left, string right) =>
            new LdapFilter(left + "=*" + right);

        /// <summary>
        /// Builds a search filter based on user input / preferences
        /// </summary>
        /// <param name="prop">An ldap property to search, e.g. sAMAccountName, email, dn, etc.</param>
        /// <param name="partialValue">The text to search, e.g. partial unique user identifier including account ID, display name or email when filtering user objects</param>
        /// <param name="searchMode">How to search property values in LDAP</param>
        /// <returns>An LDAP filter</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ILdapFilter Search(string prop, string partialValue, LdapSearchMode searchMode = default)
        {
            // validation
            if (string.IsNullOrWhiteSpace(prop))
                throw new ArgumentException(nameof(prop) + " must contain non-whitespace text", nameof(prop));
            if (string.IsNullOrWhiteSpace(partialValue))
                throw new ArgumentException(nameof(partialValue) + " must contain non-whitespace text", nameof(partialValue));

            // compatibility
            partialValue = partialValue.Trim().Replace('%', '*');

            // user defined search mode, e.g. "carl*" matches: carl, carlton, carlyle - non-matches: giancarlo, juan carlos
            if (partialValue.Contains('*'))
                return new LdapFilter(prop + "=" + partialValue);

            // internally defined search mode
            switch (searchMode)
            {
                case LdapSearchMode.Contains:
                default:
                    return Contains(prop, partialValue);
                case LdapSearchMode.StartsWith:
                    return StartsWith(prop, partialValue);
                case LdapSearchMode.EndsWith:
                    return EndsWith(prop, partialValue);
            }
        }

        public static ILdapFilter Literal(string filter) =>
            new LdapFilter(filter);

        public static ILdapFilter ObjectCategory(ObjectCategory objectCategory) =>
            ObjectCategory(objectCategory.ToString());

        public static ILdapFilter ObjectCategory(string objectCategory) =>
            Equals("objectCategory", objectCategory);

        public static ILdapFilter ObjectClass(ObjectClass objectClass) =>
            ObjectClass(objectClass.ToString());

        public static ILdapFilter ObjectClass(string objectClass) =>
            Equals("objectClass", objectClass);

        public static ILdapFilter CnEquals(string cn) =>
            Equals("cn", cn);

        public static ILdapFilter CnContains(string cn) =>
            Contains("cn", cn);
        
        public static ILdapFilter CnStartsWith(string cn) =>
            StartsWith("cn", cn);

        public static ILdapFilter CnEndsWith(string cn) =>
            EndsWith("cn", cn);

        public static ILdapFilter CnSearch(string cn, LdapSearchMode searchMode = default) =>
            Search("cn", cn, searchMode: searchMode);

        public static ILdapFilter OuEquals(string ou) =>
            Equals("ou", ou);
        
        public static ILdapFilter OuContains(string ou) =>
            Contains("ou", ou);
        
        public static ILdapFilter OuStartsWith(string ou) =>
            StartsWith("ou", ou);
        
        public static ILdapFilter OuEndsWith(string ou) =>
            EndsWith("ou", ou);

        public static ILdapFilter OuSearch(string ou, LdapSearchMode searchMode = default) =>
            Search("ou", ou, searchMode: searchMode);

        public static ILdapFilter NameEquals(string name) =>
            Equals("name", name);

        public static ILdapFilter NameContains(string name) =>
            Contains("name", name);

        public static ILdapFilter NameStartsWith(string name) =>
            StartsWith("name", name);

        public static ILdapFilter NameEndsWith(string name) =>
            EndsWith("name", name);

        public static ILdapFilter NameSearch(string name, LdapSearchMode searchMode = default) =>
            Search("name", name, searchMode: searchMode);

        public static ILdapFilter And(params ILdapFilter[] filters) =>
            new AndGroup(filters);

        public static ILdapFilter Or(params ILdapFilter[] filters) =>
            new OrGroup(filters);
        
        public class AndGroup : ILdapFilter
        {
            internal List<ILdapFilter> _filters;

            public AndGroup()
            {
                _filters = new List<ILdapFilter>();
            }

            public AndGroup(params ILdapFilter[] filters)
            {
                _filters = new List<ILdapFilter>(filters);
            }

            public void Add(string filter)
            {
                _filters.Add(new LdapFilter(filter));
            }

            public void Add(ILdapFilter filter)
            {
                _filters.Add(filter);
            }

            public override string ToString()
            {
                switch (_filters.Count)
                {
                    case 0:
                        return "";
                    case 1:
                        return _filters[0].ToString();
                    default:
                        return
                            "(&" + 
                            string.Join("", _filters) +
                            ")";
                }
            }
        }
        
        public class OrGroup : ILdapFilter
        {
            internal List<ILdapFilter> _filters;

            public OrGroup()
            {
                _filters = new List<ILdapFilter>();
            }

            public OrGroup(params ILdapFilter[] filters)
            {
                _filters = new List<ILdapFilter>(filters);
            }

            public void Add(string filter)
            {
                _filters.Add(new LdapFilter(filter));
            }

            public void Add(ILdapFilter filter)
            {
                _filters.Add(filter);
            }

            public override string ToString()
            {
                switch (_filters.Count)
                {
                    case 0:
                        return "";
                    case 1:
                        return _filters[0].ToString();
                    default:
                        return
                            "(|" + 
                            string.Join("", _filters) +
                            ")";
                }
            }
        }
    }
}