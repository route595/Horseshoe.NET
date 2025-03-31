using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole
{
    class TextGridTests : RoutineX
    {
        public override string Text => "TextGrid Tests";

        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Empty grid",
                () =>
                {
                    var grid = new TextGrid(Array.Empty<string>());
                    Console.WriteLine(grid.Render());
                }
            ),
            BuildMenuRoutine
            (
                "Empty grid (no columns)",
                () =>
                {
                    var grid = new TextGrid();
                    Console.WriteLine(grid.Render());
                }
            ),
            BuildMenuRoutine
            (
                "3 List Grids",
                () =>
                {
                    var daysOfWeek = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};
                    var grid = new TextGrid(daysOfWeek, columns: 2);
                    Console.WriteLine("list grid...");
                    Console.WriteLine(grid.Render());
                    Console.WriteLine();
                    grid.BorderPolicy = BorderPolicy.All;
                    Console.WriteLine("list grid + borders...");
                    Console.WriteLine(grid.Render());
                    Console.WriteLine();
                    //grid.CellPadding = 1;
                    grid.PaddingPolicy = CellPaddingPolicy.All;
                    grid.OuterPaddingLeft = grid.OuterPaddingRight = 2;
                    Console.WriteLine("list grid + borders + padding...");
                    Console.WriteLine(grid.Render());
                }
            ),
            BuildMenuRoutine
            (
                "3 List Grids (Wide)",
                () =>
                {
                    var daysOfWeek = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};
                    var grid = new TextGrid(daysOfWeek, columns: 2, targetWidth: RenderX.ConsoleWidth);
                    Console.WriteLine("list grid (wide)...");
                    Console.WriteLine(grid.Render());
                    Console.WriteLine();
                    grid.BorderPolicy = BorderPolicy.All;
                    Console.WriteLine("list grid (wide) + borders...");
                    Console.WriteLine(grid.Render());
                    Console.WriteLine();
                    //grid.CellPadding = 1;
                    grid.PaddingPolicy = CellPaddingPolicy.All;
                    grid.OuterPaddingLeft = grid.OuterPaddingRight = 2;
                    Console.WriteLine("list grid (wide) + borders + padding...");
                    Console.WriteLine(grid.Render());
                }
            ),
            BuildMenuRoutine
            (
                "Grid with borders, labels & currency",
                () =>
                {
                    var personWages = new[]
                    {
                        new TextGridTestPersonWage { Person = "Sally", HourlyWage = 12.45m },
                        new TextGridTestPersonWage { Person = "Fred", HourlyWage = 13.56m },
                        new TextGridTestPersonWage { Person = "Bob", HourlyWage = 9.75m },
                        new TextGridTestPersonWage { Person = "Jessica", HourlyWage = 41.12m },
                        new TextGridTestPersonWage { Person = "Steve", HourlyWage = 20.02m }
                    };
                    var grid = new TextGrid{ BorderPolicy = BorderPolicy.All, PaddingPolicy = CellPaddingPolicy.Vertical };
                    var col1 = new Column { Name = "Person" };
                    var col2 = new Column { Name = "Hourly Wage", DisplayFormat = "C" };
                    foreach(var pw in personWages)
                    {
                        col1.List.Add(pw.Person);
                        col2.List.Add(pw.HourlyWage);
                    }
                    grid.Columns.Add(col1);
                    grid.Columns.Add(col2);
                    Console.WriteLine(grid.Render());
                }
            ),
            BuildMenuRoutine
            (
                "Grid with borders, labels & nullable currency",
                () =>
                {
                    var personWages = new[]
                    {
                        new TextGridTestPersonNWage { Person = "Sally", HourlyWage = 12.45m },
                        new TextGridTestPersonNWage { Person = "Fred", HourlyWage = 13.56m },
                        new TextGridTestPersonNWage { Person = "Bob", HourlyWage = 9.75m },
                        new TextGridTestPersonNWage { Person = "Jessica", HourlyWage = 0m },
                        new TextGridTestPersonNWage { Person = "Steve", HourlyWage = null }
                    };
                    var grid = new TextGrid{ BorderPolicy = BorderPolicy.All, PaddingPolicy = CellPaddingPolicy.Vertical };
                    var col1 = new Column { Name = "Person" };
                    var col2 = new Column { Name = "Hourly Wage", DisplayFormat = "C" };
                    foreach(var pw in personWages)
                    {
                        col1.List.Add(pw.Person);
                        col2.List.Add(pw.HourlyWage);
                    }
                    grid.Columns.Add(col1);
                    grid.Columns.Add(col2);
                    Console.WriteLine(grid.Render());
                }
            )
        };
    }

    class TextGridTestPersonWage
    {
        public string Person { get; set; }
        public decimal HourlyWage { get; set; }
    }

    class TextGridTestPersonNWage
    {
        public string Person { get; set; }
        public decimal? HourlyWage { get; set; }
    }
}
