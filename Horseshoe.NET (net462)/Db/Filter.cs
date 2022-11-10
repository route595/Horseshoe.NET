using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Horseshoe.NET.Db.DbUtil;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Db
{
    public class Filter
    {
        public string What { get; }
        public object[] Values { get; }
        public FilterMode FilterMode { get; }
        public bool Not { get; }
        public FilterBounds FilterBounds { get; }
        public DbPlatform? Platform { get; }

        internal Filter() 
        {
            What = "n/a";
        }

        internal Filter(string what, FilterMode filterMode, bool not = false, FilterBounds filterBounds = default, DbPlatform? platform = null)
            : this(what, null, filterMode, not: not, filterBounds: filterBounds, platform: platform)
        {
        }

        internal Filter(string what, object[] values, FilterMode filterMode, bool not = false, FilterBounds filterBounds = default, DbPlatform? platform = null)
        {
            What = what;
            Values = values;
            FilterMode = filterMode;
            Not = not;
            FilterBounds = filterBounds;
            Platform = platform;
            Validate();
        }

        private void Validate()
        {

            if (FilterMode != FilterMode.Literal)
            {
                if (What == null)
                {
                    throw new UtilityException("missing required parameter: column name a.k.a. What");
                }

                if (What.StartsWith("["))
                {
                    if (!What.EndsWith("]"))
                    {
                        throw new UtilityException("malformed column name: " + What);
                    }
                }
                else if (What.EndsWith("]"))
                {
                    throw new UtilityException("malformed column name: " + What);
                }
            }

            if (Values == null)
            {
                if (FilterMode != FilterMode.IsNull)
                {
                    throw new UtilityException("'values' is required");
                }
            }
            else
            {
                switch (FilterMode)
                {
                    case FilterMode.Equals:
                        if (Values.Length != 1)
                        {
                            throw new UtilityException("'Equality' filters require exactly 1 value, found: " + Values.Length);
                        }
                        break;
                    case FilterMode.GreaterThan:
                    case FilterMode.LessThan:
                        {
                            if (Values.Length != 1)
                            {
                                throw new UtilityException("'Comparison' filters require exactly 1 value, found: " + Values.Length);
                            }
                            if (Values[0] == null || (Values[0] is string stringValues0 && stringValues0.Trim().Length == 0))
                            {
                                throw new UtilityException("Cannot create 'comparison' filter with null or blank value");
                            }
                            break;
                        }
                    case FilterMode.Contains:
                    case FilterMode.StartsWith:
                    case FilterMode.EndsWith:
                        {
                            if (Values.Length != 1)
                            {
                                throw new UtilityException("'String search' filters require exactly 1 value, found: " + Values.Length);
                            }
                            if (Values[0] == null || (Values[0] is string stringValues0 && stringValues0.Trim().Length == 0))
                            {
                                throw new UtilityException("Cannot create 'string search' filter with null or blank value");
                            }
                            break;
                        }
                    case FilterMode.Between:
                        {
                            if (Values.Length != 2)
                            {
                                throw new UtilityException("'Between' filters require exactly 2 values, found: " + Values.Length);
                            }
                            if (Values[0] == null || Values[1] == null || (Values[0] is string stringValues0 && stringValues0.Trim().Length == 0) || (Values[1] is string stringValues1 && stringValues1.Trim().Length == 0))
                            {
                                throw new UtilityException("Cannot create 'between' filter with null or blank value");
                            }
                            break;
                        }
                    case FilterMode.Literal:
                        {
                            if (Values.Length != 1)
                            {
                                throw new UtilityException("'Literal' filters require exactly 1 value, found: " + Values.Length);
                            }
                            if (Values[0] == null || (Values[0] is string stringValues0 && stringValues0.Trim().Length == 0))
                            {
                                throw new UtilityException("Cannot create 'literal' filter with null or blank value");
                            }
                            break;
                        }
                }
            }
        }

        public override string ToString()
        {
            return Render();
        }

        public virtual string Render()
        {
            var sb = new StringBuilder();
            if (What != null)
            {
                sb.Append(What).Append(" ");
            }
            try
            {
                if (FilterMode == FilterMode.IsNull)
                {
                    sb.Append("IS ").AppendIf(Not, "NOT ").Append("NULL");
                }
                if (Values != null)
                {
                    switch (FilterMode)
                    {
                        case FilterMode.Equals:
                            sb.AppendIf(!Not, "= ", "<> ").Append(Sqlize(Values[0]));
                            break;
                        case FilterMode.Contains:
                            sb.AppendIf(Not, "NOT ").Append("LIKE ").Append("'%" + Values[0] + "%'");
                            break;
                        case FilterMode.StartsWith:
                            sb.AppendIf(Not, "NOT ").Append("LIKE ").Append("'" + Values[0] + "%'");
                            break;
                        case FilterMode.EndsWith:
                            sb.AppendIf(Not, "NOT ").Append("LIKE ").Append("'%" + Values[0] + "'");
                            break;
                        case FilterMode.GreaterThan:
                            sb.Append(FilterBounds == FilterBounds.Exclusive ? "> " : ">= ").Append(Sqlize(Values[0]));
                            break;
                        case FilterMode.LessThan:
                            sb.Append(FilterBounds == FilterBounds.Exclusive ? "< " : "<= ").Append(Sqlize(Values[0]));
                            break;
                        case FilterMode.In:
                            if (Values.Length == 0)
                            {
                                return !Not
                                    ? "1 = 0"   // return 0 rows (IN)
                                    : "1 = 1";  // return all rows (NOT IN)
                            }
                            sb.AppendIf(Not, "NOT ").Append("IN(").Append(string.Join(", ", Values.Select(v => Sqlize(v)))).Append(")");
                            break;
                        case FilterMode.Between:
                            sb.AppendIf(Not, "NOT ").Append("BETWEEN ").Append(Sqlize(Values[0])).Append(" AND ").Append(Sqlize(Values[1]));
                            break;
                        //case FilterMode.IsNull:
                        //    sb.Append("IS ").AppendIf(Not, "NOT ").Append("NULL");
                        //    break;
                        case FilterMode.Literal:
                            if (Values[0] is string stringValue0)
                            {
                                Values[0] = new SqlLiteral(stringValue0);
                            }
                            else if (Values[0] is DateTime dateTimeValue)
                            {
                                var temp = Sqlize(dateTimeValue, Platform);
                                temp = temp.Substring(1, temp.Length - 2); // shave off the single quotes
                                Values[0] = new SqlLiteral(temp);
                            }
                            sb.Append(Sqlize(Values[0]));
                            break;
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new ValidationException("Invalid value(s): " + string.Join(", ", Values.Select(v => v?.ToString() ?? "[null]")), ex);
            }
        }

        public static FilterBuilder Expression(string expression, DbPlatform? platform = null) => new FilterBuilder(expression, platform: platform);

        public static FilterBuilder Column(string columnName, DbPlatform? platform = null) => new FilterBuilder(RenderColumnName(columnName, platform: platform), platform: platform);

        public static Filter Literal(string filterSql)
        {
            return new Filter("n/a", new object[] { filterSql }, FilterMode.Literal);
        }

        public static Filter And(params Filter[] filters)
        {
            return new AndGrouping(filters);
        }

        public static Filter Or(params Filter[] filters)
        {
            return new OrGrouping(filters);
        }

        public class AndGrouping : Filter
        {
            public List<Filter> Filters { get; }

            public AndGrouping(Filter[] filters) : base()
            {
                if (filters == null) throw new UtilityException("filters cannot be null");
                if (filters.Length == 0) throw new UtilityException("filters cannot be empty");
                Filters = new List<Filter>(filters);
            }

            public override string Render()
            {
                return "(" + string.Join(" AND ", Filters.Select(f => f.Render())) + ")";
            }
        }

        public class OrGrouping : Filter
        {
            public List<Filter> Filters { get; }

            public OrGrouping(Filter[] filters) : base()
            {
                if (filters == null) throw new UtilityException("filters cannot be null");
                if (filters.Length == 0) throw new UtilityException("filters cannot be empty");
                Filters = new List<Filter>(filters);
            }

            public override string Render()
            {
                return "(" + string.Join(" OR ", Filters.Select(f => f.Render())) + ")";
            }
        }
    }
}
