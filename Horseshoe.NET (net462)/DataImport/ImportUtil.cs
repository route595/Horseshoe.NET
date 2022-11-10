using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.DataImport
{
    public static class ImportUtil
    {
        //public static IDictionary<string,PropertyInfo> MapColumnsToProperties<T>(IList<Column> columns) where T : class, new()
        //{
        //    var properties = ObjectUtil.GetInstanceProperties<T>();
        //    var dict = new Dictionary<string, PropertyInfo>();
        //    foreach (var column in columns)
        //    {
        //        // validation
        //        //if (string.IsNullOrEmpty(column.Name) || column.NotMapped)
        //        //    continue;

        //        // step 1 - direct match
        //        var prop = properties.SingleOrDefault(p => string.Equals(p.Name, column.Name));

        //        // step 2 - case insensitive match
        //        if (prop == null)
        //        {
        //            var matches = properties
        //                .Where(p => string.Equals(p.Name, column.Name, StringComparison.CurrentCultureIgnoreCase))
        //                .ToList();
        //            switch (matches.Count)
        //            {
        //                case 0: 
        //                    break;
        //                case 1:
        //                    prop = matches.Single();
        //                    break;
        //                default:
        //                    throw new DataImportException("More than one property matches the column: " + column.Name);
        //            }
        //        }

        //        // step 3 - normalized match
        //        if (prop == null)
        //        {
        //            var matches = properties
        //                .Where(p => string.Equals(p.Name.Replace("_", ""), TextClean.Remove(column.Name, ' ', '_', '-'), StringComparison.CurrentCultureIgnoreCase))
        //                .ToList();
        //            switch (matches.Count)
        //            {
        //                case 0:
        //                    break;
        //                case 1:
        //                    prop = matches.Single();
        //                    break;
        //                default:
        //                    throw new DataImportException("More than one property matches the column: " + column.Name);
        //            }
        //        }

        //        if (prop != null)
        //        {
        //            if (dict.ContainsValue(prop))
        //            {
        //                throw new DataImportException("More than one column matches the property: " + prop.Name);
        //            }
        //            dict.Add(column.Name, prop);
        //        }
        //    }
        //    return dict;
        //}

        public static string PostParseString(string str, AutoTruncate autoTrunc)
        {
            if (str[0].In(ImportConstants.DoubleQuotes) && str[str.Length - 1].In(ImportConstants.DoubleQuotes))
            {
                var strTemp = str.Substring(1, str.Length - 2);
                bool clearOfQuotes = true;
                foreach (var chr in strTemp)
                {
                    if (chr.In(ImportConstants.DoubleQuotes))
                    {
                        clearOfQuotes = false;
                        break;
                    }
                }
                if (clearOfQuotes)
                {
                    str = strTemp;
                }
            }
            switch (autoTrunc)
            {
                case AutoTruncate.None:
                default:
                    return str;
                case AutoTruncate.Trim:
                    return str.Trim();
                case AutoTruncate.Zap:
                    return Zap.String(str);
            }
        }

        internal static bool IsBlankRow(IEnumerable<string> values)
        {
            if (values.Count() == 0)
                return true;
            if (values.Count() == 1)
                return string.IsNullOrWhiteSpace(values.Single());
            return false;
        }
    }
}
