using Horseshoe.NET.Text;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents the singleton XML &lt;assembly&gt; element
    /// </summary>
    public class Assembly
    {
        /// <summary>
        /// Represents the singleton &lt;name&gt; element of the &lt;assembly&gt; element 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns a string representation of this <c>Assembly</c>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => 
            "assembly: " + TextUtil.Reveal(Name);
    }
}
