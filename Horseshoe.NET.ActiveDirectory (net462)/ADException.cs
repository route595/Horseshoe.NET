using System;
using System.Diagnostics.CodeAnalysis;

namespace Horseshoe.NET.ActiveDirectory
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "Serialization not a current priority")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Prefer not to have no-args constructor")]
    public class ADException : Exception
    {
        public ADException(string message) : base(message) { }
        public ADException(string message, Exception innerException) : base(message, innerException) { }
    }
}
