using System;
using System.Data;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Extension methods for Horsehoe.NET.Db and consumers.
    /// </summary>
    public static class ExtensionAbstractions
    {
        /// <summary>
        /// Gets the value of the specified column cast as type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">A value type or <c>string</c>.</typeparam>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <returns>The column value.</returns>
        /// <exception cref="ValidationException">If the type is neither <c>string</c> nor a value type.</exception>
        /// <exception cref="InvalidCastException">If the value cannot be cast.</exception>
        public static T Get<T>(this IDataReader reader, string columnName)
        {
            if (typeof(T) == typeof(string) || typeof(T).IsEnum)
                return Horseshoe.NET.Zap.To<T>(reader[columnName]);
            if (!typeof(T).IsValueType)
                throw new ValidationException("Not a value type: " + typeof(T).FullName);
            return (T)reader[columnName];
        }

        /// <summary>
        /// Gets the value of the specified column cast as type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">A value type or <c>string</c>.</typeparam>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnIndex">A column index.</param>
        /// <returns>The column value.</returns>
        /// <exception cref="ValidationException">If the type is neither <c>string</c> nor a value type.</exception>
        /// <exception cref="InvalidCastException">If the value cannot be cast.</exception>
        public static T Get<T>(this IDataReader reader, int columnIndex)
        {
            if (typeof(T) == typeof(string) || typeof(T).IsEnum)
                return Horseshoe.NET.Zap.To<T>(reader[columnIndex]);
            if (!typeof(T).IsValueType)
                throw new ValidationException("Not a value type: " + typeof(T).FullName);
            return (T)reader[columnIndex];
        }

        /// <summary>
        /// Gets the value of the specified column cast as type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">A value type or <c>string</c>.</typeparam>
        /// <param name="row">A <c>DataTable</c> row.</param>
        /// <param name="columnName">A column name.</param>
        /// <returns>The column value.</returns>
        /// <exception cref="ValidationException">If the type is neither <c>string</c> nor a value type.</exception>
        /// <exception cref="InvalidCastException">If the value cannot be cast.</exception>
        public static T Get<T>(this DataRow row, string columnName)
        {
            if (typeof(T) == typeof(string) || typeof(T).IsEnum)
                return Horseshoe.NET.Zap.To<T>(row[columnName]);
            if (!typeof(T).IsValueType)
                throw new ValidationException("Not a value type: " + typeof(T).FullName);
            return (T)row[columnName];
        }

        /// <summary>
        /// Gets the value of the specified column cast as type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">A value type or <c>string</c>.</typeparam>
        /// <param name="row">A <c>DataTable</c> row.</param>
        /// <param name="columnIndex">A column index.</param>
        /// <returns>The column value.</returns>
        /// <exception cref="ValidationException">If the type is neither <c>string</c> nor a value type.</exception>
        /// <exception cref="InvalidCastException">If the value cannot be cast.</exception>
        public static T Get<T>(this DataRow row, int columnIndex)
        {
            if (typeof(T) == typeof(string) || typeof(T).IsEnum)
                return Horseshoe.NET.Zap.To<T>(row[columnIndex]);
            if (!typeof(T).IsValueType)
                throw new ValidationException("Not a value type: " + typeof(T).FullName);
            return (T)row[columnIndex];
        }

        /// <summary>
        /// Gets the value of the specified column converted to type <c>T</c> or <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A nullable type (e.g. <c>string</c>, <c>int?</c>).</typeparam>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <returns></returns>
        public static T Zap<T>(this IDataReader reader, string columnName)
        {
            return Horseshoe.NET.Zap.To<T>(reader[columnName]);
        }

        /// <summary>
        /// Gets the value of the specified column converted to type <c>T</c> or <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A nullable type (e.g. <c>string</c>, <c>int?</c>).</typeparam>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnIndex">A column index.</param>
        /// <returns></returns>
        public static T Zap<T>(this IDataReader reader, int columnIndex)
        {
            return Horseshoe.NET.Zap.To<T>(reader[columnIndex]);
        }

        /// <summary>
        /// Gets the value of the specified column converted to type <c>T</c> or <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A nullable type (e.g. <c>string</c>, <c>int?</c>).</typeparam>
        /// <param name="row">A <c>DataTable</c> row.</param>
        /// <param name="columnName">A column name.</param>
        /// <returns></returns>
        public static T Zap<T>(this DataRow row, string columnName)
        {
            return Horseshoe.NET.Zap.To<T>(row[columnName]);
        }

        /// <summary>
        /// Gets the value of the specified column converted to type <c>T</c> or <c>null</c>.
        /// </summary>
        /// <typeparam name="T">A nullable type (e.g. <c>string</c>, <c>int?</c>).</typeparam>
        /// <param name="row">A <c>DataTable</c> row.</param>
        /// <param name="columnIndex">A column index.</param>
        /// <returns></returns>
        public static T Zap<T>(this DataRow row, int columnIndex)
        {
            return Horseshoe.NET.Zap.To<T>(row[columnIndex]);
        }

    }
}
