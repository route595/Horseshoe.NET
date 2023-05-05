using System;
using System.Collections.Generic;

using Horseshoe.NET.IO;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A subclass of <see cref="DirectoryCrawler"/> aimed at calculating a hash of an entire directory by hashing the hashes of every file inside
    /// </summary>
    //public class RecursiveHash : DirectoryCrawler<string>
    //{
    //    private readonly List<string> hashes = new List<string>();

    //    /// <summary>
    //    /// A reference to the internal hash collection
    //    /// </summary>
    //    public IEnumerable<string> Hashes => hashes;

    //    private HashOptions HashOptions { get; }

    //    /// <summary>
    //    /// Creates a new instance of <c>RecursiveHash</c>
    //    /// </summary>
    //    /// <param name="root"></param>
    //    /// <param name="directoryCrawled">
    //    /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks.
    //    /// <example>
    //    /// <para>
    //    /// Here's an example that prints out all the subdirectories that were hashed by the directory traversal engine.
    //    /// <code>
    //    /// using Horseshoe.NET.ConsoleX;
    //    /// using Horseshoe.NET.IO.DirectoryCrawler;
    //    /// 
    //    /// var subdirectories = new List&lt;string&gt;();
    //    /// var finalHash = new RecursiveHash
    //    /// (
    //    ///     @"C:\myFilesAndFolders",
    //    ///     directoryCrawled: (@event, dir, metadata) =&gt;
    //    ///     {
    //    ///         switch(@event) 
    //    ///         {
    //    ///             case DirectoryCrawlEvent.DirectoryEntered:
    //    ///                 subdirectories.Add(dir);
    //    ///                 break;
    //    ///         }
    //    ///     }
    //    /// ).Go();
    //    /// Console.WriteLine("Final hash: " + finalHash);
    //    /// subdirectories.Sort();
    //    /// RenderX.List(subdirectories);
    //    /// </code>
    //    /// </para>
    //    /// </example>
    //    /// </param>
    //    /// <param name="fileCrawled">
    //    /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute file oriented tasks.
    //    /// <example>
    //    /// <para>
    //    /// Here's an example that lists all files 1 MB or larger that were hashed by the directory traversal engine.
    //    /// Note: In practice is better to use <c>hashingFile</c> for this task.  See documentation for <c>hashingFile</c>.
    //    /// <code>
    //    /// using Horseshoe.NET.ConsoleX;
    //    /// using Horseshoe.NET.IO;
    //    /// using Horseshoe.NET.IO.DirectoryCrawler;
    //    /// 
    //    /// var largeFiles = new List&lt;string&gt;();
    //    /// var finalHash = new RecursiveHash
    //    /// (
    //    ///     @"C:\myFilesAndFolders",
    //    ///     fileCrawled: (@event, file, metadata) =&gt;
    //    ///     {
    //    ///         switch(@event) 
    //    ///         {
    //    ///             case FileCrawlEvent.FileEncountered:
    //    ///                 if (file.Size >= 1024000)
    //    ///                 {
    //    ///                     largeFiles.Add(file + " (" + FileUtil.GetDisplayFileSize(file.Size) + ")");
    //    ///                 }
    //    ///                 break;
    //    ///         }
    //    ///     },
    //    /// ).Go();
    //    /// Console.WriteLine("Final hash: " + finalHash);
    //    /// largeFiles.Sort();
    //    /// RenderX.List(largeFiles);
    //    /// </code>
    //    /// </para>
    //    /// </example>
    //    /// </param>
    //    /// <param name="fileHashed">
    //    /// An optional, pluggable action to perform when a file is about to be hashed.
    //    /// <example>
    //    /// <para>
    //    /// Here's an example that lists all files greater than 1 MB that were deleted by the directory traversal engine.
    //    /// <code>
    //    /// using Horseshoe.NET.ConsoleX;
    //    /// using Horseshoe.NET.IO;
    //    /// using Horseshoe.NET.IO.DirectoryCrawler;
    //    /// 
    //    /// var largeFiles = new List&lt;string&gt;();
    //    /// var finalHash = new RecursiveHash
    //    /// (
    //    ///     @"C:\myFilesAndFolders",
    //    ///     fileHashed: (@event, file, metadata) =&gt;
    //    ///     {
    //    ///         if (file.Size >= 1024000)
    //    ///         {
    //    ///             largeFiles.Add(file + " (" + FileUtil.GetDisplayFileSize(file.Size) + ")");
    //    ///         }
    //    ///     },
    //    /// ).Go();
    //    /// Console.WriteLine("Final hash: " + finalHash);
    //    /// largeFiles.Sort();
    //    /// RenderX.List(largeFiles);
    //    /// </code>
    //    /// </para>
    //    /// </example>
    //    /// </param>
    //    /// <param name="hashOptions">Hash options.</param>
    //    /// <param name="crawlOptions">Optional traversal engine options.</param>
    //    /// <param name="statistics">Optional traversal engine statistics.</param>
    //    public RecursiveHash
    //    (
    //        DirectoryPath root,
    //        Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<string>> directoryCrawled = null,
    //        Action<FileCrawlEvent, FilePath, FileMetadata<string>> fileCrawled = null,
    //        Action<FilePath, string, FileMetadata<string>> fileHashed = null,
    //        HashOptions hashOptions = null,
    //        CrawlOptions crawlOptions = null,
    //        TraversalStatistics statistics = null
    //    ) : base
    //    (
    //        root,
    //        options: crawlOptions,
    //        statistics: statistics
    //    )
    //    {
    //        HashOptions = hashOptions;
    //        DirectoryCrawled = (@event, dir, metadata) =>
    //        {
    //            directoryCrawled?.Invoke(@event, dir, metadata);  /* let client handle events first, if applicable */
    //            switch (@event)
    //            {
    //                case DirectoryCrawlEvent.OnInit:
    //                    hashes.Clear();
    //                    break;
    //            }
    //        };
    //        FileCrawled = (@event, file, metadata) =>
    //        {
    //            fileCrawled?.Invoke(@event, file, metadata);  /* let client handle events first, if applicable */
    //            switch (@event)
    //            {
    //                case FileCrawlEvent.FileProcessing:
    //                    var hash = Hash.String(file, options: HashOptions);
    //                    hashes.Add(hash);
    //                    fileHashed?.Invoke(file, hash, metadata);
    //                    break;
    //            }
    //        };
    //    }

    //    /// <summary>
    //    /// Overriding the base <c>CrawlComplete()</c> method
    //    /// </summary>
    //    public override string CrawlComplete()
    //    {
    //        switch (hashes.Count)
    //        {
    //            case 0:
    //                return "";
    //            case 1:
    //                return hashes[0];
    //            default:
    //                return Hash.String(string.Join("", hashes), options: HashOptions);
    //        }
    //    }
    //}
}
