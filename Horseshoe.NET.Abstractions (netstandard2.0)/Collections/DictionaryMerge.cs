using System.Collections.Generic;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Client code uses this during dictionary merges to programmatically choose which value is kept in case identical keys are encountered.
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    /// <param name="leftDict">The target or resulting dictionary</param>
    /// <param name="rightDict">The dictionary being merged</param>
    /// <param name="identicalKey">The duplicate key</param>
    /// <returns>The final value chosen by client code</returns>
    public delegate TValue DictionaryMerge<TKey, TValue>(IDictionary<TKey, TValue> leftDict, IDictionary<TKey, TValue> rightDict, TKey identicalKey);
}
