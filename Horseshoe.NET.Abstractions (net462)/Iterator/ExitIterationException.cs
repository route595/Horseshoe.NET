namespace Horseshoe.NET.Iterator
{
    /// <summary>
    /// A specialized exception to be thrown by the consumer by calling <c>ci.Exit()</c> to end the iteration.  
    /// This exception is caught by the system, the definition of a benign exception.
    /// </summary>
    public class ExitIterationException : BenignException
    {
    }
}
