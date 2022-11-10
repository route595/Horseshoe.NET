namespace Horseshoe.NET.Db
{
    public class FilterBuilder
    {
        public string What { get; }
        public DbPlatform? Platform { get; }

        internal FilterBuilder(string what, DbPlatform? platform = null)
        {
            What = what;
            Platform = platform;
        }

        public new Filter Equals(object value)
        {
            return value != null
                ? new Filter(What, new[] { value }, FilterMode.Equals, platform: Platform)
                : new Filter(What, FilterMode.IsNull, platform: Platform);
        }

        public Filter NotEquals(object value, bool columnIsNullable = false)
        {
            if (value != null)
            {
                return columnIsNullable
                    ? Filter.Or
                      (
                          IsNull(),
                          new Filter(What, new[] { value }, FilterMode.Equals, not: true, platform: Platform)
                      )
                    : new Filter(What, new[] { value }, FilterMode.Equals, not: true, platform: Platform);

            }
            return new Filter(What, FilterMode.Equals, not: true, platform: Platform);
        }

        public Filter NotEqualsOrNull(object value)
        {
            return value != null
                ? new Filter(What, new[] { value }, FilterMode.Equals, not: true, platform: Platform)
                : new Filter(What, FilterMode.Equals, not: true, platform: Platform);
        }

        public Filter Contains(string value)
        {
            return new Filter(What, new object[] { value }, FilterMode.Contains, platform: Platform);
        }

        public Filter NotContains(string value)
        {
            return new Filter(What, new object[] { value }, FilterMode.Contains, not: true, platform: Platform);
        }

        public Filter StartsWith(string value)
        {
            return new Filter(What, new object[] { value }, FilterMode.StartsWith, platform: Platform);
        }

        public Filter NotStartsWith(string value)
        {
            return new Filter(What, new object[] { value }, FilterMode.StartsWith, not: true, platform: Platform);
        }

        public Filter EndsWith(string value)
        {
            return new Filter(What, new object[] { value }, FilterMode.EndsWith, platform: Platform);
        }

        public Filter NotEndsWith(string value)
        {
            return new Filter(What, new object[] { value }, FilterMode.EndsWith, not: true, platform: Platform);
        }

        public Filter In(params object[] values)
        {
            return new Filter(What, values, FilterMode.In, platform: Platform);
        }

        public Filter NotIn(params object[] values)
        {
            return new Filter(What, values, FilterMode.In, not: true, platform: Platform);
        }

        public Filter Between(object value1, object value2, FilterBounds filterBounds = FilterBounds.Inclusive)
        {
            switch (filterBounds)
            {
                case FilterBounds.Inclusive:
                default:
                    return new Filter(What, new[] { value1, value2 }, FilterMode.Between, filterBounds: FilterBounds.Inclusive, platform: Platform);
                case FilterBounds.Exclusive:
                    return Filter.And
                    (
                        Between(value1, value2),
                        NotIn(value1, value2)
                    );
                case FilterBounds.ExclusiveLowerBoundOnly:
                case FilterBounds.InclusiveUpperBoundOnly:
                    return Filter.And
                    (
                        Between(value1, value2),
                        NotEquals(value1)
                    );
                case FilterBounds.ExclusiveUpperBoundOnly:
                case FilterBounds.InclusiveLowerBoundOnly:
                    return Filter.And
                    (
                        Between(value1, value2),
                        NotEquals(value2)
                    );
            }
        }

        public Filter NotBetween(object value1, object value2, FilterBounds filterBounds = FilterBounds.Exclusive)
        {
            switch (filterBounds)
            {
                case FilterBounds.Exclusive:
                default:
                    return new Filter(What, new[] { value1, value2 }, FilterMode.Between, not: true, filterBounds: FilterBounds.Exclusive, platform: Platform);
                case FilterBounds.Inclusive:
                    return Filter.Or
                    (
                        NotBetween(value1, value2),
                        In(value1, value2)
                    );
                case FilterBounds.InclusiveUpperBoundOnly:
                case FilterBounds.ExclusiveLowerBoundOnly:
                    return Filter.Or
                    (
                        NotBetween(value1, value2),
                        Equals(value2)
                    );
                case FilterBounds.InclusiveLowerBoundOnly:
                case FilterBounds.ExclusiveUpperBoundOnly:
                    return Filter.Or
                    (
                        NotBetween(value1, value2),
                        Equals(value1)
                    );
            }
        }

        public Filter LessThan(object value)
        {
            return new Filter(What, new[] { value }, FilterMode.LessThan, filterBounds: FilterBounds.Exclusive, platform: Platform);
        }

        public Filter LessThanOrEqual(object value)
        {
            return new Filter(What, new[] { value }, FilterMode.LessThan, filterBounds: FilterBounds.Inclusive, platform: Platform);
        }

        public Filter GreaterThan(object value)
        {
            return new Filter(What, new[] { value }, FilterMode.GreaterThan, filterBounds: FilterBounds.Exclusive, platform: Platform);
        }

        public Filter GreaterThanOrEqual(object value)
        {
            return new Filter(What, new[] { value }, FilterMode.GreaterThan, filterBounds: FilterBounds.Inclusive, platform: Platform);
        }

        public Filter IsNull()
        {
            return new Filter(What, FilterMode.IsNull, platform: Platform);
        }

        public Filter IsNotNull()
        {
            return new Filter(What, FilterMode.IsNull, not: true, platform: Platform);
        }

        public Filter Literal(string filterSql)
        {
            return new Filter(What, new object[] { filterSql }, FilterMode.Literal);
        }
    }
}
