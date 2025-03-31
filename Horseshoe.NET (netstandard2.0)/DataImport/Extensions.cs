//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security;
//using System.Text;
//using Microsoft.Extensions.Primitives;

//using Horseshoe.NET.IO;
//using Horseshoe.NET.Collections;
//using Horseshoe.NET.Text;

//namespace Horseshoe.NET.DataImport
//{
//    /// <summary>
//    /// A collection of extension methods for <c>Horseshoe.NET.DataImport</c>
//    /// </summary>
//    public static class Extensions
//    {
//        /// <summary>
//        /// Imports data from a delimited text file or <c>string</c>
//        /// </summary>
//        /// <param name="dataImport">A <c>DataImport</c> instance.</param>
//        /// <param name="columns"><c>IColumn</c>s define which data columns to import and other properties 
//        /// such as the data type. If ommitted, <c>string</c> columns are assumed.</param>
//        /// <returns>This <c>DataImport</c> instance.</returns>
//        /// <exception cref="DirectoryNotFoundException"></exception>
//        /// <exception cref="FileNotFoundException"></exception>
//        /// <exception cref="IOException"></exception>
//        /// <exception cref="PathTooLongException"></exception>
//        /// <exception cref="SecurityException"></exception>
//        /// <exception cref="UnauthorizedAccessException"></exception>
//        /// <exception cref="ValidationException"></exception>
//        public static TabularData ImportDelimitedText(this TabularData dataImport, params IColumn[] columns)
//        {
//            // validation
//            if (dataImport.Options.Delimiters == null || dataImport.Options.Delimiters.Length == 0)
//                throw new ValidationException("DataImport.Options must contain at least one delimiter for delimited text imports");

//            // prep
//            dataImport.Clear();
//            dataImport.Columns = columns ?? Array.Empty<IColumn>();
        
//            // import - step 1 of 3 - read file
//            string rawData;
//            if (dataImport.DataSource is FilePath file)
//            {
//                rawData = file.ReadAllText();
//            }
//            else if (dataImport.DataSource is string stringData)
//            {
//                rawData = stringData;
//            }
//            else throw new ThisShouldNeverHappenException("DataImport.DataSource is an instance of either FilePath or string");

//            // import - step 2 of 3 - parse raw rows
//            var rawRows = ListUtil.Prune(rawData.Replace("\r\n", "\n").Split('\n'), pruneOptions: dataImport.Options.PruneOptions);
//            if (dataImport.Options.HasHeaderRow && rawRows.Count > 0)
//                rawRows.RemoveAt(0);

//            // import - step 3 of 3 - parse raw row items
//            var rawImport = new List<string[]>();
//            for (int i = 0; i < rawRows.Count; i++)
//            {
//                var rawRowItems = ArrayUtil.TrimAll(rawRows[i].Split(dataImport.Options.Delimiters));
//                if (i == 0 && (columns == null || !columns.Any()))
//                {
//                    dataImport.Columns = columns = Enumerable.Range(0, rawRowItems.Length).Select(idx => Column.String("col" + idx)).ToArray();
//                }

//                // validation
//                if (rawRowItems.Length != dataImport.ColumnCount)
//                    throw new DataImportException("The row item count does not match the column count: " + dataImport.ColumnCount + " != " + rawRowItems.Length);
                
//                rawImport.Add(rawRowItems);
//            }
//            dataImport.PeekRawImportedText?.Invoke(rawImport);

//            return dataImport;
//        }

//        /// <summary>
//        /// Imports data from a fixed-width text file or <c>string</c>
//        /// </summary>
//        /// <param name="dataImport">A <c>DataImport</c> instance.</param>
//        /// <param name="columns"><c>IColumn</c>s define which data columns to import and other properties 
//        /// such as their data types.</param>
//        /// <returns>This <c>DataImport</c> instance.</returns>
//        /// <exception cref="PathTooLongException"></exception>
//        /// <exception cref="FileNotFoundException"></exception>
//        /// <exception cref="DirectoryNotFoundException"></exception>
//        /// <exception cref="IOException"></exception>
//        /// <exception cref="UnauthorizedAccessException"></exception>
//        /// <exception cref="SecurityException"></exception>
//        /// <exception cref="ValidationException"></exception>
//        public static TabularData ImportFixedWidthText(this TabularData dataImport, params IColumn[] columns)
//        {
//            // validation
//            if (columns == null || !columns.Any())
//                throw new ValidationException("At least one column must be specified for fixed-width imports");
//            if (columns.Any(c => c == null || c.StartPosition < 0 || c.Width < 1))
//                throw new ValidationException("Columns may not be null and must have non-negative start positions and non-zero widths for fixed-width imports");

