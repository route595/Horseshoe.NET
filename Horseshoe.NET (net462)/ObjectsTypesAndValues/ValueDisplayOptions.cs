namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// Settings that determine how certain values are formatted for display.
    /// </summary>
    public class ValueDisplayOptions
    {
        /// <summary>
        /// If <c>true</c>, enum values will be displayed with their type name as a prefix (e.g., "DayOfWeek.Monday").  Default is <c>false</c>.
        /// </summary>
        public bool IncludeEnumTypeName { get; set; }

        /// <summary>
        /// If <c>true</c>, DateTime values will always display the time component, even if it is 00:00:00.  Default is <c>false</c>.
        /// </summary>
        public bool AlwaysShowTimeForDateTime { get; set; }
    }
}
