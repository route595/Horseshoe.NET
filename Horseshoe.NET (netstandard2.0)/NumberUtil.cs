using System;

namespace Horseshoe.NET
{
    public static class NumberUtil
    {
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

        public static bool GreaterThan(double num1, double num2)
        {
            return num1 > num2;
        }

        public static bool GreaterThan<T>(T? num1, T? num2) where T : struct, IComparable<T>
        {
            if (!num1.HasValue || !num2.HasValue)
            {
                return false;
            }
            return num1.Value.CompareTo(num2.Value) > 0;
        }

        public static bool GreaterThan2<T,T2>(T? num1, T2? num2) where T : struct, IComparable where T2 : struct, IComparable
        {
            if (!num1.HasValue || !num2.HasValue)
            {
                return false;
            }
            return num1.Value.CompareTo(num2.Value) > 0;
        }

        public static bool LessThan(double num1, double num2)
        {
            return num1 < num2;
        }

        public static bool LessThan<T>(T? num1, T? num2) where T : struct, IComparable<T>
        {
            if (!num1.HasValue || !num2.HasValue)
            {
                return false;
            }
            return num1.Value.CompareTo(num2.Value) < 0;
        }

        public static bool LessThan2<T, T2>(T? num1, T2? num2) where T : struct, IComparable where T2 : struct, IComparable
        {
            if (!num1.HasValue || !num2.HasValue)
            {
                return false;
            }
            return num1.Value.CompareTo(num2.Value) < 0;
        }

        public static bool Equals(double num1, double num2)
        {
            return num1 == num2;
        }

        public static bool Equals<T>(T? num1, T? num2) where T : struct, IEquatable<T>
        {
            if (!num1.HasValue || !num2.HasValue)
            {
                return false;
            }
            return Equals(num1.Value, num2.Value);
        }

        public static bool Equals2<T,T2>(T? num1, T2? num2) where T : struct, IEquatable<T> where T2 : struct, IEquatable<T2>
        {
            if (!num1.HasValue || !num2.HasValue)
            {
                return false;
            }
            return Equals(num1.Value, num2.Value);
        }

        public static byte EvalAsByte(short value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ConversionException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        public static byte EvalAsByte(int value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ConversionException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        public static byte EvalAsByte(long value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ConversionException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        public static byte EvalAsByte(decimal value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ConversionException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        public static byte EvalAsByte(float value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ConversionException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        public static byte EvalAsByte(double value, bool force = false)
        {
            if (force || !(value < byte.MinValue || value > byte.MaxValue))
                return (byte)value;
            throw new ConversionException($"{value} is outside the range of {typeof(byte).FullName}");
        }

        public static short EvalAsShort(int value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ConversionException($"{value} is outside the range of {typeof(short).FullName}");
        }

        public static short EvalAsShort(long value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ConversionException($"{value} is outside the range of {typeof(short).FullName}");
        }

        public static short EvalAsShort(decimal value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ConversionException($"{value} is outside the range of {typeof(short).FullName}");
        }

        public static short EvalAsShort(float value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ConversionException($"{value} is outside the range of {typeof(short).FullName}");
        }

        public static short EvalAsShort(double value, bool force = false)
        {
            if (force || !(value < short.MinValue || value > short.MaxValue))
                return (short)value;
            throw new ConversionException($"{value} is outside the range of {typeof(short).FullName}");
        }

        public static int EvalAsInt(long value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ConversionException($"{value} is outside the range of {typeof(int).FullName}");
        }

        public static int EvalAsInt(decimal value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ConversionException($"{value} is outside the range of {typeof(int).FullName}");
        }

        public static int EvalAsInt(float value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ConversionException($"{value} is outside the range of {typeof(int).FullName}");
        }

        public static int EvalAsInt(double value, bool force = false)
        {
            if (force || !(value < int.MinValue || value > int.MaxValue))
                return (int)value;
            throw new ConversionException($"{value} is outside the range of {typeof(int).FullName}");
        }

        public static long EvalAsLong(decimal value, bool force = false)
        {
            if (force || !(value < long.MinValue || value > long.MaxValue))
                return (long)value;
            throw new ConversionException($"{value} is outside the range of {typeof(long).FullName}");
        }

        public static long EvalAsLong(float value, bool force = false)
        {
            if (force || !(value < long.MinValue || value > long.MaxValue))
                return (long)value;
            throw new ConversionException($"{value} is outside the range of {typeof(long).FullName}");
        }

        public static long EvalAsLong(double value, bool force = false)
        {
            if (force || !(value < long.MinValue || value > long.MaxValue))
                return (long)value;
            throw new ConversionException($"{value} is outside the range of {typeof(long).FullName}");
        }

        public static decimal EvalAsDecimal(float value, bool force = false)
        {
            if (force || !(value < (double)decimal.MinValue || value > (double)decimal.MaxValue))
                return (decimal)value;
            throw new ConversionException($"{value} is outside the range of {typeof(decimal).FullName}");
        }

        public static decimal EvalAsDecimal(double value, bool force = false)
        {
            if (force || !(value < (double)decimal.MinValue || value > (double)decimal.MaxValue))
                return (decimal)value;
            throw new ConversionException($"{value} is outside the range of {typeof(decimal).FullName}");
        }

        public static float EvalAsFloat(decimal value, bool force = false)
        {
            if (force || !((double)value < float.MinValue || (double)value > float.MaxValue))
                return (float)value;
            throw new ConversionException($"{value} is outside the range of {typeof(float).FullName}");
        }

        public static float EvalAsFloat(double value, bool force = false)
        {
            if (force || !(value < float.MinValue || value > float.MaxValue))
                return (float)value;
            throw new ConversionException($"{value} is outside the range of {typeof(float).FullName}");
        }

        //public static double EvalAsDouble(decimal value, bool force = false)
        //{
        //    if (force || !((double)value < double.MinValue || (double)value > double.MaxValue))
        //        return (double)value;
        //    throw new ConversionException($"{value} is outside the range of {typeof(double).FullName}");
        //}
    }
}
