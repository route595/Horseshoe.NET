using System;

namespace Horseshoe.NET.IO.Ftp
{
    /// <summary>
    /// An exception to use when FTP operations do not succeed
    /// </summary>
    public class FtpOperationException : Exception
    {
        /// <summary>
        /// Creates a new <c>FtpOperationException</c>
        /// </summary>
        /// <param name="message">a message</param>
        public FtpOperationException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <c>FtpOperationException</c>
        /// </summary>
        /// <param name="message">a message</param>
        /// <param name="innerException">an exception</param>
        public FtpOperationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
