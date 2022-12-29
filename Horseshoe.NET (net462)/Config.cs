using System;
using System.Globalization;
using System.Text;
using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET
{
#pragma warning disable IDE1006 // Naming Styles
    internal static class _Config
#pragma warning restore IDE1006 // Naming Styles
    {
        internal static string Assembly => "Horseshoe.NET.Configuration";

        internal static string ClassName => "Config";

        internal static string Get(string key, bool required = false)
        {
            var value = Lib.TryInvokeStaticMethod<string>(Assembly, null, ClassName, nameof(Get), out _, key, required);
            return value;
        }

        internal static T Get<T>
        (
            string key,
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
            var value = Get(key);
            if (value == null)
                return default;
            return Zap.To<T>(value, dateTimeStyle: dateTimeStyle, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, inheritedType: inheritedType, ignoreCase: ignoreCase);
        }

        internal static T GetInstance<T>(string key, bool ignoreCase = false, bool nonPublic = false)
        {
            var type = Get<Type>(key, inheritedType: typeof(T), ignoreCase: ignoreCase);
            if (type == null)
                return default;
            return (T)TypeUtil.GetDefaultInstance(type, nonPublic: nonPublic);
        }


        internal static string GetConnectionString(string name)
        {
            var value = Lib.TryInvokeStaticMethod<string>(Assembly, null, ClassName, nameof(GetConnectionString), out _, name, false);
            return value;
        }
    }
}