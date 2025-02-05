using System;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// A specialized exception for comparison operations.  
    /// Not used extensively in the Horseshoe.NET.Compare classes but available for public use.
    /// </summary>
    public class CompareException : Exception
    {
        /// <summary>
        /// Creates a new <c>CompareException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        public CompareException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>CompareException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="ex">An inner exception.</param>
        public CompareException(string message, Exception ex) : base(message, ex) { }
    }
}
