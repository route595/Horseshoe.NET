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

    public static class Extensions
    {
        /// <summary>
        /// Tests if a configuration value exists
        /// </summary>
        /// <param name="config">An <c>IConfiguration</c> instance / service.</param>
        /// <param name="key">A configuration key</param>
        /// <returns>bool</returns>
        public static bool Has(this IConfiguration config, string key)
        {
            return Get(config, key) != null;  // required = false
        }

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="config">An <c>IConfiguration</c> service / instance.</param>
        /// <param name="key">A configuration key</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key is not found, default is <c>false</c>.</param>
        /// <returns>An unparsed configuration value of type <c>string</c></returns>
        /// <exception cref="NoConfigurationServiceException"></exception>
        /// <exception cref="ConfigurationException"></exception>
        public static string Get(this IConfiguration config, string key, bool required = false)
        {
            // validation
            if (config == null)
                throw new NoConfigurationServiceException();
            if (string.IsNullOrWhiteSpace(key))
                throw new ConfigurationException("An invalid key was supplied: " + ConfigUtil.DisplayValue(key));

            var value = config[key];
            if (string.IsNullOrWhiteSpace(value) && required)
                throw new ConfigurationException("Required configuration not supplied: " + ConfigUtil.DisplayValue(key));
            return value;
        }

        /// <summary>
        /// <para>
        /// Gets a configuration value as an instance of the specified type.  
        /// You may need to supply a <c>parseFunc</c> for custom / complex types.
        /// </para>
        /// <para>
        /// This uses the Horseshoe.NET provided <c>Get&lt;T&gt;()</c>.
        /// Alternatively, use <c>GetValue&lt;T&gt;()</c> for the .NET provided functionality.
        /// </para>
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="config">An <c>IConfiguration</c> service / instance.</param>
        /// <param name="key">A configuration key</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key is not found, default is <c>false</c>.</param>
        /// <param name="parser">A custom parsing function.</param>
        /// <param name="numberStyle">Applies to <c>Get&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateTimeStyle">Applies to <c>Get&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="dateFormat">An optional, exact date/time format to use when parsing date time values from <c>string</c>.</param>
        /// <param name="locale">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Get&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>Get&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <returns>An instance of <c>T</c> parsed from a configuration value.</returns>
        /// <exception cref="NoConfigurationServiceException"></exception>
        /// <exception cref="ConfigurationException"></exception>
        public static T Get<T>
        (
            this IConfiguration config, 
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
            // pre-validation
            if (string.IsNullOrEmpty(key))
                throw new ConfigurationException("An invalid key was supplied: " + ConfigUtil.DisplayValue(key));

            Type type = typeof(T);
            if (Nullable.GetUnderlyingType(type) is Type nuType)
                type = nuType;

            // infer style/format from annotation (also infer correct key)
            if (ConfigUtil.IsNumeric(type))     // e.g. Config.Get<int>("red[hex]") -> key = "red", numberStyle = NumberStyles.HexNumber
                ConfigUtil.ParseNumericAnnotation(ref key, ref numberStyle);
            else if (type == typeof(DateTime))  // e.g. Config.Get<DateTime>("dateOfBirth[yyyy-MM-dd]") -> key = "dateOfBirth", dateFormat = "yyyy-MM-dd"
                ConfigUtil.ParseDateTimeAnnotation(ref key, ref dateFormat);

            var value = Get(config, key, required: required);

            // scenario 1 - null
            if (value == null)
                return default;

            // scenario 2 - user supplied parser
            if (parser != null)
                return parser.Invoke(value);

            // scenario 3 - use built-in conversions, if available
            // note: accepts values with annotations (see note at top of page)
            return ConfigUtil.ZapTo<T>
            (
                value, 
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
        /// Gets a configuration value as an instance of a fully qualified class name (expected value).
        /// </summary>
        /// <param name="config">An <c>IConfiguration</c> service / instance.</param>
        /// <param name="key">A configuration key</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key is not found, default is <c>false</c>.</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <param name="ignoreCase">If <c>true</c>, allows searching assemblies/types that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <param name="strict">If <c>true</c> then not finding a <c>Type</c> matching the supplied name causes an exception to be thrown, default is <c>false</c>.</param>
        /// <returns>A dynamically created instance of the configured type</returns>
        /// <exception cref="NoConfigurationServiceException"></exception>
        /// <exception cref="ConfigurationException"></exception>
        /// <exception cref="TypeNotFoundException"></exception>
        public static object GetInstance(this IConfiguration config, string key, bool required = false, bool nonPublic = false, bool ignoreCase = false, bool strict = false)
        {
            var className = Get(config, key, required: required || strict);
            if (className == null)
                return null;
            return ConfigUtil.GetInstance(className, nonPublic: nonPublic, ignoreCase: ignoreCase, strict: strict);
        }

        /// <summary>
        /// Gets a configuration section
        /// </summary>
        /// <typeparam name="T">A runtime reference type</typeparam>
        /// <param name="config">An <c>IConfiguration</c> service / instance.</param>
        /// <param name="path">A configuration path</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key is not found, default is <c>false</c>.</param>
        /// <returns>An instance of <c>T</c> loaded from a config section</returns>
        /// <exception cref="NoConfigurationServiceException"></exception>
        /// <exception cref="ConfigurationException"></exception>
        public static T ParseSection<T>(this IConfiguration config, string path, bool required = false) where T : class
        {
            // validation
            if (config == null)
                throw new NoConfigurationServiceException();

            var section = config.GetSection(path);
            if (!section.Exists())
            {
                if (required) 
                    throw new ConfigurationException("Required configuration section not found: " + path);
                return null;
            }
            var t = ConfigUtil.GetInstance<T>();
            section.Bind(t);
            return t;
        }

        /// <summary>
        /// Gets a configuration value array
        /// </summary>
        /// <typeparam name="T">subclass of <c>System.Configuration.ConfigurationSection</c></typeparam>
        /// <param name="config">An <c>IConfiguration</c> service / instance.</param>
        /// <param name="path">A configuration path</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key is not found, default is <c>false</c>.</param>
        /// <returns>An array</returns>
        /// <exception cref="NoConfigurationServiceException"></exception>
        /// <exception cref="ConfigurationException"></exception>
        public static T[] GetArray<T>(this IConfiguration config, string path, bool required = false)
        {
            // validation
            if (config == null)
                throw new NoConfigurationServiceException();

            var section = config.GetSection(path);
            if (!section.Exists())
            {
                if (required)
                {
                    var foundMissingSection = false;
                    var sb = new StringBuilder();
                    var parts = path.Split(':');
                    foreach (var part in parts)
                    {
                        if (sb.Length > 0) 
                            sb.Append(":");
                        if (foundMissingSection || config.GetSection(sb + part).Exists())
                        {
                            sb.Append(part);
                        }
                        else
                        {
                            sb.Append("[").Append(part).Append("]");
                            foundMissingSection = true;
                        }
                    }
                    throw new ConfigurationException("Required configuration section not found: " + sb);
                }
                return null;
            }
            var array = section.Get<T[]>();  // binder
            return array;
        }

        /// <summary>
        /// Gets a configured connection string
        /// </summary>
        /// <param name="config">An <c>IConfiguration</c> service / instance.</param>
        /// <param name="name">A connection string name</param>
        /// <param name="required">If <c>true</c> throws an exception if connection string name not found, default is <c>false</c>.</param>
        /// <returns>A connection string</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NoConfigurationServiceException"></exception>
        /// <exception cref="ConfigurationException"></exception>
        public static string GetConnectionString(this IConfiguration config, string name, bool required = false)
        {
            // validation
            if (config == null)
                throw new NoConfigurationServiceException();
            if (string.IsNullOrWhiteSpace(name))
                throw new ConfigurationException(nameof(name) + " cannot be " + (name == null ? ConfigConstants.Null : (name.Length == 0 ? ConfigConstants.Empty : ConfigConstants.Whitespace)));
            
            var connectionStringSection = config.GetSection("ConnectionStrings");
            if (connectionStringSection != null)
            {
                var connectionString = connectionStringSection[name];
                if (string.IsNullOrWhiteSpace(connectionString) && required)
                    throw new ConfigurationException("Required connection string not supplied: " + name);
                return connectionString;
            }
            else if (required)
            {
                throw new ConfigurationException("Required configuration section not found: ConnectionStrings");
            }
            return null;
        }
    }
}
