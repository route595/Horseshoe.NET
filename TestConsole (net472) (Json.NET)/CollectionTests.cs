﻿using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole
{
    class CollectionTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Combine/append dictionaries",
                () =>
                {
                    var dict1 = new Dictionary<string, string>
                    {
                        { "one", "1" },
                        { "two", "2" },
                        { "three", "3" }
                    };
                    var dict2 = new Dictionary<string, string>
                    {
                        { "three", "three" },
                        { "four", "four" },
                        { "five", "five" }
                    };
                    Console.WriteLine(dict1.StringDump());
                    Console.WriteLine(dict2.StringDump());
                    Console.WriteLine("combine: " + DictionaryUtil.CombineRTL(dict1, dict2).StringDump());
                    Console.WriteLine("combine: " + DictionaryUtil.CombineLTR(dict1, dict2).StringDump());
                    Console.WriteLine("combine: " + DictionaryUtil.Combine(new[] { dict1, dict2 }, options: new MergeOptions<string, string>{ Merge = (leftDict, rightDict, key) => leftDict[key] + "/" + rightDict[key] }).StringDump());
                    Console.WriteLine("append: " + dict1.AppendRTL(dict2).StringDump());
                    Console.WriteLine("append: " + dict1.AppendLTR(dict2).StringDump());
                    Console.WriteLine("append: " + dict1.Append(dict2, options: new MergeOptions<string, string>{ Merge = (leftDict, rightDict, key) => leftDict[key] + "/" + rightDict[key] }).StringDump());
                }
            ),
            BuildMenuRoutine
            (
                "Array Append",
                () =>
                {
                    var messages = new[]
                    {
                        "DATA: ApiName = Blah",
                        "DATA: CallingEntityName = Blah"
                    };
                    messages = messages.Append("FROM_CACHE: Blah").AppendIf(false, "FROM_CANADA: Maple");
                    Console.WriteLine(string.Join(Environment.NewLine, messages));
                }
            ),
            BuildMenuRoutine
            (
                "Array Pad/Crop",
                () =>
                {
                    var messages = new[]
                    {
                        "DATA: ApiName = Blah",
                        "DATA: CallingEntityName = Blah"
                    };
                    messages = messages.Pad(4, padWith: "[pad]");
                    Console.WriteLine(string.Join(Environment.NewLine, messages));
                    messages = messages.Crop(3);
                    Console.WriteLine(string.Join(Environment.NewLine, messages));
                }
            ),
            BuildMenuRoutine
            (
                "Pad",
                () =>
                {
                    var finalList = new List<string>();
                    var namesArray = new[] { "Trinidad", "Tobago" };
                    var namesCollection = namesArray.AsEnumerable();
                    namesArray = namesArray.Pad(4);
                    namesCollection = namesCollection.Pad(4, boundary: CollectionBoundary.Start);
                    finalList.AddRange(namesArray);
                    finalList.AddRange(namesCollection);
                    var textGrid = new TextGrid(finalList, columns: 2);
                    textGrid.Columns[0].Title = "Padded Array";
                    textGrid.Columns[1].Title = "Padded Collection";
                    Console.Write(textGrid.Render());
                }
            ),
            BuildMenuRoutine
            (
                "Identical",
                () =>
                {
                    var arrays = new[]
                    {
                        new []{ "Value 1", "Value 2", "Value 3", "Value 4", "Value 1", "Value 2", "Value 3" },
                        new []{ "Value 1", "Value 2", "Value 3", "Value 4", "Value 1", "Value 2", "Value 3", "Value 4" },
                        new []{ "Value 1", "Value 2", "Value 3", "Value 4", "Value 4", "Value 3", "Value 2", "Value 1" },
                        new []{ "Value 1", "VALUE 2", "Value 3", "Value 4", "Value 1", "Value 2", "VALUE 3", "Value 4" },
                        new []{ "Value 1", "VALUE 2", "Value 3", "Value 4", "VALUE 4", "Value 3", "Value 2", "Value 1" }
                    };
                    foreach (var array in arrays)
                    {
                        var textGrid = new TextGrid(array, 2);
                        Console.Write(textGrid.Render());
                        Console.WriteLine("Equivalent?  " + CollectionUtil.IsEquivalent(textGrid.Columns[0], textGrid.Columns[1])
                            + "   **   ignore-case: " + CollectionUtil.IsEquivalentEq(textGrid.Columns[0].Cast<string>(), textGrid.Columns[1].Cast<string>(), ignoreCase: true)
                            + "   **   ignore-order: " + CollectionUtil.IsEquivalent(textGrid.Columns[0], textGrid.Columns[1], ignoreOrder: true)
                            + "   **   ignore-both: " + CollectionUtil.IsEquivalentEq(textGrid.Columns[0].Cast<string>(), textGrid.Columns[1].Cast<string>(), ignoreCase: true, ignoreOrder: true)
                            );
                        Console.WriteLine();
                    }
                }
            )
        };
    }
}
