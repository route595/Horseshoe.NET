namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Represents a column name or expression with implicit casting from <c>string</c> and <c>SqlLiteral</c>.
    /// </summary>
    public readonly struct ColumnExpression
    {
        /// <summary>
        /// The <c>SqlLiteral</c> (if created with the <c>SqlLiteral</c> arg constructor).
        /// </summary>
        public SqlLiteral SqlLiteral { get; }

        /// <summary>
        /// The <c>string</c> column name (if created with the <c>string</c> arg constructor).
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// A format to apply to the column name, for example "LEFT({0}, 1)" to use only the first letter of each column value.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Creates a new <c>ColumnNameExpression</c>.
        /// </summary>
        /// <param name="columnName">A column name.</param>
        public ColumnExpression(string columnName)
        {
            ColumnName = columnName;
            Format = null;
            SqlLiteral = null;
        }

        /// <summary>
        /// Creates a new <c>ColumnNameExpression</c>.
        /// </summary>
        /// <param name="columnName">A column name.</param>
        /// <param name="format">A format to apply to the column name, for example "LEFT({0}, 1)" to select only the first letter.</param>
        public ColumnExpression(string columnName, string format) : this(columnName)
        {
            Format = format;
        }

        /// <summary>
        /// Creates a new <c>ColumnNameExpression</c>.
        /// </summary>
        /// <param name="sqlLiteral">A <c>SqlLiteral</c>.</param>
        public ColumnExpression(SqlLiteral sqlLiteral)
        {
            ColumnName = null;
            Format = null;
            SqlLiteral = sqlLiteral;
        }

        /// <summary>
        /// Renders a column name (or literal column name substitute) to a SQL expression.
        /// </summary>
        /// <param name="provider">A DB provider lends hints about how to render SQL expressions and statements.</param>
        /// <returns>A SQL expression.</returns>
        /// <exception cref="ThisShouldNeverHappenException"></exception>
        public string Render(DbProvider provider = default)
        {
            if (ColumnName != null)
            {
                if (Format != null)
                    return string.Format(Format, DbUtilAbstractions.RenderColumnName(ColumnName, provider: provider));
                return DbUtilAbstractions.RenderColumnName(ColumnName, provider: provider);
            }
            if (SqlLiteral != null)
                return SqlLiteral.Render();
            throw new ThisShouldNeverHappenException("Not properly initiated.");
        }

        /// <summary>
        /// Implicitly converts a <c>string</c> to a <c>ColumnNameExpression</c>
        /// </summary>
        /// <param name="columnName">A <c>string</c> column name</param>
        public static implicit operator ColumnExpression(string columnName) => new ColumnExpression(columnName);

        /// <summary>
        /// Implicitly converts a <c>SqlLiteral</c> to a <c>ColumnNameExpression</c>
        /// </summary>
        /// <param name="sqlLiteral">A <c>DirectoryInfo</c> instance</param>
        public static implicit operator ColumnExpression(SqlLiteral sqlLiteral) => new ColumnExpression(sqlLiteral);
    }
}
