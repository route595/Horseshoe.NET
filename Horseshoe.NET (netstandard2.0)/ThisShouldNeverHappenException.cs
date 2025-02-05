using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception whose sole purpose is to fill in code flow logic gaps 
    /// that should by all counts never get thrown.
    /// </summary>
    public class ThisShouldNeverHappenException : Exception
    {
        /// <summary>
        /// Creates a new <c>ThisShouldNeverHappenException</c>.
        /// </summary>
        public ThisShouldNeverHappenException() : base("This should never happen.") { }

        /// <summary>
        /// Creates a new <c>ThisShouldNeverHappenException</c>.
        /// </summary>
        /// <param name="message">A message to never display.</param>
        public ThisShouldNeverHappenException(string message) : base("This should never happen: " + message) { }
    }
}
