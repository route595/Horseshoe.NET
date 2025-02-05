namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// How to handle empty rows
    /// </summary>
    public enum BlankRowPolicy
    {
        /// <summary>
        /// Default.  Allow blank rows to remain in the final data structure
        /// </summary>
        Allow,

        /// <summary>
        /// Preclude blank rows from being added to the data structure
        /// </summary>
        Drop,

        /// <summary>
        /// Preclude leading blank rows from being added to the data structure
        /// </summary>
        DropLeading,

        /// <summary>
        /// Remove trailing blank rows the data structure
        /// </summary>
        DropTrailing,

        /// <summary>
        /// Preclude leading blank rows from being added to the data structure and remove trailing blank rows
        /// </summary>
        DropLeadingAndTrailing,

        /// <summary>
        /// End the import if a blank row is detected
        /// </summary>
        StopImporting,

        /// <summary>
        /// Cause an exception to be thrown if a blank row is detected
        /// </summary>
        Error
    }
}
