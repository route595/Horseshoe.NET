using System;

namespace Horseshoe.NET.Jwt
{
    public class JwtException : Exception
    {
        public JwtException(string message) : base(message) { }
        public JwtException(string message, Exception innerException) : base(message, innerException) { }
    }
}
