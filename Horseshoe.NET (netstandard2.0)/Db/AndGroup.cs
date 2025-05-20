using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A specialized filter that contains other filters all of which must evaluate to true for a table row to be included
    /// </summary>
    public class AndGroup : List<IFilter>, IGroupFilter
    {
        /// <summary>
        /// A DB provider may lend hints about how to render column names, SQL expressions, etc.
        /// </summary>
        public DbProvider? Provider { get; set; }

        /// <summary>
        /// Creates a new <c>AndGroupFilter</c>.
        /// </summary>
        /// <param name="filters">The filters to group.</param>
        /// <exception cref="ValidationException"></exception>
        public AndGroup(params IFilter[] filters) : base()
        {
            if (filters == null) 
                throw new ValidationException("filters cannot be null");
            if (filters.Length == 0)
                throw new ValidationException("filters cannot be empty");
            AddRange(filters);
        }

        /// <summary>
        /// Renders this <c>AndGroupFilter</c> to a SQL expression.
        /// </summary>
        /// <param name="provider">A DB provider may lend hints about how to render column names, SQL expressions, etc.</param>
        /// <returns>A SQL expression.</returns>
        public string Render(DbProvider? provider = null)
        {
            return "( " + string.Join(" AND ", this.Select(f => f.Render(provider: provider))) + " )";
        }
    }
}
