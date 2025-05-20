namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses in C# using literal expressions.
    /// </summary>
    public class LiteralFilter : IFilter
    {
        /// <summary>
        /// The literal filter text
        /// </summary>
        public string LiteralText { get; }

        /// <summary>
        /// This flag, if <c>true</c>, indicates predictable system generated filter text that can be negated.  See <c>Not()</c>.
        /// </summary>
        public bool SystemGenerated { get; }

        /// <summary>
        /// Creates a new <c>IFilter</c> from the supplied <see cref="SqlLiteral" />.
        /// </summary>
        /// <param name="sqlLiteral">A <c>SqlLiteral</c>.</param>
        public LiteralFilter(SqlLiteral sqlLiteral)
        {
            LiteralText = sqlLiteral.Render();
        }

        /// <summary>
        /// Creates a new <c>IFilter</c> from a SQL literal expression.
        /// </summary>
        /// <param name="literalExpression">A literal SQL expression</param>
        public LiteralFilter(string literalExpression)
        {
            LiteralText = literalExpression;
        }

        /// <summary>
        /// Creates a new <c>IFilter</c> from a SQL literal expression.
        /// </summary>
        /// <param name="literalExpression">A literal SQL expression</param>
        /// <param name="systemGenerated">This flag, if <c>true</c>, indicates predictable system generated filter text that can be negated.  See <c>Not()</c>.</param>
        internal LiteralFilter(string literalExpression, bool systemGenerated)
        {
            LiteralText = literalExpression;
            SystemGenerated = systemGenerated;
        }

        /// <summary>
        /// Returnes the literal text of the filter.
        /// </summary>
        /// <returns>A SQL expression.</returns>
        public string Render(DbProvider? provider = null)
        {
            return LiteralText;
        }
    }
}
