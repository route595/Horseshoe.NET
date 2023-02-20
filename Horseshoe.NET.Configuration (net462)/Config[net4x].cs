using System;
using System.Configuration;
using System.Globalization;
using System.Text;

using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.Configuration
{
    public static class Config
    {
        /// <summary>
        /// Tests if a configuration value exists.
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool Has(string key)
        {
            return Get(key, required: false) != null;
        }

        /// <summary>
        /// Gets an app setting from configuration.
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <returns>A <c>string</c> value.</returns>
        public static string Get(string key, bool required = false)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null && required)
            {
                throw new ConfigurationException("Required configuration not found: " + key);
            }
            return value;
        }

        /// <summary>
        /// Gets an app setting from configuration and parses it to a <c>string[]</c>.
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, bool required = false)
        {
            return GetArray(key, delimiter: new[] { ',' }, required: required);
        }

        /// <summary>
        /// Gets an app setting from configuration and parses it to a <c>string[]</c>
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, char delimiter, bool required = false)
        {
            var rawArray = Get(key, required: required);
            if (rawArray == null)
                return Array.Empty<string>();
            return Zap.Strings(rawArray.Split(delimiter));
        }

        /// <summary>
        /// Gets an app setting from configuration and parses it to a <c>string[]</c>
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, char[] delimiter, bool required = false)
        {
            var rawArray = Get(key, required: required);
            if (rawArray == null)
                return Array.Empty<string>();
            return Zap.Strings(rawArray.Split(delimiter));
        }

        /// <summary>
        /// Gets an app setting from configuration as an instance of a fully qualified class name (expected value).
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <param name="strict">If a <c>Type</c> matching <c>className</c> cannot be found then <c>strict == true</c> causes an exception to be thrown, default is <c>false</c>.</param>
        /// <returns></returns>
        public static object GetInstance(string key, bool required = false, bool strict = false)
        {
            var className = Get(key, required: required);
            if (className == null)
                return null;
            return TypeUtil.GetInstance(className, strict: strict);
        }

        /// <summary>
        /// Gets an app setting from configuration as an instance of the specified type.  
        /// You may need to supply a <c>parseFunc</c> for custom / complex types.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="key">A configuration key.</param>
        /// <param name="parseFunc">A custom parsing function.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
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
            string key, 
            Func<string, T> parseFunc = null, 
            bool required = false,
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
            var value = Get(key, required: required);
            if (value == null)
                return default;
            if (parseFunc != null)
                return parseFunc.Invoke(value);
            return Zap.To<T>(value, dateTimeStyle: dateTimeStyle, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, inheritedType: inheritedType, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Gets a configuration section and parsed it to an object.
        /// </summary>
        /// <typeparam name="T">A subclass of <c>System.Configuration.ConfigurationSection</c>.</typeparam>
        /// <param name="path">The path of the section in the configuration hierarchy.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <returns>A config section parsed to an object.</returns>
        public static T ParseSection<T>(string path, bool required = false) where T : ConfigurationSection
        {
            var collection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection(path) as T;
            if (collection == null)
            {
                if (required)
                {
                    throw new ConfigurationException("Required configuration section not found: " + path);
                }
                return null;
            }
            return collection;
        }

        /// <summary>
        /// Gets a configured connection string.
        /// </summary>
        /// <param name="name">A connection string name.</param>
        /// <param name="required">If <c>true</c>, throws error if configuration key not found.</param>
        /// <returns>A connection string.</returns>
        public static string GetConnectionString(string name, bool required = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (required)
                    throw new ConfigurationException("Connection string name required but not supplied");
                return null;
            }
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString == null)
            {
                if (required)
                {
                    throw new ConfigurationException("Connection string not found: " + name);
                }
                return null;
            }
            return connectionString.ConnectionString;
        }
    }
}