using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Horseshoe.NET;
using Horseshoe.NET.Compare;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.FileTraversal;

namespace TestConsole.IOTests
{
    //class FileTraversalTests : RoutineX
    //{
    //    public override IList<MenuObject> Menu => new MenuObject[]
    //    {
    //        BuildMenuRoutine
    //        (
    //            "Traverse project folder",
    //            () =>
    //            {
    //                int level = 0;
    //                var engine = new TraversalEngine(new DirectoryPath(Environment.CurrentDirectory).Parent.Parent)
    //                {
    //                    // events
    //                    Begin = (_engine) =>
    //                    {
    //                        Console.WriteLine("Root: " + _engine.Root);
    //                        Console.WriteLine();
    //                    },
    //                    DirectoryEntered = (dir, _engine, metadata) =>
    //                    {
    //                        if (dir == _engine.Root)
    //                            Console.WriteLine(new string(' ', level++ * 2) + dir.Name);
    //                        else
    //                            Console.WriteLine(new string(' ', level++ * 2) + "├─ " + dir.Name);
    //                    },
    //                    DirectoryExited = (dir, _) =>
    //                    {
    //                        Console.WriteLine(new string(' ', --level * 2) + "/");
    //                    },
    //                    FileFound = (file, _, metadata) =>
    //                    {
    //                        Console.WriteLine(new string(' ', level * 2) + "├─ " + file.Name + " (" + FileUtil.GetDisplayFileSize(file) + ")");
    //                    }
    //                };
    //                engine.Start();
    //                Console.WriteLine();
    //                Console.WriteLine("Traversal Statistics");
    //                Console.WriteLine("====================");
    //                Console.WriteLine(engine.RenderStatistics());
    //            }
    //        ),
    //        BuildMenuRoutine
    //        (
    //            "Traverse only bin, obj",
    //            () =>
    //            {
    //                var root = new DirectoryPath(Environment.CurrentDirectory).Parent.Parent;
    //                var level = 0;
    //                var engine = new TraversalEngine(root)
    //                {
    //                    // optimization
    //                    Optimization = new TraversalOptimization
    //                    {
    //                        DirectoryFilter = (dir) => dir.Name.In("bin", "obj")
    //                    },
    //                    // events
    //                    OnBegin = (_engine) =>
    //                    {
    //                        Console.WriteLine("Root: " + _engine.Root);
    //                        Console.WriteLine();
    //                    },
    //                    DirectoryEntered = (dir, _engine, metadata) =>
    //                    {
    //                        if (dir == _engine.Root)
    //                            Console.WriteLine(new string(' ', level++ * 2) + dir.Name);
    //                        else
    //                            Console.WriteLine(new string(' ', level++ * 2) + "├─ " + dir.Name);
    //                    },
    //                    DirectoryExited = (dir, _) =>
    //                    {
    //                        Console.WriteLine(new string(' ', --level * 2) + "/");
    //                    },
    //                    FileFound = (file, _, metadata) =>
    //                    {
    //                        Console.WriteLine(new string(' ', level * 2) + "├─ " + file.Name + " (" + FileUtil.GetDisplayFileSize(file) + ")");
    //                    }
    //                };
    //                engine.Start();
    //                Console.WriteLine();
    //                Console.WriteLine("Traversal Statistics");
    //                Console.WriteLine("====================");
    //                Console.WriteLine(engine.RenderStatistics());
    //            }
    //        ),
    //        BuildMenuRoutine
    //        (
    //            "Empty bin, obj",
    //            () =>
    //            {
    //                var root = new DirectoryPath(Environment.CurrentDirectory).Parent.Parent;
    //                new EmptyBinObj(root).Start();
    //            }
    //        ),
    //        BuildMenuRoutine
    //        (
    //            "Empty bin, obj - skipping bin",
    //            () =>
    //            {
    //                var root = new DirectoryPath(Environment.CurrentDirectory).Parent.Parent;
    //                new EmptyBinObj(root)
    //                {
    //                    DirectoryFlaggedForDeletion = (dir, engine) =>
    //                    {
    //                        if (dir.Name == "bin")
    //                            TraversalActions.SkipDirectory();
    //                    },
    //                }.Start();
    //            }
    //        ),
    //        BuildMenuRoutine
    //        (
    //            "Empty all bin, obj folders - except project DirectoryX and ~\\packages\\**\\bin",
    //            () =>
    //            {
    //                var listSelection = PromptX.List
    //                (
    //                    new[]
    //                    {
    //                        "Console",
    //                        "File: output.txt"
    //                    },
    //                    title: "Which output do you want to use?"
    //                );
    //                if (listSelection.SelectedIndex == 1)
    //                {
    //                    DeleteAllBinObj_ExceptProjectDirectoryX(new Output(Console.WriteLine), false);
    //                }
    //                else
    //                {
    //                    FilePath outputFile = "output.txt";
    //                    outputFile.Delete();
    //                    using (var fileWriter = new StreamWriter(outputFile.OpenWrite()))
    //                    {
    //                        DeleteAllBinObj_ExceptProjectDirectoryX(new Output(fileWriter.WriteLine), true);
    //                        fileWriter.Flush();
    //                    }
    //                    System.Diagnostics.Process.Start(outputFile.Name);
    //                }
    //            }
    //        ),
    //        BuildMenuRoutine
    //        (
    //            "Empty all bin, obj, packages except project DirectoryX",
    //            () =>
    //            {
    //                var listSelection = PromptX.List
    //                (
    //                    new[]
    //                    {
    //                        "Console",
    //                        "File: output.txt"
    //                    },
    //                    title: "Which output do you want to use?"
    //                );
    //                if (listSelection.SelectedIndex == 1)
    //                {
    //                    DeleteAllBinObjPackages_ExceptProjectDirectoryX(new Output(Console.WriteLine), false);
    //                }
    //                else
    //                {
    //                    FilePath outputFile = "output.txt";
    //                    outputFile.Delete();
    //                    using (var fileWriter = new StreamWriter(outputFile.OpenWrite()))
    //                    {
    //                        DeleteAllBinObjPackages_ExceptProjectDirectoryX(new Output(fileWriter.WriteLine), true);
    //                        fileWriter.Flush();
    //                    }
    //                    System.Diagnostics.Process.Start(outputFile.Name);
    //                }
    //            }
    //        ),
    //        BuildCustomRoutine("exit", ConsoleNavigation.ExitApp)
    //    };