//            // prep
//            dataImport.Clear();
//            dataImport.Columns = columns;

//            // import - step 1 of 3 - read file
//            string rawData;
//            if (dataImport.DataSource is FilePath file)
//            {
//                rawData = file.ReadAllText();
//            }
//            else if (dataImport.DataSource is string stringData)
//            {
//                rawData = stringData;
//            }
//            else throw new ThisShouldNeverHappenException("DataImport.DataSource is an instance of either FilePath or string");

//            // import - step 2 of 3 - parse raw rows
//            var rawRows = ListUtil.Prune(rawData.Replace("\r\n", "\n").Split('\n'), pruneOptions: dataImport.Options.PruneOptions);
//            if (dataImport.Options.HasHeaderRow && rawRows.Count > 0)
//                rawRows.RemoveAt(0);

//            // import - step 3 of 3 - parse raw row items
//            var rawImport = new List<string[]>();
//            foreach (var rawRow in rawRows)
//            {
//                var rawRowItems = new string[dataImport.ColumnCount];
//                for (var i = 0; i < dataImport.ColumnsCount; i++)
//                {
//                    rawRowItems[i] = rawRow.Substring(dataImport.Columns[i].StartPosition, dataImport.Columns[i].Width);
//                }
//                //dataImport.RawImportedData.Add(Array.ConvertAll(rawRowItems, new Converter<string, object>(s => (object)s)));
//                rawImport.Add(rawRowItems);
//            }
//            dataImport.PeekRawImportedText?.Invoke(rawImport);

//            return dataImport;
//        }

//        /// <summary>
//        /// Fills the <c>DataImport</c> collection with the imported data
//        /// </summary>
//        /// <param name="dataImport"></param>
//        /// <param name="autoTrunc"></param>
//        /// <returns>This <c>DataImport</c> instance.</returns>
//        public static TabularData Render(this TabularData dataImport, AutoTruncate autoTrunc = AutoTruncate.Trim)
//        {
//            // prep
//            dataImport.Clear();

//            // populate
//            object[] items;
//            foreach (var dataRow in dataImport.RawImportedData)
//            {
//                if (dataRow == null)
//                {
//                    dataImport.Add(null);
//                    continue;
//                }

//                items = new object[dataRow.Length];
//                for (int i = 0; i < dataRow.Length; i++)
//                {
//                    // column
//                    if (dataImport.Columns.Length > i)
//                    {
//                        // client-supplied parser
//                        if (dataImport.Columns[i].Parser != null)
//                        {
//                            items[i] = dataImport.Columns[i].Parser.Invoke(dataRow[i]);
//                        }

//                        // system auto-parser
//                        else
//                        {
//                            items[i] = Zap.To
//                            (
//                                dataImport.Columns[i].DataType, 
//                                dataRow[i],
//                                numberStyle: dataImport.Columns[i].ParseNumberStyle,
//                                provider: dataImport.Columns[i].ParseFormatProvider,
//                                dateTimeStyle: dataImport.Columns[i].ParseDateTimeStyle,
//                                dateFormat: dataImport.Columns[i].ParseDateFormat,
//                                locale: dataImport.Columns[i].ParseLocale,
//                                trueValues: dataImport.Columns[i].ParseTrueValues,
//                                falseValues: dataImport.Columns[i].ParseFalseValues,
//                                encoding: dataImport.Options.Encoding,
//                                ignoreCase: dataImport.Columns[i].IgnoreCase,
//                                strict: dataImport.Columns[i].Strict
//                            );
//                        }
//                    }

//                    // no column
//                    else
//                    {
//                        items[i] = dataRow[i];
//                    }
//                }
//                dataImport.Add(items);
//            }
//            return dataImport;
//        }

