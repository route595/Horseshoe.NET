using System.Reflection;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Objects
{
    public class MemberValue : MemberValue<object>
    {
        public MemberValue(MemberInfo member, object value)
            : base(member, value) { }
    }

    public class MemberValue<T>
    {
        public MemberInfo Member { get; }
        public T Value { get; }

        public MemberValue(MemberInfo member, object value)
        {
            Member = member;
            if (value != null)
            {
                Value = (T)value;
            }
        }

        public override string ToString()
        {
            return Member.Name + " = " + TextUtil.Reveal(Value);
        }
    }
}
