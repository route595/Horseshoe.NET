using System;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// An exception for <c>Type</c>-related issues
    /// </summary>
    public class TypeException : Exception
    {
        /// <summary>
        /// Creates a new <c>TypeException</c>
        /// </summary>
        /// <param name="message">A message</param>
        public TypeException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>TypeException</c>
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="innerException">An inner exception</param>
        public TypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
