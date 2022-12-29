using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for failed assertions.
    /// </summary>
    public class AssertionFailedException : Exception
    {
        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException() : base("Assertion failed") { }

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException(string message) : base("Assertion failed: " + message) { }

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException(string message, Exception innerException) : base("Assertion failed: " + message, innerException) { }
    }
}