    //    private static void DeleteAllBinObj_ExceptProjectDirectoryX(Output output, bool printStatistics)
    //    {
    //        var protectedRoot = new DirectoryPath(Environment.CurrentDirectory).Parent.Parent;
    //        var roots = new[]
    //        {
    //            new DirectoryPath(@"C:\Users\E029791\Dev\Git"),
    //            new DirectoryPath(@"C:\Users\E029791\Dev\Rider"),
    //            new DirectoryPath(@"C:\Users\E029791\Dev\Tests")
    //        };
    //        IDictionary<string, object> cumulativeStatistics = null;
    //        TraversalEngine engine = null;
    //        if (printStatistics)
    //            Console.WriteLine("Working...       (this may take a while)");
    //        foreach (var root in roots)
    //        {
    //            var level = 0;
    //            engine = new TraversalEngine
    //            (
    //                root,
    //                recursiveAction: RecursiveAction.Delete,
    //                optimization: new TraversalOptimization
    //                {
    //                    DirectoryFilter = (dir) => dir.Name.In("bin", "obj")
    //                },
    //                statistics: cumulativeStatistics
    //            )
    //            {
    //                // events
    //                OnBegin = (_engine) =>
    //                {
    //                    output.WriteLine("Root: " + _engine.Root);
    //                    output.WriteLine("Protected Root: " + protectedRoot);
    //                    output.WriteLine();
    //                },
    //                DirectoryEntered = (dir, _engine, metadata) =>
    //                {
    //                    if (dir == _engine.Root)
    //                        output.WriteLine(new string(' ', level++ * 2) + dir.Name);
    //                    else if (_engine.FilterMatch.HasValue && _engine.FilterMatch.Value == dir)
    //                        output.WriteLine(new string(' ', level++ * 2) + "├─" + dir.Name + " (" + dir.Parent + ")");
    //                    else
    //                        output.WriteLine(new string(' ', level++ * 2) + "├─" + dir.Name);
    //                },
    //                DirectorySkipped = (dir, _, metadata) =>
    //                {
    //                    if (!metadata.ClientSkipped)
    //                        return;
    //                    if (skippedDeletion)
    //                    {
    //                        output.WriteLine(new string(' ', level * 2) + "SKIPPED DELETION " + dir.Name + "...");
    //                    }
    //                    else
    //                    {
    //                        output.WriteLine(new string(' ', level-- * 2) + "SKIPPED...");
    //                    }
    //                },
    //                DirectoryFlaggedForDeletion = (dir, _) =>
    //                {
    //                    if (dir.IsChildOf(protectedRoot) || dir.ContainsParent("packages"))
    //                        TraversalActions.SkipDirectory();
    //                },
    //                DirectoryDeleting = (dir, _engine, metadata) =>
    //                {
    //                    if (_engine.FilterMatch.HasValue && dir == _engine.FilterMatch.Value)
    //                        TraversalActions.SkipDirectory();  // prevents bin, obj from being deleted
    //                    output.WriteLine(new string(' ', level * 2) + "DELETING " + dir.Name + "...");
    //                },
    //                // DirectoryDeleted = (dir, _) =>
    //                // {
    //                //     output.WriteLine(new string(' ', level * 2) + "DELETED");
    //                // },
    //                DirectoryExited = (dir, _) =>
    //                {
    //                    output.WriteLine(new string(' ', --level * 2) + "/");
    //                },
    //                FileFound = (file, _, metadata) =>
    //                {
    //                    output.WriteLine(new string(' ', level * 2) + "├─" + file.Name + " (" + FileUtil.GetDisplayFileSize(file) + ")");
    //                },
    //                FileDeleted = (file, size, _) =>
    //                {
    //                    output.WriteLine(new string(' ', level * 2 + 2) + "DELETED");
    //                }
    //            };
    //            engine.Start();
    //            cumulativeStatistics = engine.Statistics;
    //            output.WriteLine();
    //            output.WriteLine("Cumulative Traversal Statistics");
    //            output.WriteLine("===============================");
    //            output.WriteLine(engine.RenderStatistics());
    //            output.WriteLine();
    //        }

