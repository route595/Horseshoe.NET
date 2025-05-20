using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses.
    /// </summary>
    public static class Filter
    {
        /// <summary>
        /// Sets a global DB Provider.
        /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
        /// </summary>
        public static DbProvider? Provider { get; set; }

        /// <summary>
        /// Creates a new column based 'Literal' filter.
        /// </summary>
        /// <param name="literalExpression">The SQL expression to use.</param>
        public static IFilter Literal(string literalExpression) =>
            new LiteralFilter(literalExpression);

        /// <summary>
        /// Creates a specialized filter that contains other filters all of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'And' group filter.</returns>
        public static IGroupFilter And(params IFilter[] filters)
        {
            return new AndGroup(filters);
        }

        /// <summary>
        /// Creates a specialized filter that contains other filters all of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'And' group filter.</returns>
        public static IGroupFilter And(IEnumerable<IFilter> filters)
        {
            return And(filters is IFilter[] array ? array : filters?.ToArray() ?? Array.Empty<IFilter>());
        }

        /// <summary>
        /// Creates a specialized filter that contains other filters only one of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'Or' group filter.</returns>
        public static IGroupFilter Or(params IFilter[] filters)
        {
            return new OrGroup(filters);
        }

        /// <summary>
        /// Creates a specialized filter that contains other filters only one of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'Or' group filter.</returns>
        public static IGroupFilter Or(IEnumerable<IFilter> filters)
        {
            return Or(filters is IFilter[] array ? array : filters?.ToArray() ?? Array.Empty<IFilter>());
        }

        /// <summary>
        /// Creates a filter that negates another filter.
        /// </summary>
        /// <returns>A negated filter.</returns>
        public static IFilter Not(IFilter filter)
        {
            return new NotFilter(filter);
        }

        /// <summary>
        /// Creates a new column-based 'Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criterium">Value from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter Equals(ColumnExpression columnName, object criterium, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? DbSettings.DefaultProvider) +
                " = " +
                DbUtil.Sqlize(criterium, provider: provider ?? DbSettings.DefaultProvider),
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'In' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Values from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter In(ColumnExpression columnName, object[] criteria, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? DbSettings.DefaultProvider) +
                " IN (" +
                string.Join(", ", criteria.Select(c => DbUtil.Sqlize(c, provider: provider ?? DbSettings.DefaultProvider))) +
                ")",
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Contains' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criterium">Value from which to build the filter 'WHERE' expression.</param>
        /// <param name="likeMode">Contains (default), StartsWith or EndsWith.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter Like(ColumnExpression columnName, string criterium, LikeMode likeMode = default, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? DbSettings.DefaultProvider) +
                " LIKE '" +
                (likeMode.In(LikeMode.StartsWith, LikeMode.Contains) ? "%" : "") +
                criterium +
                (likeMode.In(LikeMode.EndsWith, LikeMode.Contains) ? "%" : "") +
                "'",
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Greater Than' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criterium">Value from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter GreaterThan(ColumnExpression columnName, object criterium, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) +
                " > " +
                DbUtil.Sqlize(criterium, provider: provider ?? Provider ?? DbSettings.DefaultProvider),
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Greater Than or Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criterium">Value from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter GreaterThanOrEquals(ColumnExpression columnName, object criterium, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) +
                " >= " +
                DbUtil.Sqlize(criterium, provider: provider ?? Provider ?? DbSettings.DefaultProvider),
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Less Than' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criterium">Value from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter LessThan(ColumnExpression columnName, object criterium, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) +
                " < " +
                DbUtil.Sqlize(criterium, provider: provider ?? Provider ?? DbSettings.DefaultProvider),
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Less Than or Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criterium">Value from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter LessThanOrEquals(ColumnExpression columnName, object criterium, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) +
                " <= " +
                DbUtil.Sqlize(criterium, provider: provider ?? Provider ?? DbSettings.DefaultProvider),
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Less Than or Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="loCriterium">Lowest of a pair of values from which to build the filter 'WHERE' expression.</param>
        /// <param name="hiCriterium">Highest of a pair of values from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter Between(ColumnExpression columnName, object loCriterium, object hiCriterium, DbProvider? provider = null) =>
            new LiteralFilter
            (
                columnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) +
                " BETWEEN " +
                DbUtil.Sqlize(loCriterium, provider: provider ?? Provider ?? DbSettings.DefaultProvider) +
                " AND " +
                DbUtil.Sqlize(hiCriterium, provider: provider ?? Provider ?? DbSettings.DefaultProvider),
                systemGenerated: true
            );

        /// <summary>
        /// Creates a new column-based 'Less Than or Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="loCriterium">Lowest of a pair of values from which to build the filter 'WHERE' expression.</param>
        /// <param name="hiCriterium">Highest of a pair of values from which to build the filter 'WHERE' expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter BetweenExclusive(ColumnExpression columnName, object loCriterium, object hiCriterium, DbProvider? provider = null) =>
            And
            (
                GreaterThan(columnName, loCriterium, provider: provider),
                LessThan(columnName, hiCriterium, provider: provider)
            );

        /// <summary>
        /// Creates a new column-based 'Is Null' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A filter.</returns>
        public static IFilter IsNull(ColumnExpression columnName, DbProvider? provider = null) =>
            new LiteralFilter(columnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) + " IS NULL");
    }
}
