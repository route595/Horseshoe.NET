namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// The base delegate for relaying messages. Use case: code library sends messages which calling code outputs to Console 
    /// </summary>
    /// <param name="message">A message to relay to calling code.</param>
    /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
    /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
    /// <param name="indent">A fixed length indent that implementations may use when rendering messages. This trumps <c>indentHint</c>, if supplied.</param>
    /// <param name="indentHint">An indentation strategy that implementations may use when rendering messages.</param>
    public delegate void RelayerOfMessages(string message, string group = null, int? id = null, int? indent = null, IndentHint? indentHint = null);
}
