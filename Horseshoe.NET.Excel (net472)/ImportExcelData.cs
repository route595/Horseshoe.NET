using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DataImport;
using Horseshoe.NET.IO;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Horseshoe.NET.Excel
{
    public static class ImportExcelData
    {
        private static string MessageRelayGroup => ExcelImportConstants.MessageRelayGroup;

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
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);

                var arrays = dataImport.ExportToStringArrays();
                SystemMessageRelay.RelayMethodReturn(returnDescription: "array count: " + arrays.Count(), group: MessageRelayGroup);
                return arrays;
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
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);

                var arrays = dataImport.ExportToObjectArrays();
                SystemMessageRelay.RelayMethodReturn(returnDescription: "array count: " + arrays.Count(), group: MessageRelayGroup);
                return arrays;
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
            /// <returns></returns>
            public static ExcelDataImport AsDataImport(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

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
                    SystemMessageRelay.RelayMessage("reading Excel row " + dataImport.NextRow, group: MessageRelayGroup);
                    ReadExcelRow(rawValues, excelRow, autoTrunc, errorHandlingPolicy);
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
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
            /// <returns></returns>
            public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
            /// <returns></returns>
            public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

                var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
            /// <returns></returns>
            public static ExcelDataImport AsDataImport(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
            {
                SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

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
                    SystemMessageRelay.RelayMessage("reading Excel row " + dataImport.NextRow, group: MessageRelayGroup);
                    ReadExcelRow(rawValues, excelRow, autoTrunc, errorHandlingPolicy);
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

                dataImport.FinalizeImport();
                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
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
        /// <returns></returns>
        public static IEnumerable<string[]> AsStrings(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);

            var arrays = dataImport.ExportToStringArrays();
            SystemMessageRelay.RelayMethodReturn(returnDescription: "array count: " + arrays.Count(), group: MessageRelayGroup);
            return arrays;
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
        /// <returns></returns>
        public static IEnumerable<object[]> AsObjects(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var dataImport = AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);

            var arrays = dataImport.ExportToObjectArrays(); 
            SystemMessageRelay.RelayMethodReturn(returnDescription: "array count: " + arrays.Count(), group: MessageRelayGroup);
            return arrays;
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
        /// <returns></returns>
        public static ExcelDataImport AsDataImport(FilePath file, IEnumerable<Column> columns = null, bool enforceColumnCount = false, int sheetNum = 0, bool hasHeaderRow = false, BlankRowPolicy blankRowPolicy = default, DataErrorHandlingPolicy errorHandlingPolicy = default, AutoTruncate autoTrunc = AutoTruncate.Trim)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            ExcelDataImport dataImport;

            switch (file.Extension.ToLower())
            {
                case ".xls":
                    dataImport = Xls.AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);
                    break;
                case ".xlsx":
                    dataImport = Xlsx.AsDataImport(file, columns: columns, enforceColumnCount: enforceColumnCount, sheetNum: sheetNum, hasHeaderRow: hasHeaderRow, blankRowPolicy: blankRowPolicy, errorHandlingPolicy: errorHandlingPolicy, autoTrunc: autoTrunc);
                    break;
                default:
                    throw new DataImportException("Only .xls and .xlsx files are supported at this time");
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
            return dataImport;
        }

        static void ReadExcelRow(List<object> rawValues, IRow row, AutoTruncate autoTrunc, DataErrorHandlingPolicy dataErrorHandling)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            if (row.IsBlank())
            {
                SystemMessageRelay.RelayMessage("encountered blank row", group: MessageRelayGroup);
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
                        SystemMessageRelay.RelayMessage("blank cell " + cellAddress + " -> [null]", group: MessageRelayGroup);
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
                                SystemMessageRelay.RelayMessage("string cell " + cellAddress + " -> " + TextUtil.Reveal(value), group: MessageRelayGroup);
                                break;
                            case CellType.Numeric:
                                value = cell.NumericCellValue;
                                SystemMessageRelay.RelayMessage("numeric cell " + cellAddress + " -> " + value, group: MessageRelayGroup);
                                break;
                            case CellType.Boolean:
                                value = cell.BooleanCellValue;
                                SystemMessageRelay.RelayMessage("boolean cell " + cellAddress + " -> " + value, group: MessageRelayGroup);
                                break;
                            case CellType.Error:
                                switch (dataErrorHandling)
                                {
                                    case DataErrorHandlingPolicy.Throw:
                                    default:
                                        throw new DataImportException("Cell error: " + cellAddress + " (code = " + cell.ErrorCellValue + ")");
                                    case DataErrorHandlingPolicy.Embed:
                                        value = new DataError("Cell error: " + cellAddress + " (code = " + cell.ErrorCellValue + ")");
                                        SystemMessageRelay.RelayMessage("error cell " + cellAddress + " (code = " + cell.ErrorCellValue + ") -> " + TextUtil.Reveal(value), group: MessageRelayGroup);
                                        break;
                                    case DataErrorHandlingPolicy.IgnoreAndUseDefaultValue:
                                        value = autoTrunc == AutoTruncate.Zap ? null : "";
                                        SystemMessageRelay.RelayMessage("error cell " + cellAddress + " (code = " + cell.ErrorCellValue + ") -> " + TextUtil.Reveal(value), group: MessageRelayGroup);
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
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }
    }
}
