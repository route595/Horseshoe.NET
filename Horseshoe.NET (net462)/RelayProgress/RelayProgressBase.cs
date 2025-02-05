using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.RelayProgress
{
    /// <summary>
    /// Base utility class for handling <c>RelayProgress</c> instances.
    /// </summary>
    public abstract class RelayProgressBase : IProgressRelay
    {
        /// <summary>
        /// Indicates how many spaces of indentation to render.
        /// </summary>
        public int IndentLevel { get; set; }

        /// <summary>
        /// Indicates how many spaces to increment or decrement, default is <c>2</c>.
        /// </summary>
        public int IndentInterval { get; set; }

        /// <inheritdoc cref="IMessageRelay.ExceptionLeadingIndicator"/>
        public string ExceptionLeadingIndicator { get; }

        /// <inheritdoc cref="IMessageRelay.ExceptionTrailingIndicator"/>
        public string ExceptionTrailingIndicator { get; }

        /// <summary>
        /// Inheriting classes must specify how / where to render the relayed progress.
        /// </summary>
        /// <param name="taskNumber">
        /// The task number.  Together with <c>totalTasks</c> can be used in lieu of <c>progress</c>. 
        /// 0 (zero) should indicate a pre-run or initializing status or the first task is
        /// incomplete and 1 (one) should mean the first task is complete, etc.
        /// </param>
        /// <param name="totalTasks">The number of tasks whose progress to track.</param>
        /// <param name="progress">
        /// A value between 0.0 and 1.0 (inclusive) indicating a percent complete. Most granular option, 
        /// can be used to 'creep' a.k.a. visually suggest progress during a delayed or long running 
        /// operation such as a slow download or during timeouts and retries.
        /// </param>
        /// <param name="category">The task category or a broad description of the current task, could be used like a title.</param>
        /// <param name="description">A description of the current task.</param>
        /// <param name="indeterminate">If <c>true</c>, can set a UI widget to 'spin' or enter indeterminate mode, default is <c>false</c>.</param>
        public abstract void RenderProgress(int? taskNumber, int? totalTasks, double? progress, string category, string description, bool indeterminate);

        /// <summary>
        /// Inheriting classes must specify how / where to render the relayed message.
        /// </summary>
        /// <param name="message">A message to relay to calling code.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        public abstract void RenderMessage(string message, int? id = 0);

        /// <inheritdoc cref="IProgressRelay.Progress" />
        public RelayerOfProgress Progress => (taskNumber, totalTasks, progress, category, description, indeterminate) =>
        {
            RenderProgress(taskNumber, totalTasks, progress, category, description, indeterminate);    
        };

        /// <inheritdoc cref="IMessageRelay.Message" />
        public RelayerOfMessages Message => (message, group, id, indent) =>
        {
            // process indentation requests
            if (indent != null)
            {
                if (indent.Level.HasValue)
                {
                    IndentLevel = indent.Level.Value;
                }
                else
                {
                    switch (indent.Hint)
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
            }

            RenderMessage(message, id: id);

            if (indent != null)
            {
                switch (indent.Hint)
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
        public RelayerOfExceptions Exception => (exception, group, inlineWithMessages) =>
        {
            var renderedException = (ExceptionLeadingIndicator ?? "") + exception.GetType().FullName + ": " + exception.Message + (ExceptionTrailingIndicator ?? "");

            RenderMessage(renderedException);
        };
    }
}
