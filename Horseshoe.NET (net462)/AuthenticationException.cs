using System;

namespace Horseshoe.NET
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base("Authentication failed") { }
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
        public AuthenticationException(Exception innerException) : base("Authentication failed", innerException) { }
    }
}
