using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DataImport;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Excel
{
    public class ExcelDataImport : List<ImportedExcelRow>
    {
        private readonly List<Column> _columns;

        public Column[] Columns => _columns.Any() ? _columns.ToArray() : null;

        public int ColumnCount => _columns.Count;

        public bool EnforceColumnCount { get; }

        public int SkippedRows { get; internal set; }

        public int RowCount => Count;

        public int NextRow => RowCount + SkippedRows + 1;

        public AutoTruncate AutoTrunc { get; set; }

        public BlankRowPolicy BlankRowPolicy { get; set; }

        public DataErrorHandlingPolicy DataErrorHandlingPolicy { get; set; }

        /// <summary>
        /// Populated once a method such as <c>ExportToObjectArrays()</c> has executed
        /// </summary>
        public IEnumerable<ImportError> DataErrors { get; set; }

        public int DataErrorCount => DataErrors?.Count() ?? 0;

        /// <summary>
        /// Creates a <c>DataImport</c> instance
        /// </summary>
        public ExcelDataImport()
        {
            _columns = new List<Column>();
        }

        /// <summary>
        /// Creates a <c>DataImport</c> instance
        /// </summary>
        /// <param name="columns">a collection of columns</param>
        /// <param name="enforceColumnCount">if <c>true</c> produces table-shaped data as well as unlocks methods such as <c>ExportAsObjects()</c></param>
        public ExcelDataImport(IEnumerable<Column> columns, bool enforceColumnCount = false) : this()
        {
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    AddColumn(column);
                }
            }
            EnforceColumnCount = enforceColumnCount;
        }

        public void AddColumn(Column column)
        {
            if (EnforceColumnCount && RowCount > 0)
                throw new DataImportException("columns cannot be added once data import has begun: " + RowCount + " imported row(s)");

            if (column == null)
                throw new DataImportException("column may not be null");
            _columns.Add(column);
        }

        public void AddColumn(string name, Type dataType = null, Func<object, object> sourceParser = null, Func<object, string> displayFormatter = null)
        {
            AddColumn(new Column{ Name = name, DataType = dataType, SourceParser = sourceParser, DisplayFormatter = displayFormatter });
        }

        public void ImportRaw(List<object> rawImportedRow, int sourceRowNumber)
        {
            if (EnforceColumnCount && ColumnCount == 0)
                throw new DataImportException("data cannot be imported until 1 or more columns has been added");

            if (ExcelImportUtil.IsBlankRow(rawImportedRow))
            {
                switch (BlankRowPolicy)
                {
                    case BlankRowPolicy.Allow:
                        Add(new ImportedExcelRow(sourceRowNumber, null));
                        break;
                    case BlankRowPolicy.Drop:
                        SkippedRows++;
                        break;
                    case BlankRowPolicy.DropLeading:  // note: DropTrailing handled later, see FinalizeImport()
                    case BlankRowPolicy.DropLeadingAndTrailing:
                        if (RowCount == 0)
                        {
                            SkippedRows++;
                        }
                        else
                        {
                            Add(new ImportedExcelRow(sourceRowNumber, null));
                        }
                        break;
                    case BlankRowPolicy.StopImporting:
                        throw new StopImportingDataException();
                    case BlankRowPolicy.Error:
                        throw new DataImportException("encounterd blank row on source row #" + NextRow);
                }
            }
            else if (EnforceColumnCount && ColumnCount > 0 && rawImportedRow.Count > ColumnCount)
            {
                throw new DataImportException(rawImportedRow.Count + " items on source row #" + NextRow + " could not be imported (the import mechanism is only tracking " + ColumnCount + " columns and EnforceColumnCount is 'true')");
            }
            else
            {
                if (Columns != null)
                {
                    for (int i = 0, colsToConvert = Math.Min(rawImportedRow.Count, ColumnCount); i < colsToConvert; i++)
                    {
                        if (rawImportedRow[i] != null && !(rawImportedRow[i] is ImportError) && Columns[i].SourceParser != null)
                        {
                            rawImportedRow[i] = Columns[i].SourceParser.Invoke(rawImportedRow[i]);
                        }
                    }
                }
                if (EnforceColumnCount)
                {
                    while (rawImportedRow.Count < ColumnCount)
                    {
                        rawImportedRow.Add(null);
                    }
                }
                Add(new ImportedExcelRow(sourceRowNumber, rawImportedRow));
            }
        }

        public void FinalizeImport()
        {
            switch (BlankRowPolicy)
            {
                case BlankRowPolicy.Drop:
                    for (int r = RowCount - 1; r >= 0; r--)
                    {
                        if (this[r].IsEmpty)
                        {
                            SkippedRows++;
                            RemoveAt(r);
                        }
                    }
                    break;
                case BlankRowPolicy.DropLeading:
                    while (this[0].IsEmpty)
                    {
                        SkippedRows++;
                        RemoveAt(0);
                    }
                    break;
                case BlankRowPolicy.DropTrailing:
                    while (this[RowCount - 1].IsEmpty)
                    {
                        SkippedRows++;
                        RemoveAt(RowCount - 1);
                    }
                    break;
                case BlankRowPolicy.DropLeadingAndTrailing:
                    while (this[0].IsEmpty)
                    {
                        SkippedRows++;
                        RemoveAt(0);
                    }
                    while (this[RowCount - 1].IsEmpty)
                    {
                        SkippedRows++;
                        RemoveAt(RowCount - 1);
                    }
                    break;
                case BlankRowPolicy.Error:
                    foreach (var row in this)
                    {
                        if (row.IsEmpty)
                            throw new DataImportException("found empty row at source row #" + row.SourceRowNumber);
                    }
                    break;
            }
            DataErrors = this
                .Where(row => row.Values != null)
                .Select(row => row.Values)
                .SelectMany(oArray => oArray.Where(o => o is ImportError).Select(o => (ImportError)o))
                .ToList();

        }

        public IEnumerable<object[]> ExportToObjectArrays()
        {
            var list = new List<object[]>();
            foreach (var row in this)
            {
                list.Add(row.Values);
            }
            return list;
        }

        public IEnumerable<string[]> ExportToStringArrays()
        {
            var list = new List<string[]>();
            string[] stringArray;
            foreach (var row in this)
            {
                if (row.Values != null)
                {
                    stringArray = new string[row.Values.Length];    // create new array
                    for (int i = 0; i < row.Values.Length; i++)
                    {
                        stringArray[i] = Columns?.Length > i        // fill array
                            ? Columns[i].Format(row.Values[i])
                            : row.Values[i]?.ToString();
                    }
                    list.Add(stringArray);                          // add array to result
                }
                else
                {
                    list.Add(null);
                }
            }
            return list;
        }
    }
}
