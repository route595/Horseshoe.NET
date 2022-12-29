using System;
using System.Globalization;
using System.Text;
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
        /// <param name="strict">If a <c>Type</c> matching <c>className</c> cannot be found then <c>strict == true</c> causes an exception to be thrown, default is <c>false</c>.</param>
        /// <returns></returns>
        public static object GetInstance(string key, bool required = false, bool strict = false)
        {
            try
            {
                return Configuration.GetInstance(key, required: required, strict: strict);
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
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <param name="parseFunc">A custom parsing function.</param>
        /// <param name="dateTimeStyle">Applies to <c>Get&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="numberStyle">Applies to <c>Get&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Get&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>Get&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <returns></returns>
        public static T Get<T>
        (
            string key,
            bool required = false,
            Func<string, T> parseFunc = null,
            DateTimeStyles? dateTimeStyle = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            Type inheritedType = null,
            bool ignoreCase = false
        )
        {
            try
            {
                return Configuration.Get<T>(key, required: required, parseFunc: parseFunc, dateTimeStyle: dateTimeStyle, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, inheritedType: inheritedType, ignoreCase: ignoreCase);
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