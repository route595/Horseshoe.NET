using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// A specialized exception specifically for directory crawl events
    /// </summary>
    public class DirectoryCrawlException : Exception
    {
        /// <summary>
        /// Creates a new <c>DirectoryCrawlException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        public DirectoryCrawlException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>DirectoryCrawlException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">An inner exception.</param>
        public DirectoryCrawlException(string message, Exception innerException) : base(message, innerException) { }
    }
}
