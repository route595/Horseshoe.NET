using System;
using System.Collections.Generic;
using System.Text;

namespace Horseshoe.NET.Ldap
{
    public class PersonSearchParam
    {
        public string AttrName { get; }
        public SearchType SearchType { get; }

        private PersonSearchParam(string attrName, SearchType searchType = default)
        {
            if (string.IsNullOrWhiteSpace(attrName))
                throw new ArgumentException("cannot be null or blank", nameof(attrName));
            AttrName = attrName;
            SearchType = searchType;
        }

        public string Render(string searchText)
        {
            switch (SearchType)
            {
                case SearchType.Equals:
                default:
                    return "(" + AttrName + "=" + searchText + ")";
                case SearchType.Contains:
                    return "(" + AttrName + "=*" + searchText + "*)";
                case SearchType.StartsWith:
                    return "(" + AttrName + "=" + searchText + "*)";
                case SearchType.EndsWith:
                    return "(" + AttrName + "=*" + searchText + ")";
            }
        }

        public override string ToString()
        {
            return Render("searchText");
        }

        public static PersonSearchParam Equals(string attrName)
        {
            return new PersonSearchParam(attrName, searchType: SearchType.Equals);
        }

        public static PersonSearchParam Contains(string attrName)
        {
            return new PersonSearchParam(attrName, searchType: SearchType.Contains);
        }

        public static PersonSearchParam StartsWith(string attrName)
        {
            return new PersonSearchParam(attrName, searchType: SearchType.StartsWith);
        }

        public static PersonSearchParam EndsWith(string attrName)
        {
            return new PersonSearchParam(attrName, searchType: SearchType.EndsWith);
        }
    }
}