    //        if (printStatistics)
    //        {
    //            Console.WriteLine();
    //            Console.WriteLine("Cumulative Traversal Statistics");
    //            Console.WriteLine("===============================");
    //            Console.WriteLine(engine.RenderStatistics());
    //        }
    //    }

    //    private static void DeleteAllBinObjPackages_ExceptProjectDirectoryX(OutputMechanism output, bool printStatistics, bool dryRun = false)
    //    {
    //        var protectedRoot = new DirectoryPath(Environment.CurrentDirectory).Parent.Parent;
    //        var roots = new[]
    //        {
    //            new DirectoryPath(@"C:\Users\E029791\Dev\Git"),
    //            new DirectoryPath(@"C:\Users\E029791\Dev\Rider"),
    //            new DirectoryPath(@"C:\Users\E029791\Dev\Tests")
    //        };
    //        IDictionary<string, object> cumulativeStatistics = null;
    //        TraversalEngine engine = null;
    //        if (printStatistics)
    //            Console.WriteLine("Working...       (this may take a while)");
    //        foreach (var root in roots)
    //        {
    //            var level = 0;
    //            engine = new EmptyDirs
    //            (
    //                root,
    //                new[] { "bin", "obj", "packages" },
    //                cumulativeStatistics: cumulativeStatistics
    //            )
    //            {
    //                // events
    //                OnBegin = (_engine) =>
    //                {
    //                    output.WriteLine("Root: " + _engine.Root);
    //                    output.WriteLine("Protected Root: " + protectedRoot);
    //                    output.WriteLine();
    //                },
    //                DirectoryEntered = (dir, _engine, metadata) =>
    //                {
                        
