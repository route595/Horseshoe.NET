using System;
using System.Linq;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for dealing with invalid data inputs,
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// A message comprised of user supplied message and any validation messages (if applicable).
        /// </summary>
        public override string Message =>
            base.Message +
            (
                ValidationMessages == null || !ValidationMessages.Any()
                    ? "" 
                    : " (x" + ValidationMessages.Count() + "): " + string.Join(";", ValidationMessages)
            );

        /// <summary>
        /// Sets <c>ValidationMessages</c> to a 1-length array.
        /// </summary>
        public string ValidationMessage
        {
            set { ValidationMessages = new string[] { value }; }
        }

        /// <summary>
        /// Gets or sets an array of valication messages.
        /// </summary>
        public string[] ValidationMessages { get; set; }

        /// <summary>
        /// Whether this exception has any validation messages.
        /// </summary>
        public bool HasValidationMessages =>  ValidationMessages?.Any() ?? false;

        /// <summary>
        /// Creates a new <c>ValidationException</c>.
        /// </summary>
        public ValidationException() : base("Validation failed.") { }

        /// <summary>
        /// Creates a new <c>ValidationException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        public ValidationException(string message) : base("Validation failed: " + message) { }

        /// <summary>
        /// Creates a new <c>ValidationException</c>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">An inner exception.</param>
        public ValidationException(string message, Exception innerException) : base("Validation failed: " + message, innerException) { }
    }
}
