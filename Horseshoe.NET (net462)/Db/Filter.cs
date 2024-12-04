using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Compare;
using Horseshoe.NET.Primitives;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses.
    /// </summary>
    public class Filter : IFilter
    {
        /// <summary>
        /// A column name or expression.
        /// </summary>
        public ColumnExpression ColumnName { get; set; }

        /// <summary>
        /// A compare mode.
        /// </summary>
        public CompareMode Mode { get; set; }

        /// <summary>
        /// Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.  Value(s) against which to compare SQL data during 'SELECT' queries or 'UPDATE' or 'DELETE' operations.
        /// </summary>
        public ObjectValues Criteria { get; set; }

        /// <summary>
        /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
        /// </summary>
        public DbProvider? Provider { get; set; }

        /// <summary>
        /// Validates whether search criteria is valid.
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">The criteria value(s).</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public static void AssertCriteriaIsValid(CompareMode mode, ObjectValues criteria)
        {
            AssertCriteriaIsValid(mode, criteria, out _);
        }

        /// <summary>
        /// Validates whether search criteria is valid.
        /// </summary>
        /// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        /// <param name="criteria">The criteria value(s).</param>
        /// <param name="vAction">Alerts client code to perform the action identified by the validator, if any (for example, when the between hi and lo criteria are switched).</param>
        /// <exception cref="AssertionFailedException"></exception>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public static void AssertCriteriaIsValid(CompareMode mode, ObjectValues criteria, out ValidationFlaggedAction vAction)
        {
            switch (mode)
            {
                case CompareMode.Regex:
                    throw new AssertionFailedException("This compare mode is invalid in DB filters: " + CompareMode.Regex);
            }
            Comparator.AssertCriteriaIsValid(mode, criteria, out vAction);
        }

        /// <summary>
        /// Renders the filter as a SQL expression.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL expression.</returns>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public virtual string Render(DbProvider? provider = null)
        {
            var columnExpression = ColumnName.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider);
            switch (Mode)
            {
                case CompareMode.Equals:
                    return columnExpression + " = " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider);
                case CompareMode.Contains:
                    return columnExpression + " LIKE '%" + Criteria[0] + "%'";
                case CompareMode.StartsWith:
                    return columnExpression + " LIKE '" + Criteria[0] + "%'";
                case CompareMode.EndsWith:
                    return columnExpression + " LIKE '%" + Criteria[0] + "'";
                case CompareMode.GreaterThan:
                    return columnExpression + " > " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider);
                case CompareMode.GreaterThanOrEquals:
                    return columnExpression + " >= " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider);
                case CompareMode.LessThan:
                    return columnExpression + " < " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider);
                case CompareMode.LessThanOrEquals:
                    return columnExpression + " <= " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider);
                case CompareMode.In:
                    return Criteria.Count == 0
                        ? "1 = 0"
                        : columnExpression + " IN ( " + string.Join(", ", Criteria.Select(val => DbUtil.Sqlize(val, provider: provider ?? Provider ?? DbSettings.DefaultProvider))) + " )";
                case CompareMode.Between:
                    return columnExpression + " BETWEEN " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider) + " AND " + DbUtil.Sqlize(Criteria[1], provider: provider ?? Provider ?? DbSettings.DefaultProvider);
                case CompareMode.BetweenExclusive:
                    return "( " + columnExpression + " > " + DbUtil.Sqlize(Criteria[0], provider: provider ?? Provider ?? DbSettings.DefaultProvider) + " AND " + columnExpression + " < " + DbUtil.Sqlize(Criteria[1], provider: provider ?? Provider ?? DbSettings.DefaultProvider) + " )";
                case CompareMode.IsNull:
                    return columnExpression + " IS NULL";
                case CompareMode.IsNullOrWhitespace:
                    return "ISNULL (" + columnExpression + ", '') = ''";
                case CompareMode.Regex:
                    throw new ValidationException("Regex text search not compatible with DB filters.");
                default:
                    throw new ThisShouldNeverHappenException("Unrecognized compare mode: " + Mode);
            }
        }

        /// <summary>
        /// Creates a new column based filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="mode">Compare mode.</param>
        /// <param name="criteria">Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        public static Filter Build(ColumnExpression columnName, CompareMode mode, ObjectValues criteria)
        {
            AssertCriteriaIsValid(mode, criteria, out ValidationFlaggedAction vAction);
            
            if ((vAction & ValidationFlaggedAction.SwitchHiAndLoValues) == ValidationFlaggedAction.SwitchHiAndLoValues)
            {
                criteria = ObjectValues.From(criteria[1], criteria[0]);
            }

            return new Filter
            {
                ColumnName = columnName,
                Mode = mode,
                Criteria = criteria
            };
        }

        /// <summary>
        /// Creates a new column based filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="mode">Compare mode.</param>
        /// <param name="criteria">Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        public static Filter Build<T>(ColumnExpression columnName, CompareMode mode, IList<T> criteria) where T : IComparable<T>
        {
            var _criteria = new ObjectValues(criteria.Cast<object>());
            AssertCriteriaIsValid(mode, _criteria, out ValidationFlaggedAction vAction);

            if ((vAction & ValidationFlaggedAction.SwitchHiAndLoValues) == ValidationFlaggedAction.SwitchHiAndLoValues)
            {
                _criteria = ObjectValues.From(_criteria[1], _criteria[0]);
            }

            return new Filter
            {
                ColumnName = columnName,
                Mode = mode,
                Criteria = _criteria
            };
        }

        /// <summary>
        /// Creates a new column based filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="mode">Compare mode.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        public static Filter Build<T>(ColumnExpression columnName, CompareMode mode, T criteria) where T : IComparable<T>
        {
            var _criteria = ObjectValues.From(criteria);
            AssertCriteriaIsValid(mode, _criteria);
            return new Filter
            {
                ColumnName = columnName,
                Mode = mode,
                Criteria = _criteria
            };
        }

        /// <summary>
        /// Creates a new column based 'Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter Equals<T>(ColumnExpression columnName, T criteria) where T : IComparable<T> =>
            Build(columnName, CompareMode.Equals, criteria);

        /// <summary>
        /// Creates a new column based 'Contains' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter Contains(ColumnExpression columnName, string criteria) =>
            Build(columnName, CompareMode.Contains, criteria);

        /// <summary>
        /// Creates a new column based 'Starts With' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter StartsWith(ColumnExpression columnName, string criteria) =>
            Build(columnName, CompareMode.StartsWith, criteria);

        /// <summary>
        /// Creates a new column based 'Ends With' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter EndsWith(ColumnExpression columnName, string criteria) =>
            Build(columnName, CompareMode.EndsWith, criteria);

        /// <summary>
        /// Creates a new column based 'Greater Than' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter GreaterThan<T>(ColumnExpression columnName, T criteria) where T : IComparable<T> =>
            Build(columnName, CompareMode.GreaterThan, criteria);

        /// <summary>
        /// Creates a new column based 'Greater Than or Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter GreaterThanOrEquals<T>(ColumnExpression columnName, T criteria) where T : IComparable<T> =>
            Build(columnName, CompareMode.GreaterThanOrEquals, criteria);

        /// <summary>
        /// Creates a new column based 'Less Than' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter LessThan<T>(ColumnExpression columnName, T criteria) where T : IComparable<T> =>
            Build(columnName, CompareMode.LessThan, criteria);

        /// <summary>
        /// Creates a new column based 'Less Than or Equals' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter LessThanOrEquals<T>(ColumnExpression columnName, T criteria) where T : IComparable<T> =>
            Build(columnName, CompareMode.LessThanOrEquals, criteria);

        /// <summary>
        /// Creates a new column based 'In' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="criteria">Value(s) from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter In<T>(ColumnExpression columnName, IList<T> criteria) where T : IComparable<T> =>
            Build(columnName, CompareMode.In, criteria);

        /// <summary>
        /// Creates a new column based 'Between' filter that includes the high and low values.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter Between<T>(ColumnExpression columnName, T loValue, T hiValue) where T : IComparable<T> =>
            Build(columnName, CompareMode.Between, new[] { loValue, hiValue });

        /// <summary>
        /// Creates a new column based 'Between' filter that excludes the high and low values.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter BetweenExclusive<T>(ColumnExpression columnName, T loValue, T hiValue) where T : IComparable<T>
        {
            AssertCriteriaIsValid(CompareMode.Between, ObjectValues.From(loValue, hiValue), out ValidationFlaggedAction vAction);

            if ((vAction & ValidationFlaggedAction.SwitchHiAndLoValues) == ValidationFlaggedAction.SwitchHiAndLoValues)
            {
                (hiValue, loValue) = (loValue, hiValue);
            }

            return And
            (
                new Filter { ColumnName = columnName, Mode = CompareMode.GreaterThan, Criteria = ObjectValues.From(loValue) },
                new Filter { ColumnName = columnName, Mode = CompareMode.LessThan, Criteria = ObjectValues.From(hiValue) }
            );
        }

        /// <summary>
        /// Creates a new column based 'Between' filter that excludes only the low value.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter BetweenExclusiveLo<T>(ColumnExpression columnName, T loValue, T hiValue) where T : IComparable<T>
        {
            AssertCriteriaIsValid(CompareMode.Between, ObjectValues.From(loValue, hiValue), out ValidationFlaggedAction vAction);

            if ((vAction & ValidationFlaggedAction.SwitchHiAndLoValues) == ValidationFlaggedAction.SwitchHiAndLoValues)
            {
                (hiValue, loValue) = (loValue, hiValue);
            }

            return And
            (
                new Filter { ColumnName = columnName, Mode = CompareMode.GreaterThan, Criteria = ObjectValues.From(loValue) },
                new Filter { ColumnName = columnName, Mode = CompareMode.LessThanOrEquals, Criteria = ObjectValues.From(hiValue) }
            );
        }

        /// <summary>
        /// Creates a new column based 'Between' filter that excludes only the high value.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="loValue">The lower of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <param name="hiValue">The higher of two values from which to build the filter and subsequent SQL 'WHERE' expression.</param>
        /// <returns>A filter.</returns>
        public static IFilter BetweenExclusiveHi<T>(ColumnExpression columnName, T loValue, T hiValue) where T : IComparable<T>
        {
            AssertCriteriaIsValid(CompareMode.Between, ObjectValues.From(loValue, hiValue), out ValidationFlaggedAction vAction);

            if ((vAction & ValidationFlaggedAction.SwitchHiAndLoValues) == ValidationFlaggedAction.SwitchHiAndLoValues)
            {
                (hiValue, loValue) = (loValue, hiValue);
            }

            return And
            (
                new Filter { ColumnName = columnName, Mode = CompareMode.GreaterThanOrEquals, Criteria = ObjectValues.From(loValue) },
                new Filter { ColumnName = columnName, Mode = CompareMode.LessThan, Criteria = ObjectValues.From(hiValue) }
            );
        }

        /// <summary>
        /// Creates a new column based 'Is Null' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        public static IFilter IsNull(ColumnExpression columnName) =>
            Build(columnName, CompareMode.IsNull, ObjectValues.Empty);

        /// <summary>
        /// Creates a new column based 'Is Null or Whitespace' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        public static IFilter IsNullOrWhitespace(ColumnExpression columnName) =>
            Build(columnName, CompareMode.IsNullOrWhitespace, ObjectValues.Empty);

        /// <summary>
        /// Creates a new column based 'Literal' filter.
        /// </summary>
        /// <param name="literalExpression">The SQL expression to use.</param>
        public static IFilter Literal(string literalExpression) =>
            new LiteralFilter(literalExpression);

        /// <summary>
        /// Creates a new column based 'Literal' filter.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="literalExpression">The SQL expression to use.</param>
        public static IFilter Literal(ColumnExpression columnName, string literalExpression) =>
            new LiteralFilter(columnName, literalExpression);

        /// <summary>
        /// Creates a specialized filter that contains other filters all of which must evaluate to true for a table row to be included.
        /// </summary>
        /// <param name="filters">Filters to group.</param>
        /// <returns>An 'And' group filter.</returns>
        public static IGroupFilter And(params IFilter[] filters)
        {
            return new AndGroupFilter(filters);
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
            return new OrGroupFilter(filters);
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
        /// Creates a factory that creates filters that negate other filters.
        /// </summary>
        /// <returns>A negated filter.</returns>
        public static IFilterFactory Not()
        {
            return new NotFilterFactory();
        }
    }
}
