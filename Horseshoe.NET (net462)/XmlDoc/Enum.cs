using System.Collections.Generic;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents a documented <c>Enum</c>
    /// </summary>
    public class Enum : Type
    {
        /// <summary>
        /// Represents documented <c>Enum</c> values
        /// </summary>
        public IList<EnumValue> Values { get; } = new List<EnumValue>();

        /// <summary>
        /// Create a new <c>Enum</c> XmlDoc object
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="name"></param>
        public Enum(string @namespace, string name) : base(@namespace, name)
        {
        }
    }
}
