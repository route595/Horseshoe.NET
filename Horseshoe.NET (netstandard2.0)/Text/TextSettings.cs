namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Configuration-based settings.
    /// </summary>
    public static class TextSettings
    {
        private static JsonProvider? _jsonProvider;

        /// <summary>
        /// Gets or sets the default JSON provider.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Text:JsonProvider and OrganizationalDefaultSettings: key = Text.JsonProvider)
        /// </summary>
        public static JsonProvider JsonProvider
        {
            get
            {
                if (!_jsonProvider.HasValue)
                {
                    _jsonProvider = _Config.Get<JsonProvider?>("Horseshoe.NET:Text:JsonProvider")
                        ?? OrganizationalDefaultSettings.Get<JsonProvider?>("Text.JsonProvider")
                        ?? (Assemblies.Get("Newtonsoft.Json", suppressErrors: true) != null ? JsonProvider.NewtonsoftJson as JsonProvider? : null)
                        ?? (Assemblies.Get("System.Text.Json", suppressErrors: true) != null ? JsonProvider.SystemTextJson as JsonProvider? : null)
                        ?? JsonProvider.None;
                }
                return _jsonProvider.Value;
            }
            set
            {
                _jsonProvider = value;
            }
        }
    }
}
