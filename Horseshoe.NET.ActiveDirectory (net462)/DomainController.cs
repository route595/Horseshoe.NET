using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Horseshoe.NET.ActiveDirectory
{
    public class DomainController
    {
        static Regex ParseDCRegex { get; } = new Regex(@"(?<=DC\=)[^,]+");

        public string Name { get; set; }
        public string LdapUrl => ADUtil.ToLdapUrl(dc: Name);

        public static DomainController FromLdapUrl(string ldapUrl)
        {
            var parts = ParseDCRegex.Matches(ldapUrl).Cast<Match>().Select(m => m.Value);
            var name = string.Join(".", parts);
            return new DomainController { Name = name };
        }
    }
}
