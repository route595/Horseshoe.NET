using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.IO;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// The backing structure and logic behind data imports
    /// </summary>
    public class TabularData : List<ImportedDataRow>
    {
        private static string MessageRelayGroup => DataImportConstants.MessageRelayGroup;

        /// <summary>
        /// The <c>Column</c>s used to define the import
        /// </summary>
        public Column[] Columns { get; set; }

        /// <summary>
        /// Returns the number of columns added to this instance-as-a-list.
        /// </summary>
        public int ColumnCount => Columns?.Length ?? 0;

        /// <summary>
        /// Returns <c>true</c> if any columns have been added to this instance-as-a-list.
        /// </summary>
        public bool HasColumns => ColumnCount > 0;

        /// <summary>
        /// Data import options e.g. <c>bool HasHeaderRow</c>, <c>char[] Delimiters</c>
        /// </summary>
        public DataImportOptions Options { get; set; }

        /// <summary>
        /// Errors may be populated during certain import operations e.g. <c>ImportDelimitedText()</c>
        /// </summary>
        public IList<ImportError> ImportErrors { get; set; }

        /// <summary>
        /// Indicates whether or not this was a fixed-width data import.
        /// </summary>
        public bool IsFixedWidth { get; set; }

        /// <summary>
        /// Creates <c>ImportErrors</c>, if applicable, and adds the supplied error
        /// </summary>
        /// <param name="importError">An import error</param>
        public void AddImportError(ImportError importError)
        {
            if (ImportErrors == null)
                ImportErrors = new List<ImportError>();
            ImportErrors.Add(importError);
        }

        /// <summary>
        /// Gets just the values corresponding to the indicated column.
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="column">The column</param>
        /// <returns>The values corresponding to <c>column</c></returns>
        /// <exception cref="DataImportException"></exception>
        public IList<T> GetColumnValues<T>(Column column)
        {
            var columnIndex = Array.IndexOf(Columns, column);
            if (columnIndex == -1)
                throw new Exception("Column not found: name=" + TextUtil.Reveal(column.Name) + ", datatype=" + column.DataType.FullName + (IsFixedWidth ? ", startposition=" + column.StartPosition + ", width=" + column.Width : ""));
            return GetColumnValuesTypedInternal<T>(columnIndex);
        }

        /// <summary>
        /// Gets just the values corresponding to the indicated column.
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="columnName">The column name</param>
        /// <returns>The values corresponding to column name</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public IList<T> GetColumnValues<T>(string columnName)
        {
            return GetColumnValues<T>(Columns.FirstOrDefault(c => c.Name == columnName) ?? throw new Exception("Column name not found: " + columnName));
        }

        /// <summary>
        /// Gets just the values corresponding to the indicated column.
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="columnNumber">The 1-based column number</param>
        /// <returns>The values corresponding to column number</returns>
        /// <exception cref="DataImportException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public IList<T> GetColumnValues<T>(int columnNumber)
        {
            return GetColumnValuesTypedInternal<T>(columnNumber - 1);
        }

        internal IList<object> GetColumnValuesInternal(int columnIndex)
        {
            var list = new List<object>();
            foreach (var row in this)
            {
                if (row.IsBlank)
                    continue;
                list.Add(row.Values[columnIndex]);
            }
            return list;
        }

        internal IList<T> GetColumnValuesTypedInternal<T>(int columnIndex)
        {
            var list = new List<T>();
            foreach (var row in this)
            {
                if (row.IsBlank)
                    continue;
                try
                {
                    list.Add((T)row.Values[columnIndex]);
                }
                catch (Exception icex)   // e.g.g InvalidCastException, IndexOutOfBoundsException
                {
                    throw new DataImportException
                    (
                        new ImportError 
                        { 
                            Row = row, 
                            ColumnNumber = columnIndex + 1, 
                            Message = icex.Message 
                        }, 
                        icex
                    );
                }
            }
            return list;
        }

        /// <summary>
        /// Retrieves the values associated with the indicated column index using the argument format or the column's formatting, if supplied.
        /// </summary>
        /// <param name="column">A Column</param>
        /// <param name="format">An optional <c>string</c> format to which the column values will be rendered.  Overrides the column's format.</param>
        /// <returns>A list of the <c>string</c> formatted values associated with the indicated column</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public IList<string> FormatColumnValues(Column column, string format = null)
        {
            var columnIndex = Array.IndexOf(Columns, column);
            if (columnIndex == -1)
                throw new Exception("Column not found: name=" + TextUtil.Reveal(column.Name) + ", datatype=" + column.DataType.FullName + (IsFixedWidth ? ", startposition=" + column.StartPosition + ", width=" + column.Width : ""));
            return FormatColumnValuesInternal(columnIndex, column, format);
        }

        /// <summary>
        /// Retrieves the values associated with the indicated column name using the argument format or the column's formatting, if supplied.
        /// </summary>
        /// <param name="columnName">A column name</param>
        /// <param name="format">An optional <c>string</c> format to which the column values will be rendered.  Overrides the column's format.</param>
        /// <returns>A list of the <c>string</c> formatted values associated with the indicated column</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public IList<string> FormatColumnValues(string columnName, string format = null)
        {
            return FormatColumnValues(Columns.FirstOrDefault(c => c.Name == columnName) ?? throw new Exception("Column name not found: " + columnName), format: format);
        }

        /// <summary>
        /// Retrieves the values associated with the indicated column using the argument format or the column's formatting, if supplied.
        /// </summary>
        /// <param name="columnNumber">The 1-based column number</param>
        /// <param name="format">An optional <c>string</c> format to which the column values will be rendered.  Overrides the column's format.</param>
        /// <returns>A list of the <c>string</c> formatted values associated with the indicated column</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public IList<string> FormatColumnValues(int columnNumber, string format = null)
        {
            return FormatColumnValuesInternal(columnNumber - 1, Columns[columnNumber - 1], format);
        }

        internal IList<string> FormatColumnValuesInternal(int columnIndex, Column column, string format)
        {
            var list = new List<string>();
            foreach (var value in GetColumnValuesInternal(columnIndex))
            {
                list.Add(column.Format(value));
            }
            return list;
        }

        /// <summary>
        /// Export this instance to a <c>TextGrid</c>.
        /// </summary>
        /// <returns></returns>
        public TextGrid ExportToTextGrid()
        {
            var textGrid = new EZGrid(Columns.Select(c => new Horseshoe.NET.Text.TextGrid.Column(c)));
            foreach (var row in this)
            {
                textGrid.AddRow(row.Values);
            }
            return textGrid;
        }

        /// <summary>
        /// Parses tabular data from a fixed-width text source.
        /// </summary>
        /// <param name="file">The source text file.</param>
        /// <param name="columns">Required parsing and formatting instructions for each column.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportFixedWidthTextFile(FilePath file, Column[] columns, DataImportOptions options = null)
        {
            string[] rawRows = ArrayUtil.Prune
            (
                options?.Encoding != null 
                    ? file.ReadAllLines(options.Encoding) 
                    : file.ReadAllLines()
            );
            return ImportFixedWidthText(rawRows, columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a fixed-width text source.
        /// </summary>
        /// <param name="rawText">The source multiline text.</param>
        /// <param name="columns">Required parsing and formatting instructions for each column.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportFixedWidthText(string rawText, Column[] columns, DataImportOptions options = null)
        {
            string[] rawRows = rawText
                .Replace("\r\n", "\n")
                .Split(new[] { '\n' }, StringSplitOptions.None);
            return ImportFixedWidthText(rawRows, columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a fixed-width text source.
        /// </summary>
        /// <param name="rawRows">The source text presented as rows, one per array element.</param>
        /// <param name="columns">Required parsing and formatting instructions for each column.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportFixedWidthText(string[] rawRows, Column[] columns, DataImportOptions options = null)
        {
            options = options ?? new DataImportOptions();

            // validation
            if (columns == null || columns.Length == 0)
                throw new DataImportException("Fixed-width imports require at least one column to be defined");
            if (columns.Any(c => c == null || c.StartPosition <= -1 || c.Width <= 0))
                throw new DataImportException("Fixed-width imports require only non-null columns having start position > -1 and width > 0");

            var tabularData = new TabularData { Columns = columns, Options = options, IsFixedWidth = true };
            ImportedDataRow currentRow;
            var headerRowProcessed = false;

            for (int r = 0; r < rawRows.Length; r++)
            {
                if (!string.IsNullOrWhiteSpace(rawRows[r]))
                {
                    // skip header row
                    if (options.HasHeaderRow && !headerRowProcessed)  // assume current row is header row
                    {
                        headerRowProcessed = true;
                        continue;
                    }

                    currentRow = new ImportedDataRow
                    {
                        RawRow = rawRows[r],
                        RowNumber = r + 1,
                        TrimmedRawValues = new string[tabularData.Columns.Length],
                        Values = new object[tabularData.Columns.Length]
                    };

                    for (int c = 0; c < tabularData.Columns.Length; c++)
                    {
                        try
                        {
                            // step 1 of 2 - raw import
                            currentRow.TrimmedRawValues[c] = rawRows[r].Substring(tabularData.Columns[c].StartPosition, tabularData.Columns[c].Width).Trim();

                            // step 2 of 2 - polished import
                            if (tabularData.Columns[c].SourceParser != null)
                            {
                                currentRow.Values[c] = tabularData.Columns[c].SourceParser.Invoke(currentRow.TrimmedRawValues[c]);
                            }
                            else
                            {
                                currentRow.Values[c] = Zap.To
                                (
                                    tabularData.Columns[c].DataType,
                                    currentRow.TrimmedRawValues[c] as object,
                                    numberStyle: tabularData.Columns[c].SourceNumberStyle,
                                    provider: tabularData.Columns[c].SourceFormatProvider,
                                    dateTimeStyle: tabularData.Columns[c].SourceDateTimeStyle,
                                    dateFormat: tabularData.Columns[c].SourceDateFormat,
                                    locale: tabularData.Columns[c].SourceLocale,
                                    trueValues: tabularData.Columns[c].SourceTrueValues,
                                    falseValues: tabularData.Columns[c].SourceFalseValues,
                                    encoding: options.Encoding,
                                    ignoreCase: tabularData.Columns[c].SourceIgnoreCase,
                                    strict: tabularData.Columns[c].SourceStrict
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            var importError = new ImportError 
                            { 
                                Row = currentRow, 
                                ColumnNumber = c + 1, 
                                Message = ex.RenderMessage() 
                            };
                            switch (options.DataErrorHandling)
                            {
                                case DataErrorHandlingPolicy.Throw:
                                default:
                                    throw new DataImportException(importError);
                                case DataErrorHandlingPolicy.Embed:
                                    currentRow.TrimmedRawValues[c] = "";
                                    tabularData.AddImportError(importError);
                                    break;
                                case DataErrorHandlingPolicy.Ignore:
                                    currentRow.TrimmedRawValues[c] = "";
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    // skip leading blank rows
                    if (!tabularData.Any())
                        continue;

                    // add in between and trailing blank rows
                    currentRow = new ImportedDataRow
                    {
                        RawRow = rawRows[r],
                        RowNumber = r + 1
                    };
                }

                tabularData.Add(currentRow);
            }
            return tabularData;
        }

        /// <summary>
        /// Parses tabular data from a comma-delimited text source.
        /// </summary>
        /// <param name="file">The source text file.</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportCommaDelimitedTextFile(FilePath file, Column[] columns = null, DataImportOptions options = null)
        {
            return ImportDelimitedTextFile(file, ',', columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a comma-delimited text source.
        /// </summary>
        /// <param name="rawText">The source multiline text.</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportCommaDelimitedText(string rawText, Column[] columns = null, DataImportOptions options = null)
        {
            return ImportDelimitedText(rawText, ',', columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a comma-delimited text source.
        /// </summary>
        /// <param name="rawRows">The source text presented as rows, one per array element.</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportCommaDelimitedText(string[] rawRows, Column[] columns = null, DataImportOptions options = null)
        {
            return ImportDelimitedText(rawRows, ',', columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a variable width text source delimited by the specified delimiter, e.g. comma (,).
        /// </summary>
        /// <param name="file">The source text file.</param>
        /// <param name="delimiter">A <c>char</c> used as data delimiter in the text source, e.g. comma (,).</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportDelimitedTextFile(FilePath file, char delimiter, Column[] columns = null, DataImportOptions options = null)
        {
            return ImportDelimitedTextFile(file, new[] { delimiter }, columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a variable width text source delimited by the specified delimiters, e.g. comma (,).
        /// </summary>
        /// <param name="file">The source text file.</param>
        /// <param name="delimiters">One or more <c>char</c>s used as data delimiters in the text source, e.g. comma (,).</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportDelimitedTextFile(FilePath file, char[] delimiters, Column[] columns = null, DataImportOptions options = null)
        {
            string[] rawRows = ArrayUtil.Prune
            (
                options?.Encoding != null
                    ? file.ReadAllLines(options.Encoding)
                    : file.ReadAllLines()
            );
            return ImportDelimitedText(rawRows, delimiters, columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a variable width text source delimited by the specified delimiter, e.g. comma (,).
        /// </summary>
        /// <param name="rawText">The source multiline text.</param>
        /// <param name="delimiter">A <c>char</c> used as data delimiter in the text source, e.g. comma (,).</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportDelimitedText(string rawText, char delimiter, Column[] columns = null, DataImportOptions options = null)
        {
            return ImportDelimitedText(rawText, new[] { delimiter }, columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a variable width text source delimited by the specified delimiters, e.g. comma (,).
        /// </summary>
        /// <param name="rawText">The source multiline text.</param>
        /// <param name="delimiters">One or more <c>char</c>s used as data delimiters in the text source, e.g. comma (,).</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportDelimitedText(string rawText, char[] delimiters, Column[] columns = null, DataImportOptions options = null)
        {
            string[] rawRows = rawText
                .Replace("\r\n", "\n")
                .Split(new[] { '\n' }, StringSplitOptions.None);
            return ImportDelimitedText(rawRows, delimiters, columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a variable width text source delimited by the specified delimiter, e.g. comma (,).
        /// </summary>
        /// <param name="rawRows">The source text presented as rows, one per array element.</param>
        /// <param name="delimiter">A <c>char</c> used as data delimiter in the text source, e.g. comma (,).</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportDelimitedText(string[] rawRows, char delimiter, Column[] columns = null, DataImportOptions options = null)
        {
            return ImportDelimitedText(rawRows, new[] { delimiter }, columns: columns, options: options);
        }

        /// <summary>
        /// Parses tabular data from a variable width text source delimited by the specified delimiters, e.g. comma (,).
        /// </summary>
        /// <param name="rawRows">The source text presented as rows, one per array element.</param>
        /// <param name="delimiters">One or more <c>char</c>s used as data delimiters in the text source, e.g. comma (,).</param>
        /// <param name="columns">Optional parsing and formatting instructions for each column.  Basic, string-formatted columns will be applied if omitted.</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>A <c>TabularData</c> instance.</returns>
        /// <exception cref="DataImportException"></exception>
        public static TabularData ImportDelimitedText(string[] rawRows, char[] delimiters, Column[] columns = null, DataImportOptions options = null)
        {
            // validation
            if (delimiters == null || delimiters.Length == 0)
                throw new DataImportException("Delimited text imports require at least one delimiter be supplied.");

            options = options ?? new DataImportOptions();
            var tabularData = new TabularData { Columns = columns, Options = options };
            ImportedDataRow currentRow;
            var headerRowProcessed = false;

            for (int r = 0; r < rawRows.Length; r++)
            {
                if (!string.IsNullOrWhiteSpace(rawRows[r]))
                {
                    currentRow = new ImportedDataRow
                    {
                        RawRow = rawRows[r],
                        RowNumber = r + 1,
                        TrimmedRawValues = ArrayUtil.TrimAll(ParseDelimitedRowInternal(rawRows[r], delimiters, options))
                    };

                    // auto-create columns, if applicable
                    if (!tabularData.HasColumns)
                    {
                        if (options.HasHeaderRow)  // assume current row is header row
                        {
                            tabularData.Columns = currentRow.TrimmedRawValues.Select(s => Column.String(name: s)).ToArray();
                            headerRowProcessed = true;
                            continue;
                        }
                        tabularData.Columns = Enumerable.Range(1, currentRow.TrimmedRawValues.Length).Select(i => Column.String(name: "Col " + i)).ToArray();
                    }

                    // validate row data does not exceed columns
                    else if (currentRow.TrimmedRawValues.Length > tabularData.ColumnCount)
                    {
                        throw new DataImportException
                        (
                            new ImportError
                            {
                                Row = currentRow,
                                ColumnNumber = tabularData.ColumnCount,
                                Message = "The number of values in the current row exceeds the number of columns: " + currentRow.TrimmedRawValues.Length + " > " + tabularData.ColumnCount
                            }
                        );
                    }

                    // evaluate and skip header row, if applicable and if not already used to auto-generate the columns
                    if (options.HasHeaderRow && !headerRowProcessed)  // assume current row is header row
                    {
                        if (currentRow.TrimmedRawValues.Length != tabularData.ColumnCount)
                        {
                            throw new DataImportException
                            (
                                new ImportError
                                {
                                    Row = currentRow,
                                    ColumnNumber = Math.Min(currentRow.TrimmedRawValues.Length, tabularData.ColumnCount),
                                    Message = "The number of header labels in the current row does not match the number of columns: " + currentRow.TrimmedRawValues.Length + " != " + tabularData.ColumnCount
                                }
                            );
                        }
                        headerRowProcessed = true;
                        continue;
                    }

                    currentRow.Values = new object[tabularData.ColumnCount];

                    for (int c = 0; c < tabularData.Columns.Length; c++)
                    {
                        try
                        {
                            // step 1 of 2 - raw import (already imported in ImportedDataRow initialization)

                            // step 2 of 2 - polished import
                            if (tabularData.Columns[c].SourceParser != null)
                            {
                                currentRow.Values[c] = tabularData.Columns[c].SourceParser.Invoke(currentRow.TrimmedRawValues[c]);
                            }
                            else
                            {
                                currentRow.Values[c] = Zap.To
                                (
                                    tabularData.Columns[c].DataType,
                                    currentRow.TrimmedRawValues[c] as object,
                                    numberStyle: tabularData.Columns[c].SourceNumberStyle,
                                    provider: tabularData.Columns[c].SourceFormatProvider,
                                    dateTimeStyle: tabularData.Columns[c].SourceDateTimeStyle,
                                    dateFormat: tabularData.Columns[c].SourceDateFormat,
                                    locale: tabularData.Columns[c].SourceLocale,
                                    trueValues: tabularData.Columns[c].SourceTrueValues,
                                    falseValues: tabularData.Columns[c].SourceFalseValues,
                                    encoding: options.Encoding,
                                    ignoreCase: tabularData.Columns[c].SourceIgnoreCase,
                                    strict: tabularData.Columns[c].SourceStrict
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            var importError = new ImportError 
                            { 
                                Row = currentRow, 
                                ColumnNumber = c + 1, 
                                Message = ex.RenderMessage() 
                            };
                            switch (options.DataErrorHandling)
                            {
                                case DataErrorHandlingPolicy.Throw:
                                default:
                                    throw new DataImportException(importError);
                                case DataErrorHandlingPolicy.Embed:
                                    currentRow.TrimmedRawValues[c] = "";
                                    tabularData.AddImportError(importError);
                                    break;
                                case DataErrorHandlingPolicy.Ignore:
                                    currentRow.TrimmedRawValues[c] = "";
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    // skip leading blank rows
                    if (!tabularData.Any()/* && (!options.HasHeaderRow || !headerRowProcessed)*/)
                        continue;

                    // add empty DataImporRow for in between and trailing blank rows
                    currentRow = new ImportedDataRow
                    {
                        RawRow = rawRows[r],
                        RowNumber = r + 1
                    };
                }

                tabularData.Add(currentRow);
            }
            return tabularData;
        }

        /// <summary>
        /// Use this method to test pre-trim row parsing
        /// </summary>
        /// <param name="rawRow">A raw source row of delimited data</param>
        /// <param name="delimiter">A <c>char</c> used as data delimiter in the text source, e.g. comma (,).</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>Raw untrimmed data parsed from the raw data row</returns>
        public static string[] ParseDelimitedRow(string rawRow, char delimiter, DataImportOptions options = null)
        {
            return ParseDelimitedRow(rawRow, new[] { delimiter }, options: options);
        }

        /// <summary>
        /// Use this method to test pre-trim row parsing
        /// </summary>
        /// <param name="rawRow">A raw source row of delimited data</param>
        /// <param name="delimiters">One or more <c>char</c>s used as data delimiters in the text source, e.g. comma (,).</param>
        /// <param name="options">Optional data import preferences.</param>
        /// <returns>Raw untrimmed data parsed from the raw data row</returns>
        public static string[] ParseDelimitedRow(string rawRow, char[] delimiters, DataImportOptions options = null)
        {
            // validation
            if (delimiters == null || delimiters.Length == 0)
                throw new DataImportException("Delimited text imports require at least one delimiter be supplied.");

            options = options ?? new DataImportOptions();
            return ParseDelimitedRowInternal(rawRow, delimiters, options: options);
        }

        private static Regex HasQuotes { get; } = new Regex("[\"“”]");
        private static Regex QuotedEnclosures { get; } = new Regex("[\"“”]*[\"“”]");

        private static string[] ParseDelimitedRowInternal(string rawRow, char[] delimiters, DataImportOptions options)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParam(nameof(rawRow), rawRow, group: MessageRelayGroup);

            char[] delimiterSubstitutes = new char[delimiters.Length];
            var nonprintableOptions = new RevealOptions { CharCategory = CharCategory.Nonprintable };

            // prepare to split on delimiters taking into account double quoted enclosures possibly containing delimiters
            // e.g. «col 1,"col2 - this is a complete enclosure, yessirree",col 3 - this is a "partial enclosure, that it is"»
            if (HasQuotes.IsMatch(rawRow) && !options.SuppressQuotationIsolation)
            {
                // choose delimiter substitute char(s) from ASCII controls (0 - 31) for quoted enclosures
                // e.g. «a,"b,1",c"2,papa"» -> «a,"b[NUL]1",c"2[NUL]papa"»
                int dsi = -1;
                for (int i = 0; i < delimiters.Length; i++)
                {
                    while (++dsi <= 31)
                    {
                        if (!rawRow.Contains((char)dsi))
                        {
                            delimiterSubstitutes[i] = (char)dsi;
                            break;
                        }
                        if (dsi == 31)
                            throw new DataImportException("Cannot internally derive substitute delimiters from ASCII controls due to there are too many ASCII controls in the source text");
                    }
                }

                SystemMessageRelay.RelayMessage(() => "delimiters -> substitutes: " + ValueUtil.Display(delimiters) + " -> " + delimiterSubstitutes.Select(c => TextUtil.RevealChar(c)), group: MessageRelayGroup);

                // replace delimiters with substitutes
                var matches = QuotedEnclosures.Matches(rawRow);
                foreach (Match match in matches)
                {
                    for (int i = 0; i < delimiters.Length; i++)
                    {
                        if (match.Value.Contains(delimiters[i]))
                        {
                            rawRow = rawRow.Substring(0, match.Index) +
                                match.Value.Replace(delimiters[i], delimiterSubstitutes[i]) +
                                rawRow.Substring(match.Index + match.Length);
                        }
                    }
                }

                SystemMessageRelay.RelayMessage(() => nameof(rawRow) + " -> " + TextUtil.Reveal(rawRow, nonprintableOptions), group: MessageRelayGroup);
            }

            // split on delimiters
            var array = ArrayUtil.TrimAll(rawRow.Split(delimiters));

            SystemMessageRelay.RelayValue(nameof(array), () => array.Select(s => TextUtil.Reveal(s, nonprintableOptions)), group: MessageRelayGroup);

            // replace substituted delimiters with originals, if applicable, and remove double quotes on complete enclosures
            if (HasQuotes.IsMatch(rawRow) && !options.SuppressQuotationIsolation)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int d = 0; d < delimiters.Length; d++)
                    {
                        array[i] = array[i].Replace(delimiterSubstitutes[d], delimiters[d]);

                        // remove double quotes on complete enclosures
                        if (i == 0 && d == 0)
                        {
                            bool quoteAtValueStart = array[i][0].In('"', '“', '”');
                            bool quoteAtValueEnd = array[i][array[i].Length - 1].In('"', '“', '”');
                            if (quoteAtValueStart && quoteAtValueEnd)
                                array[i] = array[i].Substring(1, array[i].Length - 2);
                        }
                    }
                }
            }

            SystemMessageRelay.RelayMethodReturnValue(array, group: MessageRelayGroup);
            return array;
        }
    }
}
