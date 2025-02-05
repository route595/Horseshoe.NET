namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Options for merging dictionaries in case identical keys are encountered.
    /// </summary>
    public class MergeOptions<TKey, TValue> : MergeOptions
    {
        /// <summary>
        /// Client code can supply this to programmatically choose which value is kept in case identical keys are encountered.
        /// </summary>
        public DictionaryMerge<TKey, TValue> CustomMerge { get; set; }
    }

    /// <summary>
    /// Options for merging dictionaries with identical keys
    /// </summary>
    public class MergeOptions
    {
        /// <summary>
        /// Defines how to merge dictionaries in case of duplicate keys.
        /// </summary>
        public DictionaryMergeMode Mode { get; set; }

        /// <summary>
        /// Augment and return original dictionaries, when possible, to minimize collection 
        /// instantiation and subsequent garbage collection. Applies only to <c>Combine()</c>.
        /// </summary>
        public bool Optimize { get; set; }
    }
}
