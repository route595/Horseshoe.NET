using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A basic <c>Exception</c> descriptor class suitable for JSON serialization
    /// </summary>
    public class ExceptionInfo
    {
        /// <summary>
        /// The fully qualified class name of the original exception
        /// </summary>
        public string FullType { get; }

        /// <summary>
        /// The class name of the original exception
        /// </summary>
        public string Type => FullType != null && FullType.Contains(".")
            ? FullType.Substring(FullType.LastIndexOf(".") + 1)
            : FullType ?? "";

        /// <summary>
        /// The message copied over from the original exception
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The stack trace copied over from the original exception
        /// </summary>
        public string StackTrace { get; }

        /// <summary>
        /// Inner exception(s) (in the form of ExceptionInfo) copied over from the original exception 
        /// </summary>
        public ExceptionInfo InnerException { get; }

        /// <summary>
        /// When the exception occurred (approx.) or, more specifically, when this instance of <c>ExceptionInfo</c> was created from <c>Exception</c>
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// Where the exception occurred, e.g. remote web server
        /// </summary>
        public string MachineName { get; }

        public ExceptionInfo()
        {
            DateTime = DateTime.Now;
            try
            {
                MachineName = System.Net.Dns.GetHostName();
            }
            catch
            {
                MachineName = Environment.MachineName;
            }
        }

        public ExceptionInfo(string fullType, string message) : this()
        {
            FullType = fullType;
            Message = message;
        }

        public ExceptionInfo(string fullType, string message, string stackTrace, ExceptionInfo innerException) : this(fullType, message)
        {
            StackTrace = stackTrace;
            InnerException = innerException;
        }

        /// <summary>
        /// Creates a recursive instance of <c>ExceptionInfo</c> from an exception and all of its inner exceptions
        /// </summary>
        /// <param name="ex">And exception</param>
        /// <returns>An <c>ExceptionInfo</c> instance</returns>
        public static ExceptionInfo From(Exception ex)
        {
            return new ExceptionInfo
            (
                ex.GetType().FullName,
                ex.Message,
                ex.StackTrace,
                ex.InnerException == null
                    ? null
                    : From(ex.InnerException)
            );
        }

        public Exception ToException() => new ReconstitutedException(this);

        public void ThrowReconstituted() => throw ToException();

        public static implicit operator ExceptionInfo(Exception exception) => From(exception);
    }
}
