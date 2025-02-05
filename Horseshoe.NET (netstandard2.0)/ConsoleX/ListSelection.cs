using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A container class for returning selected list item info from <c>PromptX.List()</c>.
    /// </summary>
    /// <typeparam name="T">Type of list item.</typeparam>
    public class ListSelection<T>
    {
        /// <summary>
        /// The first selected list (or menu) item, <c>null</c> is allowed.
        /// </summary>
        public T SelectedItem => Selection.Any() ? Selection.First().Value : default;

        /// <summary>
        /// The first 0 or 1-based index of the selected list item(s) or 1-based index of the selected menu item(s).  
        /// Other allowed values include <c>-1</c>, the autoreturned value if list has 0 elements or 
        /// menu selection represents any return value other a menu item.
        /// </summary>
        public int SelectedIndex => Selection.Any() ? Selection.First().Key : -1;

        /// <summary>
        /// The storage container for multiple results
        /// </summary>
        public IDictionary<int, T> Selection { get; } = new Dictionary<int, T>();

        /// <summary>
        /// The 0 or 1-based indices of the selected list (or menu) items. 
        /// </summary>
        public int[] SelectedIndices => Selection.Keys.ToArray();

        /// <summary>
        /// The selected menu items (the actual objects).
        /// </summary>
        public T[] SelectedItems => Selection.Values.ToArray();
    }
}
