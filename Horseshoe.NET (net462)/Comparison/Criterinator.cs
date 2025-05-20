using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines a single-criterium comparator.
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public class Criterinator<T> : ICriterinator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Represents a concrete method that evaluates an item
        /// </summary>
        public Func<T, bool> Eval { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Criterinator{T}"/> class.
        /// </summary>
        /// <param name="eval">An eval function</param>
        public Criterinator(Func<T, bool> eval)
        {
            Eval = eval;
        }

        /// <inheritdoc cref="ICriterinator{T}.IsMatch(T)"/>
        public bool IsMatch(T item)
        {
            if (Eval == null)
                return false;
            return Eval.Invoke(item);
        }
    }

    /// <summary>
    /// Factory methods of <c>ICriterinator</c> instances
    /// </summary>
    public static class Criterinator
    {
        /// <summary>
        /// Builds a new 'Or' criterinator group
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criterinators">The constituent <c>ICriterinator</c>s</param>
        /// <returns>An <c>ICriterinator</c> group.</returns>
        public static ICriterinator<T> Or<T>(params ICriterinator<T>[] criterinators) where T : IComparable<T> =>
            new OrGroup<T>(criterinators);

        /// <summary>
        /// Builds a new 'And' criterinator group
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criterinators">The constituent <c>ICriterinator</c>s</param>
        /// <returns>An <c>ICriterinator</c> group.</returns>
        public static ICriterinator<T> And<T>(params ICriterinator<T>[] criterinators) where T : IComparable<T> =>
            new AndGroup<T>(criterinators);

        /// <summary>
        /// Builds a new 'Not' criterinator
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criterinator">The constituent <c>ICriterinator</c></param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> Not<T>(ICriterinator<T> criterinator) where T : IComparable<T> =>
            new Not<T>(criterinator);

        /// <summary>
        /// Creates a new 'Equals' criterinator based on the supplied criteria.
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> Equals<T>(T criterium) where T : IComparable<T> =>
            new SingleCriterinator<T>(criterium, (item, crit) =>
            {
                if (crit == null)
                    return item == null;
                return crit.CompareTo(item) == 0;
            });

        /// <summary>
        /// Creates a new 'Equals' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EqualsIgnoreCase(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                string.Equals(item, crit, StringComparison.OrdinalIgnoreCase)
            );

        /// <summary>
        /// Creates a new 'EqualsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> EqualsAny<T>(IEnumerable<T> criteria) where T : IComparable<T> =>
            new ManyCriterinator<T>(criteria, (item, crits) =>
                crits.Any(c => (c == null && item == null) || (c != null && c.CompareTo(item) == 0))
            );

        /// <summary>
        /// Creates a new 'EqualsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> EqualsAny<T>(params T[] criteria) where T : IComparable<T> =>
            new ManyCriterinator<T>(criteria, (item, crits) =>
                crits.Any(c => (c == null && item == null) || (c != null && c.CompareTo(item) == 0))
            );

        /// <summary>
        /// Creates a new 'EqualsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EqualsAnyIgnoreCase(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                crits.Any(c => string.Equals(item, c, StringComparison.OrdinalIgnoreCase))
            );

        /// <summary>
        /// Creates a new 'EqualsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EqualsAnyIgnoreCase(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                crits.Any(c => string.Equals(item, c, StringComparison.OrdinalIgnoreCase))
            );

        /// <summary>
        /// Creates a new 'Contains' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> Contains(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                item != null && crit != null && item.Contains(crit)
            );

        /// <summary>
        /// Creates a new 'Contains' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsIgnoreCase(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                item != null && crit != null && item.IndexOf(crit, StringComparison.OrdinalIgnoreCase) >= 0
            );

        /// <summary>
        /// Creates a new 'ContainsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAny(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any(c => c != null && item.Contains(c))
            );

        /// <summary>
        /// Creates a new 'ContainsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAny(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any(c => c != null && item.Contains(c))
            );

        /// <summary>
        /// Creates a new 'ContainsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAnyIgnoreCase(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                 item != null && crits.Any(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
            );

        /// <summary>
        /// Creates a new 'ContainsAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAnyIgnoreCase(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                 item != null && crits.Any(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
            );

        /// <summary>
        /// Creates a new 'ContainsAll' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAll(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any() && crits.All(c => c != null && item.Contains(c))
            );

        /// <summary>
        /// Creates a new 'ContainsAll' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAll(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any() && crits.All(c => c != null && item.Contains(c))
            );

        /// <summary>
        /// Creates a new 'ContainsAll' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAllIgnoreCase(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any() && crits.All(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
            );

        /// <summary>
        /// Creates a new 'ContainsAll' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> ContainsAllIgnoreCase(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any() && crits.All(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
            );

        /// <summary>
        /// Creates a new 'StartsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> StartsWith(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                item != null && crit != null && item.StartsWith(crit)
            );

        /// <summary>
        /// Creates a new 'StartsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> StartsWithIgnoreCase(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                item != null && crit != null && item.IndexOf(crit, StringComparison.OrdinalIgnoreCase) == 0
            );

        /// <summary>
        /// Creates a new 'StartsWithAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> StartsWithAny(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any(c => c != null && item.StartsWith(c))
            );

        /// <summary>
        /// Creates a new 'StartsWithAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> StartsWithAny(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any(c => c != null && item.StartsWith(c))
            );

        /// <summary>
        /// Creates a new 'StartsWithAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> StartsWithAnyIgnoreCase(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                 item != null && crits.Any(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) == 0)
            );

        /// <summary>
        /// Creates a new 'StartsWithAny' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> StartsWithAnyIgnoreCase(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                 item != null && crits.Any(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) == 0)
            );

        /// <summary>
        /// Creates a new 'EndsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EndsWith(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                item != null && crit != null && item.EndsWith(crit)
            );

        /// <summary>
        /// Creates a new 'EndsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EndsWithIgnoreCase(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                item != null && crit != null && item.IndexOf(crit, StringComparison.OrdinalIgnoreCase) == item.Length - crit.Length
            );

        /// <summary>
        /// Creates a new 'EndsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EndsWithAny(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any(c => c != null && item.EndsWith(c))
            );

        /// <summary>
        /// Creates a new 'EndsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EndsWithAny(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                item != null && crits.Any(c => c != null && item.EndsWith(c))
            );

        /// <summary>
        /// Creates a new 'EndsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EndsWithAnyIgnoreCase(IEnumerable<string> criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                 item != null && crits.Any(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) == item.Length - c.Length)
            );

        /// <summary>
        /// Creates a new 'EndsWith' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> EndsWithAnyIgnoreCase(params string[] criteria) =>
            new ManyCriterinator<string>(criteria, (item, crits) =>
                 item != null && crits.Any(c => c != null && item.IndexOf(c, StringComparison.OrdinalIgnoreCase) == item.Length - c.Length)
            );

        /// <summary>
        /// Creates a new criterinator based on the supplied mode and criteria.
        /// </summary>
        /// <param name="mode">Equals, Contains, StartsWith or EndsWith</param>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> Like(LikeMode mode, string criterium)
        {
            switch (mode)
            {
                case LikeMode.Equals:
                default:
                    return Equals(criterium);
                case LikeMode.Contains:
                    return Contains(criterium);
                case LikeMode.StartsWith:
                    return StartsWith(criterium);
                case LikeMode.EndsWith:
                    return EndsWith(criterium);
            }
        }

        /// <summary>
        /// Creates a new criterinator based on the supplied mode and criteria.
        /// </summary>
        /// <param name="mode">Equals, Contains, StartsWith or EndsWith</param>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> LikeIgnoreCase(LikeMode mode, string criterium)
        {
            switch (mode)
            {
                case LikeMode.Equals:
                default:
                    return EqualsIgnoreCase(criterium);
                case LikeMode.Contains:
                    return ContainsIgnoreCase(criterium);
                case LikeMode.StartsWith:
                    return StartsWithIgnoreCase(criterium);
                case LikeMode.EndsWith:
                    return EndsWithIgnoreCase(criterium);
            }
        }

        /// <summary>
        /// Creates a new criterinator based on the supplied mode and criteria.
        /// </summary>
        /// <param name="mode">Equals, Contains, StartsWith or EndsWith</param>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> LikeAny(LikeMode mode, IEnumerable<string> criteria)
        {
            switch (mode)
            {
                case LikeMode.Equals:
                default:
                    return EqualsAny(criteria);
                case LikeMode.Contains:
                    return ContainsAny(criteria);
                case LikeMode.StartsWith:
                    return StartsWithAny(criteria);
                case LikeMode.EndsWith:
                    return EndsWithAny(criteria);
            }
        }

        /// <summary>
        /// Creates a new criterinator based on the supplied mode and criteria.
        /// </summary>
        /// <param name="mode">Equals, Contains, StartsWith or EndsWith</param>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> LikeAny(LikeMode mode, params string[] criteria)
        {
            switch (mode)
            {
                case LikeMode.Equals:
                default:
                    return EqualsAny(criteria);
                case LikeMode.Contains:
                    return ContainsAny(criteria);
                case LikeMode.StartsWith:
                    return StartsWithAny(criteria);
                case LikeMode.EndsWith:
                    return EndsWithAny(criteria);
            }
        }

        /// <summary>
        /// Creates a new criterinator based on the supplied mode and criteria.
        /// </summary>
        /// <param name="mode">Equals, Contains, StartsWith or EndsWith</param>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> LikeAnyIgnoreCase(LikeMode mode, IEnumerable<string> criteria)
        {
            switch (mode)
            {
                case LikeMode.Equals:
                default:
                    return EqualsAnyIgnoreCase(criteria);
                case LikeMode.Contains:
                    return ContainsAnyIgnoreCase(criteria);
                case LikeMode.StartsWith:
                    return StartsWithAnyIgnoreCase(criteria);
                case LikeMode.EndsWith:
                    return EndsWithAnyIgnoreCase(criteria);
            }
        }

        /// <summary>
        /// Creates a new criterinator based on the supplied mode and criteria.
        /// </summary>
        /// <param name="mode">Equals, Contains, StartsWith or EndsWith</param>
        /// <param name="criteria">A collection of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> LikeAnyIgnoreCase(LikeMode mode, params string[] criteria)
        {
            switch (mode)
            {
                case LikeMode.Equals:
                default:
                    return EqualsAnyIgnoreCase(criteria);
                case LikeMode.Contains:
                    return ContainsAnyIgnoreCase(criteria);
                case LikeMode.StartsWith:
                    return StartsWithAnyIgnoreCase(criteria);
                case LikeMode.EndsWith:
                    return EndsWithAnyIgnoreCase(criteria);
            }
        }

        /// <summary>
        /// Creates a new 'GreaterThan' criterinator based on the supplied criteria.
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> GreaterThan<T>(T criterium) where T : IComparable<T> =>
            new SingleCriterinator<T>(criterium, (item, crit) =>
                 item != null && item.CompareTo(crit) > 0
            );

        /// <summary>
        /// Creates a new 'GreaterThan' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> GreaterThanIgnoreCase(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                 item != null && item.ToUpper().CompareTo(crit?.ToUpper()) > 0
            );

        /// <summary>
        /// Creates a new 'GreaterThanOrEquals' criterinator based on the supplied criteria.
        /// </summary>
        /// <typeparam name="T">Runtime type of items being compared</typeparam>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> GreaterThanOrEquals<T>(T criterium) where T : IComparable<T> =>
            Or
            (
                GreaterThan(criterium),
                Equals(criterium)
            );

        /// <summary>
        /// Creates a new 'GreaterThan' or 'Equals' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> GreaterThanOrEqualsIgnoreCase(string criterium) =>
            Or
            (
                GreaterThanIgnoreCase(criterium),
                EqualsIgnoreCase(criterium)
            );

        /// <summary>
        /// Creates a new 'LessThan' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> LessThan<T>(T criterium) where T : IComparable<T> =>
            new SingleCriterinator<T>(criterium, (item, crit) =>
                 item != null && item.CompareTo(crit) < 0
            );

        /// <summary>
        /// Creates a new 'LessThan' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> LessThanIgnoreCase(string criterium) =>
            new SingleCriterinator<string>(criterium, (item, crit) =>
                 item != null && item.ToUpper().CompareTo(crit?.ToUpper()) < 0
            );

        /// <summary>
        /// Creates a new 'LessThan' or 'Equals' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="criterium">A value used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> LessThanOrEquals<T>(T criterium) where T : IComparable<T> =>
            Or
            (
                LessThan(criterium),
                Equals(criterium)
            );

        /// <summary>
        /// Creates a new 'Between' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="loCriterium">The lowest of a pair of values used as comparison criteria</param>
        /// <param name="hiCriterium">The highest of a pair of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> Between<T>(T loCriterium, T hiCriterium) where T : IComparable<T> =>
            Or
            (
                And
                (
                    GreaterThan(loCriterium),
                    LessThan(hiCriterium)
                ),
                Equals(loCriterium),
                Equals(hiCriterium)
            );

        /// <summary>
        /// Creates a new 'Between' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="loCriterium">The lowest of a pair of values used as comparison criteria</param>
        /// <param name="hiCriterium">The highest of a pair of values used as comparison criteria</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> BetweenIgnoreCase(string loCriterium, string hiCriterium) =>
            Or
            (
                And
                (
                    GreaterThanIgnoreCase(loCriterium),
                    LessThanIgnoreCase(hiCriterium)
                ),
                EqualsIgnoreCase(loCriterium),
                EqualsIgnoreCase(hiCriterium)
            );

        /// <summary>
        /// Creates a new 'BetweenExclusive' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="loCriterium">The lowest of a pair of values used as comparison criteria</param>
        /// <param name="hiCriterium">The highest of a pair of values used as comparison criteria</param>
        public static ICriterinator<T> BetweenExclusive<T>(T loCriterium, T hiCriterium) where T : IComparable<T> =>
            And
            (
                GreaterThan(loCriterium),
                LessThan(hiCriterium)
            );

        /// <summary>
        /// Creates a new 'BetweenExclusive' criterinator based on the supplied criteria.
        /// </summary>
        /// <param name="loCriterium">The lowest of a pair of values used as comparison criteria</param>
        /// <param name="hiCriterium">The highest of a pair of values used as comparison criteria</param>
        public static ICriterinator<string> BetweenExclusiveIgnoreCase(string loCriterium, string hiCriterium) =>
            And
            (
                GreaterThanIgnoreCase(loCriterium),
                LessThanIgnoreCase(hiCriterium)
            );

        /// <summary>
        /// Creates a new 'IsNull' criterinator.
        /// </summary>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<T> IsNull<T>() where T : IComparable<T> =>
            new Criterinator<T>((item) => item == null);

        /// <summary>
        /// Creates a new 'IsNullOrEmpty' criterinator.
        /// </summary>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> IsNullOrEmpty() =>
            new Criterinator<string>((item) => string.IsNullOrEmpty(item));

        /// <summary>
        /// Creates a new 'IsNullOrWhiteSpace' criterinator.
        /// </summary>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> IsNullOrWhitespace() =>
            new Criterinator<string>((item) => string.IsNullOrWhiteSpace(item));

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="pattern">The regular expression against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> Regex(string pattern) =>
            new SingleCriterinator<string>(pattern, (item, ptrn) =>
                System.Text.RegularExpressions.Regex.IsMatch(item, ptrn)
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="pattern">The regular expression against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexIgnoreCase(string pattern) =>
            new SingleCriterinator<string>(pattern, (item, ptrn) =>
                System.Text.RegularExpressions.Regex.IsMatch(item, ptrn, RegexOptions.IgnoreCase)
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAny(IEnumerable<string> patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any(p => System.Text.RegularExpressions.Regex.IsMatch(item, p))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAny(string[] patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any(p => System.Text.RegularExpressions.Regex.IsMatch(item, p))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAnyIgnoreCase(IEnumerable<string> patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any(p => System.Text.RegularExpressions.Regex.IsMatch(item, p, RegexOptions.IgnoreCase))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAnyIgnoreCase(string[] patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any(p => System.Text.RegularExpressions.Regex.IsMatch(item, p, RegexOptions.IgnoreCase))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAll(IEnumerable<string> patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any() && ptrns.All(p => System.Text.RegularExpressions.Regex.IsMatch(item, p))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAll(string[] patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any() && ptrns.All(p => System.Text.RegularExpressions.Regex.IsMatch(item, p))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAllIgnoreCase(IEnumerable<string> patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any() && ptrns.All(p => System.Text.RegularExpressions.Regex.IsMatch(item, p, RegexOptions.IgnoreCase))
            );

        /// <summary>
        /// Creates a new 'Regex' criterinator based on the supplied pattern.
        /// </summary>
        /// <param name="patterns">The regular expressions against which to compare input items.</param>
        /// <returns>An <c>ICriterinator</c>.</returns>
        public static ICriterinator<string> RegexAllIgnoreCase(string[] patterns) =>
            new ManyCriterinator<string>(patterns, (item, ptrns) =>
                ptrns.Any() && ptrns.All(p => System.Text.RegularExpressions.Regex.IsMatch(item, p, RegexOptions.IgnoreCase))
            );

        ///// <summary>
        ///// Indicates whether the input <c>string</c> is a case-insensitive criteria match.
        ///// </summary>
        ///// <param name="inputItem">The item to compare against the criteria.</param>
        ///// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        ///// <param name="criteria">The criteria value(s) to compare against.</param>
        ///// <returns><c>true</c> or <c>false</c></returns>
        ///// <exception cref="ValidationException"></exception>
        //public static bool IsMatchIgnoreCase(string inputItem, CompareMode mode, params string[] criteria)
        //{
        //    return IsMatchInternal(inputItem, mode, criteria, ignoreCase: true);
        //}

        ///// <summary>
        ///// Indicates whether the input item is a criteria match.
        ///// </summary>
        ///// <typeparam name="T">The type of item to compare.</typeparam>
        ///// <param name="inputItem">The item to compare against the criteria.</param>
        ///// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        ///// <param name="criteria">The criteria value(s) to compare against.</param>
        ///// <returns><c>true</c> or <c>false</c></returns>
        ///// <exception cref="ValidationException"></exception>
        //public static bool IsMatch<T>(T inputItem, CompareMode mode, params T[] criteria) where T : IComparable<T>
        //{
        //    return IsMatchInternal(inputItem, mode, criteria, ignoreCase: false);
        //}

        //private static bool IsMatchInternal<T>(T inputItem, CompareMode mode, T[] criteria, bool ignoreCase) where T : IComparable<T>
        //{
        //    AssertCriteriaIsValid(mode, criteria);

        //    // special case 1 - null, empty or whitespace criteria
        //    switch (mode)
        //    {
        //        case CompareMode.IsNull:
        //            return inputItem == null;  // inputItem is not null
        //        case CompareMode.IsNullOrEmpty:
        //            return inputItem == null || (inputItem as string).Length == 0;
        //        case CompareMode.IsNullOrWhitespace:
        //            return inputItem == null || (inputItem as string).Trim().Length == 0;
        //    }

        //    // special case 2 - item to compare is null
        //    if (inputItem == null)
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.Equals:
        //                return criteria[0] == null;
        //            case CompareMode.EqualsAny:
        //                return criteria.Any(o => o == null);
        //            case CompareMode.NotEquals:
        //                return criteria[0] != null;
        //            case CompareMode.NotEqualsAny:
        //                return criteria.Any() && !criteria.Any(o => o == null);
        //        }
        //        return false;
        //    }

        //    // special case 3 - criteria is empty for XxxxAny modes
        //    if (!criteria.Any())
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.EqualsAny:
        //            case CompareMode.NotEqualsAny:
        //            case CompareMode.ContainsAny:
        //            case CompareMode.StartsWithAny:
        //            case CompareMode.EndsWithAny:
        //                return false;
        //        }
        //    }

        //    // special case 4 - string comparison with ignore case
        //    if (typeof(T) == typeof(string) && ignoreCase)
        //    {
        //        // switch out of order criteria for between comparisons
        //        switch (mode)
        //        {
        //            case CompareMode.Between:
        //            case CompareMode.BetweenExclusive:
        //            case CompareMode.NotBetween:
        //            case CompareMode.NotBetweenInclusive:
        //                if ((criteria[0] as string).ToUpper().CompareTo((criteria[1] as string).ToUpper()) > 0)
        //                {
        //                    (criteria[1], criteria[0]) = (criteria[0], criteria[1]);
        //                }
        //                break;
        //        }

        //        string inputItemString = inputItem as string;
        //        switch (mode)
        //        {
        //            case CompareMode.Equals:
        //                return string.Equals(inputItemString, criteria[0] as string, StringComparison.OrdinalIgnoreCase);
        //            case CompareMode.EqualsAny:
        //                return criteria.Any(c => string.Equals(c as string, inputItemString, StringComparison.OrdinalIgnoreCase));
        //            case CompareMode.NotEquals:
        //                return !string.Equals(inputItemString, criteria[0] as string, StringComparison.OrdinalIgnoreCase);
        //            case CompareMode.NotEqualsAny:
        //                return !criteria.Any(c => string.Equals(c as string, inputItemString, StringComparison.OrdinalIgnoreCase));
        //            case CompareMode.Contains:
        //                return inputItemString.IndexOf(criteria[0] as string, StringComparison.OrdinalIgnoreCase) > -1;
        //            case CompareMode.ContainsAny:
        //                return criteria.Any(c => inputItemString.IndexOf(c as string, StringComparison.OrdinalIgnoreCase) > -1);
        //            case CompareMode.ContainsAll:
        //                return criteria.All(c => inputItemString.IndexOf(c as string, StringComparison.OrdinalIgnoreCase) > -1);
        //            case CompareMode.StartsWith:
        //                return inputItemString.IndexOf(criteria[0] as string, StringComparison.OrdinalIgnoreCase) == 0;
        //            case CompareMode.StartsWithAny:
        //                return criteria.Any(c => inputItemString.IndexOf(c as string, StringComparison.OrdinalIgnoreCase) == 0);
        //            case CompareMode.EndsWith:
        //                return inputItemString.IndexOf(criteria[0] as string, StringComparison.OrdinalIgnoreCase) == inputItemString.Length - (criteria[0] as string).Length;
        //            case CompareMode.EndsWithAny:
        //                return criteria.Any(c => inputItemString.IndexOf(c as string, StringComparison.OrdinalIgnoreCase) == inputItemString.Length - (c as string).Length);
        //            case CompareMode.GreaterThan:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) > 0;
        //            case CompareMode.GreaterThanOrEquals:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) >= 0;
        //            case CompareMode.LessThan:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) < 0;
        //            case CompareMode.LessThanOrEquals:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) <= 0;
        //            case CompareMode.Between:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) >= 0
        //                    && inputItemString.ToUpper().CompareTo((criteria[1] as string).ToUpper()) <= 0;
        //            case CompareMode.BetweenExclusive:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) > 0
        //                    && inputItemString.ToUpper().CompareTo((criteria[1] as string).ToUpper()) < 0;
        //            case CompareMode.NotBetween:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) < 0
        //                    || inputItemString.ToUpper().CompareTo((criteria[1] as string).ToUpper()) > 0;
        //            case CompareMode.NotBetweenInclusive:
        //                return inputItemString.ToUpper().CompareTo((criteria[0] as string).ToUpper()) <= 0
        //                    || inputItemString.ToUpper().CompareTo((criteria[1] as string).ToUpper()) >= 0;
        //            case CompareMode.Regex:
        //                return System.Text.RegularExpressions.Regex.IsMatch(inputItemString, criteria[0] as string, RegexOptions.IgnoreCase);
        //            case CompareMode.RegexAny:
        //                return criteria.Any(c => System.Text.RegularExpressions.Regex.IsMatch(inputItemString, c as string, RegexOptions.IgnoreCase));
        //            case CompareMode.RegexAll:
        //                return criteria.All(c => System.Text.RegularExpressions.Regex.IsMatch(inputItemString, c as string, RegexOptions.IgnoreCase));
        //        }
        //        throw new ThisShouldNeverHappenException($"Oops, missed compare mode '{mode}' (ignore case) - please report this error to the Horseshoe.NET author(s)");
        //    }

        //    switch (mode)
        //    {
        //        case CompareMode.Equals:
        //            return Equals(inputItem, criteria[0]);
        //        case CompareMode.EqualsAny:
        //            return criteria.Any(c => Equals(c, inputItem));
        //        case CompareMode.NotEquals:
        //            return !Equals(inputItem, criteria[0]);
        //        case CompareMode.NotEqualsAny:
        //            return !criteria.Any(c => Equals(c, inputItem));
        //        case CompareMode.Contains:
        //            return (inputItem as string).IndexOf(criteria[0] as string) > -1;
        //        case CompareMode.ContainsAny:
        //            return criteria.Any(c => (inputItem as string).IndexOf(c as string) > -1);
        //        case CompareMode.ContainsAll:
        //            return criteria.All(c => (inputItem as string).IndexOf(c as string) > -1);
        //        case CompareMode.StartsWith:
        //            return (inputItem as string).IndexOf(criteria[0] as string) == 0;
        //        case CompareMode.StartsWithAny:
        //            return criteria.Any(c => (inputItem as string).IndexOf(c as string) == 0);
        //        case CompareMode.EndsWith:
        //            return (inputItem as string).IndexOf(criteria[0] as string) == (inputItem as string).Length - (criteria[0] as string).Length;
        //        case CompareMode.EndsWithAny:
        //            return criteria.Any(c => (inputItem as string).IndexOf(c as string) == (inputItem as string).Length - (c as string).Length);
        //        case CompareMode.GreaterThan:
        //            return inputItem.CompareTo(criteria[0]) > 0;
        //        case CompareMode.GreaterThanOrEquals:
        //            return inputItem.CompareTo(criteria[0]) >= 0;
        //        case CompareMode.LessThan:
        //            return inputItem.CompareTo(criteria[0]) < 0;
        //        case CompareMode.LessThanOrEquals:
        //            return inputItem.CompareTo(criteria[0]) <= 0;
        //        case CompareMode.Between:
        //            return inputItem.CompareTo(criteria[0]) >= 0
        //                && inputItem.CompareTo(criteria[1]) <= 0;
        //        case CompareMode.BetweenExclusive:
        //            return inputItem.CompareTo(criteria[0]) > 0
        //                && inputItem.CompareTo(criteria[1]) < 0;
        //        case CompareMode.NotBetween:
        //            return inputItem.CompareTo(criteria[0]) < 0
        //                || inputItem.CompareTo(criteria[1]) > 0;
        //        case CompareMode.NotBetweenInclusive:
        //            return inputItem.CompareTo(criteria[0]) <= 0
        //                || inputItem.CompareTo(criteria[1]) >= 0;
        //        case CompareMode.Regex:
        //            return System.Text.RegularExpressions.Regex.IsMatch(inputItem as string, criteria[0] as string);
        //        case CompareMode.RegexAny:
        //            return criteria.Any(c => System.Text.RegularExpressions.Regex.IsMatch(inputItem as string, c as string));
        //        case CompareMode.RegexAll:
        //            return criteria.All(c => System.Text.RegularExpressions.Regex.IsMatch(inputItem as string, c as string));
        //    }
        //    throw new ThisShouldNeverHappenException($"Oops, missed compare mode '{mode}' - please report this error to the Horseshoe.NET author(s)");
        //}

        ///// <summary>
        ///// Validates whether the criteria is valid (type, quantity and content).
        ///// </summary>
        ///// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        ///// <param name="criteria">The criteria value(s) to compare against.</param>
        ///// <exception cref="AssertionFailedException"></exception>
        //public static void AssertCriteriaIsValid<T>(CompareMode mode, T[] criteria)
        //{
        //    AssertCriteriaIsValid(mode, criteria, out _);
        //}

        ///// <summary>
        ///// Validates whether the criteria is valid (type, quantity and content).
        ///// </summary>
        ///// <param name="mode">The compare mode, e.g. Equals, Contains, Between, etc.</param>
        ///// <param name="criteria">The criteria value(s) to compare against.</param>
        ///// <param name="vAction">Alerts client code to perform the action identified by the validator, if any (for example, when the between hi and lo criteria are switched).</param>
        ///// <exception cref="AssertionFailedException"></exception>
        //public static void AssertCriteriaIsValid<T>(CompareMode mode, T[] criteria, out ValidationFlaggedAction vAction)
        //{
        //    criteria = criteria ?? Array.Empty<T>();
        //    vAction = ValidationFlaggedAction.None;

        //    if (criteria.Any(o => o == null))
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.Equals:
        //            case CompareMode.EqualsAny:
        //            case CompareMode.NotEquals:
        //            case CompareMode.NotEqualsAny:
        //                break;
        //            default:
        //                throw new AssertionFailedException($"Invalid criteria: compare mode '{mode}' only allows non-null criteria");
        //        }
        //    }

        //    if (!criteria.Any())
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.IsNull:
        //            case CompareMode.IsNullOrEmpty:
        //            case CompareMode.IsNullOrWhitespace:
        //            case CompareMode.EqualsAny:      // allowing zero criteria - will simply be a non-match
        //            case CompareMode.NotEqualsAny:   // allowing zero criteria - will simply be a non-match
        //            case CompareMode.ContainsAny:    // allowing zero criteria - will simply be a non-match
        //            case CompareMode.StartsWithAny:  // allowing zero criteria - will simply be a non-match
        //            case CompareMode.EndsWithAny:    // allowing zero criteria - will simply be a non-match
        //                break;
        //            default:
        //                throw new AssertionFailedException($"Invalid criteria: compare mode '{mode}' not compatible with 0 criteria");
        //        }
        //    }

        //    if (criteria.Length != 1)
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.Equals:
        //            case CompareMode.Contains:
        //            case CompareMode.StartsWith:
        //            case CompareMode.EndsWith:
        //            case CompareMode.GreaterThan:
        //            case CompareMode.GreaterThanOrEquals:
        //            case CompareMode.LessThan:
        //            case CompareMode.LessThanOrEquals:
        //            case CompareMode.Regex:
        //                throw new AssertionFailedException($"Invalid criteria: compare mode '{mode}' requires exactly 1 criterium; found: {criteria.Length}");
        //        }
        //    }

        //    if (criteria.Length != 2)
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.Between:
        //            case CompareMode.BetweenExclusive:
        //            case CompareMode.NotBetween:
        //            case CompareMode.NotBetweenInclusive:
        //                throw new AssertionFailedException($"Invalid criteria: compare mode '{mode}' requires exactly 2 criteria; found: {criteria.Length}");
        //        }
        //    }

        //    if (typeof(T) != typeof(string))
        //    {
        //        switch (mode)
        //        {
        //            case CompareMode.Contains:
        //            case CompareMode.ContainsAny:
        //            case CompareMode.ContainsAll:
        //            case CompareMode.StartsWith:
        //            case CompareMode.StartsWithAny:
        //            case CompareMode.EndsWith:
        //            case CompareMode.EndsWithAny:
        //            case CompareMode.Regex:
        //            case CompareMode.RegexAny:
        //            case CompareMode.RegexAll:
        //                throw new AssertionFailedException($"Invalid criteria: compare mode '{mode}' requires criteria of type {typeof(string).FullName}; found: {typeof(T).FullName}");
        //        }
        //    }
        //}
    }
}
