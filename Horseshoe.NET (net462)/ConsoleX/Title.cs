using System;
using System.Text;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A title is text rendered to the console with optional 'Xtra' text for providing
    /// additional instructions or context.  Some <c>ConsoleX</c> features add an underline
    /// dashes (-) or equal signs (=) on the line below the title.  By convention, if this
    /// is the case, any 'Xtra' text will not be underlined.
    /// </summary>
    public readonly struct Title
    {
        /// <summary>
        /// The text to display in the title
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Optional extra text providing additional instructions or context
        /// </summary>
        public string Xtra { get; }

        /// <summary>
        /// Create a new <c>Title</c>
        /// </summary>
        /// <param name="text">The title text</param>
        /// <param name="xtra">Optional, extra text providing additional instructions or context</param>
        /// <exception cref="ValidationException"></exception>
        public Title(string text, string xtra = null)
        {
            Text = text ?? throw new ValidationException("Title text may not be null");
            Xtra = xtra;
        }

        /// <summary>
        /// Renders the <c>Title</c> to a <c>string</c>
        /// </summary>
        /// <remarks><seealso cref="Render"/></remarks>
        /// <returns></returns>
        public override string ToString()
        {
            return Render();
        }

        /// <summary>
        /// Renders the title but not the underline.  Other components may or may not render the underline.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            if (Xtra == null) return Text;
            return Text + Xtra;
        }

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Title title2)
            {
                return GetHashCode() == title2.GetHashCode();
            }
            return false;
        }

        /// <summary>
        /// Generates a hash code representation of this <c>Title</c>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return -7183251
                + Text.GetHashCode()
                + (Xtra?.GetHashCode() ?? 7183251);
        }

        /// <summary>
        /// Implictly converts a <c>string</c> to a <c>Title</c>
        /// </summary>
        /// <param name="text"></param>
        public static implicit operator Title(string text) => new Title(text);

        /// <summary>
        /// Implicitly converts a <c>Title</c> back to a <c>string</c>
        /// </summary>
        /// <param name="title"></param>
        public static implicit operator string(Title title) => title.Render();

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="lhs">a <c>Title</c></param>
        /// <param name="rhs">another <c>Title</c></param>
        /// <returns></returns>
        public static bool operator ==(Title lhs, Title rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="lhs">a <c>Title</c></param>
        /// <param name="rhs">another <c>Title</c></param>
        /// <returns></returns>
        public static bool operator !=(Title lhs, Title rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
