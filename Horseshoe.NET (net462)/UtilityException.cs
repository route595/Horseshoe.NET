using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A general, catch-all exception for Horseshoe.NET.
    /// </summary>
    public class UtilityException : Exception
    {
        /// <summary>
        /// Creates a new <c>UtilityException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        public UtilityException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>UtilityException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">An inner exception.</param>
        public UtilityException(string message, Exception innerException) : base(message, innerException) { }
    }
}
