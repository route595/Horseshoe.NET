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
                    MyDirectory = FileUtil.NormalizePath(PromptX.Input("Directory", predictiveText: MyDirectory));
                    Console.WriteLine("Crawling " + MyDirectory + "...");
                    DirectoryCrawlStatistics stats = null;
                    new DirectoryCrawler
                    (
                        MyDirectory,
                        directoryCrawled: (eventType, dirArgs, fileArgs, _stats) =>
                        {
                            switch (eventType)
                            {
                                case DirectoryCrawlEventType.FileFound:
                                    Console.WriteLine("  ~\\" + fileArgs.File.FullName.Substring(MyDirectory.FullName.Length));
                                    stats = _stats;
                                    break;
                                case DirectoryCrawlEventType.DirectoryEntered:
                                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dirArgs.Directory.FullName).Substring(MyDirectory.FullName.Length));
                                    stats = _stats;
                                    break;
                            }
                        }
                    ).Go();
                    Console.WriteLine();
                    Console.WriteLine("Statistics...");
                    Console.WriteLine(string.Join(Environment.NewLine, stats.Display().Select(s => "  " + s)));
                }
            ),
            BuildMenuRoutine
            (
                "Enumerate directory contents (dll's only)",
                () =>
                {
                    MyDirectory = FileUtil.NormalizePath(PromptX.Input("Directory", predictiveText: MyDirectory));
                    Console.WriteLine("Crawling " + MyDirectory + "...");
                    DirectoryCrawlStatistics stats = null;
                    new DirectoryCrawler
                    (
                        MyDirectory,
                        directoryCrawled: (eventType, dirArgs, fileArgs, _stats) =>
                        {
                            switch (eventType)
                            {
                                case DirectoryCrawlEventType.FileFound:
                                    Console.WriteLine("  ~\\" + fileArgs.File.FullName.Substring(MyDirectory.FullName.Length));
                                    stats = _stats;
                                    break;
                                case DirectoryCrawlEventType.DirectoryEntered:
                                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dirArgs.Directory.FullName).Substring(MyDirectory.FullName.Length));
                                    stats = _stats;
                                    break;
                            }
                        },
                        options: new CrawlOptions { FileFilter = new FileExtensionFilter("dll") }
                    ).Go();
                    Console.WriteLine();
                    Console.WriteLine("Statistics...");
                    Console.WriteLine(string.Join(Environment.NewLine, stats.Display().Select(s => "  " + s)));
                }
            ),
            BuildMenuRoutine
            (
                "Enumerate directory contents (except dll's)",
                () =>
                {
                    MyDirectory = FileUtil.NormalizePath(PromptX.Input("Directory", predictiveText: MyDirectory));
                    Console.WriteLine("Crawling " + MyDirectory + "...");
                    DirectoryCrawlStatistics stats = null;
                    new DirectoryCrawler
                    (
                        MyDirectory,
                        directoryCrawled: (eventType, dirArgs, fileArgs, _stats) =>
                        {
                            switch (eventType)
                            {
                                case DirectoryCrawlEventType.FileFound:
                                    Console.WriteLine("  ~\\" + fileArgs.File.FullName.Substring(MyDirectory.FullName.Length));
                                    stats = _stats;
                                    break;
                                case DirectoryCrawlEventType.DirectoryEntered:
                                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dirArgs.Directory.FullName).Substring(MyDirectory.FullName.Length));
                                    stats = _stats;
                                    break;
                            }
                        },
                        options: new CrawlOptions { FileFilter = new FileExtensionFilter("dll", filterMode: FilterMode.FilterOutAll) }
                    ).Go();
                    Console.WriteLine();
                    Console.WriteLine("Statistics...");
                    Console.WriteLine(string.Join(Environment.NewLine, stats.Display().Select(s => "  " + s)));
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
                    var totalSize = new RecursiveSize(RecursiveDeleteRoot).Go();
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
                    DirectoryCrawlStatistics stats = null;
                    Console.WriteLine("Hashing all files: " + RecursiveDeleteRoot);
                    var megaHash = new Horseshoe.NET.Crypto.RecursiveHash
                    (
                        RecursiveDeleteRoot,
                        directoryCrawled: (eventType, dirArgs, fileArgs, _stats) =>
                        {
                            switch (eventType)
                            {
                                case DirectoryCrawlEventType.FileFound:
                                    stats = _stats;
                                    break;
                            }
                        },
                        fileHashed: (filePath, hash) =>
                        {
                            Console.WriteLine("  ~\\" + filePath.FullName.Substring(RecursiveDeleteRoot.FullName.Length) + " -- " + hash);
                        }
                    ).Go();
                    Console.WriteLine("Hash of all hashes: " + megaHash);
                    Console.WriteLine();
                    Console.WriteLine("Statistics...");
                    Console.WriteLine(string.Join(Environment.NewLine, stats.Display().Select(s => "  " + s)));
                }
            ),
        };

        static void RecursiveDeleteBase(bool dryRun)
        {
            var root = "C:\\Users\\E029791\\Dev\\DirectoryCrawl\\";
            var stats = new DirectoryCrawlStatistics();
            new DirectoryCrawler
            (
                root,
                directoryCrawled: (eventType, dirArgs, fileArgs, _stats) =>
                {
                    switch (eventType)
                    {
                        case DirectoryCrawlEventType.FileFound:
                            stats = _stats;
                            break;
                    }
                }
            ).Go();
            Console.WriteLine("Deleting " + FileUtil.GetDisplayFileSize(stats.SizeOfFilesCrawled) + " from " + root + "...");
            long cumulativeDeletedBytes = 0L;
            new RecursiveDelete
            (
                root,
                directoryCrawled: (eventType, dirArgs, fileArgs, _stats) =>
                {
                    switch (eventType)
                    {
                        case DirectoryCrawlEventType.DirectoryEntered:
                            Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dirArgs.Directory.FullName).Substring(root.Length) + " -- scanning directory");
                            break;
                        case DirectoryCrawlEventType.DirectoryErrored:
                            Console.WriteLine("  " + dirArgs.Exception.Message + " -- " + dirArgs.Exception.GetType().FullName);
                            break;
                        case DirectoryCrawlEventType.DirectoryCrawlHalted:
                            Console.WriteLine("  " + (dirArgs.Exception != null ? dirArgs.Exception.Message + " -- " + dirArgs.Exception.GetType().FullName + " -- " : "") + "halted!");
                            break;
                        case DirectoryCrawlEventType.FileFound:
                            stats = _stats;
                            break;
                    }
                },
                fileDeleted: (fileArgs) =>
                {
                    cumulativeDeletedBytes += fileArgs.File.Size;
                    Console.WriteLine("  ~\\" + fileArgs.File.FullName.Substring(root.Length) + " -- file deleted (" + fileArgs.File.GetDisplaySize() + ")");
                },
                directoryDeleted: (dirArgs) =>
                {
                    Console.WriteLine("  ~\\" + FileUtil.NormalizePath(dirArgs.Directory.FullName).Substring(root.Length) + " -- directory deleted");
                },
                options: new CrawlOptions { DryRun = dryRun }
            ).Go();
            Console.WriteLine();
            Console.WriteLine("Statistics...");
            Console.WriteLine(string.Join(Environment.NewLine, stats.Display().Select(s => "  " + s)));
            Console.WriteLine("  Cumulative Deleted Bytes: " + FileUtil.GetDisplayFileSize(cumulativeDeletedBytes));
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
