namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Configuration settings used across DB implementations.
    /// </summary>
    public static class DbSettings
    {
        static DbProvider? _defaultProvider;

        /// <summary>
        /// Gets or sets the text DB provider. Note: Override by passing directly to a DB method or via config file: key = "Horseshoe.NET:Db:Provider" or OrganizationalDefaultSettings: key = Db.Provider
        /// </summary>
        public static DbProvider DefaultProvider
        {
            get
            {
                return _defaultProvider
                    ?? _Config.Get<DbProvider?>("Horseshoe.NET:Db:Provider")
                    ?? OrganizationalDefaultSettings.Get<DbProvider?>("Db.Provider")
                    ?? DbProvider.Neutral;
            }
            set
            {
                _defaultProvider = value;
            }
        }

        /// <summary>
        /// If <c>true</c>, the <c>Credentials</c> user name and password will be copied (overwritten) into the final generated connection string, default is provider specific.
        /// </summary>
        public static bool DefaultMergeCredentialsIntoFinalConnectionString => false;

        //private static bool? _isEncryptedPassword;

        ///// <summary>
        ///// Gets or sets
        ///// </summary>
        //public static bool? IsEncryptedPassword
        //{
        //    get
        //    {
        //        return _isEncryptedPassword
        //            ?? _Config.Get<bool?>("Horseshoe.NET:Db:IsEncryptedPassword")
        //            ?? OrganizationalDefaultSettings.Get<bool?>("Db.IsEncryptedPassword");
        //    }
        //    set
        //    {
        //        _isEncryptedPassword = value;
        //    }
        //}
    }
}
