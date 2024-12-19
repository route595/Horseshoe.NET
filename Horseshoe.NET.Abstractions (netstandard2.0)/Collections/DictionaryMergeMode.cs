namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Defines how to merge dictionaries in case of duplicate keys.
    /// </summary>
    public enum DictionaryMergeMode
    {
        /// <summary>
        /// Right-to-left a.k.a. last in (default mode).  In case of key equality, the rightmost dictionary's value overwrites.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// var englishDict = new Dictionary&lt;int,string&gt; { {1, "one"}, {2, "two"}, {3, "three"} }
        /// var spanishDict = new Dictionary&lt;int,string&gt; { {3, "tres"}, {4, "cuatro"}, {5, "cinco"} }
        /// var merged = DictionaryUtil.Combine(new[] { englishDict, spanishDict });    // DictionaryMergeMode.LI is assumed
        /// Console.WriteLine(merged[3]);  // value is "tres" (rightmost dictionary's value for key = 3)
        /// </code>
        /// </example>
        /// </summary>
        RTL,

        /// <summary>
        /// Left-to-right a.k.a. first in.  In case of key equality, the leftmost dictionary's value is kept.
        /// <example>
        /// <para>Example</para>
        /// <code>
        /// var englishDict = new Dictionary&lt;int,string&gt; { {1, "one"}, {2, "two"}, {3, "three"} }
        /// var spanishDict = new Dictionary&lt;int,string&gt; { {3, "tres"}, {4, "cuatro"}, {5, "cinco"} }
        /// var merged = DictionaryUtil.Combine(new[] { englishDict, spanishDict }, mode: DictionaryMergeMode.FI);
        /// Console.WriteLine(merged[3]);  // value is "three" (leftmost dictionary's value for key = 3)
        /// </code>
        /// </example>
        /// </summary>
        LTR
    }
}
