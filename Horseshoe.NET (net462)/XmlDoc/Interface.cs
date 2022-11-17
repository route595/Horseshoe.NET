using System.Collections.Generic;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents a documented <c>Interface</c>
    /// </summary>
    public class Interface : Type
    {
        /// <summary>
        /// Represents documented <c>Interface</c> properties
        /// </summary>
        public IList<Property> Properties { get; } = new List<Property>();

        /// <summary>
        /// Represents documented <c>Interface</c> methods
        /// </summary>
        public IList<Method> Methods { get; } = new List<Method>();

        /// <summary>
        /// Create a new <c>Interface</c> XmlDoc object
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="name"></param>
        public Interface(string @namespace, string name) : base(@namespace, name)
        {
        }
    }
}
