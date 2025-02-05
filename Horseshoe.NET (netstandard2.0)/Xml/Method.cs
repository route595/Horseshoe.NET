using System.Collections.Generic;

namespace Horseshoe.NET.Xml
{
    /// <summary>
    /// Represents method documentation
    /// </summary>
    public class Method : Member
    {
        /// <summary>
        /// Represents the current type (e.g. class, interface or struct - including enum) or parent type for properties, methods and enum values
        /// </summary>
        public override Type MemberType { get; }

        /// <summary>
        /// The &lt;typeparam&gt; elements
        /// </summary>
        public IList<Param> TypeParams { get; } = new List<Param>();

        /// <summary>
        /// The &lt;param&gt; elements
        /// </summary>
        public IList<Param> Params { get; } = new List<Param>();

        /// <summary>
        /// The &lt;returns&gt; element
        /// </summary>
        public string Returns { get; set; }

        /// <summary>
        /// The &lt;exception&gt; elements
        /// </summary>
        public IList<Exception> Exceptions { get; } = new List<Exception>();

        /// <summary>
        /// Create a new method
        /// </summary>
        /// <param name="type">this <c>Method</c>'s owning <c>Type</c></param>
        /// <param name="name">method name</param>
        public Method(Type type, string name) : base(name)
        {
            MemberType = type;
        }

        /// <summary>
        /// A <c>Method's</c>'s original, fully qualified name is ['type.namespace'.]'type.name'.'name' (does not include member type indicator)
        /// </summary>
        /// <returns>the restored fully qualified name</returns>
        public override string ToOriginalString()
        {
            return MemberType.ToOriginalString() + "." + Name;
        }
    }
}
