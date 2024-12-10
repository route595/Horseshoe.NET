using System;

namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// Base utility class for bundling <c>RelayMessage</c> and <c>RelayException</c> instances
    /// with indentation.  Subclasses may or may not honor indentation and other implementations 
    /// may or may not support it.  By default, exceptions reset indentation but this behavior 
    /// is easily modified (see <c>IndentExceptionsInlineWithMessages</c>).
    /// </summary>
    public abstract class RelayIndentedMessagesBase : IIndentedMessageRelay
    {
        private int? LastIndentLevel { get; set; }

        /// <inheritdoc cref="IMessageRelay.GroupFilter"/>
        public Func<string, bool> GroupFilter { get; set; }

        /// <inheritdoc cref="IMessageRelay.ExceptionLeadingIndicator"/>
        public string ExceptionLeadingIndicator { get; }

        /// <inheritdoc cref="IMessageRelay.ExceptionTrailingIndicator"/>
        public string ExceptionTrailingIndicator { get; }

        /// <inheritdoc cref="IIndentedMessageRelay.IndentLevel"/>
        public int IndentLevel { get; set; }

        /// <inheritdoc cref="IIndentedMessageRelay.IndentInterval"/>
        public int IndentInterval { get; set; }

        /// <inheritdoc cref="IIndentedMessageRelay.IndentExceptionsInlineWithMessages"/>
        public bool IndentExceptionsInlineWithMessages { get; set; }

        /// <summary>
        /// Inheriting classes must specify how / where to render the relayed message.
        /// </summary>
        /// <param name="message">A message to relay to calling code.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        public abstract void RenderToOutput(string message, int? id = null);

        /// <inheritdoc cref="IMessageRelay.Message" />
        public RelayerOfMessages Message => (message, group, id, indent, indentHint) =>
        {
            // check group filter
            if (GroupFilter != null && !GroupFilter.Invoke(group))
                return;

            // process indentation requests
            if (indent.HasValue)
            {
                SetIndentation(indent.Value);
            }
            else if (indentHint.HasValue)
            {
                switch (indentHint.Value)
                {
                    case IndentHint.Reset:
                        ResetIndentation();
                        break;
                    case IndentHint.Increment:
                        IncrementIndentation();
                        break;
                    case IndentHint.Decrement:
                        DecrementIndentation();
                        break;
                    case IndentHint.Restore:
                        RestoreIndentation();
                        break;
                }
            }

            RenderToOutput(message ?? "[null-message]", id: id);

            if (indentHint.HasValue)
            {
                switch (indentHint.Value)
                {
                    case IndentHint.IncrementNext:
                        IncrementIndentation();
                        break;
                    case IndentHint.DecrementNext:
                        DecrementIndentation();
                        break;
                }
            }
        };

        /// <inheritdoc cref="IMessageRelay.Exception"/>
        public RelayerOfExceptions Exception => (exception, group, inlineWithMessages) =>
        {
            var renderedException = (ExceptionLeadingIndicator ?? "") + (exception != null ? exception.GetType().FullName + ": " + exception.Message : "[null-exception]") + (ExceptionTrailingIndicator ?? "");
            
            if (inlineWithMessages || IndentExceptionsInlineWithMessages)
            {
                Message(renderedException, group: group);
                return;
            }
            
            // check group filter
            if (GroupFilter != null && !GroupFilter.Invoke(group))
                return;

            ResetIndentation();
            RenderToOutput(renderedException);
        };

        /// <inheritdoc cref="IIndentedMessageRelay.SetIndentation"/>
        public void SetIndentation(int indentLevel)
        {
            IndentLevel = indentLevel < 0 ? 0 : indentLevel;
            LastIndentLevel = IndentLevel;
        }

        /// <inheritdoc cref="IIndentedMessageRelay.IncrementIndentation"/>
        public void IncrementIndentation()
        {
            IndentLevel += IndentInterval;
        }

        /// <inheritdoc cref="IIndentedMessageRelay.DecrementIndentation"/>
        public void DecrementIndentation()
        {
            IndentLevel -= IndentInterval;
            if (IndentLevel < 0)
                IndentLevel = 0;
        }

        /// <inheritdoc cref="IIndentedMessageRelay.RestoreIndentation"/>
        public void RestoreIndentation()
        {
            if (LastIndentLevel.HasValue)
                IndentLevel = LastIndentLevel.Value;
        }

        /// <inheritdoc cref="IIndentedMessageRelay.ResetIndentation"/>
        public void ResetIndentation() => SetIndentation(0);
    }
}
