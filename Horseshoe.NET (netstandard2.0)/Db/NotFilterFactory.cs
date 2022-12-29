using System;
using System.Collections.Generic;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A factory for filters that negate other filters. Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public class NotFilterFactory : IFilterFactory
    {
        /// <inheritdoc />
        public IFilter Build(string expression, CompareMode mode, ObjectValues criteria)
        {
            return new NotFilter(Filter.Build(expression, mode, criteria));
        }

        /// <inheritdoc />
        public IFilter Build<T>(string expression, CompareMode mode, IList<T> criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.Build(expression, mode, criteria));
        }

        /// <inheritdoc />
        public IFilter Build<T>(string expression, CompareMode mode, T criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.Build(expression, mode, criteria));
        }

        /// <inheritdoc />
        public IFilter Equals<T>(string expression, T criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.Equals(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter Contains(string expression, string criteria)
        {
            return new NotFilter(Filter.Contains(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter StartsWith(string expression, string criteria)
        {
            return new NotFilter(Filter.StartsWith(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter EndsWith(string expression, string criteria)
        {
            return new NotFilter(Filter.EndsWith(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter GreaterThan<T>(string expression, T criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.GreaterThan(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter GreaterThanOrEquals<T>(string expression, T criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.GreaterThanOrEquals(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter LessThan<T>(string expression, T criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.LessThan(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter LessThanOrEquals<T>(string expression, T criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.LessThanOrEquals(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter Between<T>(string expression, T loValue, T hiValue) where T : IComparable<T>
        {
            return new NotFilter(Filter.Between(expression, loValue, hiValue));
        }

        /// <inheritdoc />
        public IFilter BetweenExclusive<T>(string expression, T loValue, T hiValue) where T : IComparable<T>
        {
            return new NotFilter(Filter.BetweenExclusive(expression, loValue, hiValue));
        }

        /// <inheritdoc />
        public IFilter BetweenExclusiveLo<T>(string expression, T loValue, T hiValue) where T : IComparable<T>
        {
            return new NotFilter(Filter.BetweenExclusiveLo(expression, loValue, hiValue));
        }

        /// <inheritdoc />
        public IFilter BetweenExclusiveHi<T>(string expression, T loValue, T hiValue) where T : IComparable<T>
        {
            return new NotFilter(Filter.BetweenExclusiveHi(expression, loValue, hiValue));
        }

        /// <inheritdoc />
        public IFilter In<T>(string expression, IList<T> criteria) where T : IComparable<T>
        {
            return new NotFilter(Filter.In(expression, criteria));
        }

        /// <inheritdoc />
        public IFilter IsNull(string expression)
        {
            return new NotFilter(Filter.IsNull(expression));
        }

        /// <inheritdoc />
        public IFilter IsNullOrWhitespace(string expression)
        {
            return new NotFilter(Filter.IsNull(expression));
        }

        /// <inheritdoc />
        public IFilter And(params IFilter[] filters)
        {
            return new NotFilter(Filter.And(filters));
        }

        /// <inheritdoc />
        public IFilter And(IEnumerable<IFilter> filters)
        {
            return new NotFilter(Filter.And(filters));
        }

        /// <inheritdoc />
        public IFilter Or(params IFilter[] filters)
        {
            return new NotFilter(Filter.Or(filters));
        }

        /// <inheritdoc />
        public IFilter Or(IEnumerable<IFilter> filters)
        {
            return new NotFilter(Filter.Or(filters));
        }

        /// <inheritdoc />
        public IFilter Not(IFilter filter)
        {
            throw new ValidationException("A 'Not' filter cannot be directly nested in another 'Not' filter.");
        }
    }
}
