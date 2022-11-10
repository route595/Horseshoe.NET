using System;
using System.Globalization;
using System.Text;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET
{
    public static class EnvVar
    {
        /// <summary>
        /// Tests if an environment variable exists
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <returns>bool</returns>
        public static bool Has(string varName)
        {
            return Get(varName, required: false) != null;
        }

        /// <summary>
        /// Gets an environment variable
        /// </summary>
        /// <param name="varName">enfironment variable name</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
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
        /// <typeparam name="T">reference type</typeparam>
        /// <param name="varName">environment variable name</param>
        /// <param name="parseFunc">parsing function</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <returns></returns>
        public static T Get<T>(string varName, Func<string, T> parseFunc = null, bool required = false) where T : class
        {
            var value = Get(varName, required: required);
            if (value == null) return null;
            if (parseFunc != null) return parseFunc.Invoke(value);
            try
            {
                return ObjectUtil.GetInstance<T>(value);
            }
            catch (Exception ex)
            {
                throw new UtilityException("Cannot convert " + value + " to " + typeof(T).FullName, ex);
            }
        }

        /// <summary>
        /// Gets an environment variable as a <c>byte</c>.  This method understands value decorations (i.e. "2f[hex]").
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="defaultValue">returns this value if <c>required == false</c> and environment variable is not found, default is 0</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="numberStyles">if supplied, dictates the expected number format</param>
        /// <param name="provider">if supplied, dictates the expected number format provider</param>
        /// <returns>a <c>byte</c></returns>
        public static byte GetByte(string varName, byte defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return GetNByte(varName, required: required, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        /// <summary>
        /// Gets an environment variable as a <c>Nullable byte</c>.  This method understands value decorations (i.e. "2f[hex]").
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="numberStyles">if supplied, dictates the expected number format</param>
        /// <param name="provider">if supplied, dictates the expected number format provider</param>
        /// <returns>a <c>Nullable byte</c></returns>
        public static byte? GetNByte(string varName, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Get(varName, required: required) is string stringValue)
            {
                if (stringValue.EndsWith("[hex]"))
                {
                    stringValue = stringValue.Substring(0, stringValue.Length - 5);
                    numberStyles = numberStyles ?? NumberStyles.HexNumber;
                }
                return Zap.NByte(stringValue, numberStyles: numberStyles, provider: provider);
            }
            return null;
        }

        /// <summary>
        /// Gets an environment variable as a <c>byte[]</c>.
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="encoding">if supplied, dictates the expected byte encoding</param>
        /// <returns>a <c>byte[]</c></returns>
        public static byte[] GetBytes(string varName, bool required = false, Encoding encoding = default)
        {
            var value = Get(varName, required: required);
            if (value == null) return null;
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// Gets an environment variable as an <c>int</c>.  This method understands value decorations (i.e. "2f[hex]").
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="defaultValue">returns this value if <c>required == false</c> and environment variable is not found, default is 0</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="numberStyles">if supplied, dictates the expected number format</param>
        /// <param name="provider">if supplied, dictates the expected number format provider</param>
        /// <returns>an <c>int</c></returns>
        public static int GetInt(string varName, int defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return GetNInt(varName, required: required, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        /// <summary>
        /// Gets an environment variable as a <c>Nullable int</c>.  This method understands value decorations (i.e. "2f[hex]").
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="numberStyles">if supplied, dictates the expected number format</param>
        /// <param name="provider">if supplied, dictates the expected number format provider</param>
        /// <returns>a <c>Nullable int</c></returns>
        public static int? GetNInt(string varName, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Get(varName, required: required) is string stringValue)
            {
                if (stringValue.EndsWith("[hex]"))
                {
                    stringValue = stringValue.Substring(0, stringValue.Length - 5);
                    numberStyles = numberStyles ?? NumberStyles.HexNumber;
                }
                return Zap.NInt(stringValue, numberStyles: numberStyles, provider: provider);
            }
            return null;
        }

        /// <summary>
        /// Gets an environment variable as a <c>bool</c>.
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="defaultValue">returns this value if <c>required == false</c> and environment variable is not found, default is <c>false</c></param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <returns>a <c>bool</c></returns>
        public static bool GetBool(string varName, bool defaultValue = false, bool required = false)
        {
            var value = Get(varName, required: required);
            return Zap.Bool(value, defaultValue: defaultValue);
        }

        /// <summary>
        /// Gets an environment variable as a <c>Nullable bool</c>.
        /// </summary>
        /// <param name="varName">environment variable name</param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <returns>a <c>Nullable bool</c></returns>
        public static bool? GetNBool(string varName, bool required = false)
        {
            var value = Get(varName, required: required);
            return Zap.NBool(value);
        }

        /// <summary>
        /// Gets an environment variable as an <c>enum</c>.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="varName">environment variable name</param>
        /// <param name="defaultValue">returns this value if <c>required == false</c> and environment variable is not found, or if <c>suppressErrors == true</c> and a conversion error occurs</param>
        /// <param name="ignoreCase"></param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="suppressErrors">if true, ignores errors related to converting to <c>enum</c> and returns the default</param>
        /// <returns>an <c>enum</c></returns>
        public static T GetEnum<T>(string varName, T defaultValue = default, bool ignoreCase = false, bool required = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(varName, required: required);
            return Zap.Enum<T>(value, defaultValue: defaultValue, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        /// <summary>
        /// Gets an environment variable as a <c>Nullable enum</c>.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="varName">environment variable name</param>
        /// <param name="ignoreCase"></param>
        /// <param name="required">if true, throws error if environment variable not found</param>
        /// <param name="suppressErrors">if true, ignores errors related to converting to <c>enum</c> and returns the default</param>
        /// <returns>a <c>Nullable enum</c></returns>
        public static T? GetNEnum<T>(string varName, bool ignoreCase = false, bool required = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(varName, required: required);
            return Zap.NEnum<T>(value, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }
    }
}