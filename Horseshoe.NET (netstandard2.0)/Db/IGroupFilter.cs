namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Used in generating SQL 'WHERE' clauses in C#.
    /// </summary>
    public interface IGroupFilter : IFilter
    {
        /// <summary>
        /// Adds a new filter to this <c>IGroupFilter</c>.
        /// </summary>
        /// <param name="filter">A filter.</param>
        void Add(IFilter filter);
    }
}
