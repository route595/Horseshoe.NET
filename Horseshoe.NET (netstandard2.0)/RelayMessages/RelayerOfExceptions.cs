using System;

namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// The base delegate for relaying exceptions. Use case: code library sends caught exceptions which calling code outputs to Console 
    /// </summary>
    /// <param name="exception">An exception to relay to calling code.</param>
    /// <param name="group">A message group optionally used by implementations for grouping, searching or filtering messages.</param>
    /// <param name="inlineWithMessages">
    /// If <c>true</c>, indents this exception at the same level as the last relayed message.  Default is <c>false</c>.  
    /// The only built-in implementation that is guaranteed to honor this argument is <c>RelayIndentedMessageBase</c>  
    /// (i.e. subclasses <c>RelayToConsole</c> and <c>RelayToFile</c>).
    /// </param>
    public delegate void RelayerOfExceptions(Exception exception, string group = null, bool inlineWithMessages = false);
}
