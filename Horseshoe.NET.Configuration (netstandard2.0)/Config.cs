using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Horseshoe.NET.Configuration
{
    public static class Config
    {
        private static IConfiguration configuration;

        public static IConfiguration Configuration =>
            ConfigurationAccessor?.Invoke() ?? configuration;

        public static Func<IConfiguration> ConfigurationAccessor { get; set; }

        /// <summary>
        /// Loads an <c>IConfiguration</c> instance into Horseshoe.NET
        /// </summary>
        /// <param name="configuration"></param>
        public static void Load(IConfiguration configuration)
        {
            Config.configuration = configuration;
        }

        /// <summary>
        /// Checks to see if a configuration instance is loaded
        /// </summary>
        /// <returns></returns>
        public static bool IsLoaded()
        {
            return Configuration != null;
        }

        /// <summary>
        /// Tests if a configuration value exists
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <returns>bool</returns>
        public static bool Has(string key)
        {
            return Get(key, required: false) != null;
        }

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if <c>true</c>, throws error if configuration instance or key is not found</param>
        /// <returns>a <c>string</c> value</returns>
        public static string Get(string key, bool required = false)
        {
            try
            {
                return Configuration.Get(key, required: required);
            }
            catch (NoConfigurationException)
            {
                throw new NoConfigurationException("Configuration service not loaded: try Config.Load(<configuration>) or Config.ConfigurationAccessor = () => <configuration>");
            }
        }

        /// <summary>
        /// Gets a configuration value and parses it to a <c>string[]</c>
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if true, throws error if configuration key not found</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, bool required = false)
        {
            return GetArray(key, delimiter: new[] { ',' }, required: required);
        }

        /// <summary>
        /// Gets a configuration value and parses it to a <c>string[]</c>
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if true, throws error if configuration key not found</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, char delimiter, bool required = false)
        {
            var rawArray = Get(key, required: required);
            if (rawArray == null)
                return Array.Empty<string>();
            return Zap.Strings(rawArray.Split(delimiter));
        }

        /// <summary>
        /// Gets a configuration value and parses it to a <c>string[]</c>
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if true, throws error if configuration key not found</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, char[] delimiter, bool required = false)
        {
            var rawArray = Get(key, required: required);
            if (rawArray == null)
                return Array.Empty<string>();
            return Zap.Strings(rawArray.Split(delimiter));
        }

        /// <summary>
        /// Gets a configuration value as an instance of a fully qualified class name (expected value).
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if <c>true</c>, throws error if configuration instance or key is not found</param>
        /// <param name="suppressErrors">if <c>true</c>, suppresses any object instantiation errors</param>
        /// <returns></returns>
        public static object GetInstance(string key, bool required = false, bool suppressErrors = false)
        {
            try
            {
                return Configuration.GetInstance(key, required: required, suppressErrors: suppressErrors);
            }
            catch (NoConfigurationException)
            {
                throw new NoConfigurationException("Configuration service not loaded: try Config.Load(<configuration>) or Config.ConfigurationAccessor = () => <configuration>");
            }
        }

        /// <summary>
        /// Gets a configuration value as an instance of the specified type.  
        /// You may need to supply a <c>parseFunc</c> for custom / complex types.
        /// </summary>
        /// <typeparam name="T">type parameter</typeparam>
        /// <param name="key">configuration key</param>
        /// <param name="parseFunc">parsing function</param>
        /// <param name="numberStyles">Available for parsing numeric formats (e.g. hexadecimal)</param>
        /// <param name="dateTimeStyles">Available for parsing date time formats</param>
        /// <param name="provider">Available for parsing numeric or date time formats</param>
        /// <param name="ignoreCase">if <c>true</c>, allows case insensitive enum parsing</param>
        /// <param name="required">if <c>true</c>, throws error if configuration instance or key is not found</param>
        /// <param name="suppressErrors">if <c>true</c>, throws error if enum parse error occurs</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<string, T> parseFunc = null, NumberStyles? numberStyles = null, DateTimeStyles? dateTimeStyles = null, IFormatProvider provider = null, bool ignoreCase = false, bool required = false, bool suppressErrors = false)
        {
            try
            {
                return Configuration.Get<T>(key, parseFunc: parseFunc, numberStyles: numberStyles, dateTimeStyles: dateTimeStyles, provider: provider, ignoreCase: ignoreCase, required: required, suppressErrors: suppressErrors);
            }
            catch (NoConfigurationException)
            {
                throw new NoConfigurationException("Configuration service not loaded: try Config.Load(<configuration>) or Config.ConfigurationAccessor = () => <configuration>");
            }
        }


        /// <summary>
        /// Gets a configuration section
        /// </summary>
        /// <typeparam name="T">type parameter</typeparam>
        /// <param name="path">configuration path</param>
        /// <param name="required">if <c>true</c>, throws error if configuration instance or section is not found</param>
        /// <returns>instance loaded from config section</returns>
        public static T ParseSection<T>(string path, bool required = false) where T : class
        {
            try
            {
                return Configuration.ParseSection<T>(path, required: required);
            }
            catch (NoConfigurationException)
            {
                throw new NoConfigurationException("Configuration service not loaded: try Config.Load(<configuration>) or Config.ConfigurationAccessor = () => <configuration>");
            }
        }

        /// <summary>
        /// Gets a configuration value array
        /// </summary>
        /// <typeparam name="T">subclass of <c>System.Configuration.ConfigurationSection</c></typeparam>
        /// <param name="path">configuration path</param>
        /// <param name="filter">an item filter</param>
        /// <param name="required">if <c>true</c>, throws error if configuration instance or section is not found</param>
        /// <returns></returns>
        public static T[] GetArray<T>(string path, bool required = false)
        {
            try
            {
                return Configuration.GetArray<T>(path, required: required);
            }
            catch (NoConfigurationException)
            {
                throw new NoConfigurationException("Configuration service not loaded: try Config.Load(<configuration>) or Config.ConfigurationAccessor = () => <configuration>");
            }
        }

        /// <summary>
        /// Gets a configured connection string
        /// </summary>
        /// <param name="name">connection string name</param>
        /// <param name="required">if <c>true</c>, throws error if configuration instance or connection string is not found</param>
        /// <returns></returns>
        public static string GetConnectionString(string name, bool required = false)
        {
            try
            {
                return Configuration.GetConnectionString(name, required: required);
            }
            catch (NoConfigurationException)
            {
                throw new NoConfigurationException("Configuration service not loaded: try Config.Load(<configuration>) or Config.ConfigurationAccessor = () => <configuration>");
            }
        }
    }
}