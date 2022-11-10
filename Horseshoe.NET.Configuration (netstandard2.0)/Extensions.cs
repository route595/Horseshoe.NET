using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;

using Horseshoe.NET.Objects;

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
        /// <param name="suppressErrors">if <c>true</c>, suppresses any object instantiation errors</param>
        /// <returns></returns>
        public static object GetInstance(this IConfiguration config, string key, bool required = false, bool suppressErrors = false)
        {
            var className = Get(config, key, required: required);
            if (className == null)
                return null;
            return ObjectUtil.GetInstance(className, suppressErrors: suppressErrors);
        }

        /// <summary>
        /// Gets a configuration value as an instance of the specified type.  
        /// You may need to supply a <c>parseFunc</c> for custom / complex types.
        /// </summary>
        /// <typeparam name="T">type parameter</typeparam>
        /// <param name="config">An instance of <c>IConfiguration</c></param>
        /// <param name="key">configuration key</param>
        /// <param name="parseFunc">parsing function</param>
        /// <param name="numberStyles">Available for parsing numeric formats (e.g. hexadecimal)</param>
        /// <param name="dateTimeStyles">Available for parsing date time formats</param>
        /// <param name="provider">Available for parsing numeric or date time formats</param>
        /// <param name="ignoreCase">if <c>true</c>, allows case insensitive enum parsing</param>
        /// <param name="required">if <c>true</c>, throws error if configuration is null or key is not found</param>
        /// <param name="suppressErrors">if <c>true</c>, throws error if enum parse error occurs</param>
        /// <returns></returns>
        public static T Get<T>(this IConfiguration config, string key, Func<string, T> parseFunc = null, NumberStyles? numberStyles = null, DateTimeStyles? dateTimeStyles = null, IFormatProvider provider = null, bool ignoreCase = false, bool required = false, bool suppressErrors = false)
        {
            var value = Get(config, key, required: required);
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
            var t = ObjectUtil.GetDefaultInstance<T>();
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
