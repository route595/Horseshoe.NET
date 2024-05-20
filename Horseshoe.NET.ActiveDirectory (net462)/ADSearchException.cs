using System;

namespace Horseshoe.NET.ActiveDirectory
{
    public class ADSearchException : Exception
    {
        public ADSearchException(string message) : base(message)
        {
        }
        public ADSearchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}