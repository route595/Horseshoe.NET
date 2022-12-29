using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// This <c>DirectoryCrawler</c> traverses a directory and returns the total size of the files within
    /// </summary>
    public class RecursiveSize : DirectoryCrawler<long>
    {
        /// <summary>
        /// Creates a new <c>RecursiveSize</c> file crawler
        /// </summary>
        /// <param name="root"></param>
        /// <param name="directoryCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks.
        /// <example>
        /// Here's an example that prints out all the subdirectories encountered during the 'recursive size' directory crawler operation.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// var totalSize = new RecursiveSize
        /// (
        ///     "C:\myFilesAndFolders",
        ///     directoryCrawled: (@event, dir, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case DirectoryCrawlEvent.DirectoryEntered:
        ///                 subdirectories.Add(dir);
        ///                 break;
        ///         }
        ///     }
        /// ).Go();
        /// Console.WriteLine("Total size: " + FileUtil.GetDisplayFileSize(totalSize));
        /// subdirectories.Sort();
        /// RenderX.List(subdirectories);
        /// </code>
        /// </example>
        /// </param>
        /// <param name="fileCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute file oriented tasks
        /// <example>
        /// Here's an example that lists all files 1 MB or larger encountered during the 'recursive size' directory crawler operation.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// var totalSize = new RecursiveSize
        /// (
        ///     "C:\myFilesAndFolders",
        ///     fileCrawled: (@event, file, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case FileCrawlEvent.FileEncountered:
        ///                 if (file.Size >= 1024000)
        ///                 {
        ///                     largeFiles.Add(file + " (" + FileUtil.GetDisplayFileSize(file.Size) + ")");
        ///                 }
        ///                 break;
        ///         }
        ///     }
        /// ).Go();
        /// Console.WriteLine("Total size: " + FileUtil.GetDisplayFileSize(totalSize));
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </param>
        /// <param name="options">crawl options</param>
        public RecursiveSize
        (
            DirectoryPath root,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<long>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<long>> fileCrawled = null,
            CrawlOptions options = null
        ) : base
        (
            root,
            directoryCrawled: directoryCrawled,
            fileCrawled: fileCrawled,
            options: options
        )
        {
        }

        /// <summary>
        /// Return a final value after the directory crawl operation is complete
        /// </summary>
        /// <returns></returns>
        public override long CrawlComplete()
        {
            return Statistics.SizeOfFilesCrawled;
        }
    }
}

