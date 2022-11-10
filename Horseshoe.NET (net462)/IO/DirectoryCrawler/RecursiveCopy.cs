using System;
using System.IO;
using System.Linq;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class RecursiveCopy : DirectoryCrawler
    {
        public int RootLength => Root.FullName.Length;
        public DirectoryPath DestinationRoot { get; }
        public Action<FileArgs<DirectoryPath>> DeletingDestinationFile { get; }
        public Action<FileArgs<DirectoryPath>> DestinationFileDeleted { get; }
        public Action<DirectoryArgs<DirectoryPath>> CreatingDestinationRootDirectory { get; }
        public Action<DirectoryArgs<DirectoryPath>> DestinationRootDirectoryCreated { get; }
        public Action<DirectoryArgs<DirectoryPath>> DeletingDestinationDirectory { get; }
        public Action<DirectoryArgs<DirectoryPath>> DestinationDirectoryDeleted { get; }
        public Action<CopyOptions> RequestCopyMode { get; }

        public RecursiveCopy
        (
            DirectoryPath sourceRoot,
            DirectoryPath destinationRoot,
            DirectoryCrawledEvent<DirectoryPath> directoryCrawled = null,
            Action<FileArgs<DirectoryPath>, FilePath> copyingFile = null,
            Action<FileArgs<DirectoryPath>, FilePath, FileProcessedAction> fileCopied = null,
            Action<DirectoryArgs<DirectoryPath>> creatingDestinationRootDirectory = null,
            Action<DirectoryArgs<DirectoryPath>> destinationRootDirectoryCreated = null,
            Action<DirectoryArgs<DirectoryPath>, DirectoryPath> creatingDestinationDirectory = null,
            Action<DirectoryArgs<DirectoryPath>, DirectoryPath> destinationDirectoryCreated = null,
            Action<CopyOptions> requestCopyMode = null,
            Action<FileArgs<DirectoryPath>> deletingFile = null,
            Action<FileArgs<DirectoryPath>> fileDeleted = null,
            Action<DirectoryArgs<DirectoryPath>> deletingDirectory = null,
            Action<DirectoryArgs<DirectoryPath>> directoryDeleted = null,
            CopyOptions options = null
        ) : base
        (
            sourceRoot,
            directoryCrawled: (eventType, dirArgs, fileArgs, stats) =>
            {
                var fileCopy = (RecursiveCopy)fileArgs.DirectoryCrawler;
                FilePath destinationFile = Path.Combine(fileCopy.DestinationRoot.FullName, fileArgs.File.FullName.Substring(fileCopy.RootLength));
                DirectoryPath destinationDirectory = Path.Combine(fileCopy.DestinationRoot.FullName, dirArgs.Directory.FullName.Substring(fileCopy.RootLength));
                switch (eventType)
                {
                    case DirectoryCrawlEventType.FileFound:
                        copyingFile?.Invoke(fileArgs, destinationFile);
                        if (destinationFile.Exists)
                        {
                            switch (((CopyOptions)fileCopy.Options).CopyMode)
                            {
                                case CopyMode.Normal:
                                    throw new Exception("File already exists: " + destinationFile);  // this should never happen, why?
                                case CopyMode.NormalWithRestartAfterIncompleteCopy:
                                    fileArgs.SkipThisFile(SkipReason.AlreadyExists_RestartedAfterIncompleteCopy);
                                    break;
                                case CopyMode.Overwrite:
                                    if (!fileArgs.DryRun)
                                    {
                                        fileArgs.File.CopyTo(destinationFile.FullName, true);
                                    }
                                    fileCopied?.Invoke(fileArgs, destinationFile, FileProcessedAction.Overwritten);
                                    break;
                                case CopyMode.OverwriteIfNewer:
                                    if (fileArgs.File.DateModified > destinationFile.DateModified)
                                    {
                                        if (!fileArgs.DryRun)
                                        {
                                            fileArgs.File.CopyTo(destinationFile.FullName, true);
                                        }
                                        fileCopied?.Invoke(fileArgs, destinationFile, FileProcessedAction.Overwritten);
                                    }
                                    else
                                    {
                                        fileArgs.SkipThisFile(SkipReason.AlreadyExists_DateModified);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (!fileArgs.DryRun)
                            {
                                fileArgs.File.CopyTo(destinationFile.FullName);
                            }
                            fileCopied?.Invoke(fileArgs, destinationFile, FileProcessedAction.Copied);
                        }
                        break;
                    case DirectoryCrawlEventType.DirectoryEntered:
                        if (destinationDirectory.Exists)
                        {
                            switch (((CopyOptions)dirArgs.DirectoryCrawler.Options).CopyMode)
                            {
                                case CopyMode.Overwrite:
                                case CopyMode.OverwriteIfNewer:
                                    if (((CopyOptions)dirArgs.DirectoryCrawler.Options).DeleteDestinationsWithNoMatchingSources)
                                    {
                                        var fileNames = (dirArgs.DirectoryCrawler.Options.FileSearchPattern == null
                                            ? dirArgs.Directory.GetFiles()
                                            : dirArgs.Directory.GetFiles(dirArgs.DirectoryCrawler.Options.FileSearchPattern)).Select(f => f.Name);
                                        var destinationFiles = dirArgs.DirectoryCrawler.Options.FileSearchPattern == null
                                            ? destinationDirectory.GetFiles()
                                            : destinationDirectory.GetFiles(dirArgs.DirectoryCrawler.Options.FileSearchPattern);
                                        var directoryNames = ((DirectoryInfo)dirArgs.Directory).GetDirectories().Select(d => d.Name); ;
                                        var destinationDirectories = ((DirectoryInfo)destinationDirectory).GetDirectories();

                                        foreach (var destFile in destinationFiles)
                                        {
                                            var fileArgsDE = new FileArgs<DirectoryPath>(destFile, dirArgs.Level + 1, dirArgs.DirectoryCrawler);
                                            if (!fileNames.Contains(destFile.Name))
                                            {
                                                //try
                                                //{
                                                    deletingFile?.Invoke(fileArgsDE);
                                                    if (!dirArgs.DirectoryCrawler.Options.DryRun)
                                                    {
                                                        destFile.Delete();
                                                    }
                                                //}
                                                //catch (Exception ex)
                                                //{
                                                //    if (dirArgs.DirectoryCrawler.Options.ReportErrorsAndContinue)
                                                //    {
                                                //        var fileArgsEx = new FileArgs<DirectoryPath>(destFile, 0, dirArgs.DirectoryCrawler, dryRun: dirArgs.DirectoryCrawler.Options.DryRun, exception: ex);
                                                //        directoryCrawled?.Invoke(DirectoryCrawlEventType.FileErrored, dirArgs, fileArgsEx, stats: stats);
                                                //    }
                                                //    else throw ex;
                                                //}
                                            }
                                        }

                                        foreach (var destDir in destinationDirectories)
                                        {
                                            if (!directoryNames.Contains(destDir.Name))
                                            {
                                                new RecursiveDelete
                                                (
                                                    destDir,
                                                    deletingFile: deletingFile,
                                                    fileDeleted: fileDeleted,
                                                    deletingDirectory: deletingDirectory,
                                                    directoryDeleted: directoryDeleted,
                                                    options: new CrawlOptions { DryRun = dirArgs.DirectoryCrawler.Options.DryRun }
                                                ).Go();
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            creatingDestinationDirectory?.Invoke(dirArgs, destinationDirectory);
                            if (!dirArgs.DryRun)
                            {
                                destinationDirectory.Create();
                            }
                            destinationDirectoryCreated?.Invoke(dirArgs, destinationDirectory);
                        }
                        break;
                }
                directoryCrawled?.Invoke(eventType, dirArgs, fileArgs, stats);
            },
            options: options ?? new CopyOptions()
        )
        {
            DestinationRoot = destinationRoot;
            DeletingDestinationFile = deletingFile;
            DestinationFileDeleted = fileDeleted;
            CreatingDestinationRootDirectory = creatingDestinationRootDirectory;
            DestinationRootDirectoryCreated = destinationRootDirectoryCreated;
            DeletingDestinationDirectory = deletingDirectory;
            DestinationDirectoryDeleted = directoryDeleted;
            RequestCopyMode = requestCopyMode;
        }

        public override void InitCrawl()
        {
            if (DestinationRoot.Exists)
            {
                var dirArgs = new DirectoryArgs<DirectoryPath>(Root, 0, this, dryRun: Options.DryRun);
                if (DestinationRoot.GetFiles().Any() || DestinationRoot.GetDirectories().Any())
                {
                    // does copy mode need to be set?
                    if (!((CopyOptions)Options).CopyMode.HasValue)
                    {
                        // ask the user to choose one
                        RequestCopyMode?.Invoke((CopyOptions)Options);
                    }

                    // handle root directory
                    switch (((CopyOptions)Options).CopyMode)
                    {
                        case CopyMode.Normal:
                        case null:
                            throw new Exception("Directory already exists and is not empty: " + DestinationRoot);
                        case CopyMode.Overwrite:
                        case CopyMode.OverwriteIfNewer:
                            if (((CopyOptions)Options).DeleteDestinationsWithNoMatchingSources)
                            {
                                var fileNames = (Options.FileSearchPattern == null
                                    ? Root.GetFiles()
                                    : Root.GetFiles(Options.FileSearchPattern)).Select(f => f.Name);
                                var destinationFiles = Options.FileSearchPattern == null
                                    ? DestinationRoot.GetFiles()
                                    : DestinationRoot.GetFiles(Options.FileSearchPattern);
                                var directoryNames = Root.GetDirectories().Select(d => d.Name); ;
                                var destinationDirectories = DestinationRoot.GetDirectories();

                                foreach (var destFile in destinationFiles)
                                {
                                    var fileArgs = new FileArgs<DirectoryPath>(destFile, dirArgs.Level + 1, dirArgs.DirectoryCrawler, dryRun: Options.DryRun);
                                    if (!fileNames.Contains(destFile.Name))
                                    {
                                        try
                                        {
                                            DeletingDestinationFile?.Invoke(fileArgs);
                                            if (!Options.DryRun)
                                            {
                                                destFile.Delete();
                                            }
                                            DestinationFileDeleted?.Invoke(fileArgs);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (Options.ReportErrorsAndContinue)
                                            {
                                                var fileArgsEx = new FileArgs<DirectoryPath>(destFile, dirArgs.Level + 1, this, dryRun: Options.DryRun, exception: ex);
                                                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.FileErrored, dirArgs, fileArgsEx, stats: Statistics);
                                            }
                                            else throw ex;
                                        }
                                    }
                                }

                                foreach (var destDir in destinationDirectories)
                                {
                                    if (!directoryNames.Contains(destDir.Name))
                                    {
                                        try
                                        {
                                            new RecursiveDelete
                                            (
                                                destDir,
                                                deletingFile: DeletingDestinationFile,
                                                fileDeleted: DestinationFileDeleted,
                                                deletingDirectory: DeletingDestinationDirectory,
                                                directoryDeleted: DestinationDirectoryDeleted,
                                                options: new CrawlOptions { DryRun = Options.DryRun }
                                            ).Go();
                                        }
                                        catch (Exception ex)
                                        {
                                            if (Options.ReportErrorsAndContinue)
                                            {
                                                var dirArgsEx = new DirectoryArgs<DirectoryPath>(destDir, 0, this, dryRun: Options.DryRun, exception: ex);
                                                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.FileErrored, dirArgsEx, null, stats: Statistics);
                                            }
                                            else throw ex;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                var destDirArgs = new DirectoryArgs<DirectoryPath>(DestinationRoot, 0, this, dryRun: Options.DryRun);
                CreatingDestinationRootDirectory?.Invoke(destDirArgs);
                if (!Options.DryRun)
                {
                    DestinationRoot.Create();
                }
                DestinationRootDirectoryCreated?.Invoke(destDirArgs);
            }
        }
    }
}

