namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A suite of utility methods for value type (struct) values.
    /// </summary>
    public static class ValueUtil
    {
        /// <summary>
        /// An easy-to-use value sniffing method for nullable structs.  Built for numeric types.
        /// </summary>
        /// <param name="inValue">The nullable value.</param>
        /// <param name="value">The non-nullable value, if any.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool TryHasValue<T>(T? inValue, out T value) where T : struct
        {
            if (inValue.HasValue)
            {
                value = inValue.Value;
                return true;
            }
            value = default;
            return false;
        }
    }
}
