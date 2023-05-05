using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A collection of utility methods for different numeric types.
    /// </summary>
    public static class NumberUtil
    {
        /// <summary>
        /// Rounds off a <c>double</c> value to the supplied number of decimal places.
        /// </summary>
        /// <param name="value">A <c>double</c> value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        public static double Trunc(double value, int decimalPlaces = 0)
        {
            if (decimalPlaces == 0)
            {
                return Math.Truncate(value);
            }
            else if (decimalPlaces > 0)
            {
                var multiplier = Math.Pow(10, decimalPlaces);
                return Math.Truncate(value * multiplier) / multiplier;
            }
            else
            {
                var multiplier = Math.Pow(10, -decimalPlaces);
                return Math.Truncate(value / multiplier) * multiplier;
            }
        }

        /// <summary>
        /// Evaluates a <c>short</c> value as a <c>byte</c>.
        /// </summary>
        /// <param name="value">A <c>short</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte EvalAsByte(short value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        /// <summary>
        /// Evaluates an <c>int</c> value as a <c>byte</c>.
        /// </summary>
        /// <param name="value">An <c>int</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte EvalAsByte(int value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>long</c> value as a <c>byte</c>.
        /// </summary>
        /// <param name="value">A <c>long</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte EvalAsByte(long value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>decimal</c> value as a <c>byte</c>.
        /// </summary>
        /// <param name="value">A <c>decimal</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte EvalAsByte(decimal value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>float</c> value as a <c>byte</c>.
        /// </summary>
        /// <param name="value">A <c>float</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte EvalAsByte(float value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>double</c> value as a <c>byte</c>.
        /// </summary>
        /// <param name="value">A <c>double</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>byte</c> to <c>byte</c> regardless if the value is greater than the max value of <c>byte</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>byte</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte EvalAsByte(double value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        /// <summary>
        /// Evaluates an <c>int</c> value as a <c>short</c>.
        /// </summary>
        /// <param name="value">An <c>int</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>short</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short EvalAsShort(int value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(short).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>long</c> value as a <c>short</c>.
        /// </summary>
        /// <param name="value">A <c>long</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>short</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short EvalAsShort(long value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(short).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>decimal</c> value as a <c>short</c>.
        /// </summary>
        /// <param name="value">A <c>decimal</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>short</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short EvalAsShort(decimal value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(short).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>float</c> value as a <c>short</c>.
        /// </summary>
        /// <param name="value">A <c>float</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>short</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short EvalAsShort(float value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(short).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>double</c> value as a <c>short</c>.
        /// </summary>
        /// <param name="value">A <c>double</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>short</c> to <c>short</c> regardless if the value is greater than the max value of <c>short</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>short</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short EvalAsShort(double value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(short).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>long</c> value as an <c>int</c>.
        /// </summary>
        /// <param name="value">A <c>long</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>int</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int EvalAsInt(long value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(int).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>decimal</c> value as an <c>int</c>.
        /// </summary>
        /// <param name="value">A <c>decimal</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>int</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int EvalAsInt(decimal value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(int).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>float</c> value as an <c>int</c>.
        /// </summary>
        /// <param name="value">A <c>float</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>int</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int EvalAsInt(float value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(int).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>double</c> value as an <c>int</c>.
        /// </summary>
        /// <param name="value">A <c>double</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>int</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int EvalAsInt(double value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(int).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>decimal</c> value as a <c>long</c>.
        /// </summary>
        /// <param name="value">A <c>decimal</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>long</c> to <c>long</c> regardless if the value is greater than the max value of <c>long</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>long</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long EvalAsLong(decimal value, bool force = false)
        {
            if (force || !(value < long.MinValue || value > long.MaxValue))
                return (long)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(long).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>float</c> value as a <c>long</c>.
        /// </summary>
        /// <param name="value">A <c>float</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>long</c> to <c>long</c> regardless if the value is greater than the max value of <c>long</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>long</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long EvalAsLong(float value, bool force = false)
        {
            if (force || !(value < long.MinValue || value > long.MaxValue))
                return (long)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(long).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>double</c> value as a <c>long</c>.
        /// </summary>
        /// <param name="value">A <c>double</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>long</c> to <c>long</c> regardless if the value is greater than the max value of <c>long</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>long</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long EvalAsLong(double value, bool force = false)
        {
            if (force || !(value < long.MinValue || value > long.MaxValue))
                return (long)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(long).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>float</c> value as a <c>decimal</c>.
        /// </summary>
        /// <param name="value">A <c>float</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>decimal</c> to <c>decimal</c> regardless if the value is greater than the max value of <c>decimal</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>decimal</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal EvalAsDecimal(float value, bool force = false)
        {
            if (force || !(value < (float)decimal.MinValue || value > (float)decimal.MaxValue))
                return (decimal)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(decimal).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>double</c> value as a <c>decimal</c>.
        /// </summary>
        /// <param name="value">A <c>double</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>decimal</c> to <c>decimal</c> regardless if the value is greater than the max value of <c>decimal</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>decimal</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal EvalAsDecimal(double value, bool force = false)
        {
            if (force || !(value < (double)decimal.MinValue || value > (double)decimal.MaxValue))
                return (decimal)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(decimal).FullName}");
        }

        /// <summary>
        /// Evaluates a <c>double</c> value as a <c>float</c>.
        /// </summary>
        /// <param name="value">A <c>double</c>.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>float</c> to <c>float</c> regardless if the value is greater than the max value of <c>float</c> or less than the min value, the default is <c>false</c>.</param>
        /// <returns>A <c>float</c> whose pre-cast value is <c>value</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float EvalAsFloat(double value, bool force = false)
        {
            if (force || !(value < float.MinValue || value > float.MaxValue))
                return (float)value;
            throw new ArgumentOutOfRangeException($"{value} is outside the range of {typeof(float).FullName}");
        }
    }
}
