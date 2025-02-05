using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

    public static class ConfigUtil
    {
        // copied from TypeUtil on 2025-01-24
        internal static object GetInstance(Type type, bool nonPublic = false)
        {
            AssertIsReferenceType(type);
            return Activator.CreateInstance(type, nonPublic: nonPublic);
        }

        // copied from TypeUtil on 2025-01-25
        public static T GetInstance<T>(bool nonPublic = false) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), nonPublic);
        }

        // copied from TypeUtil on 2025-01-24
        internal static object GetInstance(Type type, params object[] args)
        {
            AssertIsReferenceType(type);
            return Activator.CreateInstance(type, args);
        }

        // copied from TypeUtil on 2025-01-24 - modified
        internal static object GetInstance(string className, string assemblyName = null, object[] args = null, Type inheritedType = null, bool nonPublic = false, bool ignoreCase = false, bool strict = false)
        {
            var type = GetType(className, assemblyName: assemblyName, inheritedType: inheritedType, ignoreCase: ignoreCase, strict: strict);

            if (type == null && !strict)
                return null;

            return args != null && args.Any()
                ? GetInstance(type, args: args)
                : GetInstance(type, nonPublic: nonPublic);
        }

        // copied from TypeUtil on 2025-01-25
        public static T GetInstance<T>(params object[] args) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        // copied from TypeUtil on 2025-01-24 - modified
        internal static Type GetType(string typeName, string assemblyName = null, Type inheritedType = null, bool ignoreCase = false, bool strict = false)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            Exception lastException = null;

            // step 1 - direct type loading (seldom works)
            Type type = Type.GetType(typeName, throwOnError: false, ignoreCase: ignoreCase);
            if (type == null)
            {
                // step 2 - load type from loaded assemblies (e.g. System.Core, Horseshoe.NET, etc.)
                try
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = assembly.GetType(typeName);
                        if (type != null)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }

                if (type == null && assemblyName != null)
                {
                    // step 3 - load assembly and type
                    try
                    {
                        var assembly = Assembly.Load(assemblyName);
                        if (assembly != null)
                        {
                            type = assembly.GetType(typeName);
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }
            }

            if (type == null)
            {
                if (strict)
                    throw new ConfigurationException("Type \"" + typeName + "\" could not be loaded" + (lastException != null ? ": " + lastException.Message : "") + ".  Please ensure you are using the fully qualified class name and that the proper assembly is accessible, if applicable (e.g. is present in the executable's path or installed in the GAC)", lastException);
                return null;
            }
            if (inheritedType != null && !inheritedType.IsAssignableFrom(type))
            {
                throw new ConfigurationException("\"" + inheritedType.FullName + "\" is not assignable from " + "\"" + type.FullName + "\"");
            }
            return type;
        }

        // copied from TypeUtil on 2025-01-24 - modified
        internal static void AssertIsReferenceType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), nameof(type) + " cannot be null");
            if (!type.IsClass)
                throw new ConfigurationException(type.FullName + " is not a reference type");
        }

        // copied from ValueUtil.Display() on 2025-01-24
        internal static string DisplayValue(object value)
        {
            if (value == null)
                return ConfigConstants.Null;
            if (value is DBNull)
                return "[db-null]";
            if (value is char @char)
                return "'" + @char + "'";
            if (value is string @string)
            {
                if (@string.Length == 0)
                    return ConfigConstants.Empty;
                if (@string.Trim().Length == 0)
                    return ConfigConstants.Whitespace;
                return @string.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"") + "\"";
            }
            if (value is DateTime dateTime)
                return
                    "\"" +
                    dateTime.ToShortDateString() +
                    (
                        HasTime(dateTime)
                        ? " " + dateTime.ToShortTimeString()
                        : ""
                    ) +
                    "\"";
            if (value is Type _type)
                return _type.FullName;
            if (value is IDictionary dict)  // incl. StringValues, string[], List<string>, etc.
            {
                if (dict.Keys.Count == 0)
                    return "[]";

                var list = new List<string>();
                foreach (var key in dict.Keys)
                {
                    list.Add(DisplayValue(key) + ": " + DisplayValue(dict[key]));
                }
                return "[ " + string.Join(", " + list) + " ]";
            }
            if (value is IEnumerable enumerable)
            {
                var objs = enumerable.Cast<object>();
                if (!objs.Any())
                    return "[]";
                return "[ " + string.Join(", " + objs.Select(o => DisplayValue(o))) + " ]";
            }

            var type = value.GetType();
            if (type.IsEnum)
                return type.Name + "." + value;

            return value.ToString();
        }

        // borrowed from DateAndTime\Extensions
        internal static bool HasTime(DateTime time)
        {
            return time.Hour > 0 || time.Minute > 0 || time.Second > 0 || time.Millisecond > 0;
        }

        // copied from Zap.To() on 2025-01-24 - modified
        internal static T ZapTo<T>
        (
            string stringValue,
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
            return (T)ZapTo
            (
                typeof(T), 
                stringValue, 
                numberStyle: numberStyle, 
                provider: provider, 
                dateTimeStyle: dateTimeStyle, 
                dateFormat: dateFormat, 
                locale: locale, 
                trueValues: trueValues, 
                falseValues: falseValues, 
                encoding: encoding, 
                ignoreCase: ignoreCase
            );
        }

        // copied from Zap on 2025-01-24 - modified
        internal static object ZapTo
        (
            Type type,
            string stringValue,
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
            // null check
            if (stringValue == null)
                return null;

            // intervention: zap string value
            stringValue = stringValue.Trim();
            if (stringValue.Length == 0)
                return null;
            if (type == typeof(string))
                return stringValue;

            // to simplify, convert nullable value type to non-nullable, if applicable
            if (Nullable.GetUnderlyingType(type) is Type uType)
                type = uType;

            // use mainly built-in parsers for common value types and enums
            provider = GetProvider(provider, locale);

            if (IsNumeric(type))
            {
                // infer style from formatting
                var match = HexFormatPattern.Match(stringValue);  // e.g. "0x5A"
                if (match.Success)
                {
                    numberStyle = numberStyle.HasValue ? numberStyle.Value | NumberStyles.HexNumber : NumberStyles.HexNumber;
                    stringValue = match.Value.Substring(2);       // "0x5A" -> "5A"
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
                    throw new ConfigurationException("Cannot parse \"" + stringValue + "\" to " + type.FullName, ex);
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
                    throw new ConfigurationException("Cannot parse \"" + stringValue + "\" to " + typeof(char).FullName, fex);
                }
            }

            else if (type == typeof(bool))
            {
                if (trueValues.Split('|', ',').Any(trueVal => ignoreCase ? string.Equals(trueVal, stringValue, StringComparison.OrdinalIgnoreCase) : trueVal.Equals(stringValue)))
                    return true;
                if (falseValues.Split('|', ',').Any(falseVal => ignoreCase ? string.Equals(falseVal, stringValue, StringComparison.OrdinalIgnoreCase) : falseVal.Equals(stringValue)))
                    return false;
                throw new ConfigurationException("Cannot convert \"" + stringValue + "\" to " + typeof(bool).FullName);
            }

            else if (type == typeof(DateTime))
            {
                // infer style from annotations
                ParseDateTimeAnnotation(ref stringValue, ref dateFormat);

                try
                {
                    if (dateFormat != null)
                        return dateTimeStyle.HasValue
                            ? DateTime.ParseExact(stringValue, dateFormat, provider, dateTimeStyle.Value)
                            : DateTime.ParseExact(stringValue, dateFormat, provider);
                    return dateTimeStyle.HasValue
                        ? DateTime.Parse(stringValue, provider, dateTimeStyle.Value)
                        : DateTime.Parse(stringValue, provider);
                }
                catch (Exception)
                {
                    throw new ConfigurationException("Cannot parse \"" + stringValue + "\" to " + typeof(DateTime).FullName + (dateFormat != null ? " (" + dateFormat + ")" : ""));
                }
            }

            else if (type.IsEnum)
            {
                if (int.TryParse(stringValue, out int intValue))
                {
                    var euType = Enum.GetUnderlyingType(type);
                    if (IsNumeric(euType, maxBits: 32))
                    {
                        object enumValue;
                        try
                        {
                            enumValue = Enum.GetValues(type)
                                .Cast<object>()
                                .FirstOrDefault(o => (int)o == intValue);
                        }
                        catch (Exception ex)
                        {
                            throw new ConfigurationException("Cannot cast " + type.FullName + " to " + typeof(int).FullName, ex);
                        }

                        return enumValue
                            ?? throw new ConfigurationException("Cannot match [" + intValue + "] to any enum value of type " + type.FullName);
                    }
                    throw new ConfigurationException("Unsupported enum underlying type: " + euType.FullName + " - (currently supported types include sbyte, byte, short, ushort, int and uint)");
                }

                try
                {
                    return Enum.Parse(type, stringValue, ignoreCase);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationException("Cannot parse \"" + stringValue + "\" to " + type.FullName, ex);
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

            // other value types
            throw new ConfigurationException("No built-in converter exists for this value type: " + type.FullName);
        }

        private static Regex AnnotationPattern { get; } = new Regex(@"((\A\[[^\[\]]+\])|(\[[^\[\]]+\]\Z))"); // begin or end of key name e.g. "[hex]red" or "red[hex]"
        private static Regex HexFormatPattern { get; } = new Regex(@"\A0x[0-9a-fA-F]+\Z");  // begin value e.g. "0xFF"

        public static void ParseNumericAnnotation(ref string keyOrValue, ref NumberStyles? numberStyle)
        {
            var match = AnnotationPattern.Match(keyOrValue);
            if (match.Success)
            {
                var annotationNameOrValue = match.Value.TrimStart('[').TrimEnd(']');
                switch (annotationNameOrValue)
                {
                    case "hex":
                        numberStyle = numberStyle.HasValue ? numberStyle.Value | NumberStyles.HexNumber : NumberStyles.HexNumber;
                        keyOrValue = match.Index == 0
                            ? keyOrValue.Substring(match.Length)                          // "[hex]red" -> "red"  or  "[hex]5A" -> "5A"
                            : keyOrValue.Substring(0, keyOrValue.Length - match.Length);  // "red[hex]" -> "red"  or  "5A[hex]" -> "5A"
                        break;
                }
            }
        }

        public static void ParseDateTimeAnnotation(ref string keyOrValue, ref string dateFormat)
        {
            var match = AnnotationPattern.Match(keyOrValue);
            if (match.Success)
            {
                dateFormat = match.Value.TrimStart('[').TrimEnd(']');
                keyOrValue = match.Index == 0
                    ? keyOrValue.Substring(match.Length)                          // "[yyyy-MM-dd]dateOfBirth" -> "dateOfBirth"
                    : keyOrValue.Substring(0, keyOrValue.Length - match.Length);  // "dateOfBirth[yyyy-MM-dd]" -> "dateOfBirth"
            }
        }

        // copied from TypeUtil on 2025-01-24 - modified
        internal static object GetDefaultValue(Type type, bool nonPublic = false)
        {
            if (type.IsClass || Nullable.GetUnderlyingType(type) != null)  // e.g. MyClass || int?
                return null;

            if (type.IsValueType)
                return Activator.CreateInstance(type, nonPublic: nonPublic);

            throw new ConfigurationException(type.FullName + " is not associated with a default value");
        }

        // copied from TextUtil on 2025-01-25
        internal static IFormatProvider GetProvider(IFormatProvider provider, string locale)
        {
            if (provider != null)
                return provider;
            if (locale != null)
                return CultureInfo.GetCultureInfo(locale);
            return null;
        }

        internal static bool IsNumeric(Type type, int maxBits = default)
        {
            switch (maxBits)
            {
                case 0:
                    return IsNumeric(type, maxBits: 64) ||
                        type == typeof(decimal) ||
                        type == typeof(float) ||
                        type == typeof(double);
                case 16:
                    return
                        type == typeof(sbyte) ||
                        type == typeof(byte) ||
                        type == typeof(short) ||
                        type == typeof(ushort);
                case 32:
                    return IsNumeric(type, maxBits: 16) ||
                        type == typeof(int) ||
                        type == typeof(uint);
                case 64:
                    return IsNumeric(type, maxBits: 32) ||
                        type == typeof(long) ||
                        type == typeof(ulong);
                default:
                    throw new ArgumentException("Invalid value: " + maxBits + " - try 16, 32 or 64", nameof(maxBits));
            }
        }
    }
}
