using System;
using System.Collections.Generic;

namespace Horseshoe.NET
{
    public static class ConvertString
    {
        private static IDictionary<Type, Func<string, object>> converters = new Dictionary<Type, Func<string, object>>
        {
            { typeof(bool), (value) => Zap.Bool(value) },
            //{ typeof(bool?), (value) => Zap.NBool(value) },
            { typeof(byte), (value) => Zap.Byte(value) },
            //{ typeof(byte?), (value) => Zap.NByte(value) },
            { typeof(short), (value) => Zap.Short(value) },
            //{ typeof(short?), (value) => Zap.NShort(value) },
            { typeof(int), (value) => Zap.Int(value) },
            //{ typeof(int?), (value) => Zap.NInt(value) },
            { typeof(long), (value) => Zap.Long(value) },
            //{ typeof(long?), (value) => Zap.NLong(value) },
            { typeof(float), (value) => Zap.Float(value) },
            //{ typeof(float?), (value) => Zap.NFloat(value) },
            { typeof(double), (value) => Zap.Double(value) },
            //{ typeof(double?), (value) => Zap.NDouble(value) },
            { typeof(decimal), (value) => Zap.Decimal(value) },
            //{ typeof(decimal?), (value) => Zap.Decimal(value) },
            { typeof(DateTime), (value) => Zap.DateTime(value) }
            //{ typeof(DateTime?), (value) => Zap.NDateTime(value) }
        };

        public static bool RegisterConverter(Type type, Func<string, object> converter, bool overwrite = false)
        {
            if (type == typeof(string))
            {
                return false;
            }
            if (converters.ContainsKey(type))
            {
                if (overwrite)
                {
                    converters[type] = converter;
                    return true;
                }
                return false;
            }
            converters.Add(type, converter);
            return true;
        }

        public static T To<T>(string value, Func<string, object> converter = null)
        {
            object obj = To(typeof(T), value, converter: converter);
            return obj == null ? default : (T)obj;
        }

        public static object To(Type type, string value, Func<string, object> converter = null)
        {
            if (type == typeof(string))
            {
                return value;
            }

            try
            {
                if (converter != null)
                {
                    return converter.Invoke(value);
                }
                if (converters.ContainsKey(type))
                {
                    return converters[type].Invoke(value);
                }
            }
            catch (Exception ex)
            {
                throw new ConversionException(typeof(string), type, ex);
            }

            throw new ConversionException(typeof(string), type, "A compatible converter has not been supplied", isConverterNotSupplied: true);
        }
    }
}
