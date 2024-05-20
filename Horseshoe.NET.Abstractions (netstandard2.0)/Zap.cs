using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET
{
    /// <summary>
    /// Factory methods for converting objects and strings.  'Zap' means converting blank values to <c>null</c>.
    /// </summary>
    public static class Zap
    {
        /// <summary>
        /// Converts <c>obj</c> for nullness (includes <c>DBNull</c>). If <c>obj</c> is a <c>string</c>, 
        /// then <c>string</c> conditions apply.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <returns>The source <c>object</c>, <c>null</c> or (in the case <c>obj</c> is a <c>string</c>) a zapped <c>string</c>.</returns>
        /// <seealso cref="String"/>
        public static object Object(object obj)
        {
            if (obj == null || obj is DBNull) return null;
            if (obj is string stringValue) return _String(stringValue);
            if (obj is StringValues stringValuesValue)
            {
                switch (stringValuesValue.Count)
                {
                    case 0: return null;
                    case 1: return _String(stringValuesValue.Single());
                    default: return string.Join(", ", stringValuesValue.Select(s => _String(s)));
                };
            }
            return obj;
        }

        /// <summary>
        /// Trims the whitespaces off a <c>string</c>'s edges and if the result is zero-length returns <c>null</c>.
        /// </summary>
        /// <param name="obj">A <c>string</c> or <c>object</c> to evaluate.</param>
        /// <returns>The source <c>object</c> to a <c>string</c> or <c>null</c>.</returns>
        public static string String(object obj)
        {
            if ((obj = Object(obj)) == null)
                return null;

            return _String
            (
                obj is string stringValue
                    ? stringValue
                    : obj.ToString()
            );
        }

        private static string _String(string stringValue)
        {
            var trimmed = stringValue.Trim();
            if (trimmed.Length == 0)
                return null;
            return trimmed;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>bool</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>bool</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>false</c>.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        /// <exception cref="ConversionException"></exception>
        public static bool Bool(object obj, bool defaultValue = false, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return NBool(obj, trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, ignoreCase: ignoreCase, treatArbitraryAsFalse: treatArbitraryAsFalse) ?? defaultValue;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>bool</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        /// <exception cref="ConversionException"></exception>
        public static bool? NBool(object obj, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            if ((obj = Object(obj)) == null) return null;
            if (obj is bool boolValue) return boolValue;
            if (obj is byte byteValue) return byteValue == 1;
            if (obj is short shortValue) return shortValue == 1;
            if (obj is int intValue) return intValue == 1;
            if (obj is long longValue) return longValue == 1L;
            if (obj is decimal decimalValue) return decimalValue == 1M;
            if (obj is float floatValue) return floatValue == 1F;
            if (obj is double doubleValue) return doubleValue == 1D;
            if (obj is string stringValue)
            {
                if (ignoreCase ? stringValue.InIgnoreCase(trueValues.Split(delimiter)) : stringValue.In(trueValues.Split(delimiter)))
                    return true;
                if (treatArbitraryAsFalse || (ignoreCase ? stringValue.InIgnoreCase(falseValues.Split(delimiter)) : stringValue.In(falseValues.Split(delimiter))))
                    return false;
                throw new ConversionException("Cannot convert \"" + stringValue + "\" to bool");
            }
            throw new ConversionException("Cannot convert object of type \"" + obj.GetType().Name + "\" to bool");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>byte</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>bool</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>byte</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static byte Byte(object obj, byte defaultValue = default, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is byte byteValue) return byteValue;
            if (obj is bool boolValue) return (byte)(boolValue ? 1 : 0);
            if (obj is short shortValue) return NumberUtil.EvalAsByte(shortValue, force: force);
            if (obj is int intValue) return NumberUtil.EvalAsByte(intValue, force: force);
            if (obj is long longValue) return NumberUtil.EvalAsByte(longValue, force: force);
            if (obj is decimal decimalValue) return NumberUtil.EvalAsByte(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsByte(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsByte(doubleValue, force: force);

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (byte.TryParse(stringValue, numberStyle ?? default, provider, out byteValue))
                {
                    return byteValue;
                }
            }
            else if (byte.TryParse(stringValue, out byteValue))
            {
                return byteValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(byte).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;byte&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;byte&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static byte? NByte(object obj, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Byte(obj, force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>byte</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="encoding">An optional text encoding, e.g. UTF8.</param>
        /// <returns>A <c>byte</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static byte[] Bytes(object obj, Encoding encoding = null)
        {
            obj = Object(obj);
            if (obj == null) return null;
            if (obj is byte[] bytes) return bytes;
            if (obj is byte byteValue) return new[] { byteValue };
            if (obj is string stringValue) return (encoding ?? Encoding.Default).GetBytes(stringValue);
            throw new ConversionException($"Cannot convert {obj.GetType().FullName} to {typeof(byte[]).FullName}");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>short</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>short</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>short</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static short Short(object obj, short defaultValue = 0, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is short shortValue) return shortValue;
            if (obj is bool boolValue) return (short)(boolValue ? 1 : 0);
            if (obj is byte byteValue) return byteValue;
            if (obj is int intValue) return NumberUtil.EvalAsShort(intValue, force: force);
            if (obj is long longValue) return NumberUtil.EvalAsShort(longValue, force: force);
            if (obj is decimal decimalValue) return NumberUtil.EvalAsShort(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsShort(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsShort(doubleValue, force: force);

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (short.TryParse(stringValue, numberStyle ?? default, provider, out shortValue))
                {
                    return shortValue;
                }
            }
            else if (short.TryParse(stringValue, out shortValue))
            {
                return shortValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(short).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;short&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;short&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static short? NShort(object obj, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Short(obj, force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> as an <c>int</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>int</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>An <c>int</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static int Int(object obj, int defaultValue = 0, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is int intValue) return intValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is long longValue) return NumberUtil.EvalAsInt(longValue, force: force);
            if (obj is decimal decimalValue) return NumberUtil.EvalAsInt(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsInt(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsInt(doubleValue, force: force);

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (int.TryParse(stringValue, numberStyle ?? default, provider, out intValue))
                {
                    return intValue;
                }
            }
            else if (int.TryParse(stringValue, out intValue))
            {
                return intValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(int).FullName }");
        }


        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;int&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;int&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static int? NInt(object obj, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Int(obj, force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>long</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>long</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>long</c> to <c>long</c> regardless if the value is greater than the max value of <c>long</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>long</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static long Long(object obj, long defaultValue = 0, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is long longValue) return longValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is decimal decimalValue) return NumberUtil.EvalAsLong(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsLong(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsLong(doubleValue, force: force);

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (long.TryParse(stringValue, numberStyle ?? default, provider, out longValue))
                {
                    return longValue;
                }
            }
            else if (long.TryParse(stringValue, out longValue))
            {
                return longValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(long).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;long&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>long</c> to <c>long</c> regardless if the value is greater than the max value of <c>long</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;long&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static long? NLong(object obj, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Long(obj, force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>decimal</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>decimal</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0.0</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>decimal</c> to <c>decimal</c> regardless if the value is greater than the max value of <c>decimal</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>decimal</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static decimal Decimal(object obj, decimal defaultValue = 0, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is decimal decimalValue) return decimalValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is float floatValue) return NumberUtil.EvalAsDecimal(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsDecimal(doubleValue, force: force);

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (decimal.TryParse(stringValue, numberStyle ?? default, provider, out decimalValue))
                {
                    return decimalValue;
                }
            }
            else if (decimal.TryParse(stringValue, out decimalValue))
            {
                return decimalValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(decimal).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;decimal&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>decimal</c> to <c>decimal</c> regardless if the value is greater than the max value of <c>decimal</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;decimal&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static decimal? NDecimal(object obj, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Decimal(obj, force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>float</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>float</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0.0</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>float</c> to <c>float</c> regardless if the value is greater than the max value of <c>float</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>float</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static float Float(object obj, float defaultValue = 0, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is float floatValue) return floatValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return (float)decimalValue;
            if (obj is double doubleValue) return NumberUtil.EvalAsFloat(doubleValue, force: force);

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (float.TryParse(stringValue, numberStyle ?? default, provider, out floatValue))
                {
                    return floatValue;
                }
            }
            else if (float.TryParse(stringValue, out floatValue))
            {
                return floatValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(float).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;float&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>float</c> to <c>float</c> regardless if the value is greater than the max value of <c>float</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;float&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static float? NFloat(object obj, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Float(obj, force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>double</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>double</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0.0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>double</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static double Double(object obj, double defaultValue = 0, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is double doubleValue) return doubleValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return (double)decimalValue;
            if (obj is float floatValue) return floatValue;

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (stringValue.StartsWith("[hex]"))
            {
                stringValue = stringValue.Substring(5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.EndsWith("[hex]"))
            {
                stringValue = stringValue.Substring(0, stringValue.Length - 5);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }
            else if (stringValue.StartsWith("0x"))
            {
                stringValue = stringValue.Substring(2);
                numberStyle = (numberStyle ?? NumberStyles.None) | NumberStyles.HexNumber;
            }

            if (numberStyle.HasValue || provider != null)
            {
                if (double.TryParse(stringValue, numberStyle ?? default, provider, out doubleValue))
                {
                    return doubleValue;
                }
            }
            else if (double.TryParse(stringValue, out doubleValue))
            {
                return doubleValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(double).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;double&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;double&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static double? NDouble(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return Double(obj, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>DateTime</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>DateTime</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>1/1/0001</c>.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>DateTime</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static DateTime DateTime(object obj, DateTime defaultValue = default, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = Object(obj);
            if (obj == null) return defaultValue;
            if (obj is DateTime dateTimeValue) return dateTimeValue;
            if (obj is int intValue) return new DateTime(intValue);     // ticks
            if (obj is long longValue) return new DateTime(longValue);  // ticks

            provider = TextUtilAbstractions.GetProvider(provider, locale);
            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (provider != null || dateTimeStyle.HasValue)
            {
                if (System.DateTime.TryParse(stringValue, provider, dateTimeStyle ?? default, out dateTimeValue))
                {
                    return dateTimeValue;
                }
            }
            else if (System.DateTime.TryParse(stringValue, out dateTimeValue))
            {
                return dateTimeValue;
            }
            throw new ConversionException($"Cannot convert { obj.GetType().FullName } \"{ stringValue }\" to { typeof(DateTime).FullName }");
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;DateTime&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;DateTime&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static DateTime? NDateTime(object obj, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            if (Object(obj) == null) 
                return null;
            return DateTime(obj, dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> as an <c>enum</c> of <c>T</c>.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>enum</c> of <c>T</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the <c>enum</c> value if it is a <c>string</c>, default is <c>false</c>.</param>
        /// <returns>An <c>enum</c> of <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static T Enum<T>(object obj, T defaultValue = default, bool ignoreCase = false) where T : struct
        {
            return NEnum<T>(obj, ignoreCase: ignoreCase) ?? defaultValue;
        }

        /// <summary>
        /// Converts <c>obj</c> as a nullable <c>enum</c> of <c>T</c>.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="obj">An object to convert.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the <c>enum</c> value if it is a <c>string</c>, default is <c>false</c>.</param>
        /// <returns>A nullable <c>enum</c> of <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static T? NEnum<T>(object obj, bool ignoreCase = false) where T : struct
        {
            obj = NEnumOf(typeof(T), obj, ignoreCase: ignoreCase);
            if (obj == null)
                return null;
            return (T)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> as an <c>enum</c>.
        /// </summary>
        /// <param name="type">An enum type.</param>
        /// <param name="obj">An object to convert.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the <c>enum</c> value if it is a <c>string</c>, default is <c>false</c>.</param>
        /// <returns>An <c>enum</c> of <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static object EnumOf(Type type, object obj, bool ignoreCase = false)
        {
            if (type.IsEnum)
            {
                try
                {
                    if (obj is string stringValue)
                        return System.Enum.Parse(type, TextCleanAbstractions.RemoveAllWhitespace(stringValue), ignoreCase);
                    return System.Enum.ToObject(type, obj);
                }
                catch (Exception ex)
                {
                    throw new ConversionException("Cannot convert \"" + obj + "\" to enum: " + type.FullName, ex);
                }
            }
            throw new ConversionException("Not an enum type: " + type.FullName);
        }

        /// <summary>
        /// Converts <c>obj</c> as a nullable <c>enum</c>.
        /// </summary>
        /// <param name="type">An enum type.</param>
        /// <param name="obj">An object to convert.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the <c>enum</c> value if it is a <c>string</c>, default is <c>false</c>.</param>
        /// <returns>A nullable <c>enum</c> of <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static object NEnumOf(Type type, object obj, bool ignoreCase = false)
        {
            if ((obj = Object(obj)) == null)
                return null;
            if (type.IsEnum)
            {
                try
                {
                    if (obj is string stringValue)
                        return System.Enum.Parse(type, TextCleanAbstractions.RemoveAllWhitespace(stringValue), ignoreCase);
                    return System.Enum.ToObject(type, obj);
                }
                catch (Exception ex)
                {
                    throw new ConversionException("Cannot convert \"" + obj + "\" to enum: " + type.FullName, ex);
                }
            }
            throw new ConversionException("Not an enum type: " + type.FullName);
        }

        /// <summary>
        /// Converts <c>obj</c> to a runtime type.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>obj</c> if it is a <c>string</c> type name, default is <c>false</c>.</param>
        /// <returns>A runtime type.</returns>
        /// <exception cref="ConversionException"></exception>
        public static Type Type(object obj, Type inheritedType = null, bool ignoreCase = false)
        {
            if ((obj = Object(obj)) == null)
                return null;
            var type = obj is Type _type
                ? _type
                : (obj is string typeName ? TypeUtilAbstractions.GetType(typeName, ignoreCase: ignoreCase) : throw new ConversionException("Cannot convert \"" + obj + "\" to type."));
            if (inheritedType != null)
            {
                if (inheritedType.IsAssignableFrom(type)) 
                    return type;
                throw new ConversionException("Cannot convert \"" + type.FullName + "\" to \"" + inheritedType.FullName + "\".");
            }
            return type;
        }

        /// <summary>
        /// Converts <c>obj</c> to the supplied type.  Methods include type compatibility checking and string parsing.  
        /// Note, built-in converters only exist for basic types (such as int, double, date/time, etc.) and enums.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="obj">An object to convert.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">Applies to <c>To&lt;Type&gt;()</c>. A constraint, the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <returns>The value of <c>obj</c> converted to <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static T To<T>
        (
            object obj,
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
            return (T)To(typeof(T), obj, dateTimeStyle: dateTimeStyle, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, inheritedType: inheritedType, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Converts <c>obj</c> to the supplied type.  Methods include type compatibility checking and string parsing.  
        /// If <c>obj</c> is already assignable from the supplied type it is returned without conversion and will need to be cast.
        /// Note, built-in converters only exist for basic types (such as int, double, date/time etc.) and enums.
        /// </summary>
        /// <param name="type">The type to convert <c>obj</c> to.</param>
        /// <param name="obj">An object to convert.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <returns>The value of <c>obj</c> converted to <c>type</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static object To
        (
            Type type, 
            object obj,
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
            // null check
            if (Object(obj) == null)
                return TypeUtilAbstractions.GetDefaultValue(type);

            // same /assignable type check
            // PASS: SymmetricAlgorithm <- TripleDES, double? <- double
            // FAIL: TripleDES <- SymmetricAlgorithm, double <- double?
            if (type.IsAssignableFrom(obj.GetType()))
                return obj;

            // same /assignable type check
            // was PASS: double? <- double    >>>>   PASS: double? <- double
            // was FAIL: double  <- double?   >>>>   PASS: double  <- double  **<- underlying type**
            if (Nullable.GetUnderlyingType(obj.GetType()) is Type uoType)
                if (type.IsAssignableFrom(uoType))
                    return obj;

            // use built-in converter for basic reference types
            if (type == typeof(string)) return String(obj);
            if (type == typeof(Type)) return Type(obj, inheritedType: inheritedType, ignoreCase: ignoreCase);

            // other reference types
            if (!type.IsValueType)
                throw new ConversionException("No built-in converter exists for this reference type: " + type.FullName);

            // convert value type to non-nullable to simplify below
            if (Nullable.GetUnderlyingType(type) is Type uType)
                type = uType;

            // use built-in converter for basic value types and enums
            if (type.IsEnum) return EnumOf(type, obj, ignoreCase: ignoreCase);
            if (type == typeof(byte)) return Byte(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(byte[])) return Bytes(obj, encoding: encoding);
            if (type == typeof(short)) return Short(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(int)) return Int(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(long)) return Long(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(decimal)) return Decimal(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(float)) return Float(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(double)) return Double(obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (type == typeof(bool)) return Bool(obj, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase);
            if (type == typeof(DateTime)) return DateTime(obj, dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
      
            // other value types
            throw new ConversionException("No built-in converter exists for this value type: " + type.FullName);
        }

        /// <summary>
        /// Zaps a collection of <c>string</c>s and optionally prunes out the <c>null</c>s.
        /// </summary>
        /// <param name="collection">A collection of <c>string</c>s.</param>
        /// <param name="prunePolicy">Dictates how to treat <c>null</c> items when zapping a collection.</param>
        /// <returns>A collection of zapped strings.</returns>
        public static IEnumerable<string> Strings(IEnumerable<string> collection, PrunePolicy prunePolicy = default)
        {
            if (collection == null)
                return Array.Empty<string>();

            if (!(collection is IList<string> list))
                list = new List<string>(collection);

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = String(list[i]);
            }

            if (list.Any(s => s == null))
            {
                if ((prunePolicy & PrunePolicy.All) == PrunePolicy.All)
                {
                    return list
                        .Where(s => s != null)
                        .ToList();
                }

                if ((prunePolicy & PrunePolicy.Leading) == PrunePolicy.Leading)
                {
                    while (list.Any() && list[0] == null)
                        list.RemoveAt(0);
                }

                if ((prunePolicy & PrunePolicy.Trailing) == PrunePolicy.Trailing)
                {
                    while (list.Any() && list.Last() == null)
                        list.RemoveAt(list.Count - 1);
                }
            }
            return list;
        }

        /// <summary>
        /// Zaps an array of <c>string</c>s and optionally prunes out the <c>null</c>s.
        /// </summary>
        /// <param name="array">An array of <c>string</c>s.</param>
        /// <param name="prunePolicy">Dictates how to treat <c>null</c> items when zapping a collection.</param>
        /// <returns>An array of zapped strings.</returns>
        public static string[] Strings(string[] array, PrunePolicy prunePolicy = default)
        {
            var strings = Strings(array as IEnumerable<string>, prunePolicy: prunePolicy);
            return strings.ToArray();
        }

        /// <summary>
        /// Zaps an <c>object</c>'s <c>string</c> properties.
        /// </summary>
        /// <param name="obj">An <c>object</c> whose <c>string</c> properties to zap.</param>
        /// <param name="bindingFlags">Flags for indicating which properties to zap.</param>
        public static void StringProperties(object obj, BindingFlags? bindingFlags = null)
        {
            if (obj != null)
            {
                var propValues = 
                (
                    bindingFlags.HasValue
                        ? obj.GetType().GetProperties(bindingFlags.Value).Select(p => new PropertyValue(p, p.GetValue(obj)))
                        : ObjectUtilAbstractions.GetInstancePropertyValues(obj)
                ).Where(pv => pv.Property.PropertyType == typeof(string));
                foreach (var pv in propValues)
                {
                    pv.Property.SetValue(obj, String(pv.Value));
                }
            }
        }

        /// <summary>
        /// Zaps an <c>object</c>'s <c>string</c> properties.
        /// </summary>
        /// <typeparam name="T">A reference type.</typeparam>
        /// <param name="objs">A collection of <c>object</c>s whose <c>string</c> properties to zap.</param>
        /// <param name="bindingFlags">Flags for indicating which properties to zap.</param>
        public static void StringProperties<T>(IEnumerable<T> objs, BindingFlags? bindingFlags = null) where T : class
        {
            if (objs == null)
                return;
            foreach (object obj in objs)
            {
                StringProperties(obj, bindingFlags: bindingFlags);
            }
        }

        /// <summary>
        /// Zaps a <c>Types</c>'s <c>string</c> properties.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <param name="bindingFlags">Flags for indicating which properties to zap.</param>
        public static void StaticStringProperties(Type type, BindingFlags? bindingFlags = null)
        {
            var propValues = 
            (
                bindingFlags.HasValue
                ? type.GetProperties(bindingFlags.Value).OfPropertyType<string>().Select(p => new PropertyValue(p, p.GetValue(null)))
                : TypeUtilAbstractions.GetStaticPropertyValues(type)
            ).Where(pv => pv.Property.PropertyType == typeof(string));
            foreach (var pv in propValues)
            {
                pv.Property.SetValue(null, String(pv.Value));
            }
        }

        /// <summary>
        /// Zaps a <c>Types</c>'s <c>string</c> properties.
        /// </summary>
        /// <typeparam name="T">A type.</typeparam>
        /// <param name="bindingFlags">Flags for indicating which properties to zap.</param>
        public static void StaticStringProperties<T>(BindingFlags? bindingFlags = null) where T : class
        {
            var propValues = 
            (
                bindingFlags.HasValue
                ? typeof(T).GetProperties(bindingFlags.Value).Select(p => new PropertyValue(p, p.GetValue(null)))
                : TypeUtilAbstractions.GetStaticPropertyValues<T>()
            ).Where(pv => pv.Property.PropertyType == typeof(string));
            foreach (var pv in propValues)
            {
                pv.Property.SetValue(null, String(pv.Value));
            }
        }
    }
}
