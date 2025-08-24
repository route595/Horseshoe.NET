using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Dotnet
{
    /// <summary>
    /// Utility methods that involve certain .NET language specific elements
    /// </summary>
    public static class DotnetUtil
    {
        /// <summary>
        /// Gets source code type name e.g. "int" instead of "Int32"
        /// </summary>
        /// <param name="type">A type</param>
        /// <param name="lang">Optional .NET language, e.g. C# (default), VB.NET</param>
        /// <returns>Short name</returns>
        public static string GetSourceCodeFormattedName(Type type, DotnetLanguage lang = default)
        {
            if (type == null)
                return TextConstants.Null;

            switch (lang)
            {
                case DotnetLanguage.CSharp:
                default:
                    {
                        // ref
                        if (type == typeof(string))
                            return "string";
                        if (type == typeof(object))
                            return "object";
                        if (type.IsClass)
                            return type.Name;

                        // value
                        var suffix = "";
                        if (Nullable.GetUnderlyingType(type) is Type _type)
                        {
                            type = _type;
                            suffix = "?";
                        }
                        if (type == typeof(short))   // System.Int16
                            return "short" + suffix;
                        if (type == typeof(ushort))  // System.UInt16
                            return "ushort" + suffix;
                        if (type == typeof(int))     // System.Int32
                            return "int" + suffix;
                        if (type == typeof(uint))    // System.UInt32
                            return "uint" + suffix;
                        if (type == typeof(long))    // System.Int64
                            return "long" + suffix;
                        if (type == typeof(ulong))   // System.UInt64
                            return "ulong" + suffix;
                        //if (type == typeof(nint))    // System.IntPtr (C# 9.0+)
                        //    return "nint" + suffix;
                        //if (type == typeof(nuint))   // System.UIntPtr (C# 9.0+)
                        //    return "nuint" + suffix;
                        var toLowerArray = new[]
                        {
                            typeof(sbyte),
                            typeof(byte),
                            typeof(float),
                            typeof(double),
                            typeof(decimal),
                            typeof(bool)
                        };
                        if (toLowerArray.Contains(type))
                            return type.Name.ToLower() + suffix;
                        return type.Name + suffix;
                    }
                case DotnetLanguage.VB:
                    {
                        // ref
                        if (type.IsClass)
                            return type.Name;  // e.g. String, Object, MyClass

                        // value
                        var suffix = "";
                        if (Nullable.GetUnderlyingType(type) is Type _type)
                        {
                            type = _type;
                            suffix = "?";
                        }
                        if (type == typeof(short))   // System.Int16
                            return "Short" + suffix;
                        if (type == typeof(ushort))  // System.UInt16
                            return "UShort" + suffix;
                        if (type == typeof(int))     // System.Int32
                            return "Integer" + suffix;
                        if (type == typeof(uint))    // System.UInt32
                            return "UInteger" + suffix;
                        if (type == typeof(long))    // System.Int64
                            return "Long" + suffix;
                        if (type == typeof(ulong))   // System.UInt64
                            return "ULong" + suffix;
                        return type.Name + suffix;
                    }
            }
        }

        /// <summary>
        /// Displays values as they might appear in source code except <c>DateTime</c>s which are 
        /// square braced by default and any type that does not override <c>ToString()</c>. Clients
        /// may add custom formatters (see <see cref="TypeFormatters.AddFormatter(Type, Func{object, string})"/>).
        /// </summary>
        /// <param name="value">A value</param>
        /// <param name="lang">A .NET language</param>
        /// <returns></returns>
        public static string GetSourceCodeFormattedValue(object value, DotnetLanguage lang = default)
        {
            if (value == null)
                return lang == DotnetLanguage.VB
                    ? "Nothing"
                    : "null";
            var formatter = TypeFormatters.GetFormatter(value.GetType());
            if (formatter != null)
                return formatter.Invoke(value);
            if (value is DateTime dateTimeValue)
                value = dateTimeValue.ToShortDateString() + (dateTimeValue.HasTime() ? " " + dateTimeValue.ToShortTimeString() : "");
            if (value is string stringValue)
                return "\"" + stringValue.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\"", "\\\"") + "\"";
            if (value is char charValue)
                return lang == DotnetLanguage.VB
                    ? "\"" + charValue + "\""
                    : "'" + charValue + "'";
            if (value is Type _type)
                return _type.FullName;
            if (value is IDictionary dict)  // incl. StringValues, string[], List<string>, etc.
            {
                if (dict.Keys.Count == 0)
                    return "[]";

                var list = new List<string>();
                foreach (var key in dict.Keys)
                {
                    list.Add(GetSourceCodeFormattedValue(key, lang: lang) + ": " + GetSourceCodeFormattedValue(dict[key], lang: lang));
                }
                return "[ " + string.Join(", ", list) + " ]";
            }
            if (value is IEnumerable enumerable)
            {
                var objs = enumerable.Cast<object>();
                if (!objs.Any())
                    return "[]";
                return "[ " + string.Join(", ", objs.Select(o => GetSourceCodeFormattedValue(o, lang: lang))) + " ]";
            }

            var type = value.GetType();
            if (type.IsEnum)
            {
                return type.Name + "." + value;
            }

            return value.ToString();
        }

        /// <summary>
        /// Attempts to construct a source code style string representation of a method (from reflection).
        /// </summary>
        /// <param name="method">The current method.</param>
        /// <param name="includeDeclaringType">If <c>true</c>, returns "fully qualified" method call e.g. <c>MyClass.MyMethod()</c>.  Default is <c>false</c> e.g. <c>MyMethod()</c></param>
        /// <param name="includeParams">If <c>true</c>, a close approximation of the coded parameters will be included in the relayed message, default is <c>false</c>..</param>
        /// <param name="lang">Optional .NET language, e.g. C# (default), VB.NET</param>
        public static string ReconstructMethod(MethodBase method, bool includeDeclaringType = false, bool includeParams = false, DotnetLanguage lang = default)
        {
            StringBuilder strb = new StringBuilder(method.Name);
            if (method.Name == ".ctor")
                strb.Insert(0, method.DeclaringType.Name); // constructor
            if (includeDeclaringType)
            {
                strb.Insert(0, '.')
                    .Insert(0, method.DeclaringType.Name);
            }
            if (method.IsGenericMethod && method is MethodInfo methodInfo)
            {
                strb.Append("<")
                    .Append(string.Join(",", methodInfo.GetGenericArguments().Select(t => GetSourceCodeFormattedName(t, lang: lang))))
                    .Append(">");
            }
            strb.Append("(");
            if (includeParams)
            {
                switch (lang)
                {
                    case DotnetLanguage.CSharp:
                    default:
                        strb.Append(string.Join(", ", method.GetParameters().Select(p => GetSourceCodeFormattedName(p.ParameterType, lang: lang) + " " + p.Name)));
                        break;
                    case DotnetLanguage.VB:
                        strb.Append(string.Join(", ", method.GetParameters().Select(p => p.Name + " As " + GetSourceCodeFormattedName(p.ParameterType, lang: lang))));
                        break;
                }
            }
            strb.Append(")");
            return strb.ToString();
        }
    }
}
