using System;
using System.Globalization;
using System.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// Factory methods for reading environment variables.
    /// </summary>
    public static class EnvVar
    {
        /// <summary>
        /// Tests if an environment variable exists.
        /// </summary>
        /// <param name="varName">An environment variable name.</param>
        /// <returns>bool</returns>
        public static bool Has(string varName)
        {
            return Get(varName, required: false) != null;
        }

        /// <summary>
        /// Gets an environment variable.
        /// </summary>
        /// <param name="varName">Environment variable name.</param>
        /// <param name="required">If <c>true</c>, throws error if environment variable is not found, default is <c>false</c>.</param>
        /// <returns>string or null</returns>
        public static string Get(string varName, bool required = false)
        {
            var value = Environment.GetEnvironmentVariable(varName);
            if (value == null && required)
            {
                throw new EnvironmentVariableNotFoundException(varName);
            }
            return value;
        }

        /// <summary>
        /// Gets an environment variable as an instance of the specified type.  By default, the 
        /// environment variable will be assumed to be a class name and an object of that type
        /// will be created.  Alternatively, the value can be an object representation.  To 
        /// hydrate an object representation into an instance you need to supply a <c>parseFunc</c>.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="varName">Environment variable name.</param>
        /// <param name="parseFunc">A parsing function.</param>
        /// <param name="required">If <c>true</c>, throws error if environment variable is not found, default is <c>false</c>.</param>
        /// <param name="numberStyle">Applies to <c>Get&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>Get&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Get&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Get&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>Get&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <returns>An instance of <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static T Get<T>
        (
            string varName, 
            Func<string, T> parseFunc = null, 
            bool required = false,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            bool ignoreCase = false
        )
        {
            var value = Get(varName, required: required);
            if (parseFunc != null)
                return parseFunc.Invoke(value);
            return Zap.To<T>(value, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, ignoreCase: ignoreCase);
        }
    }
}