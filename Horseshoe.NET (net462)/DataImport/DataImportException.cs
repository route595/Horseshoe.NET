using System;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Base class for data import specific exceptions
    /// </summary>
    public class DataImportException : Exception
    {
        /// <summary>
        /// Create a new <c>DataImportException</c>
        /// </summary>
        public DataImportException()
        {
        }

        /// <summary>
        /// Create a new <c>DataImportException</c>
        /// </summary>
        public DataImportException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new <c>DataImportException</c>
        /// </summary>
        public DataImportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
