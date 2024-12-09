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
        /// Increase indentation (default convention is 2 spaces).  Implementations should apply 
        /// the same indentation to subsequent messages.
        /// </summary>
        Increment,

        /// <summary>
        /// Increase indentation for subsequent messages (default convention is 2 spaces). 
        /// </summary>
        IncrementNext,

        /// <summary>
        /// Decrease indentation (default convention is 2 spaces).  Implementations should apply 
        /// the same indentation to subsequent messages.
        /// </summary>
        Decrement,

        /// <summary>
        /// Decrease indentation for subsequent messages (default convention is 2 spaces). 
        /// </summary>
        DecrementNext,

        /// <summary>
        /// Restore indentation to last explicitly set level
        /// <example>
        /// <code>
        /// // Example
        /// relay.Message("Indented 6 spaces", indent: 6);
        /// relay.Message("Indented > 6 spaces", indentHint: IndentHint.Increment);
        /// relay.Message("Indented 6 spaces", indentHint: IndentHint.Restore);
        /// 
        /// relay.Message("Indented 0 spaces", indentHint: IndentHint.Reset);
        /// relay.Message("Indented > 0 spaces", indentHint: IndentHint.Increment);
        /// relay.Message("Indented 0 spaces", indentHint: IndentHint.Restore);
        /// </code>
        /// </example>
        /// </summary>
        Restore
    }
}
