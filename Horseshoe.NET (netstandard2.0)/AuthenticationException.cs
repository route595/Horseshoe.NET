using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for authentication issues (e.g. Active Directory, OAuth, etc.)
    /// </summary>
    public class AuthenticationException : Exception
    {
        /// <summary>
        /// Creates a new <c>AuthenticationException</c>.
        /// </summary>
        public AuthenticationException() : base("Authentication failed.") { }
 
        /// <summary>
        /// Creates a new <c>AuthenticationException</c>.
        /// </summary>
        public AuthenticationException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>AuthenticationException</c>.
        /// </summary>
        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new <c>AuthenticationException</c>.
        /// </summary>
        public AuthenticationException(Exception innerException) : base("Authentication failed.", innerException) { }
    }
}
