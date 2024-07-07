namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Ways <c>ConsoleX</c> can render an input prompt
    /// </summary>
    public enum PromptType
    {
        /// <summary>
        /// Applies ":", "(required)", " " and/or ">" intelligently to the prompt supplied by the user to improve readability
        /// </summary>
        Auto,
        /// <summary>
        /// Displays "> " for menu and list selection
        /// </summary>
        MenuOrList,
        /// <summary>
        /// Uses the prompt supplied by the user with no alteration
        /// </summary>
        Literal,
        /// <summary>
        /// Intelligently applies "[y/n]", "[Y/n]" or "[y/N]" the prompt supplied by the user depending on the default value.
        /// </summary>
        Bool
    }
}
