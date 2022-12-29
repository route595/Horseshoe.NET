using System.Linq;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A filter that negates another filter. Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public class NotFilter : Filter
    {
        /// <summary>
        /// A filter to negate.
        /// </summary>
        public IFilter Filter { get; }

        /// <summary>
        /// Creates a new "Not" filter.
        /// </summary>
        /// <param name="filter">A filter to negate.</param>
        public NotFilter(IFilter filter)
        {
            if (filter is NotFilter)
                throw new ValidationException("A 'Not' filter cannot be directly nested in another 'Not' filter.");
            Filter = filter;
        }

        /// <summary>
        /// Renders the filter as a SQL expression.
        /// </summary>
        /// <param name="platform">A DB platform may lend hints about how to render column names and parameters.</param>
        /// <returns>A SQL expression.</returns>
        public override string Render(DbPlatform? platform = null)
        {
            if (Filter is Filter filter)
            {
                var columnExpression = filter.ColumnName.Render(platform: platform ?? Platform);
                switch (filter.Mode)
                {
                    case Compare.CompareMode.Equals:
                        return columnExpression + " <> " + DbUtil.Sqlize(filter.Criteria[0], platform: platform ?? Platform);
                    case Compare.CompareMode.Contains:
                        return columnExpression + " NOT LIKE '%" + filter.Criteria[0] + "%'";
                    case Compare.CompareMode.StartsWith:
                        return columnExpression + " NOT LIKE '" + filter.Criteria[0] + "%'";
                    case Compare.CompareMode.EndsWith:
                        return columnExpression + " NOT LIKE '%" + filter.Criteria[0] + "'";
                    case Compare.CompareMode.In:
                        return filter.Criteria.Count == 0
                            ? "1 = 1"
                            : columnExpression + " NOT IN ( " + string.Join(", ", filter.Criteria.Select(val => DbUtil.Sqlize(val, platform: platform ?? Platform))) + " )";
                    case Compare.CompareMode.Between:
                        return columnExpression + " NOT BETWEEN " + DbUtil.Sqlize(filter.Criteria[0], platform: platform ?? Platform) + " AND " + DbUtil.Sqlize(filter.Criteria[1], platform: platform ?? Platform);
                    case Compare.CompareMode.IsNull:
                        return columnExpression + " IS NOT NULL";
                }
            }

            var rendered = Filter.Render(platform: platform ?? Platform);
            return (rendered.StartsWith("(") && rendered.EndsWith(")"))
                ? "NOT " + rendered
                : "NOT ( " + rendered + " )";
        }
    }
}
