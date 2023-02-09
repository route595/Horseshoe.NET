namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// An internal class for preventing prompt deadlock, i.e. sets menus / lists unselectable
    /// if they contain zero items.  Otherwise users would have nothing to select at the prompt.
    /// </summary>
    public class MenuAndListRealtimeConfigurator
    {
        /// <summary>
        /// A flag indicating whether the list to display is selectable e.g. has more than zero items.
        /// </summary>
        public bool IsNotSelectable { get; private set; }

        /// <summary>
        /// Used internally to set <c>IsNotSelectable = true</c>.
        /// </summary>
        public void SetNotSelectable()
        {
            IsNotSelectable = true;
        }
    }
}
