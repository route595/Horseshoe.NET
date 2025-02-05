namespace Horseshoe.NET.Xml
{
    /// <summary>
    /// Represents the documentation-oriented XML element currently being processed
    /// </summary>
    public enum DocElement
    {
        /// <summary>
        /// Indicates that no element is currently being processed
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the &lt;assembly&gt; element is currently being processed
        /// </summary>
        Assembly,

        /// <summary>
        /// Indicates that a &lt;member&gt; element is currently being processed
        /// </summary>
        Member
    }
}
