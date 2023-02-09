using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// <c>PromptX.Menu()</c> returns instances of this class when a menu item is selected
    /// </summary>
    /// <typeparam name="T">a menu item type</typeparam>
    public class MenuSelection<T>
    {
        /// <summary>
        /// The selected menu item (the actual object), otherwise null if arbitary input was allowed and has been input by the user.
        /// </summary>
        public T SelectedItem { get; set; }

        /// <summary>
        /// The 1-based index of the selected menu item, if 0 then arbitrary text was entered or multiple indexes were selected 
        /// </summary>
        public int SelectedIndex { get; set; }

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

    internal static class MenuSelection
    { 
        static readonly Regex NumericRangePattern = new Regex("^[0-9]+(-[0-9]+)?,?", RegexOptions.IgnoreCase);

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
