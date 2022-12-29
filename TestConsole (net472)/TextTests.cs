using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET;
using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextClean;

namespace TestConsole
{
    class TextTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Reveal",
                () =>
                {
                    var phrase1 = "Å¢t Øñę\u0000";
                    var phrase2 = "Rudolph the red-nosed reindeer" + Environment.NewLine + "had a very shiny nose!";
                    Console.WriteLine("phrase:");
                    Console.WriteLine(phrase1);
                    Console.WriteLine("revealed:");
                    Console.WriteLine(TextUtil.Reveal(phrase1, new RevealOptions{ CharsToReveal = RevealCharCategory.All }));
                    Console.WriteLine();
                    Console.WriteLine("phrase:");
                    Console.WriteLine(phrase2);
                    Console.WriteLine("revealed:");
                    Console.WriteLine(TextUtil.Reveal(phrase2, new RevealOptions{ CharsToReveal = RevealCharCategory.All }));
                    Console.WriteLine("revealed (newlines preserved):");
                    Console.WriteLine(TextUtil.Reveal(phrase2, new RevealOptions{ CharsToReveal = RevealCharCategory.All, PreserveNewLines = true}));
                }
            ),
            BuildMenuRoutine
            (
                "Quick ToASCIIPrintable() tests",
                () =>
                {
                    var phrase = "Å¢t Øñę\u0000";
                    Console.WriteLine("phrase: " + phrase);
                    Console.WriteLine("to ASCII: " + TextClean.ToASCIIPrintable(phrase));
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.DefaultEntries));
                    Console.WriteLine();
                    Console.WriteLine("to xtd ASCII: " + TextClean.ToASCIIPrintable(phrase, extendedASCII: true));
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.DefaultEntries));
                }
            ),
            BuildMenuRoutine
            (
                "Scan for duplicates in CharLib",
                /* 
output on 10/26/2022, then cleaned up...
Original : symbols - h - unicode: ['h'-8462] - pos: 1
Duplicate: symbols - h - unicode: ['h'-8462] - pos: 3
Original : symbols - h - unicode: ['?'-8463] - pos: 2
Duplicate: symbols - h - unicode: ['?'-8463] - pos: 4
Original : symbols - l - unicode: ['?'-737] - pos: 2
Duplicate: symbols - l - unicode: ['?'-737] - pos: 3
Original : symbols - m - unicode: ['ⁿ'-8319] - pos: 2
Duplicate: symbols - n - unicode: ['ⁿ'-8319] - pos: 1
Original : symbols - s - unicode: ['?'-738] - pos: 1
Duplicate: symbols - s - unicode: ['?'-738] - pos: 2
Original : symbols - x - unicode: ['?'-735] - pos: 1
Duplicate: symbols - x - unicode: ['?'-735] - pos: 4
Original : symbols - x - unicode: ['?'-739] - pos: 2
Duplicate: symbols - x - unicode: ['?'-739] - pos: 5
Original : symbols - | - unicode: ['?'-8286] - pos: 1
Duplicate: symbols - | - unicode: ['?'-8286] - pos: 7
Original : symbols - / - unicode: ['/'-8260] - pos: 0
Duplicate: symbols - / - unicode: ['/'-8260] - pos: 1
Original : symbols - t - unicode: ['+'-8224] - pos: 1
Duplicate: complex symbols - [dagger] - unicode: ['+'-8224] - pos: 0
                 */
                () =>
                {
                    var trackingDict = new Dictionary<int, string>();
                    foreach (var category_convDict in CharLib.AllUnicodeToASCIIConversions)
                    {
                        foreach (var replacement_unicodeCharArray in category_convDict.Value)
                        {
                            for (int i = 0; i < replacement_unicodeCharArray.Value.Length; i++)
                            {
                                char unicodeChar = replacement_unicodeCharArray.Value[i];
                                string display = category_convDict.Key + " - " + replacement_unicodeCharArray.Key + " - unicode: " + TextUtil.Reveal(string.Concat(unicodeChar), RevealOptions.All) + " - pos: " + i;
                                if (trackingDict.ContainsKey(unicodeChar))
                                {
                                    Console.WriteLine("Original : " + trackingDict[unicodeChar]);
                                    Console.WriteLine("Duplicate: " + display);
                                }
                                else
                                {
                                    trackingDict.Add(unicodeChar, display);
                                }
                            }
                        }
                    }
                    foreach (var category_convDict in CharLib.AllUnicodeToASCIIComplexConversions)
                    {
                        foreach (var replacement_unicodeCharArray in category_convDict.Value)
                        {
                            for (int i = 0; i < replacement_unicodeCharArray.Value.Length; i++)
                            {
                                char unicodeChar = replacement_unicodeCharArray.Value[i];
                                string display = category_convDict.Key + " - " + replacement_unicodeCharArray.Key + " - unicode: " + TextUtil.Reveal(string.Concat(unicodeChar), RevealOptions.All) + " - pos: " + i;
                                if (trackingDict.ContainsKey(unicodeChar))
                                {
                                    Console.WriteLine("Original : " + trackingDict[unicodeChar]);
                                    Console.WriteLine("Duplicate: " + display);
                                }
                                else
                                {
                                    trackingDict.Add(unicodeChar, display);
                                }
                            }
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "test (char) cast of 129, 141, 143, 144, 157, 173",
                () =>
                {
                    var nums = new[] { 129, 141, 143, 144, 157, 173 };
                    var nums2 = new[] { 71, 72, 73 };
                    foreach (var num in nums)
                    {
                        Console.Write("Casting " + num + "...");
                        try
                        {
                            char c = (char)num;
                            Console.WriteLine(" succeeded");
                        }
                        catch
                        {
                            Console.WriteLine(" failed");
                        }
                    }
                    Console.Write("Linq casting all...");
                    try
                    {
                        var chars = nums.Cast<char>().ToArray();
                        Console.WriteLine(" succeeded - " + chars.Length);
                    }
                    catch
                    {
                        Console.WriteLine(" failed");
                    }
                    Console.Write("Linq casting 71, 72, 73...");
                    try
                    {
                        var chars = nums2.Cast<char>().ToArray();
                        Console.WriteLine(" succeeded - " + chars.Length);
                    }
                    catch
                    {
                        Console.WriteLine(" failed");
                    }
                    Console.Write("Linq casting all w/ .Select()...");
                    try
                    {
                        var chars = nums.Select(n => (char)n).ToArray();
                        Console.WriteLine(" succeeded - " + chars.Length);
                    }
                    catch
                    {
                        Console.WriteLine(" failed");
                    }
                    Console.Write("Linq casting 71, 72, 73 w/ .Select()...");
                    try
                    {
                        var chars = nums2.Select(n => (char)n).ToArray();
                        Console.WriteLine(" succeeded - " + chars.Length);
                    }
                    catch
                    {
                        Console.WriteLine(" failed");
                    }
                }
            )
        };
    }
}
