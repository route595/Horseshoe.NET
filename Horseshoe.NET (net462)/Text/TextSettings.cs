using System;
using System.Reflection;

namespace Horseshoe.NET.Text
{
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
                    _jsonProvider = _Config.GetNEnum<JsonProvider>("Horseshoe.NET:Text:JsonProvider")
                        ?? OrganizationalDefaultSettings.GetNullable<JsonProvider>("Text.JsonProvider")
                        ?? (IsLoadable("Newtonsoft.Json") ? JsonProvider.NewtonsoftJson as JsonProvider? : null)
                        ?? (IsLoadable("System.Text.Json") ? JsonProvider.SystemTextJson as JsonProvider? : null)
                        ?? JsonProvider.None;
                }
                return _jsonProvider.Value;
            }
            set
            {
                _jsonProvider = value;
            }
        }

        static bool IsLoadable(string assemblyName)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                return assembly != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
