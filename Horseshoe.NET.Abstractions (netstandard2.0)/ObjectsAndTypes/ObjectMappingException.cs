using System;
using System.Reflection;
using System.Text;

namespace Horseshoe.NET.ObjectsAndTypes
{
    /// <summary>
    /// A specialized exception for errors in object property mapping
    /// </summary>
    public class ObjectMappingException : Exception
    {
        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        public ObjectMappingException() : base() { }

        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        public ObjectMappingException(Type sourceType, Type destType) : base(BuildMessage(sourceType, destType, null, null)) { }

        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="innerException"></param>
        public ObjectMappingException(Type sourceType, Type destType, Exception innerException) : base(BuildMessage(sourceType, destType, null, innerException?.Message), innerException) { }

        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="message"></param>
        public ObjectMappingException(Type sourceType, Type destType, string message) : base(BuildMessage(sourceType, destType, null, message)) { }

        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ObjectMappingException(Type sourceType, Type destType, string message, Exception innerException) : base(BuildMessage(sourceType, destType, null, message ?? innerException?.Message), innerException) { }


        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="mem"></param>
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem) : base(BuildMessage(sourceType, destType, mem, null)) { }


        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="mem"></param>
        /// <param name="innerException"></param>
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem, Exception innerException) : base(BuildMessage(sourceType, destType, mem, innerException?.Message), innerException) { }

        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="mem"></param>
        /// <param name="message"></param>
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem, string message) : base(BuildMessage(sourceType, destType, mem, message)) { }

        /// <summary>
        /// Creates a new <c>ObjectMappingException</c>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <param name="mem"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem, string message, Exception innerException) : base(BuildMessage(sourceType, destType, mem, message ?? innerException?.Message), innerException) { }
        
        static string BuildMessage(Type sourceType, Type destType, MemberInfo mem, string message)
        {
            var sb = new StringBuilder("Could not map ");
            if (mem != null)
            {
                sb.Append("'" + mem.Name + "' ");
            }
            sb.Append("from " + (sourceType?.FullName ?? "[null]") + " to " + (destType?.FullName ?? "[null]"));
            if (message != null)
            {
                sb.Append(": " + message);
            }
            return sb.ToString();
        }
    }
}
