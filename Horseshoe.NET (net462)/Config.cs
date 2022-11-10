using System;
using System.Globalization;
using System.Text;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET
{
    internal static class _Config
    {
        internal static string Assembly => "Horseshoe.NET.Configuration";

        internal static string ClassName => "Config";

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <returns>configuration value</returns>
        public static string Get(string key)
        {
            var value = Lib.TryInvokeStaticMethod<string>(Assembly, null, ClassName, nameof(Get), out _, key, false);
            return value;
        }

        /// <summary>
        /// Gets a configuration value as an instance of the specified type.  By default, the 
        /// configuration value will be assumed to be a class name and an object of that type
        /// will be created.  Alternatively, the value can be an object representation.  To 
        /// hydrate an object representation into an instance you need to supply a <c>parseFunc</c>.
        /// </summary>
        /// <typeparam name="T">reference type</typeparam>
        /// <param name="key">configuration key</param>
        /// <param name="parseFunc">parsing function</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<string, T> parseFunc = null) where T : class
        {
            var value = Get(key);
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
        /// Gets a configuration value as a <c>Nullable byte</c>.  This method understands value decorations (i.e. "2f[hex]").
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="numberStyles">if supplied, dictates the expected number format</param>
        /// <param name="provider">if supplied, dictates the expected number format provider</param>
        /// <returns>a <c>Nullable byte</c></returns>
        public static byte? GetNByte(string key, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Get(key) is string stringValue)
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
        /// Gets a configuration value as a <c>byte[]</c>.
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="encoding">if supplied, dictates the expected byte encoding</param>
        /// <returns>a <c>byte[]</c></returns>
        public static byte[] GetBytes(string key, Encoding encoding = null)
        {
            var value = Get(key);
            if (value == null) return null;
            return (encoding ?? Encoding.Default).GetBytes(value);
        }

        /// <summary>
        /// Gets a configuration value as a <c>Nullable int</c>.  This method understands value decorations (i.e. "2f[hex]").
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="numberStyles">if supplied, dictates the expected number format</param>
        /// <param name="provider">if supplied, dictates the expected number format provider</param>
        /// <returns>a <c>Nullable int</c></returns>
        public static int? GetNInt(string key, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Get(key) is string stringValue)
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
        /// Gets a configuration value as a <c>bool</c>.
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <param name="defaultValue">returns this value if <c>required == false</c> and configuration value is not found, default is <c>false</c></param>
        /// <returns>a <c>bool</c></returns>
        public static bool GetBool(string key, bool defaultValue = false)
        {
            var value = Get(key);
            return Zap.Bool(value, defaultValue: defaultValue);
        }

        /// <summary>
        /// Gets a configuration value as a <c>Nullable bool</c>.
        /// </summary>
        /// <param name="key">configuration key</param>
        /// <returns>a <c>Nullable bool</c></returns>
        public static bool? GetNBool(string key)
        {
            var value = Get(key);
            return Zap.NBool(value);
        }

        /// <summary>
        /// Gets a configuration value as a <c>Nullable enum</c>.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="key">configuration key</param>
        /// <param name="ignoreCase"></param>
        /// <param name="suppressErrors">if true, ignores errors related to converting to <c>enum</c> and returns the default</param>
        /// <returns>a <c>Nullable enum</c></returns>
        public static T? GetNEnum<T>(string key, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(key);
            return Zap.NEnum<T>(value, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        /// <summary>
        /// Gets a configured connection string
        /// </summary>
        /// <param name="name">connection string name</param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            var value = Lib.TryInvokeStaticMethod<string>(Assembly, null, ClassName, nameof(GetConnectionString), out _, name, false);
            return value;
        }
    }
}