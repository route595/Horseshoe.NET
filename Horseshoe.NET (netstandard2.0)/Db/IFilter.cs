namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// A DB platform lends hints about how to render SQL expressions and statements.
        /// </summary>
        DbPlatform? Platform { get; set; }

        /// <summary>
        /// Renders the filter as a SQL expression.
        /// </summary>
        /// <param name="platform">A DB platform lends hints about how to render SQL expressions and statements.</param>
        /// <returns>A SQL expression.</returns>
        string Render(DbPlatform? platform = null);
    }
}
