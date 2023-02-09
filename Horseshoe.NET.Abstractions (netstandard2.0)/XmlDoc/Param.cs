namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents a method param or type param
    /// </summary>
    public class Param
    {
        /// <summary>
        /// param name (from "name" attribute)
        /// </summary>
        public string Name { get; } // e.g. "T", "TKey", "TValue"

        /// <summary>
        /// param description (from XML text)
        /// </summary>
        public string Description { get; set; } = ""; // e.g. "Type of item"

        /// <summary>
        /// create a new <c>Param</c> 
        /// </summary>
        /// <param name="name">param name</param>
        public Param(string name)
        {
            Name = name;
        }

        /// <summary>
        /// create a new <c>Param</c> 
        /// </summary>
        /// <param name="name">param name</param>
        /// <param name="description">param description</param>
        public Param(string name, string description) : this(name)
        {
            Description = description;
        }

        /// <summary>
        /// format this <c>Param</c> as text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "param: " + Name + (string.IsNullOrEmpty(Description) ? "" : " - " + Description);
        }
    }
}
