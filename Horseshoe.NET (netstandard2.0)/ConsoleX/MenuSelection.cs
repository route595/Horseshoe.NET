using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A container class for returning selected menu item info from <c>PromptX.Menu()</c>.
    /// </summary>
    /// <typeparam name="T">a menu item type</typeparam>
    public class MenuSelection<T> : ListSelection<T>
    {
        /// <summary>
        /// The storage container for multiple results
        /// </summary>
        public IDictionary<int, T> MultipleSelection { get; set; }

        /// <summary>
        /// The 1-based indices of the selected menu items. 
        /// </summary>
        public int[] SelectedIndices => MultipleSelection?.Keys.ToArray();

        /// <summary>
        /// The 0-based indices of the selected menu items. 
        /// </summary>
        public int[] SelectedSourceIndices => MultipleSelection?.Keys.Select(i => i - 1).ToArray();

        /// <summary>
        /// The selected menu items (the actual objects).
        /// </summary>
        public T[] SelectedItems => MultipleSelection?.Values.ToArray();

        /// <summary>
        /// True if 'All' was entered at the prompt for a multi-select menu, false otherwise.
        /// </summary>
        public bool SelectedAll { get; set; }

        /// <summary>
        /// Returns the selected <c>Routine</c> (custom menu item or menu item)
        /// </summary>
        public RoutineX SelectedRoutine
        {
            get
            {
                if (CustomMenuItem is RoutineX cRoutine)
                {
                    return cRoutine;
                }
                if (SelectedItem is RoutineX moRoutine)
                {
                    return moRoutine;
                }
                return null;
            }
        }

        /// <summary>
        /// Arbitrary input if allowed and has been input by the user.
        /// </summary>
        public string ArbitraryInput { get; set; }

        /// <summary>
        /// The custom routine selected by the user via <c>Routine.Command</c>.
        /// </summary>
        public RoutineX CustomMenuItem { get; set; }
    }

    public static class MenuSelection
    {
        static readonly Regex NumericRangePattern = new Regex("^[0-9]+(-[0-9]+)?,?", RegexOptions.IgnoreCase);
        static readonly Regex NumericRangesPattern = new Regex("^([,]*[0-9]+(-[0-9]+)?)*[,]*$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Extracts a list of multiple selections from user input.  
        /// </summary>
        /// <param name="input">User input.</param>
        /// <param name="count">The count of the collection being multi-selected. Used in generating errors.</param>
        /// <param name="oneBased">If <c>true</c>, indices input by user are one-based.  Default is <c>false</c>.</param>
        /// <returns>A list of indices (0 or 1-based matches input)</returns>
        public static IEnumerable<int> ParseMultipleSelectionIndices(string input, int count, bool oneBased)
        {
            var indices = new List<int>();
            int[] minMax;
            switch (input?.ToLower() ?? "")
            {
                case "":
                case "none":
                    break;
                case "all":
                    for (var i = oneBased ? 1 : 0; i <= (oneBased ? count : count - 1); i++)
                    {
                        indices.Add(i);
                    }
                    break;
                default:
                    if (!NumericRangesPattern.IsMatch(input))
                    {
                        throw new ValidationException("Invalid multiple selection: " + input + " (try one of these: \"\", \"none\", \"all\", \"5\", \"5,7-10\")");
                    }
                    var ranges = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);  // 1-3,6,,9-10 -> [1-3][6][9-10]
                    foreach (var range in ranges)
                    {
                        try
                        {
                            minMax = range.Split('-')
                                .Select(s => int.Parse(s.Trim()))
                                .ToArray();
                        }
                        catch (FormatException)
                        {
                            throw new ValidationException("Invalid range: \"" + range + "\" (valid examples: \"5\", \"7-10\")");
                        }
                        switch(minMax.Length)
                        {
                            case 1:
                                ValidateSelectedIndex(minMax[0], count, oneBased);
                                indices.Add(minMax[0]);
                                break;
                            case 2:
                                if (minMax[0] > minMax[1])
                                {
                                    throw new ValidationException("Invalid range: \"" + range + "\" (valid example: \"7-10\" -- invalid example: \"10-7\")");
                                }
                                for (var i = minMax[0]; i <= minMax[1]; i++)
                                {
                                    ValidateSelectedIndex(i, count, oneBased);
                                    indices.Add(i);
                                }
                                break;
                            default:                                                                           // splitting on ',' with StringSplitOptions.RemoveEmptyEntries precludes 0
                                throw new ThisShouldNeverHappenException("minMax.Length = " + minMax.Length);  // NumericRangesPattern regex matching precludes 3+
                        }
                    }
                    break;
            }
            return indices;
        }

        private static void ValidateSelectedIndex(int index, int count, bool oneBased)
        {
            if (oneBased)
            {
                if (index <= 0 || index > count)
                {
                    throw new IndexOutOfRangeException("The selected index is out of range [1 - " + count + "]: " + index);
                }
            }
            else if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException("The selected index is out of range [0 - " + (count - 1) + "]: " + index);
            }
        }

        internal static IEnumerable<int> ParseMultipleIndexes(string input, int menuItemsCount, out bool all)
        {
            input = input.Trim();
            all = false;

            var indexes = new List<int>(); // 1-based indexes
            var except = false;

            if (input.ToLower().StartsWith("all"))
            {
                for (int i = 1; i <= menuItemsCount; i++) indexes.Add(i);
                all = true;
                input = input.Substring(3).Trim();

                if (input.ToUpper().StartsWith("X"))
                {
                    except = true;
                    input = input.Substring(1).Trim();
                }
                else if (input.ToUpper().StartsWith("EXCEPT"))
                {
                    except = true;
                    input = input.Substring(6).Trim();
                }
                else if (input.Length > 0)
                {
                    throw new BenignException("invalid input: " + input);
                }
            }

            input = TextClean.RemoveAllWhitespace(input);

            var match = NumericRangePattern.Match(input);

            while (match.Success)
            {
                var token = match.Value.Replace(",", "");
                var ints = token
                    .Split('-')
                    .Select(s => int.Parse(s))
                    .ToArray();

                if (ints.Length == 1)
                {
                    if (ints[0] <= menuItemsCount)
                    {
                        if (except)
                        {
                            indexes.Remove(ints[0]);
                        }
                        else
                        {
                            indexes.Add(ints[0]);
                        }
                    }
                    else throw new BenignException("invalid selection: " + token);
                }
                else
                {
                    if (ints[1] <= menuItemsCount)
                    {
                        for (int i = ints[0]; i <= ints[1]; i++)
                        {
                            if (except)
                            {
                                indexes.Remove(i);
                            }
                            else
                            {
                                indexes.Add(i);
                            }
                        }
                    }
                    else throw new BenignException("invalid range: " + token);
                }

                input = input.Replace(match.Value, "");
                match = NumericRangePattern.Match(input);
            }

            if (input.Length > 0)
            {
                throw new BenignException("invalid input: " + input);
            }

            return indexes;
        }
    }
}
