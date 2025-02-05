namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Defines how to merge dictionaries in case of duplicate keys.
    /// </summary>
    public enum DictionaryMergeMode
    {
        /// <summary>
        /// Default dictionary merge option i.e. duplicate key naturally causes an exception.
        /// </summary>
        Error,

        /// <summary>
        /// Right-to-left dictionary merge option i.e. last-in duplicate overwrites.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// var englishDict = new Dictionary&lt;int,string&gt; { {1, "one"}, {2, "two"}, {3, "three"} }
        /// var spanishDict = new Dictionary&lt;int,string&gt; { {3, "tres"}, {4, "cuatro"}, {5, "cinco"} }
        /// var merged = DictionaryUtil.Combine
        /// (
        ///     new[] { englishDict, spanishDict }, 
        ///     options: new MergeOptions { Mode = DictionaryMergeMode.RTL }
        /// );
        /// -- or --
        /// var merged = DictionaryUtil.CombineRTL(englishDict, spanishDict);
        /// Console.WriteLine(merged[3]);  // "tres" (overwritten by last-in duplicate)
        /// </code>
        /// </example>
        /// </summary>
        RTL,

        /// <summary>
        /// Left-to-right dictionary merge option i.e. last-in duplicate ignored.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// var englishDict = new Dictionary&lt;int,string&gt; { {1, "one"}, {2, "two"}, {3, "three"} }
        /// var spanishDict = new Dictionary&lt;int,string&gt; { {3, "tres"}, {4, "cuatro"}, {5, "cinco"} }
        /// var merged = DictionaryUtil.Combine
        /// (
        ///     new[] { englishDict, spanishDict }, 
        ///     options: new MergeOptions { Mode = DictionaryMergeMode.LTR }
        /// );
        /// -- or --
        /// var merged = DictionaryUtil.CombineLTR(englishDict, spanishDict);
        /// Console.WriteLine(merged[3]);  // "three" (last-in duplicate was ignored)
        /// </code>
        /// </example>
        /// </summary>
        LTR
    }
}
