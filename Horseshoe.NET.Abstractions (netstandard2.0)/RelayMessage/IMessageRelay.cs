using System;

namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// Base utility interface for bundling <c>RelayMessage</c> and <c>RelayException</c> instances
    /// </summary>
    public interface IMessageRelay
    {
        /// <summary>
        /// A mechanism used to relay messages.  For example, a long running process 
        /// in a code library can relay debug messages or status updates to calling code.
        /// </summary>
        RelayerOfMessages Message { get; }

        /// <summary>
        /// A mechanism used to relay exceptions.  For example, a squashed exception 
        /// in library code can be relayed to calling code to render or rethrow.
        /// </summary>
        RelayerOfExceptions Exception { get; }

        /// <summary>
        /// Add this to suspend output to console for all but the matching group(s).
        /// <para>
        /// For example...
        /// <code>
        /// var relay = new RelayToConsole()
        /// {
        ///     GroupFilter = (grp) => grp.StartsWith("Authentication")
        /// };
        /// </code>
        /// </para>
        /// </summary>
        Func<string, bool> GroupFilter { get; }

        /// <summary>
        /// An optional indicator to prepend to an exception message.  For example,
        /// "!! " -> "!! System.ArgumentNullException - arg1".
        /// </summary>
        string ExceptionLeadingIndicator { get; }

        /// <summary>
        /// An optional indicator to append to an exception message.  For example,
        /// " !!" -> "System.ArgumentNullException - arg1 !!".
        /// </summary>
        string ExceptionTrailingIndicator { get; }
    }
}
