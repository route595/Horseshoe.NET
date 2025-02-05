using System;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Represents an error condition in <c>Collection</c> comparisons
    /// </summary>
    public class CollectionException : Exception
    {
        /// <summary>
        /// Creates a new <c>CollectionException</c>
        /// </summary>
        /// <param name="message">A message</param>
        public CollectionException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// Creates a new <c>CollectionException</c>
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="innerException">An exception</param>
        public CollectionException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
