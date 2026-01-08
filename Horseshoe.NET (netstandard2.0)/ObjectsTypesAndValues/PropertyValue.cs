using System.Reflection;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// Represents an object's property, value included
    /// </summary>
    public class PropertyValue : PropertyValue<object>
    {
        /// <summary>
        /// Creates a new <c>PropertyValue</c>
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public PropertyValue(PropertyInfo property, object value)
            : base(property, value) { }
    }

    /// <summary>
    /// Represents an object's property, value included
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyValue<T>
    {
        /// <summary>
        /// Property info
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Property value
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Creates a new <c>PropertyValue</c>
        /// </summary>
        /// <param name="property">A property</param>
        /// <param name="value">A property value</param>
        public PropertyValue(PropertyInfo property, object value)
        {
            Property = property;
            if (value != null)
            {
                Value = (T)value;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Property.Name + " = " + ValueUtil.Display(Value);
        }
    }
}
