namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// The base delegate for relaying messages. Use case: code library sends messages which calling code outputs to Console 
    /// </summary>
    /// <param name="message">A message to relay to calling code.</param>
    /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
    /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
    /// <param name="indent">Instructions for indenting relayed messages.</param>
    public delegate void RelayerOfMessages(string message, string group = null, int? id = null, Indent indent = null);
}
