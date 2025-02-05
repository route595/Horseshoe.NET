using System;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET
{
    /// <summary>
    /// A collection of utility methods for different numeric types.
    /// </summary>
    public static class NumberUtil
    {
        /// <summary>
        /// Converts the supplied value to <c>sbyte</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>sbyte</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>sbyte</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static sbyte ConvertToSByte(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is sbyte sbyteValue)
                return sbyteValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < sbyte.MinValue || doubleValue > sbyte.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(sbyte).FullName}: {sbyte.MinValue} to {sbyte.MaxValue}");
            }
            return Convert.ToSByte(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>byte</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>byte</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>byte</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static byte ConvertToByte(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is byte byteValue)
                return byteValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < byte.MinValue || doubleValue > byte.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}: {byte.MinValue} to {byte.MaxValue}");
            }
            return Convert.ToByte(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>short</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>short</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>short</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static short ConvertToInt16(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is short shortValue)
                return shortValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < short.MinValue || doubleValue > short.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(short).FullName}: {short.MinValue} to {short.MaxValue}");
            }
            return Convert.ToInt16(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>ushort</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>ushort</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>ushort</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static ushort ConvertToUInt16(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is ushort ushortValue)
                return ushortValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < ushort.MinValue || doubleValue > ushort.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(ushort).FullName}: {ushort.MinValue} to {ushort.MaxValue}");
            }
            return Convert.ToUInt16(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>int</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>int</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>int</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static int ConvertToInt32(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is int intValue)
                return intValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < int.MinValue || doubleValue > int.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(int).FullName}: {int.MinValue} to {int.MaxValue}");
            }
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>int</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>int</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>int</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static uint ConvertToUInt32(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is uint uintValue)
                return uintValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < uint.MinValue || doubleValue > uint.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(uint).FullName}: {uint.MinValue} to {uint.MaxValue}");
            }
            return Convert.ToUInt32(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>long</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>long</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>long</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static long ConvertToInt64(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is long longValue)
                return longValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < long.MinValue || doubleValue > long.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(long).FullName}: {long.MinValue} to {long.MaxValue}");
            }
            return Convert.ToInt64(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>ulong</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>ulong</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>ulong</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static ulong ConvertToUInt64(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is ulong ulongValue)
                return ulongValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < ulong.MinValue || doubleValue > ulong.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(ulong).FullName}: {ulong.MinValue} to {ulong.MaxValue}");
            }
            return Convert.ToUInt64(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>decimal</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>decimal</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>decimal</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static decimal ConvertToDecimal(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is decimal decimalValue)
                return decimalValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < (double)decimal.MinValue || doubleValue > (double)decimal.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(decimal).FullName}: {decimal.MinValue} to {decimal.MaxValue}");
            }
            return Convert.ToDecimal(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>float</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <param name="strict">If <c>true</c>, throws an exception if value does not fall within the min/max values of <c>float</c>.  Default is <c>false</c>.</param>
        /// <returns>A value of type <c>float</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static float ConvertToSingle(object value, bool strict = false)
        {
            if (value == null)
                return default;
            if (value is float floatValue)
                return floatValue;
            if (value.GetType().IsNumeric())
            {
                double doubleValue = (double)value;
                if (strict && (doubleValue < float.MinValue || doubleValue > float.MaxValue))
                    throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(float).FullName}: {float.MinValue} to {float.MaxValue}");
            }
            return Convert.ToSingle(value);
        }

        /// <summary>
        /// Converts the supplied value to <c>double</c> with optional strictness.
        /// </summary>
        /// <param name="value">A <c>string</c> or numeric value</param>
        /// <returns>A value of type <c>double</c></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static double ConvertToDouble(object value)
        {
            if (value == null)
                return default;
            if (value is double doubleValue)
                return doubleValue;
            return Convert.ToDouble(value);
        }
    }
}
