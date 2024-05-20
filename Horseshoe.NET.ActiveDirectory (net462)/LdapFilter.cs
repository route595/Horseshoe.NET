using System.Collections.Generic;

namespace Horseshoe.NET.ActiveDirectory
{
    public class LdapFilter : ILdapFilter
    {
        internal string _text;

        private LdapFilter(string text)
        {
            _text = text;
        }
        
        public override string ToString()
        {
            return "(" + _text + ")";
        }

        public static ILdapFilter Equals(string left, string right) =>
           new LdapFilter(left + "=" + right);

        public static ILdapFilter Contains(string left, string right) =>
            new LdapFilter(left + "=*" + right + "*");

        public static ILdapFilter StartsWith(string left, string right) =>
            new LdapFilter(left + "=" + right + "*");

        public static ILdapFilter EndsWith(string left, string right) =>
            new LdapFilter(left + "=*" + right);
        
        public static ILdapFilter Literal(string filter) =>
            new LdapFilter(filter);

        public static ILdapFilter ObjectCategory(string objectCategory) =>
            Equals("objectCategory", objectCategory);

        public static ILdapFilter ObjectClass(string objectClass) =>
            Equals("objectClass", objectClass);

        public static ILdapFilter Cn(string cn) =>
            Equals("cn", cn);

        public static ILdapFilter CnContains(string cn) =>
            Contains("cn", cn);
        
        public static ILdapFilter CnStartsWith(string cn) =>
            StartsWith("cn", cn);

        public static ILdapFilter CnEndsWith(string cn) =>
            EndsWith("cn", cn);
        
        public static ILdapFilter Ou(string ou) =>
            Equals("ou", ou);
        
        public static ILdapFilter OuContains(string ou) =>
            Contains("ou", ou);
        
        public static ILdapFilter OuStartsWith(string ou) =>
            StartsWith("ou", ou);
        
        public static ILdapFilter OuEndsWith(string ou) =>
            EndsWith("ou", ou);

        public static ILdapFilter Name(string name) =>
            Equals("name", name);

        public static ILdapFilter NameContains(string name) =>
            Contains("name", name);

        public static ILdapFilter NameStartsWith(string name) =>
            StartsWith("name", name);

        public static ILdapFilter NameEndsWith(string name) =>
            EndsWith("name", name);

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