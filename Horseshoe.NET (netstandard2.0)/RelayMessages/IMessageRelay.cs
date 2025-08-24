using System;

namespace Horseshoe.NET.RelayMessages
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
        /// An optional way to dynamically filter relayed messages by group (e.g. namespaces).
        /// <para>
        /// For example...
        /// <code>
        /// var relay = new RelayToConsole()
        /// {
        ///     GroupFilter = new GroupFilter("InternalUsers.Authentication", "ExternalUsers.Authentication")
        ///     -or-
        ///     GroupFilter = new GroupFilter("Authentication", likeMode: LikeMode.Contains)
        ///     -or-
        ///     GroupFilter = new GroupFilter(grp => grp.Contains("Authentication"))
        /// };
        /// </code>
        /// </para>
        /// </summary>
        GroupFilter GroupFilter { get; }

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
