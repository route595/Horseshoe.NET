namespace Horseshoe.NET.Xml
{
    /// <summary>
    /// Represents a property of a class, interface or struct
    /// </summary>
    public class Property : Member
    {
        /// <summary>
        /// Represents the current type (e.g. class, interface or struct - including enum) or parent type for properties, methods and enum values
        /// </summary>
        public override Type MemberType { get; }

        /// <summary>
        /// Create a new <c>Property</c>
        /// </summary>
        /// <param name="type">owning type (e.g. class, struct or interface)</param>
        /// <param name="name">property name</param>
        public Property(Type type, string name) : base(name)
        {
            MemberType = type;
        }

        /// <summary>
        /// A <c>Property's</c>'s original, fully qualified name is ['type.namespace'.]'type.name'.'name' (does not include member type indicator)
        /// </summary>
        /// <returns>the restored fully qualified name</returns>
        public override string ToOriginalString()
        {
            return MemberType.ToOriginalString() + "." + Name;
        }
    }
}
