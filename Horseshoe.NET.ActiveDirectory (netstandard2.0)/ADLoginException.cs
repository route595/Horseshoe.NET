using System;

namespace Horseshoe.NET.ActiveDirectory
{
    public class ADLoginException : Exception
    {
        public ADLoginException(string message) : base(message)
        {
        }
        public ADLoginException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}