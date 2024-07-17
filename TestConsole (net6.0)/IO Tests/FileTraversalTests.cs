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
                        OnDirectoryHello = (dp, eng, cmd) => Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp, eng, cmd) => dirLevel--,
                        OnFileHello = (fp, eng, cmd) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")")
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
                        OnDirectoryHello = (dp, eng, cmd) => Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp, eng, cmd) => dirLevel--,
                        OnFileHello = (fp, eng, cmd) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")")
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
                        OnDirectoryHello = (dp, eng, _) => Console.WriteLine(new string(' ', N(dirLevel++) * 2) + dp.Name + "\\"),
                        OnDirectoryGoodbye = (dp, eng, cmd) => dirLevel--,
                        OnDirectorySkipped = (dp, eng) => { Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"); },
                        OnFileHello = (fp, eng, _) => Console.WriteLine(new string(' ', N(dirLevel) * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")")
                    };
                    engine.Optimizations.DirectoryFilter = (dp) => dp.Name.In("mammalia");
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                    int N(int n){if (n < 0) return 0; return n;}
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' via filter + intercom callback",
                () =>
                {
                    ResetFiles();
                    var optimizations = new TraversalOptimizations
                    {
                        DirectoryFilter = (dp) => dp.Name.In("mammalia")
                    };
                    var dryRun = PromptX.Bool("Dry run?");
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia", optimizations: optimizations)
                    {
                        OnDirectoryHello = (dp, eng, cmd) =>
                        {
                            Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\");
                            cmd.RequestDelete();
                        },
                        OnDirectoryGoodbye = (dp, eng, cmd) =>
                            dirLevel--,
                        OnDirectorySkipped = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"),
                        OnDirectoryDeleting = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(deleting)"),
                        OnDirectoryDeleted = (dp, eng) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + dp.Name + "\\)"),
                        OnFileHello = (fp, eng, _) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDelete = (fp, eng, sz) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + FileUtil.GetDisplayFileSize(sz) + ")"),
                        OnWarning = (msg) =>
                            RenderX.Alert(msg),
                        DryRun = dryRun
                    };
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' contents via filter + intercom callback",
                () =>
                {
                    ResetFiles();
                    var optimizations = new TraversalOptimizations
                    {
                        DirectoryFilter = (dp) => dp.Name.In("mammalia")
                    };
                    var dryRun = PromptX.Bool("Dry run?");
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia", optimizations: optimizations)
                    {
                        OnDirectoryHello = (dp, eng, cmd) =>
                        {
                            Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\");
                            cmd.RequestDelete(deleteContents: true);
                        },
                        OnDirectoryGoodbye = (dp, eng, cmd) =>
                            dirLevel--,
                        OnDirectorySkipped = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"),
                        OnDirectoryDeleting = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(deleting)"),
                        OnDirectoryDeleted = (dp, eng) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + dp.Name + "\\)"),
                        OnFileHello = (fp, eng, _) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDelete = (fp, eng, sz) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + FileUtil.GetDisplayFileSize(sz) + ")"),
                        OnWarning = (msg) =>
                            RenderX.Alert(msg),
                        DryRun = dryRun
                    };
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' via filter + intercom callback - directories only mode",
                () =>
                {
                    ResetFiles();
                    var optimizations = new TraversalOptimizations
                    {
                        DirectoryFilter = (dp) => dp.Name.In("mammalia"),
                        DirectoriesOnlyMode = true
                    };
                    var dryRun = PromptX.Bool("Dry run?");
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia", optimizations: optimizations)
                    {
                        OnDirectoryHello = (dp, eng, cmd) =>
                        {
                            Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\");
                            cmd.RequestDelete();
                        },
                        OnDirectoryGoodbye = (dp, eng, cmd) =>
                            dirLevel--,
                        OnDirectorySkipped = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"),
                        OnDirectoryDeleting = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(deleting)"),
                        OnDirectoryDeleted = (dp, eng) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + dp.Name + "\\)"),
                        OnFileHello = (fp, eng, _) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDelete = (fp, eng, sz) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + FileUtil.GetDisplayFileSize(sz) + ")"),
                        OnWarning = (msg) =>
                            RenderX.Alert(msg),
                        DryRun = dryRun
                    };
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' via intercom callback",
                () =>
                {
                    ResetFiles();
                    var dryRun = PromptX.Bool("Dry run?");
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        OnDirectoryHello = (dp, eng, cmd) =>
                        {
                            Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\");
                            if (dp.Name.In("mammalia"))
                                cmd.RequestDelete();
                        },
                        OnDirectoryGoodbye = (dp, eng, cmd) =>
                            dirLevel--,
                        OnDirectorySkipped = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"),
                        OnDirectoryDeleting = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(deleting)"),
                        OnDirectoryDeleted = (dp, eng) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + dp.Name + "\\)"),
                        OnFileHello = (fp, eng, _) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDelete = (fp, eng, sz) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + FileUtil.GetDisplayFileSize(sz) + ")"),
                        OnWarning = (msg) =>
                            RenderX.Alert(msg),
                        DryRun = dryRun
                    };
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' contents via intercom callback",
                () =>
                {
                    ResetFiles();
                    var dryRun = PromptX.Bool("Dry run?");
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia")
                    {
                        OnDirectoryHello = (dp, eng, cmd) =>
                        {
                            Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\");
                            if (dp.Name.In("mammalia"))
                                cmd.RequestDelete(deleteContents: true);
                        },
                        OnDirectoryGoodbye = (dp, eng, cmd) =>
                            dirLevel--,
                        OnDirectorySkipped = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"),
                        OnDirectoryDeleting = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(deleting)"),
                        OnDirectoryDeleted = (dp, eng) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + dp.Name + "\\)"),
                        OnFileHello = (fp, eng, _) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        OnFileDelete = (fp, eng, sz) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + FileUtil.GetDisplayFileSize(sz) + ")"),
                        OnWarning = (msg) =>
                            RenderX.Alert(msg),
                        DryRun = dryRun
                    };
                    engine.Start();
                    Console.Write(engine.Statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Delete 'mammalia' via intercom callback - directories only mode - inline delete msg",
                () =>
                {
                    ResetFiles();
                    var optimizations = new TraversalOptimizations
                    {
                        DirectoriesOnlyMode = true
                    };
                    var dryRun = PromptX.Bool("Dry run?");
                    int dirLevel = 0;
                    var engine = new TraversalEngine("animalia", optimizations: optimizations)
                    {
                        OnDirectoryHello = (dp, eng, cmd) =>
                        {
                            Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\");
                            if (dp.Name.In("mammalia"))
                                cmd.RequestDelete();
                        },
                        OnDirectoryGoodbye = (dp, eng, cmd) =>
                            dirLevel--,
                        OnDirectorySkipped = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"),
                        OnDirectoryDeleting = (dp, eng) =>
                            Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(deleting)"),
                        OnDirectoryDeleted = (dp, eng) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + dp.Name + "\\)"),
                        OnFileHello = (fp, eng, _) =>
                            Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        //OnFileDelete = (fp, eng, sz) =>
                        //    Console.WriteLine(new string(' ', dirLevel * 2) + "(deleted " + FileUtil.GetDisplayFileSize(sz) + ")"),
                        OnWarning = (msg) =>
                            RenderX.Alert(msg),
                        DryRun = dryRun
                    };
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
