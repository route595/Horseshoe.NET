using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;

using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.Configuration
{
    public static class Extensions
    {
        /// <summary>
        /// Tests if a configuration value exists
        /// </summary>
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="key">configuration key</param>
        /// <returns>bool</returns>
        public static bool Has(this IConfiguration config, string key)
        {
            return Get(config, key, required: false) != null;
        }

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="key">configuration key</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or key is not found</param>
        /// <returns>configuration value</returns>
        public static string Get(this IConfiguration config, string key, bool required = false)
        {
            if (config == null)
            {
                if (!required) 
                    return null;
                throw new NoConfigurationException("Configuration is null");
            }
            var value = config[key];
            if (value == null && required)
            {
                throw new ConfigurationException("Required configuration not found: " + key);
            }
            return value;
        }

        /// <summary>
        /// Gets a configuration value as an instance of a fully qualified class name (expected value).
        /// </summary>
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="key">configuration key</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or key is not found</param>
        /// <param name="strict">If a <c>Type</c> matching <c>className</c> cannot be found then <c>strict == true</c> causes an exception to be thrown, default is <c>false</c>.</param>
        /// <returns></returns>
        public static object GetInstance(this IConfiguration config, string key, bool required = false, bool strict = false)
        {
            var className = Get(config, key, required: required);
            if (className == null)
                return null;
            return TypeUtil.GetInstance(className, strict: strict);
        }

        /// <summary>
        /// Gets a configuration value as an instance of the specified type.  
        /// You may need to supply a <c>parseFunc</c> for custom / complex types.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="config">An instance of <c>IConfiguration</c>.</param>
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
        /// <returns>A configuration value converted to an instance of <c>T</c>.</returns>
        public static T Get<T>
        (
            this IConfiguration config, 
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
            var value = Get(config, key, required: required);
            if (parseFunc != null)
                return parseFunc.Invoke(value);
            return Zap.To<T>(key, dateTimeStyle: dateTimeStyle, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, inheritedType: inheritedType, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Gets a configuration section
        /// </summary>
        /// <typeparam name="T">type parameter</typeparam>
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="path">configuration path</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or section is not found</param>
        /// <returns>instance loaded from config section</returns>
        public static T ParseSection<T>(this IConfiguration config, string path, bool required = false) where T : class
        {
            if (config == null)
            {
                if (!required) 
                    return null;
                throw new NoConfigurationException("Configuration is null");
            }
            var section = config.GetSection(path);
            if (!section.Exists())
            {
                if (!required) 
                    return null;
                throw new ConfigurationException("Required configuration section not found: " + path);
            }
            var t = TypeUtil.GetDefaultInstance<T>();
            section.Bind(t);  // binder
            return t;
        }

        /// <summary>
        /// Gets a configuration value array
        /// </summary>
        /// <typeparam name="T">subclass of <c>System.Configuration.ConfigurationSection</c></typeparam>
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="path">configuration path</param>
        /// <param name="filter">an item filter</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or section is not found</param>
        /// <returns></returns>
        public static T[] GetArray<T>(this IConfiguration config, string path, bool required = false)
        {
            if (config == null)
            {
                if (!required) 
                    return null;
                throw new NoConfigurationException("Configuration is null");
            }
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
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="name">connection string name</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or connection string is not found</param>
        /// <returns></returns>
        public static string GetConnectionString(this IConfiguration config, string name, bool required = false)
        {
            if (config == null)
            {
                if (!required) 
                    return null;
                throw new NoConfigurationException("Configuration is null");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid connection string name: " + (name == null ? "[null]" : "[blank]"));
            }
            var connectionString = config.GetSection("ConnectionStrings")[name];
            if (connectionString == null && required)
            {
                throw new ConfigurationException("Connection string not found: " + name);
            }
            return connectionString;
        }
    }
}
