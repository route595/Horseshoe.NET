namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// How to handle parse/convert errors
    /// </summary>
    public enum DataErrorHandlingPolicy
    {
        /// <summary>
        /// Causes an exception to be thrown if an error is identified
        /// </summary>
        Throw,

        /// <summary>
        /// Causes errors to be stored in <c>DataImportRow</c> instances
        /// </summary>
        Embed,

        /// <summary>
        /// Ignores any errors and replaces the parsed value with the default
        /// </summary>
        Ignore
    }
}
