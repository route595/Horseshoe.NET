using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.Caching
{
    public static class CacheSettings
    {
        private static int? _defaultCacheDuration;

        /// <summary>
        /// Gets or sets the default cache duration used by Caching.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Caching:CacheDuration and OrganizationalDefaultSettings: key = Caching.CacheDuration)
        /// </summary>
        public static int DefaultCacheDurationInSeconds
        {
            get
            {
                return _defaultCacheDuration
                    ?? Config.Get<int?>("Horseshoe.NET:Caching:CacheDuration")
                    ?? OrganizationalDefaultSettings.Get<int?>("Caching.CacheDuration")
                    ?? CacheConstants.DefaultCacheDurationInSeconds;
            }
            set
            {
                _defaultCacheDuration = value;
            }
        }
    }
}
