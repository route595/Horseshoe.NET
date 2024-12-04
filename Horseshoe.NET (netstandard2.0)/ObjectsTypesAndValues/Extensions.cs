using System;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A set of extension methods for Horseshoe.NET.ObjectsAndTypes.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sets the properties of <c>dest</c> with the property values of <c>src</c>
        /// </summary>
        /// <param name="src">The source <c>object</c></param>
        /// <param name="dest">The target <c>object</c></param>
        /// <param name="tryMapAll"></param>
        /// <param name="preventNullOverwrite">If <c>true</c>, prevents a non-null destination property from being overwritten by null, default is <c>false</c> and this is not commonly used.</param>
        /// <param name="ignoreCase">If <c>true</c>, allows mapping of properties that are identically named if not for the letter case, default is <c>false</c>.</param>
        /// <param name="ignoreErrors">If <c>true</c>, bypasses mapping errors leaving the values unchanged, default is <c>false</c>.</param>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ObjectMappingException"></exception>
        public static void MapProperties(this object src, object dest, bool tryMapAll = false, bool preventNullOverwrite = false, bool ignoreCase = false, bool ignoreErrors = false) =>
            ObjectUtil.MapProperties(src, dest, tryMapAll: tryMapAll, preventNullOverwrite: preventNullOverwrite, ignoreCase: ignoreCase, ignoreErrors: ignoreErrors);

        /// <summary>
        /// Creates a shallow duplicate of an <c>object</c>
        /// </summary>
        /// <param name="src">The source <c>object</c></param>
        /// <param name="ignoreErrors">If <c>true</c>, bypasses mapping errors leaving the values unchanged, default is <c>false</c>.</param>
        /// <returns>A new <c>object</c> identical to <c>src</c></returns>
        public static object Duplicate(this object src, bool ignoreErrors = false) =>
            ObjectUtil.Duplicate(src, ignoreErrors: ignoreErrors);

        /// <summary>
        /// Tests whether this type is a numeric value type.
        /// </summary>
        /// <param name="type">A runtime type.</param>
        /// <param name="includeChar">If <c>true</c>, <c>System.Char</c> will be considered numeric, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsNumeric(this Type type, bool includeChar = false)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is Type uType)
                type = uType;

            return
                type == typeof(byte) ||
                type == typeof(short) ||
                (type == typeof(char) && includeChar) ||
                type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(decimal) ||
                type == typeof(float) ||
                type == typeof(double);
        }

        /// <summary>
        /// Tests whether this type is an enum value type.
        /// </summary>
        /// <param name="type">A runtime type.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsEnumType(this Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsValueType && Nullable.GetUnderlyingType(type) is Type uType)
                    type = uType;

                return type.IsEnum;
            }
            return false;
        }
    }
}
