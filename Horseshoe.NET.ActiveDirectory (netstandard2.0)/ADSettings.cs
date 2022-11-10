using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.ActiveDirectory
{
    public static class ADSettings
    {
        static string _defaultDomain;

        /// <summary>
        /// Gets or sets the default Active Directory domain name used by ActiveDirectory.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:ActiveDirectory.Domain)
        /// </summary>
        public static string DefaultDomain
        {
            get
            {
                return _defaultDomain
                    ?? Config.Get("Horseshoe.NET:ActiveDirectory:Domain")
                    ?? OrganizationalDefaultSettings.GetString("ActiveDirectory.Domain");
            }
            set
            {
                _defaultDomain = value;
            }
        }

    }
}
