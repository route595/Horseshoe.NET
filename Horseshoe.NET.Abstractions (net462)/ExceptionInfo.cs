using System;
using System.Dynamic;

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
        /// When the exception occurred (approx.) or, more specifically, when this instance of <c>ExceptionInfo</c> was created from <c>Exception</c>.
        /// </summary>
        public DateTime? DateTime { get; }

        /// <summary>
        /// Where the exception occurred e.g. machine name, URL, etc.
        /// </summary>
        public string SourceLocation { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullType"></param>
        /// <param name="message"></param>
        public ExceptionInfo(string fullType, string message)
        {
            FullType = fullType;
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullType"></param>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        /// <param name="innerException"></param>
        /// <param name="dateTime">Optional. When the exception occurred (approx.) or, more specifically, when this instance of <c>ExceptionInfo</c> was created from <c>Exception</c></param>
        /// <param name="sourceLocation">Optional. Where the exception occurred e.g. machine name, URL, etc.</param>
        public ExceptionInfo(string fullType, string message, string stackTrace, ExceptionInfo innerException, DateTime? dateTime = null, string sourceLocation = null) : this(fullType, message)
        {
            StackTrace = stackTrace;
            InnerException = innerException;
            DateTime = dateTime;
            SourceLocation = sourceLocation;
        }

        /// <summary>
        /// Recursively creates an instance of <c>ExceptionInfo</c> from an exception including inner exceptions.
        /// </summary>
        /// <param name="ex">An exception.</param>
        /// <param name="dateTime">Optional. When the exception occurred (approx.) or, more specifically, when this instance of <c>ExceptionInfo</c> was created from <c>Exception</c></param>
        /// <param name="sourceLocation">Optional. Where the exception occurred e.g. machine name, URL, etc.</param>
        /// <returns>An <c>ExceptionInfo</c> instance.</returns>
        public static ExceptionInfo From(Exception ex, DateTime? dateTime = null, string sourceLocation = null)
        {
            return _From(ex, dateTime ?? System.DateTime.Now, sourceLocation ?? GetMachineName());
        }

        private static ExceptionInfo _From(Exception ex, DateTime? dateTime, string sourceLocation)
        {
            return new ExceptionInfo
            (
                ex.GetType().FullName,
                ex.Message,
                ex.StackTrace,
                ex.InnerException == null
                    ? null
                    : _From(ex.InnerException, null, null),
                dateTime,
                sourceLocation
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Exception ToException() => new ReconstitutedException(this);

        /// <summary>
        /// 
        /// </summary>
        public void ThrowReconstituted() => throw ToException();

        private static string GetMachineName()
        {
            try
            {
                return System.Net.Dns.GetHostName();
            }
            catch
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public static implicit operator ExceptionInfo(Exception exception) => From(exception);
    }
}
