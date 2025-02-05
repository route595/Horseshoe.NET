using System;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Everything needed to perform a standard criteria comparison bundled into a single class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Criterinator<T> : ICriterinator<T>
    {
        /// <summary>
        /// The criteria to compare against.
        /// </summary>
        public Func<T, bool> Criteria { get; set; }

        /// <summary>
        /// Indicates whether the input item is a criteria match.
        /// </summary>
        /// <param name="inputItem">An instance of <c>T</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool IsMatch(T inputItem)
        {
            return Criteria.Invoke(inputItem);
        }
    }
}
