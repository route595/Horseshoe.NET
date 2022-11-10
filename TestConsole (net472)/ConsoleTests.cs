﻿using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.ConsoleX.Plugins;

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
                    input = PromptX.Input("input", requiredIndicator: "$");
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
                "Prompt Enum",
                () =>
                {
                    var enum1 = PromptX.Enum<ConsoleTestEnum>(except: new[] { ConsoleTestEnum.Except });
                    Console.WriteLine("choice: " + enum1);
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
                "Prompt Enum (no except)",
                () =>
                {
                    var enum1 = PromptX.Enum<ConsoleTestEnum>();
                    Console.WriteLine("choice: " + enum1);
                }
            ),
            BuildMenuRoutine
            (
                "Prompt NEnum",
                () =>
                {
                    var enum1 = PromptX.NEnum<ConsoleTestEnum>(except: new[] { ConsoleTestEnum.Except });
                    Console.WriteLine("choice: " + (enum1?.ToString() ?? "[null]"));
                }
            ),
            BuildMenuRoutine
            (
                "Render list titles",
                () =>
                {
                    RenderX.ListTitle(new Title("A Title w/o Required Indicator"));
                    RenderX.ListTitle(new Title("A Title", xtra: " - w/o Required Indicator"));
                    RenderX.ListTitle(new Title("A Title w/ Required Indicator"), requiredIndicator: "*");
                    RenderX.ListTitle(new Title("A Title", xtra: " - w/ Required Indicator"), requiredIndicator: "*");
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
                "Prompt NInt (range = 1 - 3)",
                () =>
                {
                    int? nint = -1;
                    Console.WriteLine("Enter ints, range is 1 - 3.  Press enter to exit...");
                    while(nint.HasValue)
                    {
                        nint = PromptX.NNumeric<int>(min: 1, max: 3);
                    }
                }
            ),
            new FileSystemNavigator
            (
                text: "File Search", 
                options: new FileSystemNavigatorOptions 
                { 
                    StartDirectory = @"C:\MyThirdPartyProjects" 
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
                    StartDirectory = @"C:\MyThirdPartyProjects", 
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