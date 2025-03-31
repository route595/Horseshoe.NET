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

        /// <summary>
        /// Create a new <c>DataImportException</c>
        /// </summary>
        public DataImportException(ImportError error, PositionNotation positionNotation = default) : base(error.ToString(positionNotation))
        {
        }

        /// <summary>
        /// Create a new <c>DataImportException</c>
        /// </summary>
        public DataImportException(ImportError error, Exception innerException, PositionNotation positionNotation = default) : base(error.ToString(positionNotation), innerException)
        {
        }
    }
}
