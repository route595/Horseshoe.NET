using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    /// <summary>
    /// An exception used in instances of parsing values or data from <c>string</c> sources
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Creates a new <c>ParseException</c>
        /// </summary>
        /// <param name="message">An error message</param>
        public ParseException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>ParseException</c>
        /// </summary>
        /// <param name="message">An error message</param>
        /// <param name="innerException">The causing exception</param>
        public ParseException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
