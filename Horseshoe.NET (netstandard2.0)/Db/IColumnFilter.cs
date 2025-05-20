namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public interface IColumnFilter : IFilter
    {
        /// <summary>
        /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
        /// </summary>
        DbProvider? Provider { get; set; }
    }
}
