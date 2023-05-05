using System;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Everything needed to perform a standard criteria comparison bundled into a single class.
    /// </summary>
    public static class Criterinator
    {
        /// <summary>
        /// Factory methods for validating, building and running <c>Comparator</c> instances.
        /// </summary>
        /// <typeparam name="T">A type.</typeparam>
        /// <param name="criteria">A function that accepts an instance of <c>T</c> and returns <c>true</c> or <c>false</c>.</param>
        /// <returns></returns>
        public static ICriterinator<T> Build<T>(Func<T, bool> criteria) =>
            new Criterinator<T> { Criteria = criteria };
    }
}
