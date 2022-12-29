namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Configuration settings used across DB implementations.
    /// </summary>
    public static class DbSettings
    {
        static DbPlatform? _defaultPlatform;

        /// <summary>
        /// Gets or sets the text platform used in DB operations. Note: Override by passing directly to a DB method or via config file: key = "Horseshoe.NET:Db:Platform" or OrganizationalDefaultSettings: key = Db.Platform
        /// </summary>
        public static DbPlatform? DefaultPlatform
        {
            get
            {
                return _defaultPlatform
                    ?? _Config.Get<DbPlatform?>("Horseshoe.NET:Db:Platform")
                    ?? OrganizationalDefaultSettings.Get<DbPlatform?>("Db.Platform");
            }
            set
            {
                _defaultPlatform = value;
            }
        }

    }
}
