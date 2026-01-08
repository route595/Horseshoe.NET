using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ObjectsTypesAndValues;

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
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        /// <exception cref="ConversionException"></exception>
        public static bool Bool(object obj, bool defaultValue = false, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", bool ignoreCase = true)
        {
            return To(obj, defaultValue: defaultValue, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>bool</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        /// <exception cref="ConversionException"></exception>
        public static bool? NBool(object obj, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", bool ignoreCase = true)
        {
            obj = To(typeof(bool), obj, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase);
            if (obj == null)
                return null;
            return (bool)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>byte</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>bool</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>byte</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>byte</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static byte Byte(object obj, byte defaultValue = default, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;byte&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>byte</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>Nullable&lt;byte&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static byte? NByte(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            obj = To(typeof(byte), obj, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
            if (obj == null)
                return null;
            return (byte)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>byte[]</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="encoding">An optional text encoding, e.g. UTF8.</param>
        /// <returns>A <c>byte</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static byte[] Bytes(object obj, Encoding encoding = null)
        {
            return To<byte[]>(obj, encoding: encoding);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>short</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>short</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>short</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>short</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static short Short(object obj, short defaultValue = default, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;short&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>short</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>Nullable&lt;short&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static short? NShort(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            obj = To(typeof(short), obj, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
            if (obj == null)
                return null;
            return (short)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> as an <c>int</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>int</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>int</c>>.  Default is <c>false</c>.</param>
        /// <returns>An <c>int</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static int Int(object obj, int defaultValue = 0, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
        }


        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;int&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>int</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>Nullable&lt;int&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static int? NInt(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            obj = To(typeof(int), obj, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
            if (obj == null)
                return null;
            return (int)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>long</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>long</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>long</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>long</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static long Long(object obj, long defaultValue = default, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;long&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>long</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>Nullable&lt;long&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static long? NLong(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            obj = To(typeof(long), obj, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
            if (obj == null)
                return null;
            return (long)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>decimal</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>decimal</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0.0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>decimal</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>decimal</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static decimal Decimal(object obj, decimal defaultValue = default, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;decimal&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>decimal</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>Nullable&lt;decimal&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static decimal? NDecimal(object obj,NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            obj = To(typeof(decimal), obj, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
            if (obj == null)
                return null;
            return (decimal)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>float</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>float</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0.0</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>float</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>float</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static float Float(object obj, float defaultValue = default, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;float&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of <c>float</c>>.  Default is <c>false</c>.</param>
        /// <returns>A <c>Nullable&lt;float&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static float? NFloat(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null, bool strict = false)
        {
            obj = To(typeof(float), obj, numberStyle: numberStyle, provider: provider, locale: locale, strict: strict);
            if (obj == null)
                return null;
            return (float)obj;
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
        public static double Double(object obj, double defaultValue = default, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return To(obj, defaultValue: defaultValue, numberStyle: numberStyle, provider: provider, locale: locale);
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
        /// <exception cref="InvalidCastException"></exception>
        public static double? NDouble(object obj, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            obj = To(typeof(double), obj, numberStyle: numberStyle, provider: provider, locale: locale);
            if (obj == null)
                return null;
            return (double)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>DateTime</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>DateTime</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>1/1/0001</c>.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateFormat">An optional exact date format to use if parsing date/time from a <c>string</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>DateTime</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static DateTime DateTime(object obj, DateTime defaultValue = default, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string dateFormat = null, string locale = null)
        {
            return To(obj, defaultValue: defaultValue, dateTimeStyle: dateTimeStyle, provider: provider, dateFormat: dateFormat, locale: locale);
        }

        /// <summary>
        /// Converts <c>obj</c> to a <c>Nullable&lt;DateTime&gt;</c>.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateFormat">An optional exact date format to use if parsing date/time from a <c>string</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>A <c>Nullable&lt;DateTime&gt;</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static DateTime? NDateTime(object obj, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string dateFormat = null, string locale = null)
        {
            obj = To(typeof(DateTime), obj, dateTimeStyle: dateTimeStyle, provider: provider, dateFormat: dateFormat, locale: locale);
            if (obj == null)
                return null;
            return (DateTime)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to <c>enum</c> of type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="obj">An object to convert.</param>
        /// <param name="defaultValue">The <c>enum</c> of <c>T</c> to return if <c>obj</c> evaluates to <c>null</c>, default is <c>0</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the <c>enum</c> value if it is a <c>string</c>, default is <c>false</c>.</param>
        /// <returns>An <c>enum</c> of type <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        public static T Enum<T>(object obj, T defaultValue = default, bool ignoreCase = false) where T : struct, Enum
        {
            return To(obj, defaultValue: defaultValue, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Converts <c>obj</c> as a nullable <c>enum</c> of <c>T</c>.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="obj">An object to convert.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of the <c>enum</c> value if it is a <c>string</c>, default is <c>false</c>.</param>
        /// <returns>A nullable <c>enum</c> of <c>T</c>.</returns>
        /// <exception cref="ConversionException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static T? NEnum<T>(object obj, bool ignoreCase = false) where T : struct, Enum
        {
            obj = To(typeof(T), obj, ignoreCase: ignoreCase);
            if (obj == null)
                return null;
            return (T)obj;
        }

        /// <summary>
        /// Converts <c>obj</c> to the supplied type.  Methods include type compatibility checking and string parsing.  
        /// Note, built-in converters only exist for basic types (such as int, double, date/time, etc.) and enums.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time style.</param>
        /// <param name="dateFormat">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the exact format from which the date/time will be parsed.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US") used to infer the format provider (if not supplied).</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an <c>enum</c> value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="strict">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of the target type.  Default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to return if converting from a null object or an empty string.</param>
        /// <returns>The value of <c>obj</c> converted to <c>T</c>.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ConversionException"></exception>
        /// <exception cref="CultureNotFoundException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="TypeException"></exception>
        public static T To<T>
        (
            object obj,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            DateTimeStyles? dateTimeStyle = null,
            string dateFormat = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            bool ignoreCase = false,
            bool strict = false,
            T defaultValue = default
        )
        {
            return (T)To
            (
                typeof(T),
                obj,
                numberStyle: numberStyle,
                provider: provider,
                dateTimeStyle: dateTimeStyle,
                dateFormat: dateFormat,
                locale: locale,
                trueValues: trueValues,
                falseValues: falseValues,
                encoding: encoding,
                ignoreCase: ignoreCase,
                strict: strict,
                defaultValue: defaultValue
            );
        }

        /// <summary>
        /// Converts <c>obj</c> to the supplied type.  Methods include type compatibility checking and string parsing.  
        /// If <c>obj</c> is already assignable from the supplied type it is returned without conversion and will need to be cast.
        /// Note, built-in converters only exist for basic types (such as int, double, date/time etc.) and enums.
        /// </summary>
        /// <param name="type">The type to convert <c>obj</c> to.</param>
        /// <param name="obj">An object to convert.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="dateFormat">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates exact format from which the date/time will be parsed.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="strict">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If <c>true</c>, throws an exception if value does not fall within the numerical min/max values of the target type.  Default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to return if converting from a null object or an empty string.</param>
        /// <returns>The value of <c>obj</c> converted to <c>type</c>.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ConversionException"></exception>
        /// <exception cref="CultureNotFoundException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="TypeException"></exception>
        public static object To
        (
            Type type, 
            object obj,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null, 
            DateTimeStyles? dateTimeStyle = null,
            string dateFormat = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0", 
            Encoding encoding = null,
            bool ignoreCase = false,
            bool strict = false, 
            object defaultValue = null
        )
        {
            // null check
            if (Object(obj) == null)
                return defaultValue;

            // intervention: zap string value
            if (obj is string _stringValue0) 
            {
                _stringValue0 = _stringValue0.Trim();
                if (_stringValue0.Length == 0)
                    return defaultValue;
                if (type == typeof(string))
                    return _stringValue0;
                obj = _stringValue0;
            }
            if (type == typeof(string))
                return obj.ToString();

            // check if obj is already of desired (or compatible) type
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

            // convert value type to non-nullable to simplify below
            if (Nullable.GetUnderlyingType(type) is Type uType)
                type = uType;

            // use mainly built-in parsers for common value types and enums
            provider = provider ?? (locale != null ? CultureInfo.GetCultureInfo(locale) : null);
            if (obj is string stringValue)
            {
                if (type.IsNumeric())
                {
                    // infer style from formatting
                    var match = HexFormatPattern.Match(stringValue);  // e.g. "0xFF"
                    if (match.Success)
                    {
                        numberStyle = numberStyle.HasValue ? numberStyle.Value | NumberStyles.HexNumber : NumberStyles.HexNumber;
                        stringValue = match.Value.Substring(2);       // "0xFF" -> "FF"
                    }

                    // infer style from annotation
                    ParseNumericAnnotation(ref stringValue, ref numberStyle);

                    try
                    {
                        if (type == typeof(sbyte))
                            return sbyte.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(byte))
                            return byte.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(short))
                            return short.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(ushort))
                            return ushort.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(int))
                            return int.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(uint))
                            return uint.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(long))
                            return long.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(ulong))
                            return ulong.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(decimal))
                            return decimal.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(float))
                            return float.Parse(stringValue, numberStyle ?? default, provider);
                        if (type == typeof(double))
                            return double.Parse(stringValue, numberStyle ?? default, provider);
                    }
                    catch (Exception ex)
                    {
                        throw new ParseException("Cannot parse \"" + stringValue + "\" to " + type.FullName, ex);
                    }
                }

                else if (type == typeof(char))
                {
                    try
                    {
                        return char.Parse(stringValue);
                    }
                    catch (FormatException fex)
                    {
                        throw new ParseException("Cannot parse \"" + stringValue + "\" to " + typeof(char).FullName, fex);
                    }
                }

                else if (type == typeof(bool))
                {
                    if (trueValues.Split('|', ',').Any(trueVal => ignoreCase ? string.Equals(trueVal, stringValue, StringComparison.OrdinalIgnoreCase) : trueVal.Equals(stringValue)))
                        return true;
                    if (falseValues.Split('|', ',').Any(falseVal => ignoreCase ? string.Equals(falseVal, stringValue, StringComparison.OrdinalIgnoreCase) : falseVal.Equals(stringValue)))
                        return false;
                    throw new ParseException("Cannot parse \"" + stringValue + "\" to " + typeof(bool).FullName);
                }

                else if (type == typeof(DateTime))
                {
                    // infer style from annotation
                    ParseDateTimeAnnotation(ref stringValue, ref dateFormat);

                    try
                    {
                        if (dateFormat != null)
                            return dateTimeStyle.HasValue
                                ? System.DateTime.ParseExact(stringValue, dateFormat, provider, dateTimeStyle.Value)
                                : System.DateTime.ParseExact(stringValue, dateFormat, provider);
                        return dateTimeStyle.HasValue
                            ? System.DateTime.Parse(stringValue, provider, dateTimeStyle.Value)
                            : System.DateTime.Parse(stringValue, provider);
                    }
                    catch (Exception)
                    {
                        throw new ParseException("Cannot parse \"" + stringValue + "\" to " + typeof(DateTime).FullName + (dateFormat != null ? " (" + dateFormat + ")" : ""));
                    }
                }

                else if (type.IsEnum)
                {
                    try
                    {
                        return System.Enum.Parse(type, stringValue, ignoreCase);
                    }
                    catch (Exception ex)
                    {
                        throw new ParseException("Cannot parse \"" + stringValue + "\" to " + type.FullName, ex);
                    }
                }

                else if (type == typeof(byte[]))
                {
                    return (encoding ?? Encoding.Default).GetBytes(stringValue);
                }

                else if (type == typeof(string[]))
                {
                    return new[] { stringValue };
                }
            }

            // use mainly built-in converters for common value types and enums
            else if (type == typeof(sbyte))
                return NumberUtil.ConvertToSByte(obj, strict: strict);
            else if (type == typeof(byte))
                return NumberUtil.ConvertToByte(obj, strict: strict);
            else if (type == typeof(short))
                return NumberUtil.ConvertToInt16(obj, strict: strict);
            else if (type == typeof(ushort))
                return NumberUtil.ConvertToUInt16(obj, strict: strict);
            else if (type == typeof(int) || type == typeof(char))
                return NumberUtil.ConvertToInt32(obj, strict: strict);
            else if (type == typeof(uint))
                return NumberUtil.ConvertToUInt32(obj, strict: strict);
            else if (type == typeof(long))
                return NumberUtil.ConvertToInt64(obj, strict: strict);
            else if (type == typeof(ulong))
                return NumberUtil.ConvertToUInt64(obj, strict: strict);
            else if (type == typeof(decimal))
                return NumberUtil.ConvertToDecimal(obj, strict: strict);
            else if (type == typeof(float))
                return NumberUtil.ConvertToSingle(obj, strict: strict);
            else if (type == typeof(double))
                return NumberUtil.ConvertToDouble(obj);
            else if (type == typeof(bool))
                return Convert.ToBoolean(obj);
            else if (type == typeof(DateTime))
                return Convert.ToDateTime(obj);
            else if (type.IsEnum)
            {
                var euType = System.Enum.GetUnderlyingType(type);
                if (euType.IsNumeric(maxBits: 32))
                {
                    int enumIntValue;
                    try
                    {
                        enumIntValue = NumberUtil.ConvertToInt32(obj, strict: strict);
                    }
                    catch (Exception ex)
                    {
                        throw new TypeException("Cannot convert this " + obj.GetType().FullName + " to " + typeof(int).FullName, ex);
                    }

                    object enumValue;
                    try
                    {
                        enumValue = System.Enum.GetValues(type)
                            .Cast<object>()
                            .FirstOrDefault(o => (int)o == enumIntValue);
                    }
                    catch (InvalidCastException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException($"Encountered {ex.GetType().Name} when casting {type.FullName} to {typeof(int).FullName}", ex);
                    }

                    return enumValue 
                        ?? throw new ArgumentException("Cannot match [" + enumIntValue + "] to any enum value of type " + type.FullName);
                }
                throw new TypeException("Unsupported enum underlying type: " + euType.FullName + " - (currently supported types include sbyte, byte, short, ushort, int and uint)");
            }

            else if (type == typeof(byte[]))
            {
                if (obj is sbyte sbyteValue)
                    return new[] { (byte)sbyteValue };
                if (obj is byte byteValue)
                    return new[] { byteValue };
                if (obj is short shortValue)
                    return new[] { (byte)shortValue };
                if (obj is ushort ushortValue)
                    return new[] { (byte)ushortValue };
                if (obj is int intValue)
                    return new[] { (byte)intValue };
                if (obj is uint uintValue)
                    return new[] { (byte)uintValue };
                throw new ConversionException("No known conversion exists from " + obj.GetType().FullName + " to byte[]");
            }

            else if (type == typeof(string[]))
            {
                if (obj is StringValues stringValuesValue)
                {
                    return stringValuesValue.ToArray();
                }
                throw new ConversionException("No known conversion exists from " + obj.GetType().FullName + " to string[]");
            }

            // other value types
            throw new ConversionException("No built-in converter exists for this value type: " + type.FullName);
        }

        private static Regex AnnotationPattern { get; } = new Regex(@"((\A\[[^\[\]]+\])|(\[[^\[\]]+\]\Z))"); // begin or end of key name e.g. "[hex]red" or "red[hex]"
        private static Regex HexFormatPattern { get; } = new Regex(@"\A0x[0-9a-fA-F]+\Z");  // begin value e.g. "0xFF"

        /// <summary>
        /// Parses the annotation, if applicable, inferring the number style and removing the annotation from the value.
        /// Assumes the formatted <c>string</c> represents a numeric value.
        /// </summary>
        /// <param name="value">A <c>string</c> formatted value, potentially including annotation</param>
        /// <param name="numberStyle">The number style to infer from the annotation, if applicable.</param>
        public static void ParseNumericAnnotation(ref string value, ref NumberStyles? numberStyle)
        {
            var match = AnnotationPattern.Match(value);
            if (match.Success)
            {
                switch (match.Value.TrimStart('[').TrimEnd(']'))
                {
                    case "hex":
                        numberStyle = numberStyle.HasValue ? numberStyle.Value | NumberStyles.HexNumber : NumberStyles.HexNumber;
                        value = match.Index == 0
                            ? value.Substring(match.Length)                     // "[hex]FF" -> "FF", NumberStyles.HexNumber
                            : value.Substring(0, value.Length - match.Length);  // "FF[hex]" -> "FF", NumberStyles.HexNumber
                        break;
                }
            }
        }

        /// <summary>
        /// Parses the annotation, if applicable, inferring the date format and removing the annotation from the value.
        /// Assumes the formatted <c>string</c> represents a date/time.
        /// </summary>
        /// <param name="value">A <c>string</c> formatted value, potentially including annotation</param>
        /// <param name="dateFormat">The date format to infer from the annotation, if applicable.</param>
        public static void ParseDateTimeAnnotation(ref string value, ref string dateFormat)
        {
            var match = AnnotationPattern.Match(value);
            if (match.Success)
            {
                dateFormat = match.Value.TrimStart('[').TrimEnd(']');
                value = match.Index == 0
                    ? value.Substring(match.Length)                     // "[yyyy-MM-dd]2000-11-28" -> "2000-11-28", date format "yyyy-MM-dd"
                    : value.Substring(0, value.Length - match.Length);  // "2000-11-28[yyyy-MM-dd]" -> "2000-11-28", date format "yyyy-MM-dd"
            }
        }

        /// <inheritdoc cref="CollectionUtil.Zap(IEnumerable{string}, PruneOptions)"/>
        public static IEnumerable<string> Strings(IEnumerable<string> collection, PruneOptions pruneOptions = default)
        {
            return CollectionUtil.Zap(collection, pruneOptions);
        }

        /// <inheritdoc cref="ArrayUtil.Zap(string[], PruneOptions)"/>
        public static string[] Strings(string[] array, PruneOptions pruneOptions = default)
        {
            return ArrayUtil.Zap(array, pruneOptions: pruneOptions);
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
                        : ObjectUtil.GetInstancePropertyValues(obj)
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
                : TypeUtil.GetStaticPropertyValues(type)
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
                : TypeUtil.GetStaticPropertyValues<T>()
            ).Where(pv => pv.Property.PropertyType == typeof(string));
            foreach (var pv in propValues)
            {
                pv.Property.SetValue(null, String(pv.Value));
            }
        }
    }
}
