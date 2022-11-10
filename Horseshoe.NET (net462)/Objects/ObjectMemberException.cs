using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Objects
{
    public class ObjectMemberException : Exception
    {
        public ObjectMemberException() : base() { }
        public ObjectMemberException(string message) : base(message) { }
        public ObjectMemberException(string message, Exception innerException) : base(message, innerException) { }
        public ObjectMemberException(Type type, MemberInfo mem) : base(BuildMessage(type, mem, null)) { }
        public ObjectMemberException(Type type, MemberInfo mem, Exception innerException) : base(BuildMessage(type, mem, innerException?.Message), innerException) { }
        public ObjectMemberException(Type type, MemberInfo mem, string message) : base(BuildMessage(type, mem, message)) { }
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
