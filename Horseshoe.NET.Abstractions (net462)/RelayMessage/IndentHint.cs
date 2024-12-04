namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// A way for code to suggest indentation of relayed messages
    /// </summary>
    public enum IndentHint
    {
        /// <summary>
        /// Reset indentation to zero
        /// </summary>
        Reset,

        /// <summary>
        /// Increase indentation (recommendation is 2 spaces).  Implementations should apply 
        /// the same indentation to subsequent messages.
        /// </summary>
        Increment,

        /// <summary>
        /// Increase indentation for subsequent messages (recommendation is 2 spaces). 
        /// </summary>
        IncrementNext,

        /// <summary>
        /// Decrease indentation (recommendation is 2 spaces).  Implementations should apply 
        /// the same indentation to subsequent messages.
        /// </summary>
        Decrement,

        /// <summary>
        /// Decrease indentation for subsequent messages (recommendation is 2 spaces). 
        /// </summary>
        DecrementNext
    }
}
