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
    /// Import data from text or from a file
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
            /// <param name="rawData"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(string rawData, char[] delimiters, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedText.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, delimiters, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text using column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(string rawData, char[] delimiters, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedText.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, delimiters, columns, enforceColumnCount: enforceColumnCount, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text using column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="columns"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(string rawData, char[] delimiters, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedText.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, delimiters, columns, enforceColumnCount: true, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports delimited text and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static DataImport AsDataImport(string rawData, char[] delimiters, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
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
                        ParseRawValuesInternal(rawValues, raw.AsSpan(), delimiters, journal);
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
            /// Imports delimited text using column metadata and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static DataImport AsDataImport(string rawData, char[] delimiters, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedText.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns, enforceColumnCount: enforceColumnCount)
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
                        ParseRawValuesInternal(rawValues, raw.AsSpan(), delimiters, journal);
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

            internal static void ParseRawValuesInternal(IList<string> rawValues, ReadOnlySpan<char> rawRow, char[] delimiters, TraceJournal journal)
            {
                // validation
                if (delimiters == null || delimiters.Length == 0)
                    throw new DataImportException("cannot parse if 1 or more delimiters are not supplied");
                if (delimiters.Contains('\"'))
                    throw new DataImportException("cannot use double quote (\\\") as a delimiter");

                journal.Level++;

                // variable declaration
                StringBuilder valueBuilder;
                var inQuotes = false;

                // handle special cases
                if (rawRow.IsEmpty)
                {
                    journal.WriteEntry("encountered empty row");
                }
                else
                {
                    valueBuilder = new StringBuilder();
                    var lastCharWasDelimiter = false;

                    foreach (char c in rawRow)
                    {
                        if (c == '\r')
                            throw new ValidationException("Illegal char found: \\r");
                        if (c == '\n')
                            throw new ValidationException("Illegal char found: \\n");

                        // we've reached a delimiter, process the value
                        if (delimiters.Any(d => d == c) && !inQuotes)
                        {
                            if (!lastCharWasDelimiter)
                            {
                                processValue();
                            }
                            lastCharWasDelimiter = true;
                            continue;
                        }

                        // handle quotes - example 1: "whole value, in quotes" - example 2: Tim "The Tool Man" Taylor
                        if (c == '"')
                        {
                            inQuotes = !inQuotes;
                        }

                        // build the string
                        valueBuilder.Append(c);

                        // reset variables
                        lastCharWasDelimiter = false;
                    }

                    // end of row reached
                    processValue();
                }

                // local function
                void processValue()
                {
                    if (inQuotes)
                        throw new DataImportException("Unclosed quotation marks");

                    rawValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                    journal.WriteEntry("parsed value: \"" + rawValues.Last() + "\"");
                }

                journal.Level--;
            }
        }

        /// <summary>
        /// Contains methods for importing delimited data files such as CSV
        /// </summary>
        public static class DelimitedTextFile
        {
            /// <summary>
            /// Imports delimited text file and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, char[] delimiters, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, delimiters, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text file and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<string[]>> AsStringsAsync(FilePath file, char[] delimiters, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStringsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, delimiters, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text file using column metadata and returns returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, char[] delimiters, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, delimiters, columns, enforceColumnCount: enforceColumnCount, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text file using column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<string[]>> AsStringsAsync(FilePath file, char[] delimiters, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsStringsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, delimiters, columns, enforceColumnCount: enforceColumnCount, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports delimited text file using column metadata and returns returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, char[] delimiters, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, delimiters, columns, enforceColumnCount: true, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports delimited text file using column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync(FilePath file, char[] delimiters, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsObjectsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, delimiters, columns, enforceColumnCount: true, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports delimited text file and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static DataImport AsDataImport(FilePath file, char[] delimiters, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
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
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiters, journal);
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
            /// Imports delimited text file and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<DataImport> AsDataImportAsync(FilePath file, char[] delimiters, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
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
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiters, journal);
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
            /// Imports delimited text file using column metadata and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static DataImport AsDataImport(FilePath file, char[] delimiters, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns, enforceColumnCount: enforceColumnCount)
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
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiters, journal);
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
            /// Imports delimited text file using column metadata and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="delimiters"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<DataImport> AsDataImportAsync(FilePath file, char[] delimiters, IEnumerable<Column> columns, bool enforceColumnCount = false, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = default, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.DelimitedTextFile.AsDataImportAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns, enforceColumnCount: enforceColumnCount)
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
                        DelimitedText.ParseRawValuesInternal(rawValues, rawRow.AsSpan(), delimiters, journal);
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
            /// Imports fixed-width text using column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(string rawData, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthText.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports fixed-width text using column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(string rawData, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthText.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(rawData, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports fixed-width text using column metadata and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="rawData"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static DataImport AsDataImport(string rawData, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthText.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns, enforceColumnCount: true)
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
                if (columns == null || !columns.Any())
                    throw new DataImportException("cannot parse if 1 or more columns are not supplied");
                if (!columns.Any(c => c.FixedWidth > 0))
                    throw new DataImportException("cannot parse data due to zero columns have a fixed width");

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

                    foreach (var c in columns)
                    {
                        if (c.FixedWidth > 0)
                        {
                            rawValues.Add(rawRow.Slice(pos, c.FixedWidth).ToString());
                            pos += c.FixedWidth;
                            journal.WriteEntry("parsed value: \"" + rawValues.Last() + "\"");
                        }
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
            /// Imports fixed-width text file using column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports fixed-width text file using column metadata and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<string[]>> AsStringsAsync(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsStringsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports fixed-width text file using column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports fixed-width text file using column metadata and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<IEnumerable<object[]>> AsObjectsAsync(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsObjectsAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = await AsDataImportAsync(file, columns, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports fixed-width text file using column metadata and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static DataImport AsDataImport(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns, enforceColumnCount: true)
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
            /// Imports fixed-width text file using column metadata and returns the backing <c>DataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static async Task<DataImport> AsDataImportAsync(FilePath file, IEnumerable<Column> columns, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportData.FixedWidthTextFile.AsDataImportAsync()");
                journal.Level++;

                // variable declaration
                var dataImport = new DataImport(columns, enforceColumnCount: true)
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
