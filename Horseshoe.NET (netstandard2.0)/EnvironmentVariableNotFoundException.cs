using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for missing or invalid envirionment variables.
    /// </summary>
    public class EnvironmentVariableNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new <c>EnvironmentVariableNotFoundException</c>.
        /// </summary>
        public EnvironmentVariableNotFoundException() : base() { }

        /// <summary>
        /// Creates a new <c>EnvironmentVariableNotFoundException</c>.
        /// </summary>
        public EnvironmentVariableNotFoundException(string varName) : base(varName + " not found") { }
    }
}
