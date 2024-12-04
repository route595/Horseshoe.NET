using System;

namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// Base utility class for bundling <c>RelayMessage</c> and <c>RelayException</c> instances
    /// with indentation.  Subclasses may or may not honor indentation and other implementations 
    /// may or may not support it.  By default, exceptions reset indentation but this behavior 
    /// is easily modified (see <c>IndentExceptionsInlineWithMessages</c>).
    /// </summary>
    public abstract class RelayIndentedMessagesBase : IMessageRelay
    {
        /// <summary>
        /// Indicates how many spaces of indentation to render.
        /// </summary>
        public int IndentLevel { get; set; }

        /// <summary>
        /// Indicates how many spaces to increment or decrement, default by convention is 2.
        /// </summary>
        public int IndentInterval { get; set; }

        /// <inheritdoc cref="IMessageRelay.GroupFilter"/>
        public Func<string, bool> GroupFilter { get; set; }

        /// <inheritdoc cref="IMessageRelay.ExceptionLeadingIndicator"/>
        public string ExceptionLeadingIndicator { get; }

        /// <inheritdoc cref="IMessageRelay.ExceptionTrailingIndicator"/>
        public string ExceptionTrailingIndicator { get; }

        /// <summary>
        /// If <c>true</c>, indents exceptions at the same level as the last relayed message.
        /// Default is <c>false</c>.
        /// </summary>
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
                IndentLevel = indent.Value;
            }
            else if (indentHint.HasValue)
            {
                switch (indentHint.Value)
                {
                    case IndentHint.Reset:
                        IndentLevel = 0;
                        break;
                    case IndentHint.Increment:
                        IndentLevel += IndentInterval;
                        break;
                    case IndentHint.Decrement:
                        IndentLevel -= IndentInterval;
                        if (IndentLevel < 0)
                            IndentLevel = 0;
                        break;
                }
            }

            RenderToOutput(message, id: id);

            if (indentHint.HasValue)
            {
                switch (indentHint.Value)
                {
                    case IndentHint.IncrementNext:
                        IndentLevel += IndentInterval;
                        break;
                    case IndentHint.DecrementNext:
                        IndentLevel -= IndentInterval;
                        if (IndentLevel < 0)
                            IndentLevel = 0;
                        break;
                }
            }
        };

        /// <inheritdoc cref="IMessageRelay.Exception"/>
        public RelayerOfExceptions Exception => (exception, group) =>
        {
            var renderedException = (ExceptionLeadingIndicator ?? "") + exception.GetType().FullName + ": " + exception.Message + (ExceptionTrailingIndicator ?? "");
            
            if (IndentExceptionsInlineWithMessages)
            {
                Message(renderedException, group: group);
                return;
            }
            
            // check group filter
            if (GroupFilter != null && !GroupFilter.Invoke(group))
                return;

            IndentLevel = 0;
            RenderToOutput(renderedException);
        };
    }
}
