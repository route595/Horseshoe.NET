namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Renders the filter as a SQL expression.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL filtering expression.</returns>
        string Render(DbProvider? provider = null);
    }
}
