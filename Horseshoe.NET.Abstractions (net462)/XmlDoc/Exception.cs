using System.Collections.Generic;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents an exception declaration in <c>Method</c> XML documentation
    /// </summary>
    public class Exception
    {
        /// <summary>
        /// The class reference (type) of exception (from "cref" attribute)
        /// </summary>
        public string Cref { get; } // e.g. "System.NullReferenceException"

        /// <summary>
        /// Exception conditions or other information (from XML text)
        /// </summary>
        public string Description { get; set; } = ""; // e.g. "if any client-supplied arg is null"

        /// <summary>
        /// Create a new <c>Exception</c> XML declaration
        /// </summary>
        /// <param name="cref">class ref</param>
        public Exception(string cref)
        {
            Cref = cref;
        }

        /// <summary>
        /// Create a new <c>Exception</c> XML declaration
        /// </summary>
        /// <param name="cref">class ref</param>
        /// <param name="description">exception conditions or other information</param>
        public Exception(string cref, string description ) : this(cref)
        {
            Description = description;
        }

        /// <summary>
        /// Format this XML doc <c>Exception</c> as text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "exception: " + Cref + (string.IsNullOrEmpty(Description) ? "" : " - " + Description);
        }
    }
}
