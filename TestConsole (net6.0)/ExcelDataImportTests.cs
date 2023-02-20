using System;
using System.Collections.Generic;

using Horseshoe.NET;
using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DataImport;
using Horseshoe.NET.Excel;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class ExcelDataImportTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Import Excel file, no columns",
                () =>
                {
                    var dataImport = ImportExcelData.AsDataImport
                    (
                        "excel.xlsx",
                        hasHeaderRow: true,
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToStringArrays().Render(separator: ","));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                    Console.WriteLine("Journal");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.LastJournal));
                }
            ),
            BuildMenuRoutine
            (
                "Import Excel file",
                () =>
                {
                    var columns = new[]
                    {
                        Column.String("Name"),
                        ExcelColumn.ExcelDate("Date of Birth"),
                        Column.Int("Age")
                    };
                    var dataImport = ImportExcelData.AsDataImport
                    (
                        "excel.xlsx",
                        columns: columns,
                        hasHeaderRow: true,
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToStringArrays().Render(separator: ","));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                    Console.WriteLine("Journal");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.LastJournal));
                }
            ),
            BuildMenuRoutine
            (
                "Import Excel file, embedded errors, no columns",
                () =>
                {
                    var dataImport = ImportExcelData.AsDataImport
                    (
                        "excel-err.xlsx",
                        hasHeaderRow: true,
                        errorHandlingPolicy: DataErrorHandlingPolicy.Embed,
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToStringArrays().Render(separator: ","));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                    Console.WriteLine("Journal");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.LastJournal));
                }
            ),
            BuildMenuRoutine
            (
                "Import Excel file, embedded errors",
                () =>
                {
                    var columns = new[]
                    {
                        Column.String("Name"),
                        ExcelColumn.ExcelDate("Date of Birth"),
                        Column.Int("Age")
                    };
                    var dataImport = ImportExcelData.AsDataImport
                    (
                        "excel-err.xlsx",
                        columns: columns,
                        hasHeaderRow: true,
                        errorHandlingPolicy: DataErrorHandlingPolicy.Embed,
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToStringArrays().Render(separator: ","));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                    Console.WriteLine("Journal");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.LastJournal));
                }
            )
        };

        void PrintDataErrors(ExcelDataImport dataImport)
        {
            if (dataImport.DataErrorCount > 0)
            {
                Console.WriteLine("Data Errors: " + dataImport.DataErrorCount);
                foreach (var dataError in dataImport.DataErrors)
                {
                    Console.WriteLine("  " + dataError.Print());
                }
            }
        }
    }
}
