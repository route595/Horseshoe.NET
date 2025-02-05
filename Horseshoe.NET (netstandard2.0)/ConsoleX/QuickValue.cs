using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX
{
    public class QuickValue<T>
    {
        public T Value { get; }

        public QuickValue(T value) 
        {
            Value = value; 
        }

        public override string ToString()
        {
            return Value?.ToString() ?? TextConstants.Null;
        }

        public static implicit operator QuickValue<T>(T value) => new QuickValue<T>(value);
    }
}
