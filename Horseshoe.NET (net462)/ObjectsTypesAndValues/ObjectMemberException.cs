using System;
using System.Reflection;
using System.Text;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A specialized exception for errors in object property mapping
    /// </summary>
    public class ObjectMemberException : Exception, IStrictSensitive
    {
        /// <summary>
        /// If <c>true</c> this exception can be either thrown or ignored depending on whether the caller chooses to switch strictness on or off.
        /// </summary>
        public bool IsStrictSensitive { get; set; }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        public ObjectMemberException() : base() { }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        /// <param name="message">A message</param>
        public ObjectMemberException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="innerException">An inner exception</param>
        public ObjectMemberException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="mem"></param>
        public ObjectMemberException(Type type, MemberInfo mem) : base(BuildMessage(type, mem, null)) { }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="mem"></param>
        /// <param name="innerException"></param>
        public ObjectMemberException(Type type, MemberInfo mem, Exception innerException) : base(BuildMessage(type, mem, innerException?.Message), innerException) { }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="mem"></param>
        /// <param name="message"></param>
        public ObjectMemberException(Type type, MemberInfo mem, string message) : base(BuildMessage(type, mem, message)) { }

        /// <summary>
        /// Creates a new <c>ObjectMemberException</c>
        /// </summary>
        /// <param name="type">A reference type</param>
        /// <param name="mem"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ObjectMemberException(Type type, MemberInfo mem, string message, Exception innerException) : base(BuildMessage(type, mem, message ?? innerException?.Message), innerException) { }
        
        static string BuildMessage(Type type, MemberInfo mem, string message)
        {
            var sb = new StringBuilder("Member " + (mem?.Name ?? "[null]") + " of " + (type?.FullName ?? "[null]"));
            if (message != null)
            {
                sb.Append(": " + message);
            }
            return sb.ToString();
        }
    }
}
