using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Horseshoe.NET.Db
{
    public static class Extensions
    {
        /// <summary>
        /// Gets an array of <c>Type</c>s corresponding to the runtype types of the source query's data columns
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
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
        /// <param name="reader"></param>
        /// <returns></returns>
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

        public static string ToDMLString(this DbParameter parameter, DbPlatform? platform = null)
        {
            return string.Format
            (
                "{0} = {1}",
                DbUtil.RenderColumnName(parameter, platform: platform),
                DbUtil.Sqlize(parameter.Value, platform: platform)
            );
        }
    }
}
