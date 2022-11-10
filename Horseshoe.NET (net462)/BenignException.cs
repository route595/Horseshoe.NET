using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// An exception that is caught promptly and quietly, client code is never aware.  Used for control flow and messaging between classes.
    /// </summary>
    public class BenignException : Exception
    {
        public BenignException() : this("(control flow / message delivery)") { }
        public BenignException(string message) : base(message) { }
    }
}
