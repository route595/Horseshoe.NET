using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Utility methods for handling user input
    /// </summary>
    public static class UserInputUtil
    {
        static readonly Regex NumericRangePattern = new Regex("^[0-9]+-[0-9]+$", RegexOptions.IgnoreCase);

        /// <summary>
        /// The maximum allowable index from user input.  This cap exists to prevent malicious input from incurring long 
        /// running loops causing lag by unnecessarily tasking the CPU and spiking memory usage.
        /// </summary>
        public static int MaxIndex { get; private set; } = 5000;

        private const int ExtremeMaxIndex = 100000;

        /// <summary>
        /// Set the max index allowable by user input parser. 
        /// </summary>
        /// <param name="maxIndex">The new max index</param>
        /// <exception cref="ArgumentException"></exception>
        public static void SetMaxIndex(int maxIndex)
        {
            if (maxIndex > ExtremeMaxIndex)
                throw new ArgumentException(nameof(maxIndex) + " cannot exceed " + ExtremeMaxIndex);
            MaxIndex = maxIndex;
        }

        /// <summary>
        /// Parses multiple indexes from user input based on a range of possible indexes.
        /// <para>Examples</para>
        /// <code>
        /// ParseMultipleIndices("all", 10);  // result: [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ]
        /// ParseMultipleIndices("all", 10, indexStyle: ListIndexPolicy.ZeroBased);  // result: [ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 ]
        /// ParseMultipleIndices("1-5", 10);  // result: [ 1, 2, 3, 4, 5 ]
        /// ParseMultipleIndices("all except 1-5", 10);  // result: [ 6, 7, 8, 9, 10 ]
        /// ParseMultipleIndices("0-10", 10);  // IndexOutOfRangeException
        /// ParseMultipleIndices("1-r;", 10);  // ValidationException
        /// ParseMultipleIndices("10-1", 10);  // ValidationException
        /// </code>
        /// </summary>
        /// <param name="input">A <c>string</c> input by the user.  Examples; "none", "all", "1-5,7", "all except 2-4"</param>
        /// <param name="contiguousIndexCount">Simple collection indexing, see example.</param>
        /// <param name="indexStyle">Whether the displayed index and the index input by the user is 0 or 1-based (default is 1-based).</param>
        /// <returns>The set of valid indices parsed from user input.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<int> ParseMultipleIndices(string input, int contiguousIndexCount, ListIndexStyle indexStyle = ListIndexStyle.OneBased)
        {
            if (indexStyle == ListIndexStyle.ZeroBased)
                return ParseMultipleIndices(input, Enumerable.Range(0, contiguousIndexCount - 1));
            if (indexStyle == ListIndexStyle.OneBased)
                return ParseMultipleIndices(input, Enumerable.Range(1, contiguousIndexCount));
            throw new ArgumentException("Unusable list index policy: " + indexStyle, nameof(indexStyle));
        }

        /// <summary>
        /// Parses multiple indexes from user input based on a set of possible indexes.
        /// <para>Examples</para>
        /// <code>
        /// ParseMultipleIndices("all",  [ 1, 2, 3, 4, 5, 6 ]);  // result: [ 1, 2, 3, 4, 5, 6 ]
        /// ParseMultipleIndices("1-5",  [ 1, 2, 3, 4, 5, 6 ]);  // result: [ 1, 2, 3, 4, 5 ]
        /// ParseMultipleIndices("all except 1-5", [ 1, 2, 3, 4, 5, 6 ]);  // result: [ 6 ]
        /// ParseMultipleIndices("0-6",  [ 1, 2, 3, 4, 5, 6 ]);  // IndexOutOfRangeException
        /// ParseMultipleIndices("1-r;", [ 1, 2, 3, 4, 5, 6 ]);  // ValidationException
        /// ParseMultipleIndices("6-1",  [ 1, 2, 3, 4, 5, 6 ]);  // ValidationException
        /// </code>
        /// </summary>
        /// <param name="input">A <c>string</c> input by the user.  Examples; "none", "all", "1-5,7", "all except 2-4"</param>
        /// <param name="possibleIndices">Index selection is dependent on this set of values.  Examples: [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ] or [ 2, 3, 5, 8, 13 ]</param>
        /// <returns>The set of valid indices parsed from user input.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<int> ParseMultipleIndices(string input, IEnumerable<int> possibleIndices)
        {
            // some validation
            if (possibleIndices == null || !possibleIndices.Any())
                throw new ArgumentNullException(nameof(possibleIndices), nameof(possibleIndices) + " cannot be null or empty");
            if (possibleIndices.Any(i => i > MaxIndex))
                throw new ArgumentException(nameof(possibleIndices) + " cannot contain values over the current max index: " + MaxIndex, nameof(possibleIndices));
            if (possibleIndices.Any(i => i < 0))
                throw new ArgumentException(nameof(possibleIndices) + " cannot contain negative values", nameof(possibleIndices));

            // parse user indices
            var parsedIndices = new List<int>();
            ParseMultipleIndices(input, parsedIndices, out bool all, out bool except);  // nothing returned here exceeds max index

            // validation and optimization
            //if (parsedIndices.Any(i => i > MaxIndex))
            //    throw new ArgumentException("client cannot input values over the current max index: " + MaxIndex, nameof(parsedIndices));
            if (parsedIndices.Any(i => i < 0))
                throw new ThisShouldNeverHappenException("parser returns only positive numbers");
            if (all)
            {
                if (except)
                {
                    if (!parsedIndices.Any())
                        throw new ValidationException("\"all except\" must be followed by at least one index or range");
                }
                else
                {
                    if (parsedIndices.Any())
                        throw new ValidationException("\"all\" may not be followed by any index or range");
                    return possibleIndices;
                }
            }
            //else if (!parsedIndices.Any())
            //    throw new ValidationException("user must input at least one index or range");

            // check invalid user selections and, if any, throw exception
            var invalidIndices = new List<int>();
            foreach (int index in parsedIndices)
            {
                if (!possibleIndices.Contains(index))
                {
                    invalidIndices.Add(index);
                }
            }
            if (invalidIndices.Any())
            {
                if (invalidIndices.Count == 1)
                    throw new IndexOutOfRangeException("Invalid index [" + invalidIndices.Single() + "] in input: " + input);
                
                // combine adjacent indices for exception message
                invalidIndices.Sort();
                var strb = new StringBuilder();
                int first = -1;
                int last = -2;
                for (int i = 0; i < invalidIndices.Count; i++)
                {
                    var index = invalidIndices[i];
                    if (i == 0)
                    {
                        first = index;                               // start a new sequence
                        last = index;
                    }
                    else if (index - 1 != last)                      // if not next in sequence...
                    {
                        if (last > first)
                            strb.Append(", " + first + "-" + last);      // render the last sequence
                        else
                            strb.Append(", " + last);                    // render the last single index
                        first = index;                                   // start a new sequence
                    }
                    last = index;
                    if (i == invalidIndices.Count - 1)               // if last loop...
                    {
                        if (last > first)
                            strb.Append(", " + first + "-" + last);  // render the last sequence
                        else
                            strb.Append(", " + last);                // render the last single index
                    }
                }
                //if (last > first)
                //    strb.Append(", " + first + "-" + last);
                //else
                //    strb.Append(", " + last);
                strb.Remove(0, 2);
                throw new IndexOutOfRangeException("Invalid indices [" + TextUtil.Crop(strb.ToString(), 50, position: HorizontalPosition.Center, truncateMarker: TruncateMarker.LongEllipsis) + "] in input: " + input);
            }

            if (all)
            {
                var allIndicesExcept = CollectionUtil.ToList(possibleIndices, optimize: Optimization.ReuseCollection);
                if (except)
                {
                    foreach (var index in parsedIndices)
                    {
                        allIndicesExcept.Remove(index);
                    }
                }
                return allIndicesExcept;
            }
            return parsedIndices;
        }

        /// <summary>
        /// Parses multiple indexes from user input.
        /// </summary>
        /// <param name="input">A <c>string</c> input by the user.  Examples; "none", "all", "1-5,7", "all except 2-4"</param>
        /// <param name="parsedIndices">The collection used for storing the parsed indices.</param>
        /// <param name="all">Indicates the user has input some case variation of "all" meaning to select all the possible indices</param>
        /// <param name="except">Indicates the user has input some case variation of "all except" meaning the parsed indices are to be excluded.</param>
        /// <returns>The set of valid indices parsed from user input.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static void ParseMultipleIndices(string input, List<int> parsedIndices, out bool all, out bool except)
        {
            all = false;
            except = false;

            if (string.IsNullOrWhiteSpace(input))
                return;

            input = input.Trim();                        // e.g. "15 - 21, 38", "all", "all   except   15 - 21, 38", etc.

            // eliminate/compress white spaces           // e.g. "15-21,38", "all except 15-21,38", etc.
            while (input.Contains("  ") || input.Contains(" -") || input.Contains("- ") || input.Contains(" ,") || input.Contains(", "))
                input = input.Replace("  ", " ").Replace(" -", "-").Replace("- ", "-").Replace(" ,", ",").Replace(", ", ",");

            var parts = input.Split(' ', ',').ToList();  // e.g. [ "15-21", "38" ], [ "all" ], [ "all", "except", "15-21", "38" ]

            if (string.Equals(parts[0], "none", StringComparison.OrdinalIgnoreCase))
                return;

            if (string.Equals(parts[0], "all", StringComparison.OrdinalIgnoreCase))
            {
                all = true;
                parts.RemoveAt(0);

                if (parts.Any() && string.Equals(parts[0], "except", StringComparison.OrdinalIgnoreCase))
                {
                    except = true;
                    parts.RemoveAt(0);
                }
            }

            int[] _indices_min_max;
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int index))
                {
                    if (index > MaxIndex)
                        throw new ValidationException("Invalid index \"" + part + "\" in input, cannot exceed max index: " + input + " > " + MaxIndex);
                    parsedIndices.Add(index);
                }

                else if (NumericRangePattern.IsMatch(part))
                {
                    _indices_min_max = part.Split('-')
                        .Select(sp => int.Parse(sp))
                        .ToArray();

                    if (_indices_min_max[0] > _indices_min_max[1])
                        throw new ValidationException("Invalid index range \"" + part + "\" in input: " + input + " - expected format: \"" + _indices_min_max[1] + "-" + _indices_min_max[0] + "\" (ascending order)");
                    if (_indices_min_max[1] > MaxIndex)
                        throw new ValidationException("Invalid index range \"" + part + "\" in input, cannot exceed max index: " + _indices_min_max[1] + " > " + MaxIndex);

                    parsedIndices.AddRange(Enumerable.Range(_indices_min_max[0], _indices_min_max[1] - _indices_min_max[0] + 1));
                }

                else
                    throw new ValidationException("Invalid index range \"" + part + "\" in input: " + input + " - expected format: \"2\" (index), \"1-5\" (range) or \"all [except]\" (recognized alias)");
            }
        }
    }
}
