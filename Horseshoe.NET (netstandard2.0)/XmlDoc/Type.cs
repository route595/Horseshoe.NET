using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents &lt;member&gt; elements of an XML doc for classes, interfaces and structs including enums
    /// </summary>
    public class Type : Member, IEquatable<Type>
    {
        /// <summary>
        /// The remainder of the raw "name" attribute after extracting the section after the last dot (.)
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Represents the type parameters for classes, interfaces and structs
        /// </summary>
        public IList<Param> TypeParams { get; } = new List<Param>();

        /// <summary>
        /// Represents the current type (e.g. class, interface or struct - including enum) or parent type for properties, methods and enum values
        /// </summary>
        public override Type MemberType => this;

        /// <summary>
        /// Create a new <c>Type</c> (or subclass) supplying <c>namespace</c> and <c>name</c>
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="name"></param>
        public Type(string @namespace, string name) : base(name)
        {
            Namespace = @namespace;
        }

        /// <summary>
        /// A <c>Type</c>'s original, fully qualified name is ['namespace'.]'name' (does not include member type indicator)
        /// </summary>
        /// <returns>the restored fully qualified name</returns>
        public override string ToOriginalString()
        {
            return Namespace == null
                ? Name
                : Namespace + "." + Name;
        }

        /// <summary>
        /// Compares equality of this XmlDoc <c>Type</c> to another object
        /// </summary>
        /// <param name="obj">an object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Type);
        }

        /// <summary>
        /// Compares equality of this XmlDoc <c>Type</c> to another
        /// </summary>
        /// <param name="other">an XmlDoc <c>Type</c> to compare</param>
        /// <returns></returns>
        public bool Equals(Type other)
        {
            return !(other is null) &&
                   Name == other.Name &&
                   Summary == other.Summary &&
                   Remarks == other.Remarks &&
                   Namespace == other.Namespace &&
                   TypeParams.Count == other.TypeParams.Count &&
                   (TypeParams.Any() ? string.Join("", TypeParams) == string.Join("", other.TypeParams) : true);
        }

        /// <summary>
        /// Gets the hash code representation of this XmlDoc <c>Type</c>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 1206494962;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Summary);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Remarks);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Namespace);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<Param>>.Default.GetHashCode(TypeParams);
            return hashCode;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="left">an XmlDoc <c>Type</c></param>
        /// <param name="right">another XmlDoc <c>Type</c></param>
        /// <returns></returns>
        public static bool operator ==(Type left, Type right)
        {
            return EqualityComparer<Type>.Default.Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="left">an XmlDoc <c>Type</c></param>
        /// <param name="right">another XmlDoc <c>Type</c></param>
        /// <returns></returns>
        public static bool operator !=(Type left, Type right)
        {
            return !(left == right);
        }
    }
}
