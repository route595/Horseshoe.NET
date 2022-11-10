using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.Ldap
{
    public static class Settings
    {
        static string _defaultDomain;

        /// <summary>
        /// Gets or sets the default domain for connecting to LDAP
        /// </summary>
        public static string DefaultDomain
        {
            get
            {
                return _defaultDomain ?? Config.Get("Horseshoe.NET:Ldap:Domain");
            }
            set
            {
                _defaultDomain = value;
            }
        }

        public static Regex[] BlacklistedDnNamePatterns { get; set; }
        public static string[] BlacklistedDnNames { get; set; }
        public static Func<Person,bool>[] BlacklistedPersonFunctions { get; set; }
    }
}
