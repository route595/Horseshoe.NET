using System;
using System.Text;

namespace Horseshoe.NET.ConsoleX
{
    public struct Title
    {
        public string Text { get; }
        public string Xtra { get; }

        public Title(string text, string xtra = null)
        {
            Text = text ?? throw new ValidationException("Title text may not be null");
            Xtra = xtra;
        }

        public override string ToString()
        {
            return Render();
        }

        public string Render()
        {
            if (Xtra == null) return Text;
            return Text + Xtra;
        }

        public override bool Equals(object obj)
        {
            if (obj is Title title2)
            {
                return GetHashCode() == title2.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return -7183251
                + (Text?.GetHashCode() ?? 7183251)
                + (Xtra?.GetHashCode() ?? 7183251);
        }

        public static implicit operator Title(string text) => new Title(text);
        public static implicit operator string(Title title) => title.Render();
        public static bool operator ==(Title lhs, Title rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Title lhs, Title rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
