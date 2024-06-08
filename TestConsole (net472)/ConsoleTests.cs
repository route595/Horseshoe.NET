using System;
using System.Collections.Generic;
using System.Linq;
using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.ConsoleX.Plugins;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class ConsoleTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Prompt String",
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
                    var inputs = new []
                    {
                        "1,2,3",
                        "0,1,2,3",
                        "all",
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
                    Console.WriteLine("Zero-based tests, 22 count...");
                    foreach (var input in inputs)
                    {
                        Console.Write("  " + TextUtil.Reveal(input) + " -> ");
                        try
                        {
                            var indices = MenuSelection.ParseMultipleSelectionIndices(input, 22, false);
                            Console.WriteLine(indices.Any() ? string.Join(",", indices) : "<zero-results>");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    Console.WriteLine("One-based tests, 22 count...");
                    foreach (var input in inputs)
                    {
                        Console.Write("  " + TextUtil.Reveal(input) + " -> ");
                        try
                        {
                            var indices = MenuSelection.ParseMultipleSelectionIndices(input, 22, true);
                            Console.WriteLine(indices.Any() ? string.Join(",", indices) : "<zero-results>");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
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
                    var enum1 = PromptX.Enum<ConsoleTestEnum?>(except: new[] { ConsoleTestEnum.Except as ConsoleTestEnum? });
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
                    var selection = PromptX.List(list, title: "hi", indexPolicy: ListIndexPolicy.DisplayZeroBased);
                    Console.WriteLine("choice: " + (selection.SelectedItem ?? "[null]"));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt List (1-based)*",
                () =>
                {
                    var list = new[] { "ValueA", "ValueB", "ValueC", "ValueD" };
                    var selection = PromptX.List(list, title: "hi", indexPolicy: ListIndexPolicy.DisplayOneBased, required: true);
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
                            Console.WriteLine("int=" + PromptX.Int(null, validator: (i) => Assert.InRange(i, 1, 3)));
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
