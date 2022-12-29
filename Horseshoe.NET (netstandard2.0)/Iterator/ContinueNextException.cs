namespace Horseshoe.NET.Iterator
{
    /// <summary>
    /// A specialized exception to be thrown by the consumer by calling <c>ci.Next()</c> to skip to the next loop iteration.  
    /// This exception is caught by the system, the definition of a benign exception.
    /// </summary>
    internal class ContinueNextException : BenignException
    {
    }
}
