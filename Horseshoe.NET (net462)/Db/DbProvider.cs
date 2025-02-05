namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
    /// </summary>
    public enum DbProvider
    {
        /// <summary>
        /// Generic, provider-neutral
        /// </summary>
        Neutral,

        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        SqlServer,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,

        /// <summary>
        /// MySql
        /// </summary>
        MySql,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSql
    }
}
