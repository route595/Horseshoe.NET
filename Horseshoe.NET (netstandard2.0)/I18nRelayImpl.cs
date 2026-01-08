using System.Collections.Generic;
using System.Globalization;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET
{
    /// <summary>
    /// Horseshoe.NET's service for maintaining and retrieving internationalized strings based on calling assembly, culture and key.
    /// (Internal use only.)
    /// </summary>
    /// <remarks>The <c>I18nService</c> class allows for the addition, replacement, and retrieval of localized
    /// strings using culture identifiers and specific keys. It supports defaulting to English if no culture is
    /// specified and provides options for strict retrieval, which throws exceptions if a string is not found.</remarks>
    internal static class I18nRelayImpl
    {
        private const string @default = "en";  // default language is English

        private static readonly Dictionary<string, string> _dict = new Dictionary<string, string>();

        internal static string Get(string assembly, string key, bool strict = false) =>
            Get(assembly, key, CultureInfo.CurrentCulture.Name, strict: strict);

        internal static string Get(string assembly, string key, string culture, bool strict = false)
        {
            var megaKey = ValidateAndBuildMegaKey(assembly, key, culture);
            if (_dict.TryGetValue(megaKey, out string value))
                return value;
            if (!string.IsNullOrEmpty(culture) && culture.Contains("-"))
            {
                var shortCulture = culture.Split('-')[0];
                return string.Equals(culture, @default, System.StringComparison.OrdinalIgnoreCase)
                    ? Get(assembly, null, key, strict: strict)
                    : Get(assembly, shortCulture, key, strict: strict);
            }
            if (!strict)
                return megaKey;
            throw new KeyNotFoundException($"No i18n entry found for culture / key: '{culture}' / '{key}");
        }

        internal static void AddOrReplace(string assembly, string key, string value) =>
            AddOrReplace(assembly, key, CultureInfo.CurrentCulture.Name, value);

        internal static void AddOrReplace(string assembly, string key, string culture, string value)
        {
            var cultureKey = ValidateAndBuildMegaKey(assembly, key, culture);
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException("value must contain text");

            _dict[cultureKey] = value;
        }

        private static string ValidateAndBuildMegaKey(string assembly, string key, string culture)
        {
            AssertValue.IsNotNullOrWhitespace(assembly, paramName: nameof(assembly));

            culture = string.IsNullOrEmpty(culture)
                ? @default
                : new CultureInfo(culture).Name.ToLower();
            return (assembly + "::" + key + "::" + culture.Replace('-', '.')).ToLower();
        }
    }
}
