using System;

namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// The base delegate for relaying exceptions. Use case: code library sends caught exceptions which calling code outputs to Console 
    /// </summary>
    /// <param name="exception">An exception to relay to calling code.</param>
    /// <param name="group">A message group optionally used by implementations for grouping, searching or filtering messages.</param>
    public delegate void RelayerOfExceptions(Exception exception, string group = null);
}
