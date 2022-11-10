using System;

namespace Horseshoe.NET.DataImport
{
    public class DataImportException : Exception
    {
        public DataImportException()
        {
        }

        public DataImportException(string message) : base(message)
        {
        }

        public DataImportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
