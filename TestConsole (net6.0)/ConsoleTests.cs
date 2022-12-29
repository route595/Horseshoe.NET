using System;
using System.Collections.Generic;

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
                "Prompt Input*",
                () =>
                {
                    Console.WriteLine("testing 'required'...");
                    var input = PromptX.Input("input", required: true);
                    Console.WriteLine(input);
                    Console.WriteLine("testing not required with 'requiredIndicator' = \"$\"...");
                    RenderX.RequiredIndicator = "$";
                    input = PromptX.Input("input", displayAsRequired: true);
                    RenderX.RequiredIndicator = "*";
                    Console.WriteLine(input);
                    Console.WriteLine("testing null 'prompt'...");
                    input = PromptX.Input(null);
                    Console.WriteLine(input);
                    Console.WriteLine("testing null 'prompt' with 'required'...");
                    input = PromptX.Input(null, required: true);
                    Console.WriteLine(input);
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
                    var choice = PromptX.List(list, title: "hi", indexPolicy: ListIndexPolicy.DisplayZeroBased);
                    Console.WriteLine("choice: " + (choice ?? "[null]"));
                }
            ),
            BuildMenuRoutine
            (
                "Prompt List (1-based)*",
                () =>
                {
                    var list = new[] { "ValueA", "ValueB", "ValueC", "ValueD" };
                    var choice = PromptX.List(list, title: "hi", indexPolicy: ListIndexPolicy.DisplayOneBased, required: true);
                    Console.WriteLine("choice: " + (choice ?? "[null]"));
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
                            Console.WriteLine("int=" + PromptX.Value<int>(validator: (i) => Assert.InRange(i, 1, 3)));
                        }
                        catch(ConsoleNavigation.PromptCanceledException)
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
