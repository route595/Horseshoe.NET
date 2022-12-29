using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// The backing structure and logic behind data imports
    /// </summary>
    public class DataImport : List<ImportedRow>
    {
        private readonly List<Column> _columns;

        /// <summary>
        /// A <c>Column[]</c> view of the column metadata used during import
        /// </summary>
        public Column[] Columns => _columns?.ToArray();

        /// <summary>
        /// The column count
        /// </summary>
        public int ColumnCount => _columns?.Count ?? 0;

        /// <summary>
        /// The count of mapped columns only
        /// </summary>
        public int MappedColumnCount => _columns?.Count(c => !c.NotMapped) ?? 0;

        /// <summary>
        /// If <c>true</c> the parsing engine will pad short rows with blank or <c>null</c> values and throw an exception if the row is too long
        /// </summary>
        public bool EnforceColumnCount { get; }

        /// <summary>
        /// Keeps a tally of blank rows that were skipped to keep row number integrity
        /// </summary>
        public int SkippedRows { get; internal set; }

        /// <summary>
        /// A count of the rows that have been imported
        /// </summary>
        public int RowCount => Count;

        /// <summary>
        /// The calculated row number of the source row currently being parsed
        /// </summary>
        public int NextRow => RowCount + SkippedRows + 1;

        /// <summary>
        /// How to interpret empty values
        /// </summary>
        public AutoTruncate AutoTrunc { get; set; }

        /// <summary>
        /// How to handle blank rows, specifically leading and trailing
        /// </summary>
        public BlankRowPolicy BlankRowPolicy { get; set; }

        /// <summary>
        /// How to handle data errors
        /// </summary>
        public DataErrorHandlingPolicy DataErrorHandlingPolicy { get; set; }

        /// <summary>
        /// Errors are populated during certain export operations e.g. <c>ExportToObjectArrays()</c>
        /// </summary>
        public IEnumerable<DataError> DataErrors { get; set; }

        /// <summary>
        /// The number of data errors that occurred during the last data import
        /// </summary>
        public int DataErrorCount => DataErrors?.Count() ?? 0;

        /// <summary>
        /// Creates a <c>DataImport</c> instance
        /// </summary>
        public DataImport()
        {
            _columns = new List<Column>();
        }

        /// <summary>
        /// Creates a <c>DataImport</c> instance
        /// </summary>
        /// <param name="columns">a collection of columns</param>
        /// <param name="enforceColumnCount">if <c>true</c> produces table-shaped data as well as unlocks methods such as <c>ExportAsObjects()</c></param>
        public DataImport(IEnumerable<Column> columns, bool enforceColumnCount = false) : this()
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

        /// <summary>
        /// You can add new columns at any time, even during an import.  If <c>EnforceColumnCount == true</c> then already imported rows will grow in size.
        /// </summary>
        /// <param name="column">A <c>Column</c></param>
        /// <exception cref="DataImportException"></exception>
        public void AddColumn(Column column)
        {
            //if (EnforceColumnCount && RowCount > 0)
            //    throw new DataImportException("columns cannot be added once data import has begun: " + RowCount + " imported row(s)");

            if (column == null)
                throw new DataImportException("column may not be null");
            _columns.Add(column);

            if (EnforceColumnCount && RowCount > 0 && !column.NotMapped)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    this[i].UpdateValues(ArrayUtil.Append(this[i].Values, HandleImportedDatum("")));
                }
            }
        }

        /// <summary>
        /// You can add new columns at any time, even during an import.  If <c>EnforceColumnCount == true</c> then already imported rows will grow in size.
        /// </summary>
        /// <param name="name">The column name</param>
        /// <param name="dataType">The column's data type</param>
        /// <param name="fixedWidth">The column's fixed width, only set this value for fixed-width imports</param>
        /// <param name="parser">A custom string-to-object converter, if required</param>
        /// <param name="formatter">A custom object-to-string formatter, if required</param>
        public void AddColumn(string name, Type dataType = null, int fixedWidth = 0, Func<string, object> parser = null, Func<object, string> formatter = null)
        {
            AddColumn(new Column(name) { DataType = dataType, FixedWidth = fixedWidth, Parser = parser, Formatter = formatter });
        }

        /// <summary>
        /// This method populates a <c>List&lt;string&gt;</c> with the raw parsed row data
        /// </summary>
        /// <param name="rawImportedRow"></param>
        /// <param name="sourceRowNumber"></param>
        /// <exception cref="DataImportException"></exception>
        /// <exception cref="StopImportingDataException"></exception>
        public void ImportRaw(List<string> rawImportedRow, int sourceRowNumber)
        {
            if (EnforceColumnCount && MappedColumnCount == 0)
                throw new DataImportException("data cannot be imported until 1 or more mapped columns has been added");

            if (ImportUtil.IsBlankRow(rawImportedRow))
            {
                switch (BlankRowPolicy)
                {
                    case BlankRowPolicy.Allow:
                        Add(new ImportedRow(sourceRowNumber, null));
                        break;
                    case BlankRowPolicy.Drop:
                        SkippedRows++;
                        break;
                    case BlankRowPolicy.DropLeading:  // note: DropTrailing handled later, FinalizeImport()
                    case BlankRowPolicy.DropLeadingAndTrailing:
                        if (RowCount == 0)
                        {
                            SkippedRows++;
                        }
                        else
                        {
                            Add(new ImportedRow(sourceRowNumber, null));
                        }
                        break;
                    case BlankRowPolicy.StopImporting:
                        throw new StopImportingDataException();
                    case BlankRowPolicy.Error:
                        throw new DataImportException("encounterd blank row on source line #" + NextRow);
                }
            }
            else if (EnforceColumnCount && MappedColumnCount > 0 && rawImportedRow.Count > MappedColumnCount)
            {
                throw new DataImportException(rawImportedRow.Count + " items on source line #" + NextRow + " could not be imported (the import mechanism is only tracking " + ColumnCount + " columns and EnforceColumnCount is 'true')");
            }
            else
            {
                if (EnforceColumnCount)
                {
                    while (rawImportedRow.Count < MappedColumnCount)
                    {
                        rawImportedRow.Add("");
                    }
                }
                Add(new ImportedRow(sourceRowNumber, rawImportedRow.Select(s => HandleImportedDatum(s))));
            }
        }

        /// <summary>
        /// Applies <c>AutoTrunc</c> (and potentially other future preferences) to each parsed datum
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>
        private string HandleImportedDatum(string datum)
        {
            switch (AutoTrunc)
            {
                case AutoTruncate.None:
                default:
                    return datum;
                case AutoTruncate.Trim:
                    return datum.Trim();
                case AutoTruncate.Zap:
                    return Zap.String(datum);
            }
        }

        /// <summary>
        /// Finalizes the import by applying <c>BlankRowPolicy</c> (and potentially other future preferences) to each parsed row
        /// </summary>
        /// <exception cref="DataImportException"></exception>
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
                            throw new DataImportException("Found empty row at source line #" + row.SourceLineNumber);
                    }
                    break;
            }
        }

        /// <summary>
        /// Extract the data from each <c>ImportedRow</c> and generate and return an <c>IEnumerable&lt;string[]&gt;</c>
        /// </summary>
        /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
        public IEnumerable<string[]> ExportToStringArrays()
        {
            var list = new List<string[]>();
            foreach (var row in this)
            {
                list.Add(row.Values);
            }
            return list;
        }

        /// <summary>
        /// Extract the data from each <c>ImportedRow</c> and generate and return an <c>IEnumerable&lt;object[]&gt;</c> 
        /// based on the supplied column metadata
        /// </summary>
        /// <returns>an <c>IEnumerable&lt;object[]&gt;</c></returns>
        /// <exception cref="DataImportException"></exception>
        public IEnumerable<object[]> ExportToObjectArrays()
        {
            if (!EnforceColumnCount)
                throw new DataImportException("cannot export unless EnforceColumnCount = 'true'; 'false' results in a lack of data contracts that are strict enough to guarantee exportability - see constructor for more details");
            if (ColumnCount == 0)
                throw new DataImportException("data cannot be exported (or imported) until 1 or more columns has been added");

            var list = new List<object[]>();
            for (int r = 0; r < RowCount; r++)
            {
                if (this[r].Values != null)
                {
                    var objectArray = new object[ColumnCount];       // create new array
                    for (int c = 0; c < ColumnCount; c++)
                    {                                                // fill array
                        objectArray[c] = _columns[c].Parse(this[r].Values[c], c + 1, this[r].SourceLineNumber, errorHandling: DataErrorHandlingPolicy);
                    }
                    list.Add(objectArray);                           // add array to result
                }
                else
                {
                    list.Add(null);
                }
            }
            DataErrors = list
                .Where(oArray => oArray != null)
                .SelectMany(oArray => oArray.Where(o => o is DataError).Select(o => (DataError)o))
                .ToList();
            return list;
        }

        /// <summary>
        /// Extract the data from each <c>ImportedRow</c> and generate an <c>IEnumerable&lt;object[]&gt;</c> 
        /// based on the supplied column metadata, then formatting the data back to <c>string</c> according to the column metadata
        /// </summary>
        /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
        /// <remarks><seealso cref="ExportToObjectArrays"/></remarks>
        public IEnumerable<string[]> ExportToFormattedObjectStringArrays()
        {
            return ExportToFormattedObjectStringArrays(Columns, ExportToObjectArrays());
        }


        /// <summary>
        /// Extract the data from each <c>ImportedRow</c> and generate an <c>IEnumerable&lt;object[]&gt;</c> 
        /// based on the supplied column metadata, then formatting the data back to <c>string</c> according to the column metadata
        /// </summary>
        /// <param name="columns">A collection of column metadata</param>
        /// <param name="objectArrays">an <c>IEnumerable&lt;object[]&gt;</c></param>
        /// <returns>an <c>IEnumerable&lt;string[]&gt;</c></returns>
        /// <remarks><seealso cref="ExportToObjectArrays"/></remarks>
        public static IEnumerable<string[]> ExportToFormattedObjectStringArrays(IEnumerable<Column> columns, IEnumerable<object[]> objectArrays)
        {
            if (columns == null || !columns.Any())
                throw new DataImportException("data cannot be exported unless 1 or more columns has been supplied");

            var columnArray = (columns is Column[] _columnArray)
                ? _columnArray
                : columns.ToArray();
            var list = new List<string[]>();
            foreach (var objectArray in objectArrays)
            {
                if (objectArray.Length != columnArray.Length)
                    throw new DataImportException("number of row values does not match number of columns: " + objectArray.Length + " / " + columnArray.Length);

                var stringArray = new string[objectArray.Length];    // create new array
                for (int c = 0; c < columnArray.Length; c++)
                {                                                    // fill array
                    stringArray[c] = columnArray[c].Format(objectArray[c]);
                }
                list.Add(stringArray);                               // add array to result
            }
            return list;
        }
    }
}
