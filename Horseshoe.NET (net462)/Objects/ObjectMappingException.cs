using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Objects
{
    public class ObjectMappingException : Exception
    {
        public ObjectMappingException() : base() { }
        public ObjectMappingException(Type sourceType, Type destType) : base(BuildMessage(sourceType, destType, null, null)) { }
        public ObjectMappingException(Type sourceType, Type destType, Exception innerException) : base(BuildMessage(sourceType, destType, null, innerException?.Message), innerException) { }
        public ObjectMappingException(Type sourceType, Type destType, string message) : base(BuildMessage(sourceType, destType, null, message)) { }
        public ObjectMappingException(Type sourceType, Type destType, string message, Exception innerException) : base(BuildMessage(sourceType, destType, null, message ?? innerException?.Message), innerException) { }
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem) : base(BuildMessage(sourceType, destType, mem, null)) { }
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem, Exception innerException) : base(BuildMessage(sourceType, destType, mem, innerException?.Message), innerException) { }
        public ObjectMappingException(Type sourceType, Type destType, MemberInfo mem, string message) : base(BuildMessage(sourceType, destType, mem, message)) { }
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
