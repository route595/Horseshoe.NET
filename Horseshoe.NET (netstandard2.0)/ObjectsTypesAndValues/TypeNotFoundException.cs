using System;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A specialized exception for errors in object property mapping
    /// </summary>
    public class TypeNotFoundException : TypeException, IStrictSensitive
    {
        /// <summary>
        /// If <c>true</c> this exception can be either thrown or ignored depending on whether the caller chooses to switch strictness on or off.
        /// </summary>
        public bool IsStrictSensitive { get; set; }

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
