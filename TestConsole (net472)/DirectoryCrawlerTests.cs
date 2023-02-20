using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.DirectoryCrawler;
using Horseshoe.NET.IO.FileFilter;

namespace TestConsole
{
    class DirectoryCrawlerTests : RoutineX
    {
        public DirectoryPath MyDirectory { get; private set; } = Directory.GetCurrentDirectory().Replace("bin\\Debug", "");

        public DirectoryPath RecursiveDeleteRoot { get; } = "C:\\Users\\E029791\\Dev\\DirectoryCrawl\\";

        public override string Text => "DirectoryCrawler Tests";

        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Enumerate directory contents",
                () =>
                {
                    MyDirectory = FileUtil.NormalizePath(PromptX.Value<string>("Directory", quickValue: MyDirectory.FullName));
                    Console.WriteLine("Crawling " + MyDirectory + "...");
                    new DirectoryCrawler
                    (
                        MyDirectory,
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dir.FullName).Substring(MyDirectory.FullName.Length));
                                    break;
                                case DirectoryCrawlEvent.OnComplete:
                                    Console.WriteLine(metadata.Statistics.DisplayCurated(indent: 2, padBefore: 1));
                                    break;
                            }
                        },
                        fileCrawled: (@event, file, metadata) =>
                        {
                            switch (@event)
                            {
                                case FileCrawlEvent.FileProcessing:
                                    Console.WriteLine("  ~\\" + file.FullName.Substring(MyDirectory.FullName.Length));
                                    break;
                            }
                        }
                    ).Go();
                }
            ),
            BuildMenuRoutine
            (
                "Enumerate directory contents (dll's only)",
                () =>
                {
                    MyDirectory = FileUtil.NormalizePath(PromptX.Value<string>("Directory", quickValue: MyDirectory.FullName));
                    Console.WriteLine("Crawling " + MyDirectory + "...");
                    new DirectoryCrawler
                    (
                        MyDirectory,
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dir.FullName).Substring(MyDirectory.FullName.Length));
                                    break;
                                case DirectoryCrawlEvent.OnComplete:
                                    Console.WriteLine(metadata.Statistics.DisplayCurated(indent: 2, padBefore: 1));
                                    break;
                            }
                        },
                        fileCrawled: (@event, file, metadata) =>
                        {
                            switch (@event)
                            {
                                case FileCrawlEvent.FileProcessing:
                                    Console.WriteLine("  ~\\" + file.FullName.Substring(MyDirectory.FullName.Length));
                                    break;
                            }
                        },
                        options: new CrawlOptions { /*FileFilter = FileFilter.CreateFileExtensionFilter(".dll")*/ }
                    ).Go();
                }
            ),
            BuildMenuRoutine
            (
                "Enumerate directory contents (except dll's)",
                () =>
                {
                    MyDirectory = FileUtil.NormalizePath(PromptX.Value<string>("Directory", quickValue: MyDirectory.FullName));
                    Console.WriteLine("Crawling " + MyDirectory + "...");
                    new DirectoryCrawler
                    (
                        MyDirectory,
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dir.FullName).Substring(MyDirectory.FullName.Length));
                                    break;
                                case DirectoryCrawlEvent.OnComplete:
                                    Console.WriteLine(metadata.Statistics.DisplayCurated(indent: 2, padBefore: 1));
                                    break;
                            }
                        },
                        fileCrawled: (@event, file, metadata) =>
                        {
                            switch (@event)
                            {
                                case FileCrawlEvent.FileProcessing:
                                    Console.WriteLine("  ~\\" + file.FullName.Substring(MyDirectory.FullName.Length));
                                    break;
                            }
                        },
                        options: new CrawlOptions { /*FileFilter = FileFilter.CreateFileExtensionFilter(".dll")*/ }
                    ).Go();
                }
            ),
            BuildMenuRoutine
            (
                "Build 'Recursive Delete' directory/file structure",
                () =>
                {
                    Console.WriteLine("Creating and filling " + RecursiveDeleteRoot + "...");
                    Directory.CreateDirectory(RecursiveDeleteRoot);
                    File.WriteAllText(Path.Combine(RecursiveDeleteRoot, "roota.txt"), "This is me, bla bla bla.");
                    File.WriteAllText(Path.Combine(RecursiveDeleteRoot, "rootb.txt"), "This is me, bla bla bla.");
                    Directory.CreateDirectory(Path.Combine(RecursiveDeleteRoot, "subdir1"));
                    File.WriteAllText(Path.Combine(RecursiveDeleteRoot, "subdir1", "subdir1a.txt"), "This is me, bla bla bla.");
                    File.WriteAllText(Path.Combine(RecursiveDeleteRoot, "subdir1", "subdir1b.txt"), "This is me, bla bla bla.");
                    Directory.CreateDirectory(Path.Combine(RecursiveDeleteRoot, "subdir2"));
                    File.WriteAllText(Path.Combine(RecursiveDeleteRoot, "subdir2", "subdir2a.txt"), "This is me, bla bla bla.");
                    File.WriteAllText(Path.Combine(RecursiveDeleteRoot, "subdir2", "subdir2b.txt"), "This is me, bla bla bla.");
                    var totalSize = DirectoryCrawler.GetTotalSize(RecursiveDeleteRoot);
                    Console.WriteLine("done (" + FileUtil.GetDisplayFileSize(totalSize) + ")");
                }
            ),
            BuildMenuRoutine
            (
                "Recursive Delete (dry run)",
                () =>
                {
                    RecursiveDeleteBase(true);
                }
            ),
            BuildMenuRoutine
            (
                "Recursive Delete",
                () =>
                {
                    RecursiveDeleteBase(false);
                }
            ),
            BuildMenuRoutine
            (
                "Recursive Hash",
                () =>
                {
                    TraversalStatistics stats = null;
                    Console.WriteLine("Hashing all files: " + RecursiveDeleteRoot);
                    var finalHash = new Horseshoe.NET.Crypto.RecursiveHash
                    (
                        RecursiveDeleteRoot,
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.OnComplete:
                                    stats = metadata.Statistics;
                                    break;
                            }
                        },
                        fileHashed: (filePath, hash, metadata) =>
                        {
                            Console.WriteLine("  ~\\" + filePath.FullName.Substring(RecursiveDeleteRoot.FullName.Length) + " -- " + hash);
                        }
                    ).Go();
                    Console.WriteLine("Final hash: " + finalHash);
                    Console.WriteLine(stats.DisplayCurated(indent: 2, padBefore: 1));
                }
            ),
            BuildMenuRoutine
            (
                "Hi",
                () =>
                {
                    var subdirectories = new List<string>();
                    new DirectoryCrawler
                    (
                        "C:\notes",
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    subdirectories.Add(dir);
                                    break;
                            }
                        }
                    );
                    subdirectories.Sort();
                    RenderX.List(subdirectories);
                }
            )
        };

        static void RecursiveDeleteBase(bool dryRun)
        {
            var root = "C:\\Users\\E029791\\Dev\\DirectoryCrawl\\";
            var totalSize = DirectoryCrawler.GetTotalSize
            (
                root
            );
            Console.WriteLine("Deleting " + FileUtil.GetDisplayFileSize(totalSize) + " from " + root + "...");
            long cumulativeDeletedBytes = 0L;
            DirectoryCrawler.RecursiveDeleteDirectory
            (
                root,
                directoryCrawled: (@event, dir, metadata) =>
                {
                    switch (@event)
                    {
                        case DirectoryCrawlEvent.OnInit:
                            Console.WriteLine("Scanning dir for delete...");
                            break;
                        case DirectoryCrawlEvent.DirectoryEntered:
                            Console.WriteLine(new string(' ', metadata.Level) + FileUtil.NormalizePath(dir.FullName).Substring(root.Length));
                            break;
                        case DirectoryCrawlEvent.DirectoryDeleted:
                            Console.WriteLine(new string(' ', metadata.Level) + FileUtil.NormalizePath(dir.FullName).Substring(root.Length) + " -- directory deleted");
                            break;
                        case DirectoryCrawlEvent.DirectoryErrored:
                            Console.WriteLine(new string(' ', metadata.Level) + metadata.Exception.RenderMessage());
                            break;
                        case DirectoryCrawlEvent.OnHalt:
                            Console.WriteLine(new string(' ', metadata.Level) + (metadata.Exception != null ? metadata.Exception.RenderMessage() + " -- " : "") + "halted!");
                            break;
                        case DirectoryCrawlEvent.OnComplete:
                            Console.WriteLine(metadata.Statistics.DisplayCurated(indent: 2, padBefore: 1));
                            Console.WriteLine();
                            Console.WriteLine("Cumulative Deleted Bytes: " + FileUtil.GetDisplayFileSize(cumulativeDeletedBytes));
                            break;
                    }
                },
                fileCrawled: (@event, file, metadata) =>
                {
                    switch (@event)
                    {
                        case FileCrawlEvent.FileDeleting:
                            cumulativeDeletedBytes += file.Size;
                            Console.WriteLine(new string(' ', metadata.Level) + file.FullName.Substring(root.Length) + " -- deleting file (" + file.GetDisplaySize() + ")");
                            break;
                    }
                },
                options: new CrawlOptions { DryRun = dryRun }
            ).Go();
        }

        //private static int _numberCriteriaMatches;
        //private static long _currentFileLength;
        //private static long _totalBytes;
        //private static int _totalFiles;
        //private static int _totalDirectories;
        //private static int _numberErrors;

        //static void RecursiveDelete(bool dryRun = false)
        //{
        //    _numberCriteriaMatches = 0;
        //    _totalBytes = 0;
        //    _totalFiles = 0;
        //    _totalDirectories = 0;
        //    _numberErrors = 0;

        //    var searchDirectory = PromptX.Input("Please enter the directory to search");

        //    if (searchDirectory == null)
        //    {
        //        RenderX.Alert("No directory was entered.");
        //        return;
        //    }

        //    if (searchDirectory.StartsWith("\"") && searchDirectory.EndsWith("\""))
        //    {
        //        searchDirectory = searchDirectory.Substring(1, searchDirectory.Length - 2);
        //    }

        //    var dirCrawl = new DirectoryCrawler<object>
        //    (
        //        searchDirectory,
        //        directoryCrawled: (eventType, dirArgs, fileArgs, stats) =>
        //        {
        //            switch (eventType)
        //            {
        //                case DirectoryCrawlEventType.FileEncountered:
        //                    Console.WriteLine(fileArgs.File);
        //                    break;
        //                case DirectoryCrawlEventType.FileErrored:
        //                    _numberErrors++;
        //                    Console.WriteLine(fileArgs.Exception.GetType().FullName + ": " + fileArgs.Exception.Message);
        //                    break;
        //                case DirectoryCrawlEventType.DirectoryEncountered:
        //                    if (dirArgs.Directory.Name.IsIn("bin", "obj"))
        //                    {
        //                        _numberCriteriaMatches++;
        //                        var recursiveDeleteSweep = new RecursiveDelete
        //                        (
        //                            dirArgs.Directory.FullName,
        //                            deletingFile: (file) =>
        //                            {
        //                                Console.WriteLine(file);
        //                                Console.Write("Deleting (" + FileUtil.GetDisplayFileSize(file.Length) + ")... ");
        //                                _currentFileLength = file.Length;
        //                            },
        //                            fileDeleted: (file) =>
        //                            {
        //                                Console.WriteLine("Done.");
        //                                _totalBytes += _currentFileLength;
        //                                _totalFiles++;
        //                            },
        //                            directoryPendingDelete: (dir) =>
        //                            {
        //                                Console.WriteLine(dir);
        //                                Console.WriteLine("Pending Delete... ");
        //                            },
        //                            deletingDirectory: (dir) =>
        //                            {
        //                                Console.WriteLine(dir);
        //                                Console.Write("Deleting... ");
        //                            },
        //                            directoryDeleted: (file) =>
        //                            {
        //                                Console.WriteLine("Done.");
        //                                _totalDirectories++;
        //                            },
        //                            options: new CrawlOptions { DryRun = dirArgs.DryRun }
        //                        );
        //                        recursiveDeleteSweep.Go();
        //                        //dirArgs.SkipDirectory = true;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine(dirArgs.Directory);
        //                    }
        //                    break;
        //                case DirectoryCrawlEventType.DirectoryErrored:
        //                    _numberErrors++;
        //                    Console.WriteLine(dirArgs.Exception.GetType().FullName + ": " + dirArgs.Exception.Message);
        //                    break;
        //            }
        //        },
        //        options:
        //            new CrawlOptions
        //            {
        //                DirectoriesOnly = true,
        //                DryRun = dryRun
        //            }
        //    );
        //    dirCrawl.Go();
        //    Console.WriteLine();
        //    Console.WriteLine("# of criteria matches: " + _numberCriteriaMatches);
        //    Console.WriteLine("Total # of directories deleted: " + _totalDirectories);
        //    Console.WriteLine("Total # of files deleted: " + _totalFiles);
        //    Console.WriteLine("Total amount space freed: " + FileUtil.GetDisplayFileSize(_totalBytes));
        //    Console.WriteLine("# Errors: " + _numberErrors);
        //}

        //static void RecursiveCopy(bool dryRun = false)
        //{
        //    _totalBytes = 0;
        //    _totalFiles = 0;
        //    _totalDirectories = 0;
        //    _numberErrors = 0;

        //    var sourceDirectoryPath = PromptX.Input("Copying from");
        //    var destinationDirectoryPath = PromptX.Input("Copying to");
        //    var sourceDirectory_temp = new DirectoryInfo(sourceDirectoryPath);
        //    var destinationDirectory_temp = new DirectoryInfo(destinationDirectoryPath);
        //    if (!string.Equals(sourceDirectory_temp.Name, destinationDirectory_temp.Name, StringComparison.OrdinalIgnoreCase))
        //    {
        //        var proposedDestinationDirectoryPath = Path.Combine(destinationDirectoryPath, sourceDirectory_temp.Name);
        //        while (true)
        //        {
        //            Console.WriteLine("Do you want to use the following destination instead?  [Y/n]");
        //            Console.WriteLine();
        //            Console.WriteLine(proposedDestinationDirectoryPath);
        //            Console.WriteLine();
        //            var answer = PromptX.Input();
        //            if (answer.IsIn(null, "y", "Y"))
        //            {
        //                destinationDirectoryPath = proposedDestinationDirectoryPath;
        //                break;
        //            }
        //            if (answer.IsIn("n", "N"))
        //            {
        //                break;
        //            }
        //            Console.WriteLine("Invalid response.");
        //            Console.WriteLine();
        //        }
        //    }

        //    var fileCopy = new RecursiveCopy
        //    (
        //        sourceDirectoryPath,
        //        destinationDirectoryPath,
        //        directoryCrawled: (eventType, dirArgs, fileArgs, stats) =>
        //        {
        //            switch (eventType)
        //            {
        //                case DirectoryCrawlEventType.FileErrored:
        //                    _numberErrors++; 
        //                    Console.WriteLine(fileArgs.Exception.GetType().FullName + ": " + fileArgs.Exception.Message);
        //                    break;
        //            }
        //        },
        //        processingFile:
        //            (src, dest) =>
        //            {
        //                Console.WriteLine(dest);
        //                Console.Write("Copying (" + FileUtil.GetDisplayFileSize(src.Length) + ")... ");
        //            },
        //        fileProcessed:
        //            (src, dest, action) =>
        //            {
        //                Console.WriteLine(action + ".");
        //                _totalBytes += dest.Exists ? dest.Length : src.Length;  // dest does not exist in dry run mode
        //                _totalFiles++;
        //            },
        //        creatingDestinationRootDirectory:
        //            (dir) =>
        //            {
        //                Console.WriteLine(dir);
        //                Console.Write("Creating... ");
        //            },
        //        destinationRootDirectoryCreated:
        //            (dir) =>
        //            {
        //                Console.WriteLine("Done.");
        //            },
        //        creatingDestinationDirectory:
        //            (src, dest) =>
        //            {
        //                Console.WriteLine(dest);
        //                Console.Write("Creating... ");
        //            },
        //        destinationDirectoryCreated:
        //            (src, dest) =>
        //            {
        //                Console.WriteLine("Done.");
        //                _totalDirectories++;
        //            },
        //        directoryErrored:
        //            (args, ex) => { _numberErrors++; Console.WriteLine(ex.GetType().FullName + ": " + ex.Message); },
        //        exceptDirectories:
        //            (dir) =>
        //            {
        //                return dir.Name.IsIn("bin", "obj", ".vs", ".vscode", ".git", "packages", "$tf", "compile", "node_modules");
        //            },
        //        requestCopyMode:
        //            (options) =>
        //            {
        //                var modes = Enum.GetValues(typeof(CopyMode)).Cast<CopyMode>();
        //                Console.WriteLine();
        //                Console.WriteLine("Destination not empty.  Please select a file copy mode.");
        //                Console.WriteLine();
        //                var selection = PromptX.Menu(modes, title: "File copy mode");
        //                options.CopyMode = selection.SelectedItem;
        //                switch (options.CopyMode)
        //                {
        //                    case CopyMode.Overwrite:
        //                    case CopyMode.OverwriteIfNewer:
        //                        while (true)
        //                        {
        //                            Console.WriteLine("Do you want to delete destination files and folders that do not exist in the source folder?  [y/N]");
        //                            var yn = PromptX.Input();
        //                            if (yn.IsIn("Y", "y"))
        //                            {
        //                                options.DeleteDestinationsWithNoMatchingSources = true;
        //                                break;
        //                            }
        //                            if (yn.IsIn("N", "n"))
        //                            {
        //                                options.DeleteDestinationsWithNoMatchingSources = false;
        //                                break;
        //                            }
        //                            Console.WriteLine("Invalid response.");
        //                            Console.WriteLine("");
        //                        }
        //                        break;
        //                }
        //            },
        //        options:
        //            new CopyOptions
        //            {
        //                DryRun = dryRun
        //            }
        //    );
        //    fileCopy.Go();
        //    Console.WriteLine();
        //    Console.WriteLine("Total # of directories copied: " + _totalDirectories);
        //    Console.WriteLine("Total # of files copied: " + _totalFiles);
        //    Console.WriteLine("Total size of copied files: " + FileUtil.GetDisplayFileSize(_totalBytes));
        //    Console.WriteLine("# Errors: " + _numberErrors);
        //}
    }
}
