﻿using System.Linq;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// A specialized filter that contains other filters all of which must evaluate to true for a table row to be included
    /// </summary>
    public class AndGroupFilter : IGroupFilter
    {
        private DbPlatform? _platform;

        /// <summary>
        /// The filters contained by this <c>AndGroupFilter</c>.
        /// </summary>
        public IFilter[] Filters { get; private set; }

        /// <summary>
        /// A DB platform lends hints about how to render SQL expressions and statements.
        /// </summary>
        public DbPlatform? Platform
        {
            get => _platform;
            set
            {
                _platform = value;
                foreach (var filter in Filters)
                {
                    filter.Platform = _platform;
                }
            }
        }

        /// <summary>
        /// Creates a new <c>AndGroupFilter</c>.
        /// </summary>
        /// <param name="filters">The filters to group.</param>
        /// <exception cref="ValidationException"></exception>
        public AndGroupFilter(params IFilter[] filters) : base()
        {
            if (filters == null) 
                throw new ValidationException("filters cannot be null");
            if (filters.Length == 0)
                throw new ValidationException("filters cannot be empty");
            Filters = filters;
        }

        /// <summary>
        /// Adds a new filter to this <c>AndGroupFilter</c>.
        /// </summary>
        /// <param name="filter">A filter.</param>
        public void Add(IFilter filter)
        {
            var array = new IFilter[Filters.Length + 1];
            Filters.CopyTo(array, 0);
            array[array.Length - 1] = filter;
            Filters = array;
        }

        /// <summary>
        /// Renders this <c>AndGroupFilter</c> to a SQL expression.
        /// </summary>
        /// <returns>A SQL expression.</returns>
        public string Render(DbPlatform? platform = null)
        {
            return "( " + string.Join(" AND ", Filters.Select(f => f.Render(platform: platform))) + " )";
        }
    }
}
