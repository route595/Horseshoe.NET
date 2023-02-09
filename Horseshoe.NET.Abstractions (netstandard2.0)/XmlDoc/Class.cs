using System.Collections.Generic;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents <c>Class</c> documentation
    /// </summary>
    public class Class : Type
    {
        /// <summary>
        /// The &lt;property&gt; elements
        /// </summary>
        public IList<Property> Properties { get; } = new List<Property>();

        /// <summary>
        /// The &lt;functions&gt; elements
        /// </summary>
        public IList<Method> Functions { get; } = new List<Method>();

        /// <summary>
        /// Create a new <c>Class</c>
        /// </summary>
        /// <param name="namespace">a namespace</param>
        /// <param name="name">the type name</param>
        public Class(string @namespace, string name) : base(@namespace, name)
        {
        }
    }
}
