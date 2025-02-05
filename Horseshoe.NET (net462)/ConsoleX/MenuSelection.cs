namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A container class for returning selected menu item info from <c>PromptX.Menu()</c>.
    /// </summary>
    /// <typeparam name="T">a menu item type</typeparam>
    public class MenuSelection<T> : ListSelection<T>
    {
        ///// <summary>
        ///// True if 'All' was entered at the prompt for a multi-select menu, false otherwise.
        ///// </summary>
        //public bool SelectedAll { get; set; }

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
                if (SelectedItem is RoutineX routine)
                {
                    return routine;
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
}
