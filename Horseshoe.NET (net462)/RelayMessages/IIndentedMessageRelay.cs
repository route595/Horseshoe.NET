namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// Base utility interface for bundling <c>RelayMessage</c> and <c>RelayException</c> instances
    /// </summary>
    public interface IIndentedMessageRelay : IMessageRelay
    {

        /// <summary>
        /// Indicates how many spaces of indentation to render.
        /// </summary>
        int IndentLevel { get; }

        /// <summary>
        /// Indicates how many spaces to increment or decrement, default convention is 2 spaces.
        /// </summary>
        int IndentInterval { get; }

        /// <summary>
        /// If <c>true</c>, indents exceptions at the same level as the last relayed message.
        /// Default is <c>false</c>.
        /// </summary>
        bool IndentExceptionsInlineWithMessages { get; }

        /// <summary>
        /// Sets the indentation level
        /// </summary>
        void SetIndentation(int indentLevel);

        /// <summary>
        /// Increases the indentation level
        /// </summary>
        void IncrementIndentation();

        /// <summary>
        /// Decreases the indentation level
        /// </summary>
        void DecrementIndentation();

        /// <summary>
        /// Restore indentation to last explicitly set level
        /// <example>
        /// <code>
        /// // Example
        /// relay.SetIndentation(6);
        /// relay.Message("Indented 6 spaces");
        /// relay.IncrementIndentation();
        /// relay.Message("Indented > 6 spaces");
        /// relay.RestoreIndentation();
        /// relay.Message("Indented 6 spaces");
        /// 
        /// relay.ResetIndentation();
        /// relay.Message("Indented 0 spaces");
        /// relay.IncrementIndentation();
        /// relay.Message("Indented > 0 spaces");
        /// relay.RestoreIndentation();
        /// relay.Message("Indented 0 spaces");
        /// </code>
        /// </example>
        /// </summary>
        void RestoreIndentation();

        /// <summary>
        /// Sets the indentation level to zero (also affects <c>RestoreIndentation()</c>)
        /// </summary>
        void ResetIndentation();
    }
}
