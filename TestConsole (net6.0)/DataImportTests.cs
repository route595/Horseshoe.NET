using System;
using System.Collections.Generic;

using Horseshoe.NET;
using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DataImport;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class DataImportTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Direct import, no columns, exported to strings",
                () =>
                {
                    var dataImport = new DataImport
                    {
                        AutoTrunc = AutoTruncate.Zap
                    };
                    dataImport.ImportRaw(new List<string>{ "Carlos", "", "4/9/1945" }, 0);
                    dataImport.ImportRaw(new List<string>{ "Beto", "10", "" }, 1);
                    dataImport.ImportRaw(new List<string>{ "" }, 2);
                    dataImport.ImportRaw(new List<string>(), 3);
                    dataImport.ImportRaw(new List<string>{ "Katia", "27", "12/2/1995", "Readers' Digest" }, 4);
                    dataImport.FinalizeImport();
                    Console.WriteLine(dataImport.ExportToStringArrays().Render(separator: "^"));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                }
            ),
            BuildMenuRoutine
            (
                "Direct import, 3 columns, exported to objects",
                () =>
                {
                    var columns = new []
                    { 
                        Column.String("Name"), 
                        Column.Int("Age"), 
                        Column.Date("Birth Date") 
                    };
                    var dataImport = new DataImport(columns, enforceColumnCount: true)
                    {
                        AutoTrunc = AutoTruncate.Zap
                    };
                    dataImport.ImportRaw(new List<string>{ "Carlos", "", "4/9/1945" }, 0);
                    dataImport.ImportRaw(new List<string>{ "Beto", "10", "" }, 1);
                    dataImport.ImportRaw(new List<string>{ "" }, 2);
                    dataImport.ImportRaw(new List<string>(), 3);
                    dataImport.ImportRaw(new List<string>{ "Katia", "27", "12/2/1995" }, 4);
                    dataImport.FinalizeImport();
                    Console.WriteLine(dataImport.ExportToObjectArrays().Render(separator: "^"));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                    dataImport.BlankRowPolicy = BlankRowPolicy.Drop;
                    dataImport.FinalizeImport();
                    Console.WriteLine(dataImport.ExportToObjectArrays().Render(separator: "^"));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                }
            ),
            BuildMenuRoutine
            (
                "Direct import, 3 columns, embedded errors, exported to objects",
                () =>
                {
                    var columns = new []
                    {
                        Column.String("Name"), 
                        Column.Int("Age"),
                        Column.Date("Birth Date")
                    };
                    var dataImport = new DataImport(columns, enforceColumnCount: true)
                    {
                        BlankRowPolicy = BlankRowPolicy.Drop,
                        DataErrorHandlingPolicy = DataErrorHandlingPolicy.Embed,
                        AutoTrunc = AutoTruncate.Zap
                    };
                    dataImport.ImportRaw(new List<string>{ "Carlos", "", "4/9/1945" }, 0);
                    dataImport.ImportRaw(new List<string>{ "Beto", "10", "" }, 1);
                    dataImport.ImportRaw(new List<string>{ "" }, 2);
                    dataImport.ImportRaw(new List<string>(), 3);
                    dataImport.ImportRaw(new List<string>{ "Katia", "_27", "12/2/1995" }, 4);
                    dataImport.FinalizeImport();
                    Console.WriteLine(dataImport.ExportToObjectArrays().Render(separator: "^"));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "Direct import, 3 columns, embedded errors, exported to formatted strings",
                () =>
                {
                    var columns = new []
                    {
                        Column.String("Name"),
                        Column.Int("Age"),
                        Column.Date("Birth Date", displayFormat: "MMMM d")
                    };
                    var dataImport = new DataImport(columns, enforceColumnCount: true)
                    {
                        BlankRowPolicy = BlankRowPolicy.Drop,
                        DataErrorHandlingPolicy = DataErrorHandlingPolicy.Embed,
                        AutoTrunc = AutoTruncate.Zap
                    };
                    dataImport.ImportRaw(new List<string>{ "Carlos", "", "4/9/1945" }, 0);
                    dataImport.ImportRaw(new List<string>{ "Beto", "10", "" }, 1);
                    dataImport.ImportRaw(new List<string>{ "" }, 2);
                    dataImport.ImportRaw(new List<string>(), 3);
                    dataImport.ImportRaw(new List<string>{ "Katia", "_27", "12/2/1995" }, 4);
                    dataImport.FinalizeImport();
                    Console.WriteLine(dataImport.ExportToFormattedObjectStringArrays().Render(separator: "^"));
                    Console.WriteLine("Row Count: " + dataImport.RowCount);
                    Console.WriteLine("Skipped Rows: " + dataImport.SkippedRows);
                    Console.WriteLine("Next Row: " + dataImport.NextRow);
                    PrintDataErrors(dataImport);
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "Import delimited text, exported to strings",
                () =>
                {
                    var rawData = @"
Name,Age,Birth Date
Carlos,89,4/9/1945
Beto,10,9/15/2012
Katia,27,12/2/1995
";
                    var dataImport = ImportData.DelimitedText.AsDataImport(rawData, ',', hasHeaderRow: true, autoTrunc: AutoTruncate.Zap);
                    Console.WriteLine(dataImport.ExportToStringArrays().Render(separator: "^"));
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
                "Import delimited text, 3 columns (1 not-mapped), exported to formated objects, German dates",
                () =>
                {
                    var rawData = @"
Name,Age,Birth Date
Carlos,89,4/9/1945
Beto,10,9/15/2012
Katia,27,12/2/1995
";
                    var columns = new []
                    {
                        Column.String("Name"),
                        Column.NoMap("Age"),
                        Column.Date("Birth Date", displayLocale: "de-DE")
                    };
                    var dataImport = ImportData.DelimitedText.AsDataImport(rawData, ',', columns, enforceColumnCount: true, hasHeaderRow: true, autoTrunc: AutoTruncate.Zap);
                    Console.WriteLine(dataImport.ExportToFormattedObjectStringArrays().Render(separator: "^"));
                    Console.WriteLine("Column Count: " + dataImport.ColumnCount);
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
                "Import fixed-width text, exported to strings",
                () =>
                {
                    var rawData = @"
Name      Age DOB
Carlos    008919450409
Beto      000920130915
Katia     002719951202
";
                    var columns = new[]
                    {
                        Column.String("Name", fixedWidth: 10),
                        Column.Int("Age", fixedWidth: 4),
                        Column.Flat8Date("Birth Date")
                    };
                    var dataImport = ImportData.FixedWidthText.AsDataImport(rawData, columns, hasHeaderRow: true, autoTrunc: AutoTruncate.Zap);
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
                "Import fixed-width text, embedded errors, exported to objects",
                () =>
                {
                    var rawData = @"
Name      Age DOB
Carlos    008919450409
Beto      000920130915
Katia     0_2719951202
"; // -->> ^ error
                    var columns = new[]
                    {
                        Column.String("Name", fixedWidth: 10),
                        Column.Int("Age", fixedWidth: 4),
                        Column.Flat8Date("Birth Date")
                    };
                    var dataImport = ImportData.FixedWidthText.AsDataImport
                    (
                        rawData, 
                        columns, 
                        hasHeaderRow: true, 
                        errorHandlingPolicy: DataErrorHandlingPolicy.Embed,   //  <<-- 
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToObjectArrays().Render(separator: ","));
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
                "Import fixed-width file, exported to strings",
                () =>
                {
                    var columns = new[]
                    {
                        Column.String("Name", fixedWidth: 10),
                        Column.Int("Age", fixedWidth: 4),
                        Column.Flat8Date("Birth Date")
                    };
                    var dataImport = ImportData.FixedWidthTextFile.AsDataImport("fixed-width.txt", columns, hasHeaderRow: true, autoTrunc: AutoTruncate.Zap);
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
                "Import fixed-width file, 1 not-mapped column, exported to strings",
                () =>
                {
                    var columns = new[]
                    {
                        Column.String("Name", fixedWidth: 10),
                        Column.NoMap("Age", fixedWidth: 4),
                        Column.Flat8Date("Birth Date")
                    };
                    var dataImport = ImportData.FixedWidthTextFile.AsDataImport("fixed-width.txt", columns, hasHeaderRow: true, autoTrunc: AutoTruncate.Zap);
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
                "Import fixed-width file, embedded errors, exported to objects",
                () =>
                {
                    var columns = new[]
                    {
                        Column.String("Name", fixedWidth: 10),
                        Column.Int("Age", fixedWidth: 4),
                        Column.Flat8Date("Birth Date")
                    };
                    var dataImport = ImportData.FixedWidthTextFile.AsDataImport
                    (
                        "fixed-width-err.txt", 
                        columns,
                        hasHeaderRow: true,
                        errorHandlingPolicy: DataErrorHandlingPolicy.Embed,   //  <<-- 
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToObjectArrays().Render(separator: ","));
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
                "Import fixed-width file, 1 not-mapped column, embedded errors, exported to objects",
                () =>
                {
                    var columns = new[]
                    {
                        Column.String("Name", fixedWidth: 10),
                        Column.NoMap("Age", fixedWidth: 4),
                        Column.Flat8Date("Birth Date")
                    };
                    var dataImport = ImportData.FixedWidthTextFile.AsDataImport
                    (
                        "fixed-width-err.txt",
                        columns,
                        hasHeaderRow: true,
                        errorHandlingPolicy: DataErrorHandlingPolicy.Embed,   //  <<-- 
                        autoTrunc: AutoTruncate.Zap
                    );
                    Console.WriteLine(dataImport.ExportToObjectArrays().Render(separator: ","));
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

        void PrintDataErrors(DataImport dataImport)
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
