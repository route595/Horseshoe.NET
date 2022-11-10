using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole
{
    class Program : ConsoleXApp
    {
        static Program()
        {
            RenderX.ExceptionRendering.IncludeStackTrace = true;
            RenderX.ExceptionRendering.Recursive = true;
        }

        public override Action<RoutineX> ConfigureMainMenuRoutines => (routine) =>
        {
            routine.AutoAppendExitRoutineMenuItem = true;
        };

        public override Action<string> OnMainMenuSelecting => (menuObjectText) =>
        {
            Console.WriteLine("Selected => " + menuObjectText);
        };

        public override StringValues WelcomeMessage => DefaultWelcomeValues;

        public override LoopMode LoopMode => LoopMode.Continuous;

        public override IList<MenuObject> MainMenu => GetGroupedRoutines();

        public override int MainMenuColumns => 3;

        public override Action<TextGrid> ConfigureTextGrid => (textGrid) =>
        {
            textGrid.TargetWidth = RenderX.ConsoleWidth - 2;
            for (int i = 1; i < 7; i++)
            {
                if (textGrid.Columns.Count > i)
                {
                    var lastIndex = textGrid.Columns[i - 1].Count - 1;
                    if ((textGrid.Columns[i - 1] as List<string>)[lastIndex].StartsWith("   "))
                    {
                        var label = (textGrid.Columns[i - 1] as List<string>)[lastIndex];
                        (textGrid.Columns[i - 1] as List<string>).RemoveAt(lastIndex);
                        (textGrid.Columns[i] as List<string>).Insert(0, label);
                    }
                }
            }
        };

        private IList<MenuObject> GetGroupedRoutines()
        {
            var menuObjects = new List<MenuObject>();
            var routineGroups = FindMainMenuRoutines()
                .GroupBy(r => r.GetType().Namespace);
            foreach (var grp in routineGroups)
            {
                menuObjects.Add(new MenuHeader("== " + (grp.Key.Contains(".") ? grp.Key.Substring(grp.Key.LastIndexOf(".") + 1) : "Tests") + " =="));
                menuObjects.AddRange(grp);
            }
            return menuObjects;
        }

        static void Main(string[] args)
        {
            StartConsoleApp<Program>();
        }
    }
}
