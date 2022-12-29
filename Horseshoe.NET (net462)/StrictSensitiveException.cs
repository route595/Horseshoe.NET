using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// As specialized, inheritable exception that when caught can be ignored if 
    /// <c>IsStrictSensitive == true</c> and the caller can choose to switch strictness off.
    /// </summary>
    public class StrictSensitiveException : Exception
    {
        /// <summary>
        /// If <c>true</c> then this exception can be ignored in situations where caller can choose to switch strictness off.
        /// </summary>
        public bool IsStrictSensitive { get; set; }

        /// <summary>
        /// Creates new <c>StrictSensitiveException</c>.
        /// </summary>
        public StrictSensitiveException() : base() { }

        /// <summary>
        /// Creates new <c>StrictSensitiveException</c>.
        /// </summary>
        public StrictSensitiveException(string message) : base(message) { }

        /// <summary>
        /// Creates new <c>StrictSensitiveException</c>.
        /// </summary>
        public StrictSensitiveException(string message, Exception innerException) : base(message, innerException) { }
    }
}
