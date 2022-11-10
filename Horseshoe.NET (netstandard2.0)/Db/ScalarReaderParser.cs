using System;
using System.Data;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A set of basic data-to-collection converters that you can plug into any of the <c>Query.AsCollection()</c> family of functions.
    /// </summary>
    public static class ScalarReaderParser
    {
        /// <summary>
        /// Convert the first field of a data row to <c>string</c>
        /// </summary>
        public static Func<IDataReader, string> String { get; } = (IDataReader reader) => Zap.String(reader[0]);
        /// <summary>
        /// Convert the first field of a data row to <c>int</c>
        /// </summary>
        public static Func<IDataReader, int> Int { get; } = (IDataReader reader) => Zap.Int(reader[0]);
        /// <summary>
        /// Convert the first field of a data row to <c>nullable int</c>
        /// </summary>
        public static Func<IDataReader, int?> NInt { get; } = (IDataReader reader) => Zap.NInt(reader[0]);
        /// <summary>
        /// Convert the first field of a data row to <c>decimal</c>
        /// </summary>
        public static Func<IDataReader, decimal> Decimal { get; } = (IDataReader reader) => Zap.Decimal(reader[0]);
        /// <summary>
        /// Convert the first field of a data row to <c>nullable decimal</c>
        /// </summary>
        public static Func<IDataReader, decimal?> NDecimal { get; } = (IDataReader reader) => Zap.NDecimal(reader[0]);
        /// <summary>
        /// Convert the first field of a data row to <c>DateTime</c>
        /// </summary>
        public static Func<IDataReader, DateTime> DateTime { get; } = (IDataReader reader) => Zap.DateTime(reader[0]);
        /// <summary>
        /// Convert the first field of a data row to <c>nullable DateTime</c>
        /// </summary>
        public static Func<IDataReader, DateTime?> NDateTime { get; } = (IDataReader reader) => Zap.NDateTime(reader[0]);
    }
}
