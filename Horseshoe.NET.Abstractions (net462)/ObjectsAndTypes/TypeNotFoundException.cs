using System;

namespace Horseshoe.NET.ObjectsAndTypes
{
    /// <summary>
    /// A specialized exception for errors in object property mapping
    /// </summary>
    public class TypeNotFoundException : StrictSensitiveException
    {
        /// <summary>
        /// Creates a new <c>TypeNotFoundException</c>
        /// </summary>
        public TypeNotFoundException() : base() { }

        /// <summary>
        /// Creates a new <c>TypeNotFoundException</c>
        /// </summary>
        /// <param name="message">A message</param>
        public TypeNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>TypeNotFoundException</c>
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="innerException">An inner exception</param>
        public TypeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
