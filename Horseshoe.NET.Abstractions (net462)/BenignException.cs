using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// An exception that is caught promptly and quietly, client code is never aware.  
    /// Used for control flow and messaging between classes. For better results,
    /// subclass this class for each unique circumstance.
    /// </summary>
    public class BenignException : Exception
    {
        /// <summary>
        /// Creates a new <c>BenignException</c>.
        /// </summary>
        public BenignException() : this("(control flow / message delivery)") { }

        /// <summary>
        /// Creates a new <c>BenignException</c>.
        /// </summary>
        public BenignException(string message) : base(message) { }
    }
}
