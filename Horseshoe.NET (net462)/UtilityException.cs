using System;

namespace Horseshoe.NET
{
    public class UtilityException : Exception
    {
        public UtilityException(string message) : base(message) { }
        public UtilityException(string message, Exception innerException) : base(message, innerException) { }
    }
}
