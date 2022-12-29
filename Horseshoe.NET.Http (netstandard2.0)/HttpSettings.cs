using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.Http
{
    public static class HttpSettings
    {
        private static string _defaultDomain;

        /// <summary>
        /// Gets or sets the default domain for Horseshoe.NET.Http will log into web services.  Note: Override by passing directly to a Net function or via config file: key =  "Horseshoe.NET:Http:Domain"
        /// </summary>
        public static string DefaultDomain
        {
            get
            {
                return _defaultDomain
                    ?? Config.Get("Horseshoe.NET:Http:Domain")
                    ?? OrganizationalDefaultSettings.Get<string>("Http.Domain");
            }
            set
            {
                _defaultDomain = value;
            }
        }

        private static Credential? _defaultWebServiceCredentials;

        /// <summary>
        /// Gets or sets how Horseshoe.NET.Http will log into web services.  Note: Override by passing directly to a Net function or via config file: key =  "Horseshoe.NET:Http:UserName|Password|Domain"
        /// </summary>
        public static Credential? DefaultWebServiceCredentials
        {
            get
            {
                return _defaultWebServiceCredentials
                    ?? Credential.Build(Config.Get("Horseshoe.NET:Http:UserName"), Config.Get("Horseshoe.NET:Http:Password"), isEncryptedPassword: Config.Get<bool>("Horseshoe.NET:Http:IsEncryptedPassword"), domain: DefaultDomain)
                    ?? OrganizationalDefaultSettings.Get<Credential?>("Http.Credentials");
            }
            set
            {
                _defaultWebServiceCredentials = value;
            }
        }
    }
}
