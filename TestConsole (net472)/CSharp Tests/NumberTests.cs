using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;

namespace TestConsole.CSharpTests
{
    class NumberTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Round/Floor/Ceiling",
                () =>
                {
                    var decimalPlaces = new[] { -2, -1, 0, 1, 2 };
                    decimal myDecimal = 123.456789m;
                    RenderX.ListTitle("Decimal");
                    Console.WriteLine("#  " + myDecimal);
                    Console.WriteLine();
                    RenderX.ListTitle("Round");
                    foreach(var dec in decimalPlaces)
                    {
                        Console.Write(dec.ToString().PadRight(3));
                        try
                        {
                            Console.WriteLine(Math.Round(myDecimal, dec));
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"{ex.Message.Replace("\r", "").Replace("\n", " ").Replace(" Parameter name: decimals", "")} ({ex.GetType().Name})");
                        }
                    }
                    Console.WriteLine();
                    RenderX.ListTitle("Floor");
                    Console.WriteLine("".PadRight(3) + Math.Floor(myDecimal));
                    Console.WriteLine();
                    RenderX.ListTitle("Ceiling");
                    Console.WriteLine("".PadRight(3) + Math.Ceiling(myDecimal));
                }
            )
        };
    }
}
