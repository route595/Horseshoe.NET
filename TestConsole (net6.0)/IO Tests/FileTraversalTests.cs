using System;
using System.Collections.Generic;
using System.IO;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.FileTraversal;

namespace TestConsole.IOTests
{
    class FileTraversalTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Rebuild folder/file structure",
                () =>
                {
                    ResetFiles();
                }
            ),
            BuildMenuRoutine
            (
                "List txt/pdf files - w/ reset files",
                () =>
                {
                    ResetFiles();
                    var filePatterns = new[] { "*.txt", "*.pdf" };
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        OnDirectoryHello = (dp, cmd) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp) => dirLevel--,
                        OnFileHello = (fp, cmd) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + fp.GetDisplaySize() + ")")
                    };
                    foreach (var filePattern in filePatterns)
                    {
                        Console.WriteLine("Scanning \"" + (engine.Optimizations.FileSearchPattern = filePattern) + "\"...");
                        engine.Start();
                        Console.Write(engine.Statistics.Dump());
                    }
                }
            ),
            BuildMenuRoutine
            (
                "List txt/pdf files - w/ reset files - w/ clear statistics",
                () =>
                {
                    ResetFiles();
                    var filePatterns = new[] { "*.txt", "*.pdf" };
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        OnDirectoryHello = (dp, cmd) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp) => dirLevel--,
                        OnFileHello = (fp, cmd) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + fp.GetDisplaySize() + ")")
                    };
                    foreach (var filePattern in filePatterns)
                    {
                        Console.WriteLine("Scanning \"" + (engine.Optimizations.FileSearchPattern = filePattern) + "\"...");
                        engine.Statistics.Clear();
                        engine.Start();
                        Console.Write(engine.Statistics.Dump());
                    }
                }
            ),
            BuildMenuRoutine
            (
                "List 'mammalia' contents",
                () =>
                {
                    ResetFiles();
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        OnDirectoryHello = (dp, _) => Console.WriteLine(new string(' ', N(++dirLevel) * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp) => dirLevel--,
                        OnDirectorySkipping = (dp) => Console.WriteLine(new string(' ', N(++dirLevel) * 2) + dp.Name + "\\ (skipping)"),
                        OnDirectorySkipped = (dp) => dirLevel--,
                        OnFileHello = (fp, _) => Console.WriteLine(new string(' ', N(dirLevel) * 2 + 2) + fp.Name + " (" + fp.GetDisplaySize() + ")")
                    };
                    engine.Optimizations.DirectoryFilter = (dp) => dp.Name.In("mammalia");
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                    int N(int n){if (n < 0) return 0; return n;}
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' via advanced action",
                () =>
                {
                    ResetFiles();
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        AdvancedAction = AdvancedAction.Delete(),
                        OnDirectoryHello = (dp, _) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp) => dirLevel--,
                        OnDirectorySkipping = (dp) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\ (skipping)"),
                        OnDirectorySkipped = (dp) => dirLevel--,
                        OnDirectoryDeleting = (dp) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\ (deleting)"),
                        OnDirectoryDeleted = (dp) => dirLevel--,
                        OnFileHello = (fp, _) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDeleted = (fp, sz) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + FileUtil.GetDisplayFileSize(sz) + ") (deleted)")
                    };
                    engine.Optimizations.DirectoryFilter = (dp) => dp.Name.In("mammalia");
                    engine.DryRun = PromptX.Bool("Dry run?", defaultValue: true);
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' contents via advanced action",
                () =>
                {
                    ResetFiles();
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        AdvancedAction = AdvancedAction.DeleteContents(/*Path.Combine(Directory.GetCurrentDirectory(), "animalia", "mammalia"),*/ autoLoadActionRoot: true),
                        OnDirectoryHello = (dp, _) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp) => dirLevel--,
                        OnDirectorySkipping = (dp) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\ (skipping)"),
                        OnDirectorySkipped = (dp) => dirLevel--,
                        OnDirectoryDeleting = (dp) => Console.WriteLine(new string(' ', ++dirLevel * 2) + dp.Name + "\\ (deleting)"),
                        OnDirectoryDeleted = (dp) => dirLevel--,
                        OnFileHello = (fp, _) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDeleted = (fp, sz) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + FileUtil.GetDisplayFileSize(sz) + ") (deleted)")
                    };
                    engine.Optimizations.DirectoryFilter = (dp) => dp.Name.In("mammalia");
                    engine.DryRun = PromptX.Bool("Dry run?", defaultValue: true);
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            )
        };

        public static void ResetFiles()
        {
            Console.WriteLine("Resetting files...");
            Directory.CreateDirectory("animalia\\amphibia");
            File.WriteAllText("animalia\\amphibia\\frog.txt", "bla bla bla shaka shaka baby babu backa racka");
            File.WriteAllText("animalia\\amphibia\\salamander.txt", "bla bla bla shaka shaka baby babu backa racka ticki tacki");
            File.WriteAllText("animalia\\amphibia\\toad.pdf", "bla bla bla shaka shaka baby babu backa racka shady lady");
            Directory.CreateDirectory("animalia\\mammalia");
            File.WriteAllText("animalia\\mammalia\\lion.txt", "bla bla bla");
            File.WriteAllText("animalia\\mammalia\\tiger.txt", "bla bla bla blue blee");
            File.WriteAllText("animalia\\mammalia\\chimpanzee.pdf", "bla bla bla blah blo brubby");
            Directory.CreateDirectory("animalia\\reptilia");
            File.WriteAllText("animalia\\reptilia\\lizard.pdf", "bla bla bla blip blubber blast");
            File.WriteAllText("animalia\\reptilia\\iguana.txt", "bla bla bla bellows below before brunch");
            File.WriteAllText("animalia\\reptilia\\snake.txt", "bla bla bla barry bow bough stern stretchy");
        }
    }
}
