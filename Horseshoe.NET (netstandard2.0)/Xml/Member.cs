namespace Horseshoe.NET.Xml
{
    /// <summary>
    /// Represents one of possibly many &lt;member&gt; elements of an XML doc
    /// </summary>
    public abstract class Member
    {
        /// <summary>
        /// The section of the raw "name" attribute after the last dot (.)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Represents the singleton &lt;summary&gt; element of a &lt;member&gt; element 
        /// </summary>
        public string Summary { get; set; } = "";

        /// <summary>
        /// Represents the singleton &lt;remarks&gt; element of a &lt;member&gt; element 
        /// </summary>
        public string Remarks { get; set; } = "";

        /// <summary>
        /// Represents the current type (e.g. class, interface or struct - including enum) or parent type for params, properties, methods and enum values
        /// </summary>
        public virtual Type MemberType { get; }

        /// <summary>
        /// Called by <c>Member</c> subclass constructors supplying <c>name</c> only
        /// </summary>
        /// <param name="name">e.g. class or interface name</param>
        public Member(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Each <c>Member</c> subclass calculates its original, fully qualified name (does not include member type indicator)
        /// </summary>
        /// <returns>the restored fully qualified name</returns>
        public abstract string ToOriginalString();

        /// <summary>
        /// Returns a string representation of this <c>Member</c>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name.ToLower() + ": " + ToOriginalString();
        }
    }
}