//        /// <summary>
//        /// Expor
//        /// </summary>
//        /// <param name="dataImport"></param>
//        /// <returns></returns>
//        public static List<string[]> ExportToStrings(this TabularData dataImport)
//        {
//            var list = new List<string[]>();

//            // populate
//            string[] items;
//            foreach (var row in dataImport)
//            {
//                if (row == null)
//                {
//                    list.Add(null);
//                    continue;
//                }

//                items = new string[row.Length];
//                for (int i = 0; i < row.Length; i++)
//                {
//                    // column
//                    if (dataImport.Columns.Length > i)
//                    {
//                        // client-supplied formatter
//                        if (dataImport.Columns[i].Formatter != null)
//                        {
//                            items[i] = dataImport.Columns[i].Formatter.Invoke(row[i]);
//                            continue;
//                        }
//                    }

//                    // no column and/or no formatter
//                    items[i] = row[i]?.ToString();
//                }
//                list.Add(items);
//            }
//            return list;
//        }

//        ///// <summary>
//        ///// Renders of list of <c>Column</c> to text
//        ///// </summary>
//        ///// <param name="cols">a collection of <c>Column</c>s</param>
//        ///// <param name="nullReplacement">how to display <c>null</c> values</param>
//        ///// <returns>a <c>string</c> rendition of the collection</returns>
//        //public static string Render(this IList<Column> cols, string nullReplacement = "[null]")
//        //{
//        //    // render the column values
//        //    var renderedLists = cols
//        //        .Select(c => c.Render(nullReplacement: nullReplacement))
//        //        .ToList();

//        //    // calculate the rendered column widths
//        //    var widths = renderedLists
//        //        .Select(l => l.Max(s => s.Length))
//        //        .ToList();

//        //    // adjust for column name length and add 1 char padding
//        //    for (int i = 0; i < widths.Count; i++)
//        //    {
//        //        widths[i] = Math.Max(widths[i], cols[i].Name?.Length ?? 0) + 1;
//        //    }

//        //    // build the monospace grid
//        //    var strb = new StringBuilder();
//        //    for (int i = 0; i < widths.Count; i++)
//        //    {
//        //        strb.Append((cols[i].Name ?? "C0l " + (i + 1)).PadRight(widths[i]));
//        //    }
//        //    strb.AppendLine();
//        //    for (int i = 0; i < widths.Count; i++)
//        //    {
//        //        strb.Append(new string('-', (cols[i].Name ?? "C0l " + (i + 1)).Length).PadRight(widths[i]));
//        //    }
//        //    strb.AppendLine();
//        //    for (int r = 0, maxCount = renderedLists.Max(s => s.Count); r < maxCount; r++)
//        //    {
//        //        for (int i = 0; i < widths.Count; i++)
//        //        {
//        //            try
//        //            {
//        //                strb.Append(renderedLists[i][r].PadRight(widths[i]));
//        //            }
//        //            catch (ArgumentOutOfRangeException)
//        //            {
//        //                strb.Append("[void]".PadRight(widths[i]));
//        //            }
//        //        }
//        //        strb.AppendLine();
//        //    }
//        //    return strb.ToString();
//        //}

//        ///// <summary>
//        ///// Renders a <c>Column</c> to text
//        ///// </summary>
//        ///// <param name="col">a <c>Column</c></param>
//        ///// <param name="nullReplacement">how to display <c>null</c> values</param>
//        ///// <returns>a <c>string</c> rendition of the <c>Column</c></returns>
//        //public static List<string> Render(this Column col, string nullReplacement = "[null]")
//        //{
//        //    //var list = new List<string>(col.Select(o => col.Formatter == null || o is DataError ? o?.ToString() ?? nullReplacement : col.Formatter.Invoke(o) ?? nullReplacement));
//        //    var list = new List<string>();
//        //    foreach (var value in col)
//        //    {
//        //        if (value == null)
//        //        {
//        //            list.Add(nullReplacement);
//        //        }
//        //        else if (col.Formatter == null || value is DataError)
//        //        {
//        //            list.Add(value.ToString());
//        //        }
//        //        else
//        //        {
//        //            list.Add(col.Formatter.Invoke(value));
//        //        }
//        //    }
//        //    return list;
//        //}
//    }
//}
