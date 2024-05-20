using System;

namespace Horseshoe.NET.ActiveDirectory
{
    public class ADException : Exception
    {
        public ADException(string message) : base(message) { }
        public ADException(string message, Exception innerException) : base(message, innerException) { }
    }
}
