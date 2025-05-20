using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A small set of extension methods for database related tasks
    /// </summary>
    public static class Extensions
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

        /// <summary>
        /// Gets an array of <c>Type</c>s corresponding to the runtype types of the source query's data columns
        /// </summary>
        /// <param name="reader">An open data reader.</param>
        /// <returns>An array of <c>Type</c>s.</returns>
        public static Type[] GetFieldTypes(this IDataReader reader)
        {
            //if (reader.IsClosed)
            //{
            //    throw new UtilityException("Cannot perform this operation on a closed reader");
            //}
            var types = new List<Type>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                types.Add(reader.GetFieldType(i));
            }
            return types.ToArray();
        }

        /// <summary>
        /// Derives an array of <c>DataColumn</c>s approximating the structure of these results adapted to a <c>DataTable</c>
        /// </summary>
        /// <param name="reader">An open data reader.</param>
        /// <returns>An array of column data.</returns>
        public static DataColumn[] GetDataColumns(this IDataReader reader)
        {
            //if (reader.IsClosed)
            //{
            //    throw new UtilityException("Cannot perform this operation on a closed reader");
            //}
            var columns = new List<DataColumn>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add
                (
                    new DataColumn
                    {
                        ColumnName = reader.GetName(i),
                        DataType = reader.GetFieldType(i)
                    }
                );
            }
            return columns.ToArray();
        }

        /// <summary>
        /// Converts this parameter to SQL syntax for inserting or updating data
        /// </summary>
        /// <param name="parameter">A DB parameter (column name and value).</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns></returns>
        public static string ToDMLString(this DbParameter parameter, DbProvider? provider = null)
        {
            return string.Format
            (
                "{0} = {1}",
                DbUtil.RenderColumnName(parameter, provider: provider ?? DbSettings.DefaultProvider),
                DbUtil.Sqlize(parameter.Value, provider: provider ?? DbSettings.DefaultProvider)
            );
        }

        ///// <summary>
        ///// Specifies a provider to set as this filter's default.
        ///// </summary>
        ///// <param name="filter">A filter.</param>
        ///// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        ///// <returns>The filter.</returns>
        //public static IFilter On(this IFilter filter, DbProvider provider)
        //{
        //    filter.Provider = provider;
        //    return filter;
        //}
    }
}
