namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A filter that negates another filter. Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public class NotFilter : IFilter
    {
        /// <summary>
        /// A filter to negate.
        /// </summary>
        public IFilter Filter { get; }

        /// <summary>
        /// Creates a new "Not" filter.
        /// </summary>
        /// <param name="filter">A filter to negate.</param>
        /// <exception cref="ValidationException"></exception>
        public NotFilter(IFilter filter)
        {
            if (filter is NotFilter)
                throw new ValidationException("A 'Not' filter cannot be directly nested in another 'Not' filter.");
            Filter = filter;
        }

        /// <summary>
        /// Renders the filter as a SQL expression.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL expression.</returns>
        public string Render(DbProvider? provider = null)
        {
            var rendered = Filter.Render(provider: provider);
            if (Filter is LiteralFilter literalFilter && literalFilter.SystemGenerated)
            {
                if (rendered.Contains(" = "))
                    return rendered.Replace(" = ", " <> ");
                if (rendered.Contains(" < "))
                    return rendered.Replace(" < ", " >= ");
                if (rendered.Contains(" <= "))
                    return rendered.Replace(" <= ", " > ");
                if (rendered.Contains(" > "))
                    return rendered.Replace(" > ", " <= ");
                if (rendered.Contains(" >= "))
                    return rendered.Replace(" >= ", " < ");
                if (rendered.Contains(" LIKE "))
                    return rendered.Replace(" LIKE ", " NOT LIKE ");
                if (rendered.Contains(" IN "))
                    return rendered.Replace(" IN ", " NOT IN ");
                if (rendered.Contains(" BETWEEN "))
                    return rendered.Replace(" BETWEEN ", " NOT BETWEEN ");
                if (rendered.Contains(" IS NULL "))
                    return rendered.Replace(" IS NULL", " IS NOT NULL");
            }
            return "NOT ( " + rendered + " )";
        }
    }
}
