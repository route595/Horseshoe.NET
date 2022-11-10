using System;
using System.Collections.Generic;

using Horseshoe.NET.IO;
using Horseshoe.NET.IO.DirectoryCrawler;

namespace Horseshoe.NET.Crypto
{
    public class RecursiveHash : DirectoryCrawler<string>
    {
        private readonly List<string> hashes = new List<string>();

        public IEnumerable<string> Hashes => hashes;

        private HashOptions HashOptions { get; }

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

        public override void InitCrawl()
        {
            hashes.Clear();
        }

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
