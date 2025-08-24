using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Comparison;

namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// A way to dynamically filter relayed messages by group (e.g. namespaces).
    /// </summary>
    public class GroupFilter
    {
        /// <summary>
        /// A function that determines whether a given message relay group satisfies the filter criteria.
        /// </summary>
        public Func<string, bool> Filter { get; }

        /// <summary>
        /// A list of message relay groups (e.g. namespaces) whose messages to process.
        /// </summary>
        public StringValues Groups { get; }

        /// <summary>
        /// A message relay group matching strategy (i.e. 'Equals', 'Contains', 'StartsWith' or 'EndsWith').
        /// </summary>
        public LikeMode LikeMode { get; }

        /// <summary>
        /// Whether to take case sensitivity into account when matching message relay groups to the criteria.
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupFilter"/> class with the specified filter function.
        /// </summary>
        /// <param name="filter">A function that determines whether a given group satisfies the filter criteria.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is <see langword="null"/>.</exception>
        public GroupFilter(Func<string, bool> filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        /// <summary>
        /// Creates a new <c>GroupFilter</c> instance based on the specified message relay groups (e.g. namespaces).
        /// </summary>
        /// <param name="groups">A list of message relay groups (e.g. namespace) whose messages to process.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public GroupFilter(params string[] groups)
            : this(groups, LikeMode.Equals, true) { }

        /// <summary>
        /// Creates a new <c>GroupFilter</c> instance based on the specified message relay groups (e.g. namespaces).
        /// </summary>
        /// <param name="groups">A list of message relay groups (e.g. namespaces) whose messages to process.</param>
        /// <param name="likeMode">A message relay group matching strategy (i.e. 'Equals', 'Contains', 'StartsWith' or 'EndsWith').</param>
        /// <param name="ignoreCase">Whether to take case sensitivity into account when matching message relay groups to the criteria.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public GroupFilter(StringValues groups, LikeMode likeMode = LikeMode.Equals, bool ignoreCase = false)
        {
            Groups = groups;
            if (Groups.Any(g => g == null))
                throw new ArgumentException("Message relay groups cannot contain null values", nameof(groups));
            LikeMode = likeMode;
            IgnoreCase = ignoreCase;
        }

        /// <summary>
        /// Determines whether the specified message relay group matches the filter criteria.
        /// </summary>
        /// <param name="messageRelayGroup">The name of the message relay group to evaluate. If <c>null</c>, it is treated as an empty string.</param>
        /// <returns>
        /// <see langword="true"/> if the specified message relay group matches the filter criteria; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsMatch(string messageRelayGroup)
        {
            messageRelayGroup = messageRelayGroup ?? string.Empty;

            if (Filter != null)
                return Filter.Invoke(messageRelayGroup);

            //if (!Groups.Any())
            //    return true;

            switch (LikeMode)
            {
                case LikeMode.Equals:
                default:
                    return IgnoreCase
                        ? Groups.Any(group => string.Equals(messageRelayGroup, group, StringComparison.OrdinalIgnoreCase))
                        : Groups.Any(group => string.Equals(messageRelayGroup, group));
                case LikeMode.Contains:
                    return IgnoreCase
                        ? Groups.Any(group => messageRelayGroup.ToUpper().Contains(group.ToUpper()))
                        : Groups.Any(group => messageRelayGroup.Contains(group));
                case LikeMode.StartsWith:
                    return IgnoreCase
                        ? Groups.Any(group => messageRelayGroup.StartsWith(group, StringComparison.OrdinalIgnoreCase))
                        : Groups.Any(group => messageRelayGroup.StartsWith(group));
                case LikeMode.EndsWith:
                    return IgnoreCase
                        ? Groups.Any(group => messageRelayGroup.EndsWith(group, StringComparison.OrdinalIgnoreCase))
                        : Groups.Any(group => messageRelayGroup.EndsWith(group));
            }
        }
    }
}
