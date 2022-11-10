using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text.TextClean;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET
{
    public static class Zap
    {
        public static object Object(object obj, Func<object, bool> evaluatesToNull = null)
        {
            if (obj is null || obj is DBNull) return null;
            if (evaluatesToNull?.Invoke(obj) ?? false) return null;
            if (obj is string stringValue) return _String(stringValue, null);
            if (obj is StringValues stringValuesValue)
            {
                switch (stringValuesValue.Count)
                {
                    case 0: return null;
                    case 1: return _String(stringValuesValue.Single(), null);
                    default: return string.Join(", ", stringValuesValue.Select(s => _String(s, null)));
                };
            }
            return obj;
        }

        public static string String(object obj, Func<string, string> preProcess = null)
        {
            if ((obj = Object(obj)) is null) return null;
            if (obj is string stringValue) return _String(stringValue, preProcess);
            stringValue = obj.ToString().Trim();
            if (stringValue.Length == 0) return null;
            return stringValue;
        }

        private static string _String(string stringValue, Func<string, string> preProcess)
        {
            if (preProcess != null)
                stringValue = preProcess.Invoke(stringValue);
            if (stringValue == null) 
                return null;
            stringValue = stringValue.Trim();
            if (stringValue.Length == 0) 
                return null;
            return stringValue;
        }

        public static bool Bool(object obj, bool defaultValue = false, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", bool ignoreCase = false, bool treatArbitraryAsFalse = false)
        {
            return NBool(obj, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase, treatArbitraryAsFalse: treatArbitraryAsFalse) ?? defaultValue;
        }

        public static bool? NBool(object obj, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", bool ignoreCase = false, bool treatArbitraryAsFalse = false)
        {
            if ((obj = Object(obj)) is null) return null;
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
                if (ignoreCase ? stringValue.InIgnoreCase(trueValues.Split('|')) : stringValue.In(trueValues.Split('|')))
                    return true;
                if ((ignoreCase ? stringValue.InIgnoreCase(falseValues.Split('|')) : stringValue.In(falseValues.Split('|'))) || treatArbitraryAsFalse)
                    return false;
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to bool");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to bool");
        }

        public static byte Byte(object obj, byte defaultValue = default, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is byte byteValue) return byteValue;
            if (obj is bool boolValue) return (byte)(boolValue ? 1 : 0);
            if (obj is short shortValue) return NumberUtil.EvalAsByte(shortValue, force: force);
            if (obj is int intValue) return NumberUtil.EvalAsByte(intValue, force: force);
            if (obj is long longValue) return NumberUtil.EvalAsByte(longValue, force: force);
            if (obj is decimal decimalValue) return NumberUtil.EvalAsByte(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsByte(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsByte(doubleValue, force: force);

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (byte.TryParse(stringValue, numberStyles ?? default, provider, out byteValue))
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

        public static byte? NByte(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Byte(obj, force: force, numberStyles: numberStyles, provider: provider);
        }

        public static short Short(object obj, short defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is short shortValue) return shortValue;
            if (obj is bool boolValue) return (short)(boolValue ? 1 : 0);
            if (obj is byte byteValue) return byteValue;
            if (obj is int intValue) return NumberUtil.EvalAsShort(intValue, force: force);
            if (obj is long longValue) return NumberUtil.EvalAsShort(longValue, force: force);
            if (obj is decimal decimalValue) return NumberUtil.EvalAsShort(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsShort(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsShort(doubleValue, force: force);

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (short.TryParse(stringValue, numberStyles ?? default, provider, out shortValue))
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

        public static short? NShort(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Short(obj, force: force, numberStyles: numberStyles, provider: provider);
        }

        public static int Int(object obj, int defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is int intValue) return intValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is long longValue) return NumberUtil.EvalAsInt(longValue, force: force);
            if (obj is decimal decimalValue) return NumberUtil.EvalAsInt(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsInt(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsInt(doubleValue, force: force);

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (int.TryParse(stringValue, numberStyles ?? default, provider, out intValue))
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

        public static int? NInt(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Int(obj, force: force, numberStyles: numberStyles, provider: provider);
        }

        public static long Long(object obj, long defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is long longValue) return longValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is decimal decimalValue) return NumberUtil.EvalAsLong(decimalValue, force: force);
            if (obj is float floatValue) return NumberUtil.EvalAsLong(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsLong(doubleValue, force: force);

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (long.TryParse(stringValue, numberStyles ?? default, provider, out longValue))
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

        public static long? NLong(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Long(obj, force: force, numberStyles: numberStyles, provider: provider);
        }

        public static decimal Decimal(object obj, decimal defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is decimal decimalValue) return decimalValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is float floatValue) return NumberUtil.EvalAsDecimal(floatValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsDecimal(doubleValue, force: force);

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (decimal.TryParse(stringValue, numberStyles ?? default, provider, out decimalValue))
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

        public static decimal? NDecimal(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Decimal(obj, force: force, numberStyles: numberStyles, provider: provider);
        }

        public static float Float(object obj, float defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is float floatValue) return floatValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return NumberUtil.EvalAsFloat(decimalValue, force: force);
            if (obj is double doubleValue) return NumberUtil.EvalAsFloat(doubleValue, force: force);

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (float.TryParse(stringValue, numberStyles ?? default, provider, out floatValue))
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

        public static float? NFloat(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Float(obj, force: force, numberStyles: numberStyles, provider: provider);
        }

        public static double Double(object obj, double defaultValue = 0, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is double doubleValue) return doubleValue;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return (double)decimalValue;
            if (obj is float floatValue) return floatValue;

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (numberStyles.HasValue || provider != null)
            {
                if (double.TryParse(stringValue, numberStyles ?? default, provider, out doubleValue))
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

        public static double? NDouble(object obj, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return Double(obj, numberStyles: numberStyles, provider: provider);
        }

        public static DateTime DateTime(object obj, DateTime defaultValue = default, DateTimeStyles? dateTimeStyles = null, IFormatProvider provider = null)
        {
            obj = Object(obj);
            if (obj is null) return defaultValue;
            if (obj is DateTime dateTimeValue) return dateTimeValue;
            if (obj is int intValue) return new DateTime(intValue);     // ticks
            if (obj is long longValue) return new DateTime(longValue);  // ticks

            var stringValue = obj is string _stringValue
                ? _stringValue
                : obj.ToString();

            if (provider != null || dateTimeStyles.HasValue)
            {
                if (System.DateTime.TryParse(stringValue, provider, dateTimeStyles ?? default, out dateTimeValue))
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

        public static DateTime? NDateTime(object obj, DateTimeStyles? dateTimeStyles = null, IFormatProvider provider = null)
        {
            if (Object(obj) is null) 
                return null;
            return DateTime(obj, dateTimeStyles: dateTimeStyles, provider: provider);
        }

        public static T Enum<T>(object obj, T defaultValue = default, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            return NEnum<T>(obj, ignoreCase: ignoreCase, suppressErrors: suppressErrors) ?? defaultValue;
        }

        public static T? NEnum<T>(object obj, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            obj = NEnumOfType(typeof(T), obj, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
            if (obj == null)
                return null;
            return (T)obj;
        }

        public static object EnumOfType(Type type, object obj, bool ignoreCase = false, bool suppressErrors = false)
        {
            obj = NEnumOfType(type, obj, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
            if (obj == null)
                return ObjectUtil.GetDefault(type);
            return obj;
        }

        public static object NEnumOfType(Type type, object obj, bool ignoreCase = false, bool suppressErrors = false)
        {
            if ((obj = Object(obj)) is null) 
                return null;
            if (!type.IsEnum) 
                throw new UtilityException("Not an enum type: " + type.FullName);
            if (obj.GetType() == type) 
                return obj;
            object tempObj = null;
            try
            {
                if (obj is byte byteValue) tempObj = System.Enum.ToObject(type, byteValue);
                if (obj is int intValue) tempObj = System.Enum.ToObject(type, intValue);
                if (obj is long longValue) tempObj = System.Enum.ToObject(type, longValue);
            }
            catch
            {
                throw new UtilityException("Cannot convert \"" + obj + "\" to " + type.FullName);
            }
            if (tempObj != null) 
                return tempObj;
            if (obj is string stringValue)
            {
                stringValue = TextClean.RemoveWhitespace(stringValue);
                try
                {
                    return System.Enum.Parse(type, stringValue, ignoreCase);
                }
                catch { }
                if (suppressErrors) 
                    return null;
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to " + type.FullName);
            }
            if (suppressErrors) 
                return null;
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to " + type.FullName);
        }

        public static object ToType(Type type, object obj)
        {
            if (Object(obj) is null)
                return ObjectUtil.GetDefault(type);
            if (type == typeof(string))
                return String(obj);
            if (type == typeof(byte) || type == typeof(byte?))
                return Byte(obj);
            //if (type == typeof(byte?))
            //    return NByte(obj);
            if (type == typeof(short) || type == typeof(short?))
                return Short(obj);
            //if (type == typeof(short?))
            //    return NShort(obj);
            if (type == typeof(int) || type == typeof(int?))
                return Int(obj);
            //if (type == typeof(int?))
            //    return NInt(obj);
            if (type == typeof(long) || type == typeof(long?))
                return Long(obj);
            //if (type == typeof(long?))
            //    return NLong(obj);
            if (type == typeof(decimal) || type == typeof(decimal?))
                return Decimal(obj);
            //if (type == typeof(decimal?))
            //    return NDecimal(obj);
            if (type == typeof(float) || type == typeof(float?))
                return Float(obj);
            //if (type == typeof(float?))
            //    return NFloat(obj);
            if (type == typeof(double) || type == typeof(double?))
                return Double(obj);
            //if (type == typeof(double?))
            //    return NDouble(obj);
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return DateTime(obj);
            //if (type == typeof(DateTime?))
            //    return NDateTime(obj);
            if (type.IsEnum)
                return EnumOfType(type, obj);
            throw new ConversionException("We do not have a built-in converter for the supplied type: " + type.FullName, isConverterNotSupplied: true);
        }

        public static IEnumerable<string> Strings(IEnumerable<string> collection, Func<string, string> preProcess = null, PrunePolicy prunePolicy = default)
        {
            if (collection == null)
                return new string[0];
            var list = new List<string>(collection ?? Array.Empty<string>());
            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i] = String(list[i], preProcess: preProcess);
                if (list[i] == null && prunePolicy == PrunePolicy.All)
                {
                    list.RemoveAt(i);
                }
            }
            if (prunePolicy == PrunePolicy.Leading || prunePolicy == PrunePolicy.LeadingAndTrailing)
            {
                while (list.Any() && list[0] == null)
                {
                    list.RemoveAt(0);
                }
            }
            if (prunePolicy == PrunePolicy.Trailing || prunePolicy == PrunePolicy.LeadingAndTrailing)
            {
                while (list.Any() && list.Last() == null)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
            return list;
        }

        public static string[] Strings(string[] array, Func<string, string> preProcess = null, PrunePolicy prunePolicy = default)
        {
            var strings = Strings(array as IEnumerable<string>, preProcess: preProcess, prunePolicy: prunePolicy);
            return strings.ToArray();
        }

        public static void StringProperties(object obj, BindingFlags? bindingFlags = null)
        {
            if (obj != null)
            {
                var propValues = ObjectUtil.GetInstancePropertyValues
                (
                    obj,
                    bindingFlags: bindingFlags,
                    filter: p => p.CanWrite && p.PropertyType == typeof(string)
                );
                foreach (var pv in propValues)
                {
                    ((PropertyInfo)pv.Member).SetValue(obj, String(pv.Value));
                }
            }
        }

        public static void StringProperties<T>(IEnumerable<T> objs, BindingFlags? bindingFlags = null) where T : class
        {
            if (objs != null)
            {
                var props = ObjectUtil.GetInstanceProperties<T>
                (
                    bindingFlags: bindingFlags,
                    filter: p => p.CanWrite && p.PropertyType == typeof(string)
                );
                foreach (var obj in objs)
                {
                    if (obj != null)
                    {
                        foreach (var prop in props)
                        {
                            if (prop.GetValue(obj) is string stringValue)
                            {
                                prop.SetValue(obj, String(stringValue));
                            }
                        }
                    }
                }
            }
        }

        public static void StaticStringProperties<T>(BindingFlags? bindingFlags = null) where T : class
        {
            var propValues = ObjectUtil.GetStaticPropertyValues<T>
            (
                bindingFlags: bindingFlags,
                filter: p => p.CanWrite && p.PropertyType == typeof(string)
            );
            foreach (var pv in propValues)
            {
                ((PropertyInfo)pv.Member).SetValue(null, String(pv.Value));
            }
        }
    }
}
