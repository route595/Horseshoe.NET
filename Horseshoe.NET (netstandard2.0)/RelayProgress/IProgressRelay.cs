using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.RelayProgress
{
    /// <summary>
    /// Base utility interface for handling <c>RelayProgress</c> functionality
    /// </summary>
    public interface IProgressRelay
    {
        /// <summary>
        /// A mechanism used to relay progress.  For example, a code library used by 
        /// an installer UI sends progress updates to the console or a widget e.g. 
        /// progress bar, label, etc.
        /// </summary>
        RelayerOfProgress Progress { get; }

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
