namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Used internally for collection comparisons.
    /// </summary>
    public class CollectionUtilMessage : BenignException
    {
        /// <summary>
        /// Refers to exceptional cases of collection pseudoequality e.g. one is null and the other is empty.
        /// </summary>
        public bool IsIdentical_ReturnValue { get; set; }
    }
}
