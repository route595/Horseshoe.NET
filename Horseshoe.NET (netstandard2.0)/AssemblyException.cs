using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for assembly load errors.
    /// </summary>
    public class AssemblyException : Exception
    {
        /// <summary>
        /// Creates a new <c>AssemblyException</c>.
        /// </summary>
        public AssemblyException() { }

        /// <summary>
        /// Creates a new <c>AssemblyException</c>.
        /// </summary>
        public AssemblyException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>AssemblyException</c>.
        /// </summary>
        public AssemblyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
