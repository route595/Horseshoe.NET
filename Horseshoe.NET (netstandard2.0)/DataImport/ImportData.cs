using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.IO;
using Horseshoe.NET.Iterator.Memory;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Imports data from a text file
    /// </summary>
    public static class ImportData
    {
        /// <summary>
        /// Contains methods for importing delimited data
        /// </summary>
        public static class DelimitedText
        {
            /// <summary>
            /// Imports delimited text and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged.</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static IEnumerable<string[]> AsStrings(string rawData, char delimiter, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedText.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, delimiter, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text using supplied column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="enforceColumnCount">If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static IEnumerable<string[]> AsStrings(string rawData, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedText.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, delimiter, columns, enforceColumnCount: enforceColumnCount, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text using supplied column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
            public static IEnumerable<object[]> AsObjects(string rawData, char delimiter, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedText.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, delimiter, columns, enforceColumnCount: true, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports delimited text and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static DataImport AsDataImport(string rawData, char delimiter, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedText.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues;
                ReadOnlySpan<string> rawSpan;

                // handle special cases
                if (rawData == null || rawData.Trim().Length == 0)
                {
                    return dataImport;
                }

                // prepare looping variables
                rawValues = new List<string>();
                rawData = rawData.Replace("\r\n", "\n").TrimEnd('\n');
                rawSpan = rawData?.Split('\n');

                // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                if (rawSpan[0].Trim().Length == 0)
                {
                    rawSpan = rawSpan.Slice(1);  // handling 1 leading empty row
                    dataImport.SkippedRows++;
                }
                if (hasHeaderRow)
                {
                    rawSpan = rawSpan.Slice(1);
                    dataImport.SkippedRows++;
                }

                // loop
                rawSpan.Iterate
                (
                    (raw, ci) =>
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        ParseRawValuesInternal(rawValues, raw.AsSpan(), delimiter, null, false, dataImport.NextRow, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            ci.Exit();
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                    }
                );

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            /// <summary>
            /// Imports delimited text and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="enforceColumnCount">If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static DataImport AsDataImport(string rawData, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedText.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns.Where(c => !c.NotMapped).ToList(), enforceColumnCount: enforceColumnCount)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues;
                ReadOnlySpan<string> rawSpan;

                // handle special cases
                if (rawData == null || rawData.Trim().Length == 0)
                {
                    return dataImport;
                }

                // prepare looping variables
                rawValues = new List<string>();
                rawData = rawData.Replace("\r\n", "\n").TrimEnd('\n');
                rawSpan = rawData?.Split('\n');

                // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                if (rawSpan[0].Trim().Length == 0)
                {
                    rawSpan = rawSpan.Slice(1);  // handling 1 leading empty row
                    dataImport.SkippedRows++;
                }
                if (hasHeaderRow)
                {
                    rawSpan = rawSpan.Slice(1);
                    dataImport.SkippedRows++;
                }

                // loop
                rawSpan.Iterate
                (
                    (raw, ci) =>
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        ParseRawValuesInternal(rawValues, raw.AsSpan(), delimiter, columns, enforceColumnCount, dataImport.NextRow, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            ci.Exit();
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                    }
                );

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            internal static void ParseRawValuesInternal(IList<string> rawValues, ReadOnlySpan<char> rawRow, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount, int rowNum, TraceJournal journal)
            {
                // validation
                if (delimiter == '\"')
                    throw new DataImportException("cannot use double quote (\\\") as a delimiter");

                journal.Level++;
                // handle special cases
                if (rawRow.IsEmpty)
                {
                    journal.WriteEntry("encountered empty row on row " + rowNum);
                    journal.Level--;
                    return;
                }

                // variable declaration
                StringBuilder valueBuilder = new StringBuilder();
                var columnIndex = 0;
                var inQuotes = false;
                var columnCount = columns?.Count() ?? 0;
                var mappedColumnCount = columns?.Count(c => !c.NotMapped) ?? 0;
                Column column;

                foreach (char c in rawRow)
                {
                    if (c == '\r')
                        throw new ValidationException("Illegal char found: \\r");
                    if (c == '\n')
                        throw new ValidationException("Illegal char found: \\n");

                    if (c == delimiter && !inQuotes)
                    {
                        // we've reached a delimiter, get the column and process the value
                        column = columnIndex <= columnCount - 1
                            ? columns.ElementAt(columnIndex)
                            : null;
                        processValue(inQuotes, column, enforceColumnCount, rawValues, valueBuilder, journal);
                        columnIndex++;
                        continue;
                    }

                    // handle quotes - example 1: "whole value, in quotes" - example 2: Tim "The Tool Man" Taylor
                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                    }

                    // build the string
                    valueBuilder.Append(c);
                }

                // end of row reached
                column = columnIndex <= columnCount - 1
                    ? columns.ElementAt(columnIndex)
                    : null;
                processValue(inQuotes, column, enforceColumnCount, rawValues, valueBuilder, journal);

                // local function
                void processValue(bool _inQuotes, Column _column, bool _enforceColumnCount, IList< string> _rawValues, StringBuilder _valueBuilder, TraceJournal _journal)
                {
                    if (_inQuotes)
                        throw new DataImportException("Unclosed quotation marks");

                    if (_column != null && _column.NotMapped)
                    {
                        _journal.WriteEntry("not mapped: \"" + TextUtil.Crop(_valueBuilder.ToString(), 18, truncateMarker: TruncateMarker.LongEllipsis) + "\"");
                        _valueBuilder.Clear();
                        return;
                    }
                    if (_column == null && _enforceColumnCount)
                    {
                        throw new DataImportException("No metadata for source column " + (columnIndex + 1) + ". Enforcing column count. The number of data elements on row " + rowNum + " may have exceeded the number of mapped columns: " + (_rawValues.Count + 1) + ", " + mappedColumnCount);
                    }

                    _rawValues.Add(_valueBuilder.ToString());
                    _valueBuilder.Clear();
                    _journal.WriteEntry("parsed value: \"" + TextUtil.Crop(_rawValues.Last(), 18, truncateMarker: TruncateMarker.LongEllipsis) + "\"");
                }

                // finalize method
                journal.Level--;
            }
        }

        /// <summary>
        /// Contains methods for importing delimited data from files such as CSV
        /// </summary>
        public static class DelimitedTextFile
        {
            /// <summary>
            /// Imports a delimited text file and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, char delimiter, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, delimiter, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports a delimited text file asynchronously and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static async Task<IEnumerable<string[]>> AsStringsAsync(FilePath file, char delimiter, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStringsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, delimiter, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports a delimited text file and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="enforceColumnCount">If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, delimiter, columns, enforceColumnCount: enforceColumnCount, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports a delimited text file asynchronously and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="enforceColumnCount">If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static async Task<IEnumerable<string[]>> AsStringsAsync(FilePath file, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStringsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, delimiter, columns, enforceColumnCount: enforceColumnCount, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports a delimited text file and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, char delimiter, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, delimiter, columns, enforceColumnCount: true, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports a delimited text file asynchronously and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync(FilePath file, char delimiter, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsObjectsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, delimiter, columns, enforceColumnCount: true, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports a delimited text file and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static DataImport AsDataImport(FilePath file, char delimiter, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues = new List<string>();
                string rawRow;

                // loop over text file
                using (var reader = file.OpenText())
                {
                    rawRow = reader.ReadLine();
                    if (rawRow != null)
                    {
                        // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                        if (rawRow.Trim().Length == 0)  // if blank...
                        {
                            rawRow = reader.ReadLine(); // ...load the next row (presumably either header or data)
                            dataImport.SkippedRows++;
                        }
                        if (hasHeaderRow)               // if caller declares there is a header row...
                        {
                            rawRow = reader.ReadLine(); // ...assume header row is loaded and load the first data row
                            dataImport.SkippedRows++;
                        }
                    }
                    while (rawRow != null)
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiter, null, false, dataImport.NextRow, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            break;
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                        rawRow = reader.ReadLine();
                    }
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            /// <summary>
            /// Imports a delimited text file asynchronously and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static async Task<DataImport> AsDataImportAsync(FilePath file, char delimiter, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsDataImportAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues = new List<string>();
                string rawRow;

                // loop over text file
                using (var reader = file.OpenText())
                {
                    rawRow = await reader.ReadLineAsync();
                    if (rawRow != null)
                    {
                        // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                        if (rawRow.Trim().Length == 0)             // if blank...
                        {
                            rawRow = await reader.ReadLineAsync(); // ...load the next row (presumably either header or data)
                            dataImport.SkippedRows++;
                        }
                        if (hasHeaderRow)                          // if caller declares there is a header row...
                        {
                            rawRow = await reader.ReadLineAsync(); // ...assume header row is loaded and load the first data row
                            dataImport.SkippedRows++;
                        }
                    }
                    while (rawRow != null)
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiter, null, false, dataImport.NextRow, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            break;
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                        rawRow = await reader.ReadLineAsync();
                    }
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            /// <summary>
            /// Imports a delimited text file and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="enforceColumnCount">If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static DataImport AsDataImport(FilePath file, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns.Where(c => !c.NotMapped).ToList(), enforceColumnCount: enforceColumnCount)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues = new List<string>();
                string rawRow;

                // loop over text file
                using (var reader = file.OpenText())
                {
                    rawRow = reader.ReadLine();
                    if (rawRow != null)
                    {
                        // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                        if (rawRow.Trim().Length == 0)  // if blank...
                        {
                            rawRow = reader.ReadLine(); // ...load the next row (presumably either header or data)
                            dataImport.SkippedRows++;
                        }
                        if (hasHeaderRow)               // if caller declares there is a header row...
                        {
                            rawRow = reader.ReadLine(); // ...assume header row is loaded and load the first data row
                            dataImport.SkippedRows++;
                        }
                    }
                    while (rawRow != null)
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiter, columns, enforceColumnCount, dataImport.NextRow, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            break;
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                        rawRow = reader.ReadLine();
                    }
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            /// <summary>
            /// Imports a delimited text file asynchronously and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="delimiter">The <c>char</c> that determines where to split the row into values</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="enforceColumnCount">If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static async Task<DataImport> AsDataImportAsync(FilePath file, char delimiter, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.DelimitedTextFile.AsDataImportAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns.Where(c => !c.NotMapped).ToList(), enforceColumnCount: enforceColumnCount)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues = new List<string>();
                string rawRow;

                // loop over text file
                using (var reader = file.OpenText())
                {
                    rawRow = await reader.ReadLineAsync();
                    if (rawRow != null)
                    {
                        // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                        if (rawRow.Trim().Length == 0)             // if blank...
                        {
                            rawRow = await reader.ReadLineAsync(); // ...load the next row (presumably either header or data)
                            dataImport.SkippedRows++;
                        }
                        if (hasHeaderRow)                          // if caller declares there is a header row...
                        {
                            rawRow = await reader.ReadLineAsync(); // ...assume header row is loaded and load the first data row
                            dataImport.SkippedRows++;
                        }
                    }
                    while (rawRow != null)
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiter, columns, enforceColumnCount, dataImport.NextRow, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            break;
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                        rawRow = await reader.ReadLineAsync();
                    }
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }
        }

        /// <summary>
        /// Contains methods for importing fixed-with data
        /// </summary>
        public static class FixedWidthText
        {
            /// <summary>
            /// Imports fixed-width text using supplied column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static IEnumerable<string[]> AsStrings(string rawData, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthText.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports fixed-width text using supplied column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
            public static IEnumerable<object[]> AsObjects(string rawData, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthText.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports fixed-width text and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="rawData">The entire data set in <c>string</c> form; header row allowed, new lines separate rows, delimiter(s) separate columns</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static DataImport AsDataImport(string rawData, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthText.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns.Where(c => !c.NotMapped).ToList(), enforceColumnCount: true)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues;
                ReadOnlySpan<string> rawSpan;
                int initialSkippedBlankRows;

                // handle special cases
                if (rawData == null || rawData.Trim().Length == 0)
                {
                    return dataImport;
                }

                // prepare looping variables
                rawValues = new List<string>();
                rawData = rawData.Replace("\r\n", "\n").TrimEnd('\n');
                rawSpan = rawData?.Split('\n');

                // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                if (rawSpan[0].Trim().Length == 0)
                {
                    rawSpan = rawSpan.Slice(1);  // handling 1 leading empty row
                    dataImport.SkippedRows++;
                }
                if (hasHeaderRow)
                {
                    rawSpan = rawSpan.Slice(1);
                    dataImport.SkippedRows++;
                }
                initialSkippedBlankRows = dataImport.SkippedRows;

                // loop
                rawSpan.Iterate
                (
                    (raw, ci) =>
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        ParseRawValuesInternal(rawValues, raw.AsSpan(), columns, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            ci.Exit();
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                    }
                );

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            internal static void ParseRawValuesInternal(IList<string> rawValues, ReadOnlySpan<char> rawRow, IEnumerable<Column> columns, TraceJournal journal)
            {
                // validation
                if (columns == null || !columns.Any(c => !c.NotMapped))
                    throw new DataImportException("cannot parse data if 1 or more mapped columns are not supplied");
                //if (!columns.Any(c => !c.NotMapped && c.FixedWidth > 0))
                //    throw new DataImportException("cannot parse data due to zero mapped columns have a specified width");

                journal.Level++;

                // handle special cases
                if (rawRow.IsEmpty)
                {
                    journal.WriteEntry("encountered empty row");
                }
                else 
                {
                    // variable declaration
                    var pos = 0;
                    int fixedWidth;

                    foreach (var c in columns)
                    {
                        fixedWidth = Math.Max(0, c.FixedWidth);
                        if (!c.NotMapped)
                        {
                            if (fixedWidth > 0)
                            {
                                rawValues.Add(rawRow.Slice(pos, fixedWidth).ToString());
                            }
                            else
                            {
                                rawValues.Add("");
                            }
                            journal.WriteEntry("parsed value: \"" + TextUtil.Crop(rawValues.Last(), 18, truncateMarker: TruncateMarker.LongEllipsis) + "\"  (width=" + fixedWidth + ")");
                        }
                        else
                        {
                            journal.WriteEntry("not mapped: \"" + TextUtil.Crop(rawRow.Slice(pos, fixedWidth).ToString(), 18, truncateMarker: TruncateMarker.LongEllipsis) + "\"");
                        }
                        pos += fixedWidth;
                    }
                }

                journal.Level--;
            }
        }

        /// <summary>
        /// Contains methods for importing fixed-with data files
        /// </summary>
        public static class FixedWidthTextFile
        {
            /// <summary>
            /// Imports a fixed-width text file and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports a fixed-width text file asynchronously and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
            public static async Task<IEnumerable<string[]>> AsStringsAsync(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsStringsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports a fixed-width text file and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports a fixed-width text file asynchronously and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsObjectsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports a fixed-width text file and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static DataImport AsDataImport(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns.Where(c => !c.NotMapped).ToList(), enforceColumnCount: true)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues = new List<string>();
                string rawRow;

                // loop over text file
                using (var reader = file.OpenText())
                {
                    rawRow = reader.ReadLine();
                    if (rawRow != null)
                    {
                        // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                        if (rawRow.Trim().Length == 0)  // if blank...
                        {
                            rawRow = reader.ReadLine(); // ...load the next row (presumably either header or data)
                            dataImport.SkippedRows++;
                        }
                        if (hasHeaderRow)               // if caller declares there is a header row...
                        {
                            rawRow = reader.ReadLine(); // ...assume header row is loaded and load the first data row
                            dataImport.SkippedRows++;
                        }
                    }
                    while (rawRow != null)
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        FixedWidthText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), columns, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            break;
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                        rawRow = reader.ReadLine();
                    }
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }

            /// <summary>
            /// Imports a fixed-width text file asynchronously and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file">A file - accepts <c>string</c> path or <c>FileInfo</c> instance</param>
            /// <param name="columns">A pre-determined collection of columns for enforcing certain rules about the imported data such as data types and number of items per row</param>
            /// <param name="hasHeaderRow">If <c>true</c> the first row will be skipped</param>
            /// <param name="blankRowPolicy">How to handle blank rows, specifically leading and trailing</param>
            /// <param name="errorHandlingPolicy">How to handle data errors</param>
            /// <param name="autoTrunc">How to interpret empty values</param>
            /// <param name="journal">A trace journal to which each step of the process is logged</param>
            /// <returns>a <c>DataImport</c> instance</returns>
            public static async Task<DataImport> AsDataImportAsync(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                journal = journal ?? new TraceJournal();
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsDataImportAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns.Where(c => !c.NotMapped).ToList(), enforceColumnCount: true)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                List<string> rawValues = new List<string>();
                string rawRow;

                // loop over text file
                using (var reader = file.OpenText())
                {
                    rawRow = await reader.ReadLineAsync();
                    if (rawRow != null)
                    {
                        // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                        if (rawRow.Trim().Length == 0)             // if blank...
                        {
                            rawRow = await reader.ReadLineAsync(); // ...load the next row (presumably either header or data)
                            dataImport.SkippedRows++;
                        }
                        if (hasHeaderRow)                          // if caller declares there is a header row...
                        {
                            rawRow = await reader.ReadLineAsync(); // ...assume header row is loaded and load the first data row
                            dataImport.SkippedRows++;
                        }
                    }
                    while (rawRow != null)
                    {
                        journal.WriteEntry("parsing row " + dataImport.NextRow);
                        FixedWidthText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), columns, journal);
                        try
                        {
                            dataImport.ImportRaw(rawValues, dataImport.NextRow);
                        }
                        catch (StopImportingDataException)
                        {
                            break;
                        }
                        finally
                        {
                            rawValues.Clear();
                        }
                        rawRow = await reader.ReadLineAsync();
                    }
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }
        }
    }
}
