using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// Tests if a configuration app settings value exists.
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool Has(string key)
        {
            return Get(key, required: false) != null;
        }

        /// <summary>
        /// Gets an app setting from configuration app settings.
        /// </summary>
        /// <param name="key">A configuration key.</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key not found, default is <c>false</c>.</param>
        /// <returns>A <c>string</c> value.</returns>
        public static string Get(string key, bool required = false)
        {
            // validation
            if (string.IsNullOrEmpty(key))
                throw new ConfigurationException("An invalid key was supplied: " + ConfigUtil.DisplayValue(key));

            var value = ConfigurationManager.AppSettings[key];
            if (value == null && required)
            {
                throw new ConfigurationException("Required configuration not found: " + ConfigUtil.DisplayValue(key));
            }
            return value;
        }

        /// <summary>
        /// <para>
        /// Gets an app setting from configuration as an instance of the specified type.  
        /// You can parse custom / complex types if you supply a <c>parser</c>.
        /// </para>
        /// <para>
        /// The following annotations can be used with their corresponding data types in lieu of number styles
        /// or to parse dates/times.  Non-date annotations (e.g. "[hex]") are always lower case.
        /// </para>
        /// <list type="bullet">
        /// <item><c>int</c>: [hex]</item>
        /// <item><c>DateTime</c>: [mm/yyyy], [HH:mm:ss], etc.</item>
        /// </list>
        /// Example
        /// <code>
        /// &lt;appSettings&gt;
        ///   &lt;add key="hash-salt[hex]" value="6f" /&gt;              -or-  &lt;add key="hash-salt" value="6f" /&gt;
        ///   &lt;add key="min-date[dd-MMM-yy]" value="30-JUL-22" /&gt;  -or-  &lt;add key="min-date" value="30-JUL-22" /&gt;
        ///   
        /// Config.Get&lt;int&gt;("hash-salt");     -or-  Config.Get&lt;int&gt;("hash-salt[hex]");
        /// Config.Get&lt;DateTime&gt;("min-date")  -or-  Config.Get&lt;DateTime&gt;("min-date[dd-MMM-yy]")
        /// 
        /// - equivalent to -
        /// 
        /// &lt;appSettings&gt;
        ///   &lt;add key="hash-salt" value="6f" /&gt;
        ///   &lt;add key="min-date" value="30-JUL-22" /&gt;
        /// 
        /// Config.Get&lt;int&gt;("hash-salt", numberStyle: NumberStyles.HexNumber);
        /// DateTime.ParseExact(Config.Get("min-date"), "dd-MMM-yy", CultureInfo.InvariantCulture);
        /// </code>
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="key">A configuration key.</param>
        /// <param name="parser">A custom parsing function.</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key not found, default is <c>false</c>.</param>
        /// <param name="numberStyle">Applies to <c>Get&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateTimeStyle">Applies to <c>Get&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="dateFormat">Applies to <c>Get&lt;[datetime]&gt;()</c>. If supplied, the date/time will be parsed from this format.</param>
        /// <param name="locale">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Get&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>Get&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <returns>A configuration value converted to an instance of <c>T</c>.</returns>
        public static T Get<T>
        (
            string key, 
            Func<string, T> parser = null, 
            bool required = false,
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

            var value = Get(key, required: required);

            // scenario 1 - null
            if (value == null)
                return default;

            // scenario 2 - user supplied parser
            if (parser != null)
                return parser.Invoke(value);

            // scenario 3 - use built-in conversions, if available
            // note: accepts values with annotations (see note at top of page)
            return ConfigUtil.ZapTo<T>(value, numberStyle: numberStyle, provider: provider, dateTimeStyle: dateTimeStyle, dateFormat: dateFormat, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Gets an app setting from configuration as an instance of a fully qualified class name (expected value).
        /// </summary>
        /// <param name="key">A configuration key whose value indicates a runtime type.</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key not found, default is <c>false</c>.</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <param name="strict">If <c>true</c> then not finding a <c>Type</c> matching the supplied name causes an exception to be thrown, default is <c>false</c>.</param>
        /// <param name="ignoreCase">If <c>true</c>, allows searching assemblies/types that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <returns>An instance of the specified type.</returns>
        public static object GetInstance(string key, bool required = false, bool nonPublic = false, bool strict = false, bool ignoreCase = false)
        {
            var className = Get(key, required: required || strict);
            if (className == null)
                return null;
            return ConfigUtil.GetInstance(className, nonPublic: nonPublic, strict: strict, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Gets a configuration section and parsed it to an object.
        /// </summary>
        /// <typeparam name="T">A subclass of <c>System.Configuration.ConfigurationSection</c>.</typeparam>
        /// <param name="path">The path of the section in the configuration hierarchy.</param>
        /// <param name="required">If <c>true</c> throws an exception if configuration key not found, default is <c>false</c>.</param>
        /// <returns>A config section parsed to an object.</returns>
        public static T ParseSection<T>(string path, bool required = false) where T : ConfigurationSection
        {
            var collection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection(path) as T;
            if (collection == null)
            {
                if (required)
                    throw new ConfigurationException("Required configuration section not found: " + path);
                return null;
            }
            return collection;
        }

        /// <summary>
        /// Gets a configured connection string.
        /// </summary>
        /// <param name="name">A connection string name.</param>
        /// <param name="required">If <c>true</c> throws an exception if connection string name not found, default is <c>false</c>.</param>
        /// <returns>A connection string.</returns>
        /// <exception cref="ConfigurationException"></exception>
        public static string GetConnectionString(string name, bool required = false)
        {
            // validation
            if (string.IsNullOrWhiteSpace(name))
                throw new ConfigurationException(nameof(name) + " cannot be " + (name == null ? ConfigConstants.Null : (name.Length == 0 ? ConfigConstants.Empty : ConfigConstants.Whitespace)));

            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString == null)
            {
                if (required)
                    throw new ConfigurationException("Required connection string not supplied: " + name);
                return null;
            }
            return connectionString.ConnectionString;
        }
    }
}