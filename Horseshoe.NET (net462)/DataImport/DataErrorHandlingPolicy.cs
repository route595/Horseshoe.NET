namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// How to handle data errors
    /// </summary>
    public enum DataErrorHandlingPolicy
    {
        /// <summary>
        /// Causes an exception to be thrown if an error is identified
        /// </summary>
        Throw,

        /// <summary>
        /// (Popular) Causes data errors to be stored in the <c>DataImport</c> instance
        /// </summary>
        Embed,

        /// <summary>
        /// Ignores any errors and replaces them with blank or <c>null</c>
        /// </summary>
        IgnoreAndUseDefaultValue
    }
}
