namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents the <c>XmlDoc</c> nested element currently being processed
    /// </summary>
    public enum NestedDocElement
    {
        /// <summary>
        /// Indicates that no element is currently being processed
        /// </summary>
        None,

        /// <summary>
        /// Indicates that an &lt;exception&gt; element of a &lt;member&gt; element is currently being processed
        /// </summary>
        /// <remarks>has "cref" attribute</remarks>
        Exception,

        /// <summary>
        /// Indicates that the &lt;name&gt; element of the &lt;assembly&gt; element is currently being processed
        /// </summary>
        Name,

        /// <summary>
        /// Indicates that a &lt;param&gt; element of a &lt;member&gt; element is currently being processed
        /// </summary>
        /// <remarks>has "name" attribute</remarks>
        Param,

        /// <summary>
        /// Indicates that the &lt;remarks&gt; element of a &lt;member&gt; element is currently being processed
        /// </summary>
        Remarks,

        /// <summary>
        /// Indicates that the &lt;returns&gt; element of a &lt;member&gt; element is currently being processed
        /// </summary>
        Returns,

        /// <summary>
        /// Indicates that the &lt;summary&gt; element of a &lt;member&gt; element is currently being processed
        /// </summary>
        Summary,

        /// <summary>
        /// Indicates that a &lt;typeparam&gt; element of a &lt;member&gt; element is currently being processed
        /// </summary>
        /// <remarks>has "name" attribute</remarks>
        TypeParam
    }
}
