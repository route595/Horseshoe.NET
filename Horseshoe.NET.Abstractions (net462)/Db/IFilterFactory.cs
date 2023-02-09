using System;
using System.Collections.Generic;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in daisychaining filter builders.
    /// </summary>
    public interface IFilterFactory
    {
        /// <summary>
        /// Creates a new filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="mode">Compare mode.</param>
        /// <param name="criteria">Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        IFilter Build(string expression, CompareMode mode, ObjectValues criteria);

        /// <summary>
        /// Creates a new filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="mode">Compare mode.</param>
        /// <param name="criteria">Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        IFilter Build<T>(string expression, CompareMode mode, IList<T> criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="mode">Compare mode.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        IFilter Build<T>(string expression, CompareMode mode, T criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Equals' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter Equals<T>(string expression, T criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Contains' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter Contains(string expression, string criteria);

        /// <summary>
        /// Creates a new 'Starts With' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter StartsWith(string expression, string criteria);

        /// <summary>
        /// Creates a new 'Ends With' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter EndsWith(string expression, string criteria);

        /// <summary>
        /// Creates a new 'Greater Than' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter GreaterThan<T>(string expression, T criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Greater Than or Equals' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter GreaterThanOrEquals<T>(string expression, T criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Less Than' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter LessThan<T>(string expression, T criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Less Than or Equals' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter LessThanOrEquals<T>(string expression, T criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'In' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="criteria">Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter In<T>(string expression, IList<T> criteria) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Between' filter that includes the high and low values.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter Between<T>(string expression, T loValue, T hiValue) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Between' filter that excludes the high and low values.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter BetweenExclusive<T>(string expression, T loValue, T hiValue) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Between' filter that excludes only the low value.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter BetweenExclusiveLo<T>(string expression, T loValue, T hiValue) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Between' filter that excludes only the high value.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        IFilter BetweenExclusiveHi<T>(string expression, T loValue, T hiValue) where T : IComparable<T>;

        /// <summary>
        /// Creates a new 'Is Null' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        IFilter IsNull(string expression);

        /// <summary>
        /// Creates a new 'Is Null or Whitespace' filter.
        /// </summary>
        /// <param name="expression">A SQL expression.</param>
        IFilter IsNullOrWhitespace(string expression);

        /// <summary>
        /// Creates a specialized filter that contains other filters all of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'And' group filter.</returns>
        IFilter And(params IFilter[] filters);

        /// <summary>
        /// Creates a specialized filter that contains other filters all of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'And' group filter.</returns>
        IFilter And(IEnumerable<IFilter> filters);

        /// <summary>
        /// Creates a specialized filter that contains other filters only one of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'Or' group filter.</returns>
        IFilter Or(params IFilter[] filters);

        /// <summary>
        /// Creates a specialized filter that contains other filters only one of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'Or' group filter.</returns>
        IFilter Or(IEnumerable<IFilter> filters);

        /// <summary>
        /// Creates a filter that negates another filter.
        /// </summary>
        /// <param name="filter">The filter to negate.</param>
        /// <returns>A negated filter.</returns>
        IFilter Not(IFilter filter);
    }
}
