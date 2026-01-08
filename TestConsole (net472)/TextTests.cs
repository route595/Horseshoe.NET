using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

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
                    var categories = new[] { CharCategory.NotDefined, CharCategory.AllPrintables , CharCategory.Nonprintable, CharCategory.All };
                    Console.Write("phrase: ");
                    Console.WriteLine(phrase1);
                    foreach(var category in categories)
                    {
                        Console.WriteLine("  reveal: " + TextUtil.Reveal(phrase1, new RevealOptions{ CharCategory = category }) + "  (" + category + ")");
                    }
                    Console.WriteLine();
                    var phrase2 = "Rudolph the red-nosed reindeer" + Environment.NewLine + "had a very shiny nose!";
                    Console.Write("phrase: ");
                    Console.WriteLine(phrase2);
                    Console.WriteLine("revealed:");
                    Console.WriteLine(TextUtil.Reveal(phrase2, new RevealOptions{ CharCategory = CharCategory.AllWhitespaces }));
                    Console.WriteLine("revealed (newlines preserved):");
                    Console.WriteLine(TextUtil.Reveal(phrase2, new RevealOptions{ CharCategory = CharCategory.AllWhitespaces, PreserveNewLines = true}));
                }
            ),
            BuildMenuRoutine
            (
                "Quick ToASCIIPrintable() tests",
                () =>
                {
                    var phrase = "Å¢t Øñę\u0000";
                    Console.WriteLine("phrase: " + phrase + " (" + TextUtil.Reveal(phrase, options: RevealOptions.All) + ")");
                    phrase = TextClean.ToAsciiPrintable(phrase); // "Act One" char matches found and [NUL] lopped off
                    Console.WriteLine("phrase: " + phrase + " (" + TextUtil.Reveal(phrase, options: RevealOptions.All) + ")");
                }
            ),
            BuildMenuRoutine
            (
                "Scan for duplicates in CharLib",
                () =>
                {
                    var trackingDict = new Dictionary<int, string>();
                    foreach (var unicodeCharArray_Replacement_KVP in CharLib.UnicodeToASCIIConversions)
                    {
                        for (int i = 0; i < unicodeCharArray_Replacement_KVP.Value.Length; i++)
                        {
                            char unicodeChar = unicodeCharArray_Replacement_KVP.Value[i];
                            string display = unicodeCharArray_Replacement_KVP.Key + " - " + CharInfo.Get(unicodeChar) + " - pos: " + i;
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
            ),
            BuildMenuRoutine
            (
                "Contains all?",
                () =>
                {
                    var testChars = new[] { 'a', 'c' };
                    var testStrings = new[] { "abc", "abcabc", "cba", "cbacba", "", null };
                    var maxLen = testStrings.Max(s => ValueUtil.Display(s).Length);
                    Console.WriteLine("Testing " + ValueUtil.Display(testChars) + " on " + ValueUtil.Display(testStrings));
                    foreach (var testString in testStrings)
                    {
                        Console.WriteLine("ContainsAll                : " + ValueUtil.Display(testString).PadRight(maxLen) + " -> " + testString.ContainsAll(testChars));
                        Console.WriteLine("ContainsAllInSequence      : " + ValueUtil.Display(testString).PadRight(maxLen) + " -> " + testString.ContainsAllInSequence(testChars));
                        Console.WriteLine("ContainsAllInStrictSequence: " + ValueUtil.Display(testString).PadRight(maxLen) + " -> " + testString.ContainsAllInStrictSequence(testChars));
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
            ),
            BuildMenuRoutine
            (
                "TextAttributes",
                () =>
                {
                    Console.WriteLine();
                    Console.WriteLine("===== Entire string: =====");
                    var inputs = new[]
                    {
                        "[name=startDate,type=dtm,value=20250205]", // '=' delimiter
                        "[name:startDate,type:dtm,value:20250205]", // ':' delimiter
                        "[name:startDate,type=dtm,value:20250205]", // mixed delimiters: error
                        "[name=startDate,value=20250205][name=endDate,value=20250205]"
                    };
                    foreach (var input in inputs)
                    {
                        Console.WriteLine("Input: " + input);
                        try
                        {
                            var list = TextAttribute.Parse(input);
                            foreach (var attr in list)
                            {
                                Console.WriteLine("  Attribute: " + attr);
                            }
                        }
                        catch(Exception ex) 
                        {
                            Console.WriteLine("  " + ex.RenderMessage());
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("=====  Starts with:  =====");
                    inputs = new[]
                    {
                        "[name=startDate,type=dtm,value=20250205]post-text",
                        "[name=startDate,value=20250205][name=endDate,value=20250205]yadda-yadda"
                    };
                    foreach (var input in inputs)
                    {
                        Console.WriteLine("Input: " + input);
                        TextAttribute.TryParseStartsWith(input, out TextAttribute.List list, out string remainingInput);
                        Console.WriteLine("Remaining Input: " + remainingInput);
                        if (list != null)
                        {
                            foreach (var attr in list)
                            {
                                Console.WriteLine("  Attribute: " + attr);
                            }
                        }
                    }
                }
            )
        };
    }
}
