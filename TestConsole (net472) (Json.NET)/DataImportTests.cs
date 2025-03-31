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
                    var dataImport = TabularData.ImportCommaDelimitedText
                    (
                        new[]
                        {
                            "Carlos,,4/9/1945",
                            "Beto,10,",
                            "",
                            null,
                            "Katia", "27", "12/2/1995", "Readers' Digest"
                        },
                        new[]
                        {
                            Column.String(name: "Name"),
                            Column.Int(name: "Age"),
                            Column.DateTime(name: "Birthdate")
                        }
                    );

                    Console.WriteLine(dataImport.ExportToTextGrid().Render());
                    ;                    //PrintDataErrors(dataImport);
                }
            ),
            BuildMenuRoutine
            (
                "Direct import, 3 columns, exported to objects",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Direct import, 3 columns, embedded errors, exported to objects",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Direct import, 3 columns, embedded errors, exported to formatted strings",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import delimited text, exported to strings",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import delimited text, 3 columns (1 not-mapped), exported to formated objects, German dates",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import fixed-width text, exported to strings",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import fixed-width text, embedded errors, exported to objects",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import fixed-width file, exported to strings",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import fixed-width file, 1 not-mapped column, exported to strings",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import fixed-width file, embedded errors, exported to objects",
                () =>
                {
                }
            ),
            BuildMenuRoutine
            (
                "Import fixed-width file, 1 not-mapped column, embedded errors, exported to objects",
                () =>
                {
                }
            )
        };

        //void PrintDataErrors(TabularData dataImport)
        //{
        //    if (dataImport.DataErrorCount > 0)
        //    {
        //        Console.WriteLine("Data Errors: " + dataImport.DataErrorCount);
        //        foreach (var dataError in dataImport.DataErrors)
        //        {
        //            Console.WriteLine("  " + dataError.Print());
        //        }
        //    }
        //}
    }
}
