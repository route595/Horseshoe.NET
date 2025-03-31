using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DataImport;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Tests
{
    class Program : ConsoleXApp
    {
        public override IList<MenuObject> MainMenu => new MenuObject[] 
        {
            BuildMenuRoutine
            (
                "char -> hex",
                () =>
                {
                    var ci = CharInfo.Get('m');
                    Console.WriteLine(ci);
                    Console.WriteLine("(x) => " + ((int)ci.Char).ToString("x"));
                    Console.WriteLine("(X) => " + ((int)ci.Char).ToString("X"));
                    Console.WriteLine(ci.ToHexString());
                    Console.WriteLine(ci.ToUnicodeHexString());
                }
            ),
            BuildMenuRoutine
            (
                "lookup chars by category",
                () =>
                {
                    Console.WriteLine("ASCII Numeric");
                    var list = CharUtil.FilterCharInfosBy(charCategory: CharCategory.Numeric, isASCII: true);
                    foreach (var ci in list)
                    {
                        Console.WriteLine(ci);
                    }
                    Console.WriteLine();
                    Console.WriteLine("ASCII+Unicode Alphanumeric");
                    list = CharUtil.FilterCharInfosBy(charCategory: CharCategory.AlphaNumeric, maxCharIndex: 312);
                    foreach (var ci in list)
                    {
                        Console.WriteLine(ci);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "for + continuous for",
                () =>
                {
                    int dsi = -1;
                    for (int a = 'A'; a <= 'Z'; a++)
                    {
                        for (; dsi <= 21; )
                        {
                            if (++dsi % 3 == 0)
                            {
                                Console.WriteLine((char)a + "" + (dsi + 1));
                                break;
                            }
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Display Array [ \"a\", \"34\", \"20250102\" ]",
                () =>
                {
                    var values = new[]
                    {
                        "a", "34", "20250102"
                    };
                    Console.WriteLine(ValueUtil.Display(values));
                }
            ),
            BuildMenuRoutine
            (
                "Test DataImport - Parse Rows",
                () =>
                {
                    var rawRows = new[]
                    {
                        "a,34,20250102",
                        "\"Tall, dark and handsome\",44,20200405",
                        "c,54,20220809"
                    };
                    foreach (var rawRow in rawRows)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Raw data -> \"" + rawRow + "\"");
                        Console.Write("quot=fls -> ");
                        Console.WriteLine(ValueUtil.Display(TabularData.ParseDelimitedRow(rawRow, ',', new DataImportOptions{ SuppressQuotationIsolation = true })));
                        Console.Write("quot=tru -> ");
                        Console.WriteLine(ValueUtil.Display(TabularData.ParseDelimitedRow(rawRow, ',')));
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Test DataImport - Text (default)",
                () =>
                {
                    var rawData = @"
a,34,20250102
b,44,20200405
c,54,20220809
";
                    Console.WriteLine("Raw data");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("\"" + rawData + "\"");
                    Console.WriteLine();
                    Console.WriteLine("Importing tabular data with auto-generated string columns...");
                    var dataImport = TabularData.ImportCommaDelimitedText(rawData);
                    Console.WriteLine(dataImport.ExportToTextGrid().Render());
                }
            ),
            BuildMenuRoutine
            (
                "Test DataImport - Typed - Manually named columns",
                () =>
                {
                    var rawData = @"
a,34,20250102
b,44,20200405
c,54,20220809
";
                    var columns = new Column[]
                    {
                        Column.String(name: "Alpha Index"),
                        Column.Int(name: "Age"),
                        Column.DateTimeYYYYMMDD(name: "Graduation Date")
                    };
                    Console.WriteLine("Raw data");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("\"" + rawData + "\"");
                    Console.WriteLine();
                    Console.WriteLine("Importing tabular data with auto-generated string columns...");
                    var dataImport = TabularData.ImportCommaDelimitedText(rawData, columns: columns);
                    Console.WriteLine(dataImport.ExportToTextGrid().Render());
                }
            ),
            BuildMenuRoutine
            (
                "Test DataImport - Text (default) - Column names from data source",
                () =>
                {
                    var rawData = @"
Alpha Index,Age,Graduation Date
a,34,20250102
b,44,20200405
c,54,20220809
";
                    var options = new DataImportOptions { HasHeaderRow = true };
                    Console.WriteLine("Raw data");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("\"" + rawData + "\"");
                    Console.WriteLine();
                    Console.WriteLine("Importing tabular data with auto-generated string columns...");
                    var dataImport = TabularData.ImportCommaDelimitedText(rawData, options: options);
                    Console.WriteLine(dataImport.ExportToTextGrid().Render());
                }
            )
        };

        static void Main(string[] args)
        {
            StartConsoleApp<Program>();
        }
    }
}
