namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses in C# using literal expressions.
    /// </summary>
    public class LiteralFilter : IFilter
    {
        /// <summary>
        /// A column name or expression. Optional.
        /// </summary>
        public ColumnExpression? ColumnName { get; set; }

        /// <summary>
        /// The literal expression.
        /// </summary>
        public SqlLiteral SqlLiteral { get; }

        /// <summary>
        /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
        /// </summary>
        public DbProvider? Provider { get; set; }

        /// <summary>
        /// Creates a new <c>IFilter</c> from a <see cref="SqlLiteral" />.
        /// </summary>
        /// <param name="sqlLiteral">A <c>SqlLiteral</c>.</param>
        public LiteralFilter(SqlLiteral sqlLiteral)
        {
            SqlLiteral = sqlLiteral;
        }

        /// <summary>
        /// Creates a new <c>IFilter</c> from a <see cref="SqlLiteral" />.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="sqlLiteral">A <c>SqlLiteral</c>.</param>
        public LiteralFilter(ColumnExpression columnName, SqlLiteral sqlLiteral)
        {
            ColumnName = columnName;
            SqlLiteral = sqlLiteral;
        }

        /// <summary>
        /// Creates a new <c>IFilter</c> from a SQL literal expression.
        /// </summary>
        /// <param name="literalExpression">The literal expression to use.</param>
        public LiteralFilter(string literalExpression)
        {
            SqlLiteral = new SqlLiteral(literalExpression);
        }

        /// <summary>
        /// Creates a new <c>IFilter</c> from a SQL literal expression.
        /// </summary>
        /// <param name="columnName">A column name or expression.</param>
        /// <param name="literalExpression">The literal expression to use.</param>
        public LiteralFilter(ColumnExpression columnName, string literalExpression)
        {
            ColumnName = columnName;
            SqlLiteral = new SqlLiteral(literalExpression);
        }

        /// <summary>
        /// Renders the filter as a SQL expression.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL expression.</returns>
        public string Render(DbProvider? provider = null)
        {
            return (ColumnName.HasValue ? ColumnName.Value.Render(provider: provider ?? Provider ?? DbSettings.DefaultProvider) + " " : "") 
                + SqlLiteral.Render();
        }
    }
}
