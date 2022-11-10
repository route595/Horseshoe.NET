using System;
using System.IO;
using System.Linq;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class RecursiveMove : DirectoryCrawler
    {
        public int RootLength => Root.FullName.Length;
        public DirectoryPath DestinationRoot { get; }
        public Action<FilePath, FilePath> MovingFile { get; }
        public Action<FilePath, FilePath> FileMoved { get; }
        public Action<DirectoryPath> CreatingDestinationRootDirectory { get; }
        public Action<DirectoryPath> DestinationRootDirectoryCreated { get; }
        public Action<DirectoryPath> DeletingSourceRootDirectory { get; }
        public Action<DirectoryPath> SourceRootDirectoryDeleted { get; }

        public RecursiveMove
        (
            DirectoryPath sourceRoot,
            DirectoryPath destinationRoot,
            DirectoryCrawledEvent<DirectoryPath> directoryCrawled = null,
            Action<FilePath, FilePath> processingFile = null,
            Action<FilePath, FilePath, FileProcessedAction> fileProcessed = null,
            Action<DirectoryPath> creatingDestinationRootDirectory = null,
            Action<DirectoryPath> destinationRootDirectoryCreated = null,
            Action<DirectoryPath> deletingSourceRootDirectory = null,
            Action<DirectoryPath> sourceRootDirectoryDeleted = null,
            Action<DirectoryPath, DirectoryPath> creatingDestinationDirectory = null,
            Action<DirectoryPath, DirectoryPath> destinationDirectoryCreated = null,
            Action<DirectoryPath> deletingSourceDirectory = null,
            Action<DirectoryPath> sourceDirectoryDeleted = null,
            CrawlOptions options = null
        ) : base
        (
            sourceRoot,
            directoryCrawled: (eventType, dirArgs, fileArgs, stats) =>
            {
                var fileMove = (RecursiveMove)dirArgs.DirectoryCrawler;
                FilePath destinationFile = Path.Combine(fileMove.DestinationRoot.FullName, fileArgs.File.FullName.Substring(fileMove.RootLength));
                DirectoryPath destinationDirectory = Path.Combine(fileMove.DestinationRoot.FullName, dirArgs.Directory.FullName.Substring(fileMove.RootLength));
                
                switch (eventType)
                {
                    case DirectoryCrawlEventType.FileFound:
                        processingFile?.Invoke(fileArgs.File, destinationFile);
                        if (!fileArgs.DryRun)
                        {
                            fileArgs.File.MoveTo(destinationFile.FullName);
                        }
                        fileProcessed?.Invoke(fileArgs.File, destinationFile, FileProcessedAction.Copied);
                        break;
                    case DirectoryCrawlEventType.DirectoryEntered:
                        creatingDestinationDirectory?.Invoke(dirArgs.Directory, destinationDirectory);
                        if (!dirArgs.DryRun)
                        {
                            destinationDirectory.Create();
                        }
                        destinationDirectoryCreated?.Invoke(dirArgs.Directory, destinationDirectory);
                        break;
                    case DirectoryCrawlEventType.DirectoryExited:
                        deletingSourceDirectory?.Invoke(dirArgs.Directory);
                        if (!dirArgs.DryRun)
                        {
                            dirArgs.Directory.Delete();
                        }
                        sourceDirectoryDeleted?.Invoke(dirArgs.Directory);
                        break;
                }
                directoryCrawled?.Invoke(eventType, dirArgs, fileArgs, stats);
            },
            options: options
        )
        {
            DestinationRoot = destinationRoot;
            CreatingDestinationRootDirectory = creatingDestinationRootDirectory;
            DestinationRootDirectoryCreated = destinationRootDirectoryCreated;
            DeletingSourceRootDirectory = deletingSourceRootDirectory;
            SourceRootDirectoryDeleted = sourceRootDirectoryDeleted;
        }

        public override void InitCrawl()
        {
            if (DestinationRoot.Exists)
            {
                if (DestinationRoot.GetFiles().Any() || DestinationRoot.GetDirectories().Any())
                {
                    throw new Exception("Directory already exists and is not empty: " + DestinationRoot);
                }
            }
            else
            {
                CreatingDestinationRootDirectory?.Invoke(DestinationRoot);
                if (!Options.DryRun)
                {
                    DestinationRoot.Create();
                }
                DestinationRootDirectoryCreated?.Invoke(DestinationRoot);
            }
        }
    }
}

