using System;
using System.Configuration;
using System.Globalization;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Configuration
{
    public static class Config
    {
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
        /// Gets an app setting from configuration
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if true, throws error if configuration key not found</param>
        /// <returns>configuration value</returns>
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
        /// Gets an app setting from configuration and parses it to a <c>string[]</c>
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if true, throws error if configuration key not found</param>
        /// <returns>configuration value</returns>
        public static string[] GetArray(string key, bool required = false)
        {
            return GetArray(key, delimiter: new[] { ',' }, required: required);
        }

        /// <summary>
        /// Gets an app setting from configuration and parses it to a <c>string[]</c>
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
        /// Gets an app setting from configuration and parses it to a <c>string[]</c>
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
        /// Gets an app setting from configuration as an instance of a fully qualified class name (expected value).
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or key is not found</param>
        /// <param name="suppressErrors">if <c>true</c>, suppresses any object instantiation errors</param>
        /// <returns></returns>
        public static object GetInstance(string key, bool required = false, bool suppressErrors = false)
        {
            var className = Get(key, required: required);
            if (className == null)
                return null;
            return ObjectUtil.GetInstance(className, suppressErrors: suppressErrors);
        }

        /// <summary>
        /// Gets an app setting from configuration as an instance of the specified type.  
        /// You may need to supply a <c>parseFunc</c> for custom / complex types.
        /// </summary>
        /// <typeparam name="T">type parameter</typeparam>
        /// <param name="key">configuration key</param>
        /// <param name="parseFunc">parsing function</param>
        /// <param name="numberStyles">Available for parsing numeric formats (e.g. hexadecimal)</param>
        /// <param name="dateTimeStyles">Available for parsing date time formats</param>
        /// <param name="provider">Available for parsing numeric or date time formats</param>
        /// <param name="ignoreCase">if <c>true</c>, allows case insensitive enum parsing</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or key is not found</param>
        /// <param name="suppressErrors">if <c>true</c>, throws error if enum parse error occurs</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<string, T> parseFunc = null, NumberStyles? numberStyles = null, DateTimeStyles? dateTimeStyles = null, IFormatProvider provider = null, bool ignoreCase = false, bool required = false, bool suppressErrors = false)
        {
            var value = Get(key, required: required);
            if (value == null)
                return default;
            if (parseFunc == null)
                parseFunc = GetParser<T>(numberStyles, dateTimeStyles, provider, ignoreCase, suppressErrors);
            if (parseFunc != null)
                return parseFunc.Invoke(value);
            throw new UtilityException("Cannot convert " + value + " to " + typeof(T).FullName + ". Try supplying arg 'parseFunc'.");
        }

        static Func<string, T> GetParser<T>(NumberStyles? numberStyles, DateTimeStyles? dateTimeStyles, IFormatProvider provider, bool ignoreCase, bool suppressErrors)
        {
            if (typeof(T) == typeof(string))
                return (s) => (T)(object)Zap.String(s);
            if (typeof(T) == typeof(byte?))
                return (s) => (T)(object)Zap.NByte(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(byte))
                return (s) => (T)(object)Zap.Byte(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(short?))
                return (s) => (T)(object)Zap.NShort(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(short))
                return (s) => (T)(object)Zap.Short(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(int?))
                return (s) => (T)(object)Zap.NInt(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(int))
                return (s) => (T)(object)Zap.Int(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(long?))
                return (s) => (T)(object)Zap.NLong(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(long))
                return (s) => (T)(object)Zap.Long(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(float?))
                return (s) => (T)(object)Zap.NFloat(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(float))
                return (s) => (T)(object)Zap.Float(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(double?))
                return (s) => (T)(object)Zap.NDouble(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(double))
                return (s) => (T)(object)Zap.Double(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(decimal?))
                return (s) => (T)(object)Zap.NDecimal(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(decimal))
                return (s) => (T)(object)Zap.Decimal(s, numberStyles: numberStyles, provider: provider);
            if (typeof(T) == typeof(DateTime?))
                return (s) => (T)(object)Zap.NDateTime(s, dateTimeStyles: dateTimeStyles, provider: provider);
            if (typeof(T) == typeof(DateTime))
                return (s) => (T)(object)Zap.DateTime(s, dateTimeStyles: dateTimeStyles, provider: provider);
            if (typeof(T) == typeof(bool?))
                return (s) => (T)(object)Zap.NBool(s);
            if (typeof(T) == typeof(bool))
                return (s) => (T)(object)Zap.Bool(s);
            if (typeof(T).IsEnum)
                return (s) => (T)Zap.EnumOfType(typeof(T), s, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
            return null;
        }

        /// <summary>
        /// Gets a configuration section
        /// </summary>
        /// <typeparam name="T">subclass of <c>System.Configuration.ConfigurationSection</c></typeparam>
        /// <param name="path">the path of the section in the configuration hierarchy</param>
        /// <param name="required">if true, throws error if configuration section not found</param>
        /// <returns></returns>
        public static T GetSection<T>(string path, bool required = false) where T : ConfigurationSection
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
        /// Gets a configured connection string
        /// </summary>
        /// <param name="name">connection string name</param>
        /// <param name="required">if true, throws error if connection string not found</param>
        /// <returns></returns>
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