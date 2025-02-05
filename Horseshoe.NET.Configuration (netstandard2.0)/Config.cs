using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Horseshoe.NET.Configuration
{
    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
     * Note on annotations:
     * 
     * Keys and values may both be annotated to imply formatting / 
     * parsing directives, etc.  Annotations are always case-sensitive.
     * 
     * Value annotations examples...
     * 
     * Example 1 - w/out annotations
     * 
     * [c#]
     * config.Get<int>("red", numberStyle: NumberStyles.HexNumber);
     * 
     * [appsettings.json]
     * {
     *   "alpha": "0.5",
     *   "red": "66",
     *   "green": "a6",
     *   "blue": "51"
     * }
     * 
     * Example 2 - key and value annotations
     * 
     * [c#]
     * config.Get<int>("red[hex]");  // "66"       -- key annotation
     * config.Get<int>("green");     // "a6[hex]"  -- value annotation
     * config.Get<int>("blue");      // "0x51"     -- value native directive
     * 
     * [appsettings.json]
     * {
     *   "alpha": "0.5",
     *   "red": "66",
     *   "green": "a6[hex]",
     *   "blue": "0x51"
     * }
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

    public static class Config
    {
        private static IConfiguration configurationService;

        public static IConfiguration Configuration => configurationService ?? ConfigurationAccessor?.Invoke();

        public static Func<IConfiguration> ConfigurationAccessor { get; set; }

        /// <summary>
        /// Loads an <c>IConfiguration</c> service instance into Horseshoe.NET
        /// </summary>
        /// <param name="configuration"></param>
        public static void Load(IConfiguration configuration)
        {
            configurationService = configuration;
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
        /// <returns><c>true</c> if the configuration services is loaded and the key is associated with a value.</returns>
        public static bool Has(string key)
        {
            if (!IsLoaded())
                return false;
            return Configuration.Get(key) == null;  // required = false
        }

        /// <inheritdoc cref="Extensions.Get(IConfiguration, string, bool)"/>
        public static string Get(string key, bool required = false)
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return null;
            }
            return Configuration.Get(key, required: required);
        }

        /// <inheritdoc cref="Extensions.Get{T}(IConfiguration, string, bool, Func{string, T}, NumberStyles?, IFormatProvider, DateTimeStyles?, string, string, string, string, Encoding, bool)"/>
        public static T Get<T>
        (
            string key,
            bool required = false,
            Func<string, T> parser = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            DateTimeStyles? dateTimeStyle = null,
            string dateFormat = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            bool ignoreCase = false
        )
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return default;
            }
            return Configuration.Get<T>
            (
                key, 
                required: required, 
                parser: parser,
                numberStyle: numberStyle, 
                provider: provider, 
                dateTimeStyle: dateTimeStyle, 
                dateFormat: dateFormat,
                locale: locale, 
                trueValues: trueValues, 
                falseValues: falseValues, 
                encoding: encoding, 
                ignoreCase: ignoreCase
            );
        }

        /// <summary>
        /// <para>
        /// Extracts the value with the specified key and converts it to type <c>T</c>.
        /// </para>
        /// <para>
        /// This uses the .NET provided <c>GetValue&lt;T&gt;()</c>.
        /// Alternatively, use <c>Get&lt;T&gt;()</c> for the Horseshoe.NET provided functionality.
        /// </para>
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <param name="required">If <c>true</c> throws error if configuration service or key is not found, default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to use when not required and not supplied.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="NoConfigurationServiceException"></exception>
        public static T GetValue<T>(string key, bool required = false, T defaultValue = default)
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return defaultValue;
            }
            return Configuration.GetValue<T>(key, defaultValue);
        }

        /// <inheritdoc cref="Extensions.GetInstance(IConfiguration, string, bool, bool, bool, bool)"/>
        public static object GetInstance(string key, bool required = false, bool nonPublic = false, bool strict = false, bool ignoreCase = false)
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return null;
            }
            return Configuration.GetInstance(key, required: required, nonPublic: nonPublic, strict: strict, ignoreCase: ignoreCase);
        }

        /// <inheritdoc cref="Extensions.ParseSection{T}(IConfiguration, string, bool)"/>
        public static T ParseSection<T>(string path, bool required = false) where T : class
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return null;
            }
            return Configuration.ParseSection<T>(path, required: required);
        }

        /// <inheritdoc cref="Extensions.GetArray{T}(IConfiguration, string, bool)"/>
        public static T[] GetArray<T>(string path, bool required = false)
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return null;
            }
            return Configuration.GetArray<T>(path, required: required);
        }

        /// <inheritdoc cref="Extensions.GetConnectionString(IConfiguration, string, bool)"/>
        public static string GetConnectionString(string name, bool required = false)
        {
            if (!IsLoaded())
            {
                if (required)
                    throw new NoConfigurationServiceException();
                return null;
            }
            return Configuration.GetConnectionString(name, required: required);
        }
    }
}