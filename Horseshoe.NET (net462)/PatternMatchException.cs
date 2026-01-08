using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// An exception used in instances when a required pattern match is not successful.
    /// </summary>
    public class PatternMatchException : Exception
    {
        /// <summary>
        /// Creates a new <c>ParseException</c>
        /// </summary>
        /// <param name="message">An error message</param>
        public PatternMatchException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>ParseException</c>
        /// </summary>
        /// <param name="message">An error message</param>
        /// <param name="innerException">The causing exception</param>
        public PatternMatchException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
