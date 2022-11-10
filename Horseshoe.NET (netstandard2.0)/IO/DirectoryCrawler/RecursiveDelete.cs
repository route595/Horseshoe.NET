using System;
using System.IO;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class RecursiveDelete : DirectoryCrawler
    {
        public RecursiveDelete
        (
            DirectoryPath root,
            DirectoryCrawledEvent<DirectoryPath> directoryCrawled = null,
            Action<FileArgs<DirectoryPath>> deletingFile = null,
            Action<FileArgs<DirectoryPath>> fileDeleted = null,
            Action<DirectoryArgs<DirectoryPath>> deletingDirectory = null,
            Action<DirectoryArgs<DirectoryPath>> directoryDeleted = null,
            CrawlOptions options = null
        ) : base
        (
            root,
            directoryCrawled: (eventType, dirArgs, fileArgs, stats) =>
            {
                switch (eventType)
                {
                    case DirectoryCrawlEventType.FileFound:
                        deletingFile?.Invoke(fileArgs);
                        if (!fileArgs.DryRun)
                        {
                            FileInfo fileToDelete = fileArgs.File;
                            fileToDelete.Delete();
                        }
                        fileDeleted?.Invoke(fileArgs);
                        break;
                    case DirectoryCrawlEventType.DirectoryExited:
                        DirectoryInfo directoryToDelete = dirArgs.Directory;
                        deletingDirectory?.Invoke(dirArgs);
                        if (!dirArgs.DryRun)
                        {
                            directoryToDelete.Delete();
                        }
                        directoryDeleted?.Invoke(dirArgs);
                        break;
                }
                directoryCrawled?.Invoke(eventType, dirArgs, fileArgs, stats);
            },
            options: options
        )
        { 
        }
    }
}
