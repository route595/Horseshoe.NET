using System.Collections.Generic;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents a documented <c>struct</c>
    /// </summary>
    public class Struct : Type
    {
        /// <summary>
        /// Represents documented <c>struct</c> properties
        /// </summary>
        public IList<Property> Properties { get; } = new List<Property>();

        /// <summary>
        /// Represents documented <c>struct</c> methods
        /// </summary>
        public IList<Method> Methods { get; } = new List<Method>();

        /// <summary>
        /// Create a new <c>Struct</c> XmlDoc object
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="name"></param>
        public Struct(string @namespace, string name) : base(@namespace, name)
        {
        }
    }
}
