using System.Collections.Generic;

using Horseshoe.NET.DataImport;
using Horseshoe.NET.IO;
using Horseshoe.NET.Text;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Horseshoe.NET.Excel
{
    public static class ImportExcelData
    {
        public static class Xls
        {
            /// <summary>
            /// Imports Excel file (.xls) and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="sheetNum"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportExcelData.Xls.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports Excel file (.xls) and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="sheetNum"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportExcelData.Xls.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports Excel file (.xls) and returns the backing <c>ExcelDataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="sheetNum"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static ExcelDataImport AsDataImport(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportExcelData.Xls.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new ExcelDataImport(columns, enforceColumnCount: enforceColumnCount)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                var rawValues = new List<object>();
                int rowNum = 0;

                // read Excel worksheet
                var excelWorksheet = new HSSFWorkbook(new NPOIFSFileSystem(file)).GetSheetAt(sheetNum);
                var excelRow = excelWorksheet.GetRow(rowNum);
                if (excelRow != null)
                {
                    // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                    if (excelRow.IsBlank())                         // if blank...
                    {
                        excelRow = excelWorksheet.GetRow(++rowNum); // ...load the next row (presumably either header or data)
                        dataImport.SkippedRows++;
                    }
                    if (excelRow != null && hasHeaderRow)           // if caller declares there is a header row...
                    {
                        excelRow = excelWorksheet.GetRow(++rowNum); // ...assume header row is loaded, load the first data row
                        dataImport.SkippedRows++;
                    }
                }
                while (excelRow != null)
                {
                    journal.WriteEntry("reading Excel row " + dataImport.NextRow);
                    ReadExcelRow(rawValues, excelRow, autoTrunc, errorHandlingPolicy, journal);
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
                    excelRow = excelWorksheet.GetRow(++rowNum);
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }
        }

        public static class Xlsx
        {
            /// <summary>
            /// Imports Excel file (.xlsx) and returns it as <c>string[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="sheetNum"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportExcelData.Xlsx.AsStrings()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToStringArrays();
            }

            /// <summary>
            /// Imports Excel file (.xlsx) and returns it as <c>object[]</c>s to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="sheetNum"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportExcelData.Xlsx.AsObjects()");
                journal.Level++;

                // variable declaration
                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

                // finalize
                journal.Level--;
                return dataImport.ExportToObjectArrays();
            }

            /// <summary>
            /// Imports Excel file (.xlsx) and returns the backing <c>ExcelDataImport</c> instance to the requestor
            /// </summary>
            /// <param name="file"></param>
            /// <param name="columns"></param>
            /// <param name="enforceColumnCount"></param>
            /// <param name="sheetNum"></param>
            /// <param name="hasHeaderRow"></param>
            /// <param name="blankRowPolicy"></param>
            /// <param name="errorHandlingPolicy"></param>
            /// <param name="autoTrunc"></param>
            /// <param name="journal"></param>
            /// <returns></returns>
            public static ExcelDataImport AsDataImport(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
            {
                // journaling
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("ImportExcelData.Xlsx.AsDataImport()");
                journal.Level++;

                // variable declaration
                var dataImport = new ExcelDataImport(columns, enforceColumnCount: enforceColumnCount)
                {
                    BlankRowPolicy = blankRowPolicy,
                    DataErrorHandlingPolicy = errorHandlingPolicy,
                    AutoTrunc = autoTrunc
                };
                var rawValues = new List<object>();
                int rowNum = 0;

                // read Excel worksheet
                var excelWorksheet = new XSSFWorkbook(file.File).GetSheetAt(sheetNum);
                var excelRow = excelWorksheet.GetRow(rowNum);
                if (excelRow != null)
                {
                    // slice off leading blank and/or header row, if applicable, there is allowance for no more than 1 blank row and 1 header row
                    if (excelRow.IsBlank())                         // if blank...
                    {
                        excelRow = excelWorksheet.GetRow(++rowNum); // ...load the next row (presumably either header or data)
                        dataImport.SkippedRows++;
                    }
                    if (excelRow != null && hasHeaderRow)           // if caller declares there is a header row...
                    {
                        excelRow = excelWorksheet.GetRow(++rowNum); // ...assume header row is loaded, load the first data row
                        dataImport.SkippedRows++;
                    }
                }
                while (excelRow != null)
                {
                    journal.WriteEntry("reading Excel row " + dataImport.NextRow);
                    ReadExcelRow(rawValues, excelRow, autoTrunc, errorHandlingPolicy, journal);
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
                    excelRow = excelWorksheet.GetRow(++rowNum);
                }

                // finalize
                dataImport.FinalizeImport();
                journal.Level--;
                return dataImport;
            }
        }

        /// <summary>
        /// Imports Excel file (.xls or .xlsx) and returns it as <c>string[]</c>s to the requestor
        /// </summary>
        /// <param name="file"></param>
        /// <param name="columns"></param>
        /// <param name="enforceColumnCount"></param>
        /// <param name="sheetNum"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="blankRowPolicy"></param>
        /// <param name="errorHandlingPolicy"></param>
        /// <param name="autoTrunc"></param>
        /// <param name="journal"></param>
        /// <returns></returns>
        public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("ImportExcelData.AsStrings()");
            journal.Level++;

            // variable declaration
            var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

            // finalize
            journal.Level--;
            return dataImport.ExportToStringArrays();
        }

        /// <summary>
        /// Imports Excel file (.xls or .xlsx) and returns it as <c>object[]</c>s to the requestor
        /// </summary>
        /// <param name="file"></param>
        /// <param name="columns"></param>
        /// <param name="enforceColumnCount"></param>
        /// <param name="sheetNum"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="blankRowPolicy"></param>
        /// <param name="errorHandlingPolicy"></param>
        /// <param name="autoTrunc"></param>
        /// <param name="journal"></param>
        /// <returns></returns>
        public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("ImportExcelData.AsObjects()");
            journal.Level++;

            // variable declaration
            var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);

            // finalize
            journal.Level--;
            return dataImport.ExportToObjectArrays();
        }

        /// <summary>
        /// Imports Excel file (.xls or .xlsx) and returns the backing <c>ExcelDataImport</c> instance to the requestor
        /// </summary>
        /// <param name="file"></param>
        /// <param name="columns"></param>
        /// <param name="enforceColumnCount"></param>
        /// <param name="sheetNum"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="blankRowPolicy"></param>
        /// <param name="errorHandlingPolicy"></param>
        /// <param name="autoTrunc"></param>
        /// <param name="journal"></param>
        /// <returns></returns>
        public static ExcelDataImport AsDataImport(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim, TraceJournal journal = null)
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("ImportExcelData.AsDataImport()");
            journal.Level++;

            // variable declaration
            ExcelDataImport dataImport;

            switch (file.Extension.ToLower())
            {
                case ".xls":
                    dataImport = Xls.AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);
                    break;
                case ".xlsx":
                    dataImport = Xlsx.AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc, journal: journal);
                    break;
                default:
                    throw new DataImportException("Only .xls and .xlsx files are supported at this time");
            }

            journal.Level--;
            return dataImport;
        }

        static void ReadExcelRow(List<object> rawValues, IRow row, AutoTruncate autoTrunc, DataErrorHandlingPolicy dataErrorHandling, TraceJournal journal)
        {
            if (row.IsBlank())
            {
                journal.WriteEntry("encountered blank row");
            }
            else
            {
                object value;
                ICell cell;
                CellType cellType;
                string cellAddress;

                for (int i = 0, rowSize = row.LastCellNum; i < rowSize; i++)
                {
                    cell = row.GetCell(i, MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    cellAddress = ExcelImportUtil.GetDisplayCellAddress(cell.ColumnIndex + 1, cell.RowIndex + 1);

                    if (cell == null)
                    {
                        value = null;
                        journal.WriteEntry("blank cell " + cellAddress + " -> [null]");
                    }
                    else
                    {
                        cellType = cell.CellType == CellType.Formula ? cell.CachedFormulaResultType : cell.CellType;

                        switch (cellType)
                        {
                            case CellType.String:
                            case CellType.Blank:
                                switch (autoTrunc)
                                {
                                    case AutoTruncate.Trim:
                                        value = cell.StringCellValue.Trim();
                                        break;
                                    case AutoTruncate.Zap:
                                        value = Zap.String(cell.StringCellValue);
                                        break;
                                    default:
                                        value = cell.StringCellValue;
                                        break;
                                }
                                journal.WriteEntry("string cell " + cellAddress + " -> " + TextUtil.Reveal(value));
                                break;
                            case CellType.Numeric:
                                value = cell.NumericCellValue;
                                journal.WriteEntry("numeric cell " + cellAddress + " -> " + value);
                                break;
                            case CellType.Boolean:
                                value = cell.BooleanCellValue;
                                journal.WriteEntry("boolean cell " + cellAddress + " -> " + value);
                                break;
                            case CellType.Error:
                                switch (dataErrorHandling)
                                {
                                    case DataErrorHandlingPolicy.Throw:
                                    default:
                                        throw new DataImportException("Cell error: " + cellAddress + " (code = " + cell.ErrorCellValue + ")");
                                    case DataErrorHandlingPolicy.Embed:
                                        value = new DataError("Cell error: " + cellAddress + " (code = " + cell.ErrorCellValue + ")");
                                        journal.WriteEntry("error cell " + cellAddress + " (code = " + cell.ErrorCellValue + ") -> " + TextUtil.Reveal(value));
                                        break;
                                    case DataErrorHandlingPolicy.IgnoreAndUseDefaultValue:
                                        value = autoTrunc == AutoTruncate.Zap ? null : "";
                                        journal.WriteEntry("error cell " + cellAddress + " (code = " + cell.ErrorCellValue + ") -> " + TextUtil.Reveal(value));
                                        break;
                                }
                                break;
                            case CellType.Unknown:
                            default:
                                switch (dataErrorHandling)
                                {
                                    case DataErrorHandlingPolicy.Throw:
                                    default:
                                        throw new DataImportException("Unknown cell type: " + cellAddress);
                                    case DataErrorHandlingPolicy.Embed:
                                        value = new DataError("Unknown cell type: " + cellAddress, cell.ColumnIndex + 1, cell.RowIndex + 1);
                                        break;
                                    case DataErrorHandlingPolicy.IgnoreAndUseDefaultValue:
                                        value = null;
                                        break;
                                }
                                break;
                        }
                    }
                    rawValues.Add(value);
                }
            }
        }
    }
}
