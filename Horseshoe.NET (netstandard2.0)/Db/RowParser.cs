using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A flexible and efficient data row-to-object parser featuring dual modes covering tons of use cases 
    /// including out-of-the-box data column to object property support.
    /// </summary>
    public class RowParser<T>
    {
        /// <summary>
        /// A parsing mode that uses row data already extracted as plain objects.
        /// </summary>
        public Func<object[], T> ObjectParser { get; }

        /// <summary>
        /// A parsing mode that directly uses the data reader to get not only row data but also query metadata, if desired.
        /// </summary>
        public Func<IDataReader, T> ReaderParser { get; }

        /// <summary>
        /// Indicates whether this <c>RowParser</c> is in object parser mode.
        /// </summary>
        public bool IsObjectParser => ObjectParser != null;

        /// <summary>
        /// Indicates whether this <c>RowParser</c> is in reader parser mode.
        /// </summary>
        public bool IsReaderParser => ReaderParser != null;

        /// <summary>
        /// Creates a new <c>RowParser</c> in object parser mode.
        /// </summary>
        /// <param name="objectParser">An object parser</param>
        public RowParser(Func<object[], T> objectParser)
        {
            ObjectParser = objectParser;
        }

        /// <summary>
        /// Creates a new <c>RowParser</c> in reader parser mode.
        /// </summary>
        /// <param name="readerParser">A reader parser</param>
        public RowParser(Func<IDataReader, T> readerParser)
        {
            ReaderParser = readerParser;
        }

        /// <summary>
        /// A way to execute the object parser indirectly.
        /// </summary>
        /// <param name="objects">An <c>object[]</c></param>
        /// <returns>An instance of <c>T</c></returns>
        public T Parse(object[] objects)
        {
            return ObjectParser.Invoke(objects);
        }

        /// <summary>
        /// A way to execute the reader parser indirectly.
        /// </summary>
        /// <param name="reader">A data reader</param>
        /// <returns>An instance of <c>T</c></returns>
        public T Parse(DbDataReader reader)
        {
            return ReaderParser.Invoke(reader);
        }
   }


    /// <summary>
    /// A set of built-in reader parsers
    /// </summary>
    public static class RowParser
    { 
        /// <summary>
        /// Convert the first field of a data row to <c>string</c>
        /// </summary>
        public static RowParser<string> ScalarString { get; } = new RowParser<string>((IDataReader reader) => Zap.String(reader[0]));

        /// <summary>
        /// Convert the first field of a data row to <c>int</c>
        /// </summary>
        public static RowParser<int> ScalarInt { get; } = new RowParser<int>((IDataReader reader) => Zap.Int(reader[0]));

        /// <summary>
        /// Convert the first field of a data row to <c>nullable int</c>
        /// </summary>
        public static RowParser<int?> ScalarNInt { get; } = new RowParser<int?>((IDataReader reader) => Zap.NInt(reader[0]));

        /// <summary>
        /// Convert the first field of a data row to <c>decimal</c>
        /// </summary>
        public static RowParser<decimal> ScalarDecimal { get; } = new RowParser<decimal>((IDataReader reader) => Zap.Decimal(reader[0]));

        /// <summary>
        /// Convert the first field of a data row to <c>nullable decimal</c>
        /// </summary>
        public static RowParser<decimal?> ScalarNDecimal { get; } = new RowParser<decimal?>((IDataReader reader) => Zap.NDecimal(reader[0]));

        /// <summary>
        /// Convert the first field of a data row to <c>DateTime</c>
        /// </summary>
        public static RowParser<DateTime> ScalarDateTime { get; } = new RowParser<DateTime>((IDataReader reader) => Zap.DateTime(reader[0]));

        /// <summary>
        /// Convert the first field of a data row to <c>nullable DateTime</c>
        /// </summary>
        public static RowParser<DateTime?> ScalarNDateTime { get; } = new RowParser<DateTime?>((IDataReader reader) => Zap.NDateTime(reader[0]));

        /// <summary>
        /// Creates a generic RowParser based on the supplied <c>IDataReader</c> parser function/lambda/delegate.
        /// </summary>
        /// <typeparam name="T">The type of object returned by the parser.</typeparam>
        /// <param name="readerParser">An <c>IDataReader</c> parser.</param>
        /// <returns>A <c>RowParser</c></returns>
        public static RowParser<T> From<T>(Func<IDataReader, T> readerParser) => new RowParser<T>(readerParser);

        /// <summary>
        /// Creates a generic RowParser based on the supplied <c>object[]</c> parser function/lambda/delegate.
        /// </summary>
        /// <typeparam name="T">The type of object returned by the parser.</typeparam>
        /// <param name="objectParser">An <c>object[]</c> parser.</param>
        /// <returns></returns>
        public static RowParser<T> From<T>(Func<object[], T> objectParser) => new RowParser<T>(objectParser);

        /// <summary>
        /// Creates a new instance of <c>T</c> for each row and copies each field to its corresponding instance property. 
        /// <para>
        /// If a column name contains spaces it is matched with an identical property name with no spaces.
        /// </para>
        /// </summary>
        /// <typeparam name="T">A reference type with one no-args constructor including implied constructor.</typeparam>
        /// <param name="ignoreCase">If <c>true</c>, columns whose names would match property names if not for the letter case will be included, default is <c>false</c>.</param>
        /// <param name="strict">If <c>true</c>, raises an exception if all columns are not mapped, default is <c>false</c>.</param>
        /// <returns></returns>
        public static RowParser<T> BuildAutoParser<T>(bool ignoreCase = false, bool strict = false)
        {
            var type = typeof(T);
            TypeUtil.AssertIsReferenceType(type);
            var instanceProperties = TypeUtil.GetInstanceProperties(type);
            PropertyInfo[] propertiesToSet = null;

            return new RowParser<T>((IDataReader reader) =>
            {
                if (propertiesToSet == null)
                {
                    propertiesToSet = reader.GetDataColumns()
                        .Select(c => ignoreCase
                            ? instanceProperties.FirstOrDefault(p => string.Equals(p.Name, TextClean.RemoveAllWhitespace(c.ColumnName), StringComparison.OrdinalIgnoreCase))
                            : instanceProperties.SingleOrDefault(p => string.Equals(p.Name, TextClean.RemoveAllWhitespace(c.ColumnName)))
                        )
                        .ToArray();
                }
                object obj = TypeUtil.GetInstance(typeof(T));
                for (int i = 0; i < propertiesToSet.Length; i++)
                {
                    if (propertiesToSet[i] == null)
                    {
                        if (strict)
                            throw new ObjectMemberException("Column not mapped to property: " + reader.GetName(i));
                        continue;
                    }
                    if (reader.IsDBNull(i))
                        continue;

                    propertiesToSet[i].SetValue(obj, reader[i]);
                }
                return (T)obj;
            });
        }
    }
}
