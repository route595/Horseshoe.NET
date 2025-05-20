using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// A group of <c>ICriterinator</c>s which matches only if all <c>ICriterinator</c>s match
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compare</typeparam>
    public class Not<T> : ICriterinator<T> where T : IComparable<T>
    {
        private ICriterinator<T> Criterinator { get; }

        /// <summary>
        /// Creates a new 'Not' <c>criterinator</c>
        /// </summary>
        /// <param name="criterinator">The constituent <c>ICriterinator</c></param>
        public Not(ICriterinator<T> criterinator)
        {
            Criterinator = criterinator;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>Criterinator</c>s is not a match
        /// </summary>
        /// <param name="inputItem">An item to compare agains the criteria</param>
        /// <returns></returns>
        public bool IsMatch(T inputItem)
        {
            return !Criterinator.IsMatch(inputItem); 
        }
    }
}
