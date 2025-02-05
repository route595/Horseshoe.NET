using System;

using Horseshoe.NET.Text;

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
        public AssertionFailedException() : this(TextConstants.Null) { }

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException(string message) : base(AssertionConstants.EXCEPTION_MESSAGE_PREFIX + message) { }

        /// <summary>
        /// Creates a new <c>AssertionFailedException</c>.
        /// </summary>
        public AssertionFailedException(string message, Exception innerException) : base(AssertionConstants.EXCEPTION_MESSAGE_PREFIX + message, innerException) { }
    }
}
