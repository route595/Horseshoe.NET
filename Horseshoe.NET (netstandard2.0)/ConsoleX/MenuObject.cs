using System;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX
{
    public abstract class MenuObject : IEquatable<MenuObject>
    {
        /// <summary>
        /// The text to be displayed in a menu or <c>Routine</c> banner
        /// </summary>
        public virtual string Text { get; }

        public MenuObject()
        {
            Text = TextUtil.SpaceOutTitleCase(GetType().Name);
        }

        public MenuObject(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text; 
        }

        public bool Equals(MenuObject other)
        {
            return this == other;
        }
    }
}
