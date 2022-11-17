namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// An internal class for preventing prompt deadlock, i.e. sets menus / lists unselectable
    /// if they contain zero items.  Otherwise users would have nothing to select at the prompt.
    /// </summary>
    public class MenuAndListRealtimeConfigurator
    {
        internal bool IsNotSelectable { get; private set; }

        internal void SetNotSelectable()
        {
            IsNotSelectable = true;
        }
    }
}
