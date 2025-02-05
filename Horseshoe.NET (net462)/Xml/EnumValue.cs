namespace Horseshoe.NET.Xml
{
    /// <summary>
    /// Represents a documented Enum value
    /// </summary>
    public class EnumValue : Member
    {
        /// <summary>
        /// Represents the current type (e.g. class, interface or struct - including enum) or parent type for properties, methods and enum values
        /// </summary>
        public override Type MemberType { get; }

        /// <summary>
        /// Create a new <c>EnumValue</c>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public EnumValue(Type type, string name) : base(name)
        {
            MemberType = type;
        }

        /// <summary>
        /// An <c>EnumValue</c>'s original, fully qualified name is ['type.namespace'.]'type.name'.'name' (does not include member type indicator)
        /// </summary>
        /// <returns>the restored fully qualified name</returns>
        public override string ToOriginalString()
        {
            return MemberType.ToOriginalString() + "." + Name;
        }
    }
}
