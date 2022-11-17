using System;
using System.Collections.Generic;

using Horseshoe.NET.IO;
using Horseshoe.NET.IO.DirectoryCrawler;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A subclass of <see cref="DirectoryCrawler"/> aimed at calculating a hash of an entire directory by hashing the hashes of every file inside
    /// </summary>
    public class RecursiveHash : DirectoryCrawler<string>
    {
        private readonly List<string> hashes = new List<string>();

        /// <summary>
        /// A reference to the internal hash collection
        /// </summary>
        public IEnumerable<string> Hashes => hashes;

        private HashOptions HashOptions { get; }

        /// <summary>
        /// Creates a new instance of <c>RecursiveHash</c>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="directoryCrawled"></param>
        /// <param name="fileHashed"></param>
        /// <param name="hashOptions"></param>
        /// <param name="crawlOptions"></param>
        public RecursiveHash
        (
            DirectoryPath root, 
            DirectoryCrawledEvent<string> directoryCrawled = null,
            Action<FilePath, string> fileHashed = null,
            //Action<string> allFileHashed = null,  /* crawl complete */
            HashOptions hashOptions = null,
            CrawlOptions crawlOptions = null
        ) : base
        (
            root,
            directoryCrawled: (eventType, dirArgs, fileArgs, stats) =>
            {
                switch (eventType)
                {
                    case DirectoryCrawlEventType.FileFound:
                        var hashes = ((RecursiveHash)fileArgs.DirectoryCrawler).hashes;  // long way to get a reference to local field "hashes"
                        var hash = Hash.File(fileArgs.File, options: hashOptions);
                        hashes.Add(hash);
                        fileHashed?.Invoke(fileArgs.File, hash);
                        break;
                }
                directoryCrawled?.Invoke(eventType, dirArgs, fileArgs, stats);  /* let client handle same or other event types, optional */
            }, 
            options: crawlOptions
        )
        {
            HashOptions = hashOptions;
        }

        /// <summary>
        /// Overriding the base <c>InitCrawl()</c> method
        /// </summary>
        public override void InitCrawl()
        {
            hashes.Clear();
        }

        /// <summary>
        /// Overriding the base <c>CrawlComplete()</c> method
        /// </summary>
        public override string CrawlComplete()
        {
            switch (hashes.Count)
            {
                case 0:
                    return "";
                case 1:
                    return hashes[0];
                default:
                    return Hash.String(string.Join("", hashes), options: HashOptions);
            }
        }
    }
}
