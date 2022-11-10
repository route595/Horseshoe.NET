using System;

namespace Horseshoe.NET.Http
{
    public static class HttpResponseParsers
    {
        public static Func<string,T> Get<T>()
        {
            if (typeof(T) == typeof(string))
                return (s) => (T)(object)Zap.String(s);
            if (typeof(T) == typeof(byte?))
                return (s) => (T)(object)Zap.NByte(s);
            if (typeof(T) == typeof(byte))
                return (s) => (T)(object)Zap.Byte(s);
            if (typeof(T) == typeof(short?))
                return (s) => (T)(object)Zap.NShort(s);
            if (typeof(T) == typeof(short))
                return (s) => (T)(object)Zap.Short(s);
            if (typeof(T) == typeof(int?))
                return (s) => (T)(object)Zap.NInt(s);
            if (typeof(T) == typeof(int))
                return (s) => (T)(object)Zap.Int(s);
            if (typeof(T) == typeof(long?))
                return (s) => (T)(object)Zap.NLong(s);
            if (typeof(T) == typeof(long))
                return (s) => (T)(object)Zap.Long(s);
            if (typeof(T) == typeof(float?))
                return (s) => (T)(object)Zap.NFloat(s);
            if (typeof(T) == typeof(float))
                return (s) => (T)(object)Zap.Float(s);
            if (typeof(T) == typeof(double?))
                return (s) => (T)(object)Zap.NDouble(s);
            if (typeof(T) == typeof(double))
                return (s) => (T)(object)Zap.Double(s);
            if (typeof(T) == typeof(decimal?))
                return (s) => (T)(object)Zap.NDecimal(s);
            if (typeof(T) == typeof(decimal))
                return (s) => (T)(object)Zap.Decimal(s);
            if (typeof(T) == typeof(DateTime?))
                return (s) => (T)(object)Zap.NDateTime(s);
            if (typeof(T) == typeof(DateTime))
                return (s) => (T)(object)Zap.DateTime(s);
            if (typeof(T) == typeof(bool?))
                return (s) => (T)(object)Zap.NBool(s);
            if (typeof(T) == typeof(bool))
                return (s) => (T)(object)Zap.Bool(s);
            if (typeof(T).IsEnum)
                return (s) => (T)Zap.EnumOfType(typeof(T), s);
            return null;
        }

        //public static int Int(string str)
        //{
        //    if (int.TryParse(str, out int result))
        //        return result;
        //    return default;
        //}

        //public static int? NInt(string str)
        //{
        //    if (int.TryParse(str, out int result))
        //        return result;
        //    return default;
        //}

        //public static DateTime Date(string str)
        //{
        //    if (DateTime.TryParse(str, out DateTime result))
        //        return result;
        //    return default;
        //}

        //public static DateTime? NDate(string str)
        //{
        //    if (DateTime.TryParse(str, out DateTime result))
        //        return result;
        //    return default;
        //}
    }
}
