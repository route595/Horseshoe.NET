using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for failed assertions.
    /// </summary>
    public class AssertionFailedException : Exception
    {
        /// <summary>
        /// 'Assertion failed'
        /// </summary>
        public const string MESSAGE_PREFIX = "Assertion failed";

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException() : base(MESSAGE_PREFIX) { }

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException(string message) : base(MESSAGE_PREFIX + ": " + message) { }

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException(string message, Exception innerException) : base(MESSAGE_PREFIX + ": " + message, innerException) { }
    }
}
