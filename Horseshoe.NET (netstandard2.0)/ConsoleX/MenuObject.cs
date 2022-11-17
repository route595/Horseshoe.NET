using System;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// The abstract superclass of all <c>ConsoleX</c> menu items
    /// </summary>
    public abstract class MenuObject : IEquatable<MenuObject>
    {
        /// <summary>
        /// The text to be displayed in a menu or <c>Routine</c> banner
        /// </summary>
        public virtual string Text { get; }

        /// <summary>
        /// Base constructor for menu items
        /// </summary>
        public MenuObject()
        {
            Text = TextUtil.SpaceOutTitleCase(GetType().Name);
        }

        /// <summary>
        /// Base constructor for menu items
        /// </summary>
        /// <param name="text">the text to display in the menu</param>
        public MenuObject(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Display this <c>MenuObject</c>'s <c>Text</c>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text; 
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="other">another <c>MenuObject</c></param>
        /// <returns></returns>
        public bool Equals(MenuObject other)
        {
            return this == other;
        }
    }
}
