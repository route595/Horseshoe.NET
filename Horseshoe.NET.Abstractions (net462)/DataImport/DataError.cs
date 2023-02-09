namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Represents a data parsing error if the policy is to embed them rather than throw an exception
    /// </summary>
    public class DataError
    {
        /// <summary>
        /// A message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates a new <c>DataError</c>
        /// </summary>
        /// <param name="message">a message</param>
        public DataError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Creates a new <c>DataError</c>
        /// </summary>
        /// <param name="message">a message</param>
        /// <param name="col">the column in which the error occurred</param>
        /// <param name="sourceRow">the row in which the error occurred (referring to the calculated source row)</param>
        public DataError(string message, int col, int sourceRow) : this("Col " + col + ", Source Row " + sourceRow + ": " + message)
        {
        }

        /// <summary>
        /// Returns the error message
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            return Message;
        }

        /// <summary>
        /// Formats this <c>DataError</c> instance as text (i.e. "(err)")
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(err)";
        }
    }
}
