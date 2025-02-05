using System;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// An exception in one of the 'Db' family of classes
    /// </summary>
    public class DbException : Exception
    {
        /// <summary>
        /// DbException constructor
        /// </summary>
        /// <param name="message">A message</param>
        public DbException(string message) : base(message) { }

        /// <summary>
        /// DbException constructor
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="innerException">An exception</param>
        public DbException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
