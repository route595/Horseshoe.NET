using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Configuration;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole;

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
                var lastItem = textGrid.Columns[i - 1][lastIndex]?.ToString() ?? "";
                if (lastItem.StartsWith("   "))
                {
                    textGrid.Columns[i - 1].RemoveAt(lastIndex);
                    textGrid.Columns[i].Insert(0, lastItem);
                }
            }
        }
    };

    private IList<MenuObject> GetGroupedRoutines()
    {
        var menuObjects = new List<MenuObject>();
        var routineGroups = FindRoutines()
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
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("tsconfig1.json")
            .Build();
        Config.Load(configuration);
        StartConsoleApp<Program>();
    }
}