    //                },
    //                DirectoryUserSkipped = (dir, _, skippedDeletion) =>
    //                {
    //                    if (skippedDeletion)
    //                    {
    //                        output.WriteLine(new string(' ', level * 2) + "SKIPPED DELETION " + dir.Name + "...");
    //                    }
    //                    else
    //                    {
    //                        output.WriteLine(new string(' ', level-- * 2) + "SKIPPED...");
    //                    }
    //                },
    //                DirectoryFlaggedForDeletion = (dir, _) =>
    //                {
    //                    if (dir.IsChildOf(protectedRoot))
    //                        TraversalActions.SkipDirectory();
    //                },
    //                DirectoryDeleting = (dir, _engine, metadata) =>
    //                {
    //                    if (_engine.FilterMatch.HasValue && dir == _engine.FilterMatch.Value)
    //                        TraversalActions.SkipDirectory();  // prevents bin, obj from being deleted
    //                    output.WriteLine(new string(' ', level * 2) + "DELETING " + dir.Name + "...");
    //                },
    //                // DirectoryDeleted = (dir, _) =>
    //                // {
    //                //     output.WriteLine(new string(' ', level * 2) + "DELETED");
    //                // },
    //                DirectoryExited = (dir, _) =>
    //                {
    //                    output.WriteLine(new string(' ', --level * 2) + "/");
    //                },
    //                FileFound = (file, _, metadata) =>
    //                {
    //                    output.WriteLine(new string(' ', level * 2) + "├─" + file.Name + " (" + FileUtil.GetDisplayFileSize(file) + ")");
    //                },
    //                FileDeleted = (file, size, _) =>
    //                {
    //                    output.WriteLine(new string(' ', level * 2 + 2) + "DELETED");
    //                }
    //            };
    //            engine.Start(dryRun: dryRun);
    //            cumulativeStatistics = engine.Statistics;
    //            output.WriteLine();
    //            output.WriteLine("Cumulative Traversal Statistics");
    //            output.WriteLine("===============================");
    //            output.WriteLine(engine.RenderStatistics());
    //            output.WriteLine();
    //        }

    //        if (printStatistics)
    //        {
    //            Console.WriteLine();
    //            Console.WriteLine("Cumulative Traversal Statistics");
    //            Console.WriteLine("===============================");
    //            Console.WriteLine(engine.RenderStatistics());
    //        }
    //    }

    //    private class EmptyDirs : TraversalEngine
    //    {
    //        public string[] DirNames { get; }

    //        public ICriterinator<DirectoryPath> DirExceptions { get; }

    //        public override TraversalOptimization Optimization => new TraversalOptimization
    //        {
    //            DirectoryFilter = (dir) => dir.Name.In(DirNames) && (DirExceptions == null || DirExceptions.IsMatch(dir))
    //        };

    //        public OutputMechanism Output { get; } = OutputMechanism.Console;

    //        public override Action<TraversalEngine> OnBegin => (engine) =>
    //        {
    //            Output.WriteLine("Root: " + engine.Root);
    //            Output.WriteLine();
    //        };

    //        public override Action<TraversalEngine> OnEnd => (engine) =>
    //        {
    //            Output.WriteLine();
    //            Output.WriteLine("Traversal Statistics");
    //            Output.WriteLine("====================");
    //            Output.WriteLine(engine.RenderStatistics());
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectoryEntered => (dir, engine) =>
    //        {
    //            Output.WriteLine("ENTERED: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root, strict: false));
    //            engine.DirectoryMetadata.Action = ClientAction.Empty;
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectorySkipped => (dir, engine) =>
    //        {
    //            if (engine.DirectoryMetadata.ClientSkipped)
    //                Output.WriteLine("SKIPPED: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root, strict: false));
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectoryEmptying => (dir, engine) =>
    //        {
    //            Output.Write("EMPTYING DIR: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root));
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectoryEmptied => (dir, _) =>
    //        {
    //            Output.WriteLine("EMPTIED DIR: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root));
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectoryDeleting => (dir, engine) =>
    //        {
    //            Output.Write("DELETING DIR: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root));
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectoryDeleted => (dir, _) =>
    //        {
    //            Output.WriteLine("DELETING DIR: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root));
    //        };

