using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Client code uses this during dictionary merges to programmatically choose which value is kept in case identical keys are encountered.
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    /// <param name="duplicateKey">The duplicate key</param>
    /// <param name="leftValue">The left dictionary's value of the duplicate key</param>
    /// <param name="rightValue">The right dictionary's value of the duplicate key</param>
    /// <param name="leftDict">The target or resulting dictionary</param>
    /// <param name="rightDict">The dictionary being merged</param>
    /// <returns>The final value chosen by client code</returns>
    public delegate TValue DictionaryMerge<TKey, TValue>(TKey duplicateKey, TValue leftValue, TValue rightValue, IEnumerable<KeyValuePair<TKey, TValue>> leftDict, IEnumerable<KeyValuePair<TKey, TValue>> rightDict);
}
