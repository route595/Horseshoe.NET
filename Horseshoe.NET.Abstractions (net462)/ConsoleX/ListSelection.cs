namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A container class for returning selected list item info from <c>PromptX.List()</c>.
    /// </summary>
    /// <typeparam name="T">Type of list item.</typeparam>
    public class ListSelection<T>
    {
        /// <summary>
        /// The selected list (or menu) item.  Could be <c>null</c> if menu selection allows (and results in) arbitary input.
        /// </summary>
        public T SelectedItem { get; set; }

        /// <summary>
        /// The 0 or 1-based index of the selected list item or 1-based index of the selected menu item.  
        /// Could also be <c>-1</c> the autoreturned value if list has 0 elements or <c>0</c> if 
        /// menu selection allows (and results in) arbitary input.
        /// </summary>
        public int SelectedIndex { get; set; }
    }
}
