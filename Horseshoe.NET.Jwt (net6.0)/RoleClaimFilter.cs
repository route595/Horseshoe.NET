using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Jwt
{
    public struct RoleClaimFilter
    {
        public IEnumerable<string> Filters { get; }

        /// <summary>
        /// Create a <c>RoleClaimFilter</c>
        /// </summary>
        /// <param name="filterString">comma- or pipe-separated list of filters, wildcard = '*'</param>
        public RoleClaimFilter(string filterString)
        {
            if (filterString.Contains('|'))
            {
                Filters = filterString.Split('|')
                    .Select(s => s.Trim())
                    .ToList();
            }
            else if (filterString.Contains(','))
            {
                Filters = filterString.Split(',')
                    .Select(s => s.Trim())
                    .ToList();
            }
            else
            {
                Filters = Enumerable.Repeat(filterString, 1);
            }
        }

        /// <summary>
        /// Create a <c>RoleClaimFilter</c>
        /// </summary>
        /// <param name="filterString">comma- or pipe-separated list of filters, wildcard = '*'</param>
        public RoleClaimFilter(params string[] filters)
        {
            Filters = filters ?? Array.Empty<string>();
        }

        public bool Any()
        {
            return Filters != null && Filters.Any();
        }

        public override string ToString()
        {
            return Filters != null
                ? string.Join("|", Filters)
                : "";
        }

        public static implicit operator RoleClaimFilter(string filterString) => new RoleClaimFilter(filterString);
        public static implicit operator RoleClaimFilter(string[] filters) => new RoleClaimFilter(filters ?? Array.Empty<string>());
        public static implicit operator RoleClaimFilter(List<string> filters) => new RoleClaimFilter(filters?.ToArray() ?? Array.Empty<string>());
    }
}