    //        public override Action<DirectoryPath, TraversalEngine> DirectoryExited => (dir, _) =>
    //        {
    //            Output.WriteLine("EXITED DIR: " + dir.DisplayAsVirtualPathFromRoot_Optimized(Root));
    //        };

    //        public EmptyDirs
    //        (
    //            DirectoryPath root, 
    //            string[] dirNames, 
    //            ICriterinator<DirectoryPath> dirExceptions = null,
    //            Action<DirectoryPath, TraversalEngine> directoryEntered = null,
    //            Action<DirectoryPath, TraversalEngine> directorySkipped = null,
    //            Action<DirectoryPath, TraversalEngine> directoryDeleting = null,
    //            Action<DirectoryPath, TraversalEngine> directoryDeleted = null,
    //            Action<DirectoryPath, TraversalEngine> directoryEmptying = null,
    //            Action<DirectoryPath, TraversalEngine> directoryEmptied = null,
    //            Action<DirectoryPath, TraversalEngine> directoryErrored = null,
    //            Action<DirectoryPath, TraversalEngine> directoryExited = null,
    //            Action<FilePath, TraversalEngine> fileFound = null,
    //            Action<FilePath, TraversalEngine> fileDeleting = null,
    //            Action<FilePath, TraversalEngine> fileDeleted = null,
    //            Action<FilePath, TraversalEngine> fileErrored = null,
    //            IDictionary<string, object> cumulativeStatistics = null
    //        ) : base
    //        (
    //            root,
    //            directoryEntered: directoryEntered,
    //            directorySkipped: directorySkipped,
    //            directoryDeleting: directoryDeleting,
    //            directoryDeleted: directoryDeleted,
    //            directoryEmptying: directoryEmptying,
    //            directoryEmptied: directoryEmptied,
    //            directoryErrored: directoryErrored,
    //            directoryExited: (dir, engine) =>
    //            {
    //                Output.WriteLine("EXITED: " + dir.DisplayAsVirtualPathFromRoot_Optimized(engine.Root));
    //                directoryExited?.Invoke(dir, engine);
    //            },
    //            fileFound: (file, engine) =>
    //            {
    //                Console.WriteLine("FOUND: " + file.Name + " (" + FileUtil.GetDisplayFileSize(file) + ")");
    //                fileFound?.Invoke(file, engine);
    //            },
    //            fileDeleting: (file, engine) =>
    //            {
    //                Console.Write("DELETING: " + file.Name);
    //                fileDeleting?.Invoke(file, engine);
    //            },
    //            fileDeleted: (file, engine) =>
    //            {
    //                Console.WriteLine("DELETED: " + file.Name);
    //                fileDeleted?.Invoke(file, engine);
    //            },
    //            fileErrored: (file, engine) =>
    //            {
    //                var err = engine.TraversalErrors.Last();
    //                Console.WriteLine("!! " + err.Message);
    //                Console.WriteLine("!! path = " + err.Path);
    //                fileErrored?.Invoke(file, engine);
    //            },
    //            cumulativeStatistics: cumulativeStatistics
    //        )
    //        {
    //            DirNames = dirNames;
    //            DirExceptions = dirExceptions;
    //        }
    //    }

    //    class OutputMechanism
    //    {
    //        public Action<string> WriteAction { get; }

    //        public Action<string> WriteLineAction { get; }

    //        public OutputMechanism(Action<string> writeAction, Action<string> writeLineAction)
    //        {
    //            WriteAction = writeAction;
    //            WriteLineAction = writeLineAction;
    //        }

    //        public void Write(string text)
    //        {
    //            WriteAction.Invoke(text);
    //        }

    //        public void WriteLine()
    //        {
    //            WriteLine("");
    //        }

    //        public void WriteLine(string text)
    //        {
    //            WriteLineAction.Invoke(text);
    //        }

    //        public static OutputMechanism Console = new OutputMechanism
    //        (
    //            msg => Console.Write(msg),
    //            msg => Console.WriteLine(msg)
    //        );
    //    }
    //}
}
