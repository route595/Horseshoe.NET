using System;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A set of extension methods for Horseshoe.NET.ObjectsTypesAndValues
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether this type is one of the numeric types. 
        /// Includes <c>int</c>, <c>uint</c>, <c>decimal</c> and <c>double</c> but
        /// does not include nullable types (e.g. <c>int?</c>, etc.) nor does it include <c>char</c>.
        /// </summary>
        /// <param name="type">A runtime type</param>
        /// <param name="maxBits">
        /// Takes into account only types representing up to this number of bits.
        /// <list type="table">
        /// <item><c>0</c> - All numeric types (default)</item>
        /// <item><c>8</c> - <c>sbyte</c> and <c>byte</c></item>
        /// <item><c>16</c> - 8-bit types plus <c>short</c> and <c>ushort</c></item>
        /// <item><c>32</c> - 16-bit types plus <c>int</c> and <c>uint</c></item>
        /// <item><c>64</c> - 32-bit types plus <c>long</c> and <c>ulong</c></item>
        /// </list>
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool IsNumeric(this Type type, int maxBits = default)
        {
            switch (maxBits)
            {
                case 0:
                    return IsNumeric(type, maxBits: 64) ||
                        type == typeof(decimal) ||
                        type == typeof(float) ||
                        type == typeof(double);
                case 8:
                    return
                        type == typeof(sbyte) ||
                        type == typeof(byte);
                case 16:
                    return IsNumeric(type, maxBits: 8) ||
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
