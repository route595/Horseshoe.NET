using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Horseshoe.NET;
using Horseshoe.NET.Compare;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.DirectoryCrawler;
using Horseshoe.NET.IO.FileFilter;

namespace TestConsole
{
    class IOTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Display file sizes",
                () =>
                {
                    Console.WriteLine("-1 B  =>  " + FileUtil.GetDisplayFileSize(-1));
                    Console.WriteLine("-1 B in KB  =>  " + FileUtil.GetDisplayFileSize(-1, unit: FileSizeUnit.KB));
                    Console.WriteLine("0  =>  " + FileUtil.GetDisplayFileSize(0));
                    Console.WriteLine("0 B in GB  =>  " + FileUtil.GetDisplayFileSize(0, unit: FileSizeUnit.GB));
                    Console.WriteLine("1000000  =>  " + FileUtil.GetDisplayFileSize(1000000));
                    Console.WriteLine("1000000 'bi'  =>  " + FileUtil.GetDisplayFileSize(1000000, bi: true));
                    Console.WriteLine("1000000 in B  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.B));
                    Console.WriteLine("1000000 in B w/o sep  =>  " + FileUtil.GetDisplayFileSize(1000000, addSeparators: false, unit: FileSizeUnit.B));
                    Console.WriteLine("1000000 B in KB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.KB));
                    Console.WriteLine("1000000 B in KiB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.KiB));
                    Console.WriteLine("1000000 B in GB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.GB));
                    Console.WriteLine("1000000 B in GiB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSizeUnit.GiB));
                    Console.WriteLine("1000000 B in GB w/ 3 dec  =>  " + FileUtil.GetDisplayFileSize(1000000, maxDecimalPlaces: 3, unit: FileSizeUnit.GB));
                    Console.WriteLine("1000000 B in GiB w/ 3 dec  =>  " + FileUtil.GetDisplayFileSize(1000000, maxDecimalPlaces: 3, unit: FileSizeUnit.GiB));
                }
            ),
            BuildMenuRoutine
            (
                "Project dir - crawl all files",
                () =>
                {
                    var path = GetBinParent();

                    new DirectoryCrawler
                    (
                        path,
                        options: new CrawlOptions
                        {
                            StatisticsOn = true
                        },
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("ENTER: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectoryExited:
                                    Console.WriteLine("EXIT: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectorySkipped:
                                    Console.WriteLine("SKIP: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\") + " ~ " + metadata.SkipReason);
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
                                    Console.WriteLine("PROC: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length));
                                    break;
                            }
                        }
                    ).Go();
                }
            ),
            BuildMenuRoutine
            (
                "Project dir - crawl all files (no stats)",
                () =>
                {
                    var path = GetBinParent();

                    new DirectoryCrawler
                    (
                        path,
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("ENTER: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectoryExited:
                                    Console.WriteLine("EXIT: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectorySkipped:
                                    Console.WriteLine("SKIP: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\") + " ~ " + metadata.SkipReason);
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
                                    Console.WriteLine("PROC: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length));
                                    break;
                            }
                        }
                    ).Go();
                }
            ),
            BuildMenuRoutine
            (
                "Project dir - crawl bin/obj files",
                () =>
                {
                    var path = GetBinParent();

                    var dirCrawl = new DirectoryCrawler
                    (
                        path,
                        options: new CrawlOptions
                        {
                            DirectoryFilter = new DirectoryNameFilter(CompareMode.In, new[] { "bin", "obj" }),
                            StatisticsOn = true
                        },
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("DIR ENTER: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectorySkipped:
                                    if (metadata.SkipReason != SkipReason.ClientFiltered)
                                        Console.WriteLine("DIR SKIP: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\") + " ~ " + metadata.SkipReason);
                                    break;
                                case DirectoryCrawlEvent.DirectoryExited:
                                    Console.WriteLine("DIR EXIT: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
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
                                    Console.WriteLine("FILE PROC: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length));
                                    break;
                                case FileCrawlEvent.FileSkipped:
                                    if (metadata.SkipReason != SkipReason.ClientFiltered)
                                        Console.WriteLine("FILE SKIP: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length) + " ~ " + metadata.SkipReason);
                                    break;
                            }
                        }
                    );
                    dirCrawl.Go();
                }
            ),
            BuildMenuRoutine
            (
                "Project dir - delete bin/obj (dry-run)",
                () =>
                {
                    var path = GetBinParent();
                    DirectoryCrawler.RecursiveDeleteDirectory
                    (
                        path,
                        options: new CrawlOptions
                        {
                            DryRun = true,
                            DirectoryFilter = new DirectoryNameFilter(CompareMode.In, new[] { "bin", "obj" }),
                            StatisticsOn = true
                        },
                        directoryCrawled: (@event, dir, metadata) =>
                        {
                            switch (@event)
                            {
                                case DirectoryCrawlEvent.DirectoryEntered:
                                    Console.WriteLine("DIR ENTER: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectorySkipped:
                                    if (metadata.SkipReason != SkipReason.ClientFiltered)
                                        Console.WriteLine("DIR SKIP: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\") + "  ~ " + metadata.SkipReason);
                                    break;
                                case DirectoryCrawlEvent.DirectoryDeleting:
                                    Console.WriteLine("DIR DEL" + (metadata.DryRun ? " (dry-run)" : "") + ": " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
                                    break;
                                case DirectoryCrawlEvent.DirectoryExited:
                                    Console.WriteLine("DIR EXIT: " + (Zap.String(dir.FullName.Substring(metadata.DirectoryCrawler.Root.Length)) ?? "\\"));
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
                                    Console.WriteLine("FILE PROC: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length));
                                    break;
                                case FileCrawlEvent.FileSkipped:
                                    if (metadata.SkipReason != SkipReason.ClientFiltered)
                                        Console.WriteLine("FILE SKIP: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length) + " ~ " + metadata.SkipReason);
                                    break;
                                case FileCrawlEvent.FileDeleting:
                                    Console.WriteLine("FILE DEL" + (metadata.DryRun ? " (dry-run)" : "") + ": " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length));
                                    break;
                                case FileCrawlEvent.FileErrored:
                                    Console.WriteLine("FILE ERR: " + file.FullName.Substring(metadata.DirectoryCrawler.Root.Length));
                                    break;
                            }
                        }
                    );
                }
            )
        };

        private Regex BinPath { get; } = new Regex(@"[\\/]bin[\\/]?");

        private DirectoryPath GetBinParent()
        {
            DirectoryPath path = Directory.GetCurrentDirectory();

            Console.WriteLine("Start dir: " + path);
            while (BinPath.IsMatch(path.FullName))
            {
                path = path.Parent;
            }
            Console.WriteLine("Current dir: " + path);
            Console.WriteLine();

            return path;
        }
    }
}
