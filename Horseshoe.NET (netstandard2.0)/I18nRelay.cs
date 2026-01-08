using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Horseshoe.NET
{
    /// <summary>
    /// Horseshoe.NET's service for maintaining and retrieving internationalized strings based on calling assembly, culture and key.
    /// (Internal use only.)
    /// </summary>
    public class I18nRelay
    {
        /// <summary>
        /// Retrieves the internationalized string associated with the calling assembly, the specified key and the current culture.
        /// </summary>
        /// <param name="key">The key representing the specific internationalized string to retrieve.</param>
        /// <param name="strict">A boolean value indicating whether to throw an exception if the internationalized value is not found.</param>
        /// <returns>The internationalized string corresponding to the calling assembly, the specified key and the current culture.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <c>strict == true</c> and no entry is found for the specified the calling assembly, the specified key and the current culture.</exception>
        public string Get(string key, bool strict = false) =>
            I18nRelayImpl.Get(Wrangle(Assembly.GetCallingAssembly()), key, CultureInfo.CurrentCulture.Name, strict);

        /// <summary>
        /// Retrieves the internationalized string associated with the calling assembly, the specified key and the supplied culture.
        /// </summary>
        /// <param name="key">The key representing the specific internationalized string to retrieve.</param>
        /// <param name="culture">The culture identifier used to identify the language of the internationalized string. If null or empty, defaults to <c>en</c>.</param>
        /// <param name="strict">A boolean value indicating whether to throw an exception if the internationalized value is not found.</param>
        /// <returns>The internationalized string corresponding to the calling assembly, the specified key and the supplied culture.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <c>strict == true</c> and no entry is found for the specified the calling assembly, the specified key and the supplied culture.</exception>
        public string Get(string key, string culture, bool strict = false) =>
            I18nRelayImpl.Get(Wrangle(Assembly.GetCallingAssembly()), key, culture, strict);

        /// <summary>
        /// Registers a new internationalized string or replaces an existing one for the calling assembly, the specified key and the current culture.
        /// </summary>
        /// <param name="key">The key representing the specific internationalized string to retrieve.</param>
        /// <param name="value">An internationalized string</param>
        public void AddOrReplace(string key, string value) =>
            I18nRelayImpl.AddOrReplace(Wrangle(Assembly.GetCallingAssembly()), CultureInfo.CurrentCulture.Name, key, value);

        /// <summary>
        /// Registers a new internationalized string or replaces an existing one for the calling assembly, the specified key and the supplied culture.
        /// </summary>
        /// <param name="key">The key representing the specific internationalized string to retrieve.</param>
        /// <param name="culture">The culture identifier used to identify the language of the internationalized string. If null or empty, defaults to <c>en</c>.</param>
        /// <param name="value">An internationalized string</param>
        public void AddOrReplace(string key, string culture, string value) =>
            I18nRelayImpl.AddOrReplace(Wrangle(Assembly.GetCallingAssembly()), key, culture, value);

        private string Wrangle(Assembly assembly)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Name;
        }
    }
}
