using System;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception to use when converting values from one unit to another.
    /// </summary>
    public class ConversionException : Exception
    {
        /// <summary>
        /// Indicates that solving this error requires manually supplying a converter, e.g. the built-in converters were inadequate.
        /// </summary>
        public bool IsConverterNotSupplied { get; }

        /// <summary>
        /// Creates a new <c>ConversionException</c>.
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="isConverterNotSupplied">Indicates that solving this error requires manually supplying a converter.</param>
        public ConversionException(string message, bool isConverterNotSupplied = false) : base(message) { IsConverterNotSupplied = isConverterNotSupplied; }

        /// <summary>
        /// Creates a new <c>ConversionException</c>.
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="innerException"></param>
        /// <param name="isConverterNotSupplied">Indicates that solving this error requires manually supplying a converter.</param>
        public ConversionException(string message, Exception innerException, bool isConverterNotSupplied = false) : base(message, innerException) { IsConverterNotSupplied = isConverterNotSupplied; }

        /// <summary>
        /// Creates a new <c>ConversionException</c>.
        /// </summary>
        /// <param name="innerException">An exception.</param>
        /// <param name="isConverterNotSupplied">Indicates that solving this error requires manually supplying a converter.</param>
        public ConversionException(Exception innerException, bool isConverterNotSupplied = false) : base("Unable to convert: " + innerException.Message, innerException) { IsConverterNotSupplied = isConverterNotSupplied; }

        /// <summary>
        /// Creates a new <c>ConversionException</c>.
        /// </summary>
        /// <param name="sourceType">Converting from this type.</param>
        /// <param name="destType">Converting to this other type.</param>
        /// <param name="isConverterNotSupplied">Indicates that solving this error requires manually supplying a converter.</param>
        public ConversionException(Type sourceType, Type destType, bool isConverterNotSupplied = false) : base("Unable to convert " + sourceType + " to " + destType) { IsConverterNotSupplied = isConverterNotSupplied; }

        /// <summary>
        /// Creates a new <c>ConversionException</c>.
        /// </summary>
        /// <param name="sourceType">Converting from this type.</param>
        /// <param name="destType">Converting to this other type.</param>
        /// <param name="message">A message</param>
        /// <param name="isConverterNotSupplied">Indicates that solving this error requires manually supplying a converter.</param>
        public ConversionException(Type sourceType, Type destType, string message, bool isConverterNotSupplied = false) : base("Unable to convert " + sourceType + " to " + destType + ": " + message) { IsConverterNotSupplied = isConverterNotSupplied; }

        /// <summary>
        /// Creates a new <c>ConversionException</c>.
        /// </summary>
        /// <param name="sourceType">Converting from this type.</param>
        /// <param name="destType">Converting to this other type.</param>
        /// <param name="innerException"></param>
        /// <param name="isConverterNotSupplied">Indicates that solving this error requires manually supplying a converter.</param>
        public ConversionException(Type sourceType, Type destType, Exception innerException, bool isConverterNotSupplied = false) : base("Unable to convert " + sourceType + " to " + destType + ": " + innerException.Message, innerException) { IsConverterNotSupplied = isConverterNotSupplied; }
    }
}
