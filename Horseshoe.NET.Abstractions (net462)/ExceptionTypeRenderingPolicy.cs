namespace Horseshoe.NET
{
    /// <summary>
    /// Defines preferences for <c>Exception</c> rendering, specifically the exception <c>Type</c>
    /// </summary>
    public enum ExceptionTypeRenderingPolicy
    {
        /// <summary>
        /// Render the fully qualified type name
        /// </summary>
        Fqn,

        /// <summary>
        /// Render the fully qualified type name except for 'System' exceptions which are common
        /// </summary>
        FqnExceptSystem,

        /// <summary>
        /// Render only the short type name
        /// </summary>
        NameOnly
    }
}
