using System;
using System.Collections.Generic;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.ConsoleX.Plugins;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class ConsoleTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Choose day of week",
                () =>
                {
                    Console.WriteLine("Please indicate available day of week.  To select none simply press 'Enter'.");
                    var selection = PromptX.List(new []{ "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
                    switch(selection.Selection.Count)
                    {
                        case 0:
                            Console.WriteLine("You didn't select a day. Your schedule must be full to the max.");
                            break;
                        case 1:
                            Console.WriteLine("Let's put something on the books for \"" + selection.SelectedItem + "\", shall we?");
                            break;
                        default:
                            throw new ThisShouldNeverHappenException();
                    }
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "Choose day of week (required)",
                () =>
                {
                    Console.WriteLine("Please indicate available day of week.  Simply pressing 'Enter' will not work due to required == true.");
                    var selection = PromptX.List(new []{ "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }, selectionMode: ListSelectionMode.ExactlyOne);
                    switch(selection.Selection.Count)
                    {
                        case 0:
                            throw new ThisShouldNeverHappenException();
                        case 1:
                            Console.WriteLine("Let's put something on the books for \"" + selection.SelectedItem + "\", shall we?");
                            break;
                        default:
                            throw new ThisShouldNeverHappenException();
                    }
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "Choose day(s) of week",
                () =>
                {
                    Console.WriteLine("Please indicate available day(s) of week.  To select none simply press 'Enter'.");
                    var selection = PromptX.List(new []{ "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }, selectionMode: ListSelectionMode.ZeroOrMore);
                    switch(selection.Selection.Count)
                    {
                        case 0:
                            Console.WriteLine("You didn't select any days. Your schedule must be full to the max.");
                            break;
                        case 1:
                            Console.WriteLine("Let's put something on the books for \"" + selection.SelectedItem + "\", shall we?");
                            break;
                        default:
                            Console.WriteLine("Thanks for being so generous with your time on \"" + string.Join("\", \"", selection.SelectedItems) + "\"!");
                            break;
                    }
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "Choose day(s) of week (required)",
                () =>
                {
                    Console.WriteLine("Please indicate available day(s) of week.  Simply pressing 'Enter' will not work due to required == true.");
                    var selection = PromptX.List(new []{ "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }, selectionMode: ListSelectionMode.OneOrMore);
                    switch(selection.Selection.Count)
                    {
                        case 0:
                            Console.WriteLine("You didn't select any days. Your schedule must be full to the max.");
                            break;
                        case 1:
                            Console.WriteLine("Let's put something on the books for \"" + selection.SelectedItem + "\", shall we?");
                            break;
                        default:
                            Console.WriteLine("Thanks for being so generous with your time on \"" + string.Join("\", \"", selection.SelectedItems) + "\"!");
                            break;
                    }
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "Prompt different data types",
                () =>
                {
                    Console.WriteLine("testing 'required'...");
                    var input = PromptX.String("input1", required: true);
                    Console.WriteLine(input);
                    Console.WriteLine("testing 'displayAsRequired' with 'requiredIndicator' = \"$\"...");
                    RenderX.RequiredIndicator = "$";
                    input = PromptX.String("input2", required: true);
                    RenderX.RequiredIndicator = null;
                    Console.WriteLine(input);
                    Console.WriteLine("testing null 'prompt'...");
                    input = PromptX.String(null);
                    Console.WriteLine(input);
                    Console.WriteLine("testing null 'prompt' with 'required'...");
                    input = PromptX.String(null, required: true);
                    Console.WriteLine(input);
                }
            ),
            BuildMenuRoutine
            (
                "Prompt Password",
                () =>
                {
                    RenderX.Prompt("user name");
                    Console.WriteLine("bob");
                    var password = PromptX.Password();
                    Console.WriteLine("password length: " + TextUtil.Reveal(password?.Length));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt Raw Input",
                () =>
                {
                    Console.WriteLine("testing 'required'...");
                    var input = PromptX.RawConsoleInput("input", required: true);
                    Console.WriteLine(input);
                    Console.WriteLine("testing 'displayAsRequired' with 'requiredIndicator' = \"$\"...");
                    RenderX.RequiredIndicator = "$";
                    input = PromptX.RawConsoleInput("input", required: true);
                    RenderX.RequiredIndicator = null;
                    Console.WriteLine(input);
                    Console.WriteLine("testing null 'prompt'...");
                    input = PromptX.RawConsoleInput(null);
                    Console.WriteLine(input);
                    Console.WriteLine("testing null 'prompt' with 'required'...");
                    input = PromptX.RawConsoleInput(null, required: true);
                    Console.WriteLine(input);
                }
            ),
            BuildMenuRoutine
            (
                "Test multi-selection",
                () =>
                {
                    var parsedIndices = new List<int>();
                    var inputs = new []
                    {
                        "1,2,3",
                        "1-3",
                        "0,1,2,3",
                        "0-3",
                        "all",
                        "all 1-3",
                        "all except 1-2,13",
                        "all except",
                        "rafael",
                        "1-13,15-17",
                        "16-18,20-22",
                        "18-16,20-22",
                        "none",
                        "",
                        null,
                        "14-17-20",
                        "1-3a,8-19"
                    };
                    Console.WriteLine("**************************");
                    Console.WriteLine("Stage 1 - parse user input");
                    Console.WriteLine("**************************");
                    Console.WriteLine();
                    foreach (var input in inputs)
                    {
                        Console.Write(ValueUtil.Display(input) + " -> ");
                        parsedIndices.Clear();
                        try
                        {
                            UserInputUtil.ParseMultipleIndices(input, parsedIndices, out bool all, out bool except);
                            Console.WriteLine(ObjectUtil.DumpToString(new { all, except, indices = "[" + string.Join(", ", parsedIndices) + "]" }));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message.Replace("Validation failed: ", "").Replace(Environment.NewLine, " "));
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("*****************************************************");
                    Console.WriteLine("Stage 2a - parse w/ count = 20, indexPolicy = 1-based");
                    Console.WriteLine("*****************************************************");
                    Console.WriteLine();
                    foreach (var input in inputs)
                    {
                        Console.Write(ValueUtil.Display(input) + " -> ");
                        parsedIndices.Clear();
                        try
                        {
                            var indices = UserInputUtil.ParseMultipleIndices(input, 20);
                            Console.WriteLine(string.Join(", ", indices));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message.Replace("Validation failed: ", "").Replace(Environment.NewLine, " "));
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("***************************************");
                    Console.WriteLine("Stage 2b - parse w/ set 1 - 15, 16 - 20");
                    Console.WriteLine("***************************************");
                    Console.WriteLine();
                    Console.WriteLine("One-based tests, 22 count...");
                    var possibleIndices = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20 };
                    foreach (var input in inputs)
                    {
                        Console.Write(ValueUtil.Display(input) + " -> ");
                        parsedIndices.Clear();
                        try
                        {
                            var indices = UserInputUtil.ParseMultipleIndices(input, possibleIndices);
                            Console.WriteLine(string.Join(", ", indices));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message.Replace("Validation failed: ", "").Replace(Environment.NewLine, " "));
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Prompt Enum*",
                () =>
                {
                    var enum1 = PromptX.Enum<ConsoleTestEnum>(required: true, except: new[] { ConsoleTestEnum.Except });
                    Console.WriteLine("choice: " + enum1);
                }
            ),
            BuildMenuRoutine
            (
                "Prompt nullable Enum",
                () =>
                {
                    var enum1 = PromptX.NEnum<ConsoleTestEnum>(except: new[] { ConsoleTestEnum.Except });
                    Console.WriteLine("choice: " + TextUtil.Reveal(enum1));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt Enum (no except)",
                () =>
                {
                    var enum1 = PromptX.Enum<ConsoleTestEnum>();
                    Console.WriteLine("choice: " + enum1);
                }
            ),
            BuildMenuRoutine
            (
                "Render list titles",
                () =>
                {
                    RenderX.ListTitle(new Title("A Title"));
                    RenderX.ListTitle(new Title("A Title", xtra: " - w/ Extra Text!!"));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt List (0-based)",
                () =>
                {
                    var list = new[] { "ValueA", "ValueB", "ValueC", "ValueD" };
                    var selection = PromptX.List(list, title: "hi", indexStyle: ListIndexStyle.ZeroBased);
                    Console.WriteLine("choice: " + (selection.SelectedItem ?? "[null]"));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt List (1-based)*",
                () =>
                {
                    var list = new[] { "ValueA", "ValueB", "ValueC", "ValueD" };
                    var selection = PromptX.List(list, title: "hi", indexStyle: ListIndexStyle.OneBased, selectionMode: ListSelectionMode.ExactlyOne);
                    Console.WriteLine("choice: " + (selection.SelectedItem ?? "[null]"));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt Int (range = 1 - 3)",
                () =>
                {
                    var looping = true;
                    Console.WriteLine("Enter ints, range is 1 - 3.  You can also enter 'cancel' to return to the menu or 'exit' to quit.");
                    while(looping)
                    {
                        try
                        {
                            Console.WriteLine("int=" + PromptX.Int(null, validator: (i) => AssertValue.InRange(i, 1, 3)));
                        }
                        catch(ConsoleNavigation.CancelInputPromptException)
                        {
                            Console.WriteLine("[prompt canceled]");
                            looping = false;
                        }
                    }
                }
            ),
            new FileSystemNavigator
            (
                text: "File Search", 
                options: new FileSystemNavigatorOptions 
                { 
                    StartDirectory = @"C:\Users" 
                }
            )
            { 
                OnPathSelected = (path) => Console.WriteLine("Selected path: " + path) 
            },
            new FileSystemNavigator
            (
                text: "Folder Search", 
                options: new FileSystemNavigatorOptions 
                { 
                    StartDirectory = @"C:\Users", 
                    DirectoryModeOn = true 
                }
            )
            { 
                OnPathSelected = (path) => Console.WriteLine("Selected path: " + path) 
            }
        };
    }

    enum ConsoleTestEnum
    {
        Except = 0,
        Value1 = 1,
        Value2 = 2,
        Value4 = 4
    }
}
