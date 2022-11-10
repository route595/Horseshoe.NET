using System;

namespace Horseshoe.NET
{
    public class ConversionException : Exception
    {
        public bool IsConverterNotSupplied { get; }

        public ConversionException(string message, bool isConverterNotSupplied = false) : base(message) { IsConverterNotSupplied = isConverterNotSupplied; }

        public ConversionException(string message, Exception innerException, bool isConverterNotSupplied = false) : base(message, innerException) { IsConverterNotSupplied = isConverterNotSupplied; }

        public ConversionException(Exception innerException, bool isConverterNotSupplied = false) : base("Unable to convert: " + innerException.Message, innerException) { IsConverterNotSupplied = isConverterNotSupplied; }

        public ConversionException(Type sourceType, Type destType, bool isConverterNotSupplied = false) : base("Unable to convert " + sourceType + " to " + destType) { IsConverterNotSupplied = isConverterNotSupplied; }

        public ConversionException(Type sourceType, Type destType, string message, bool isConverterNotSupplied = false) : base("Unable to convert " + sourceType + " to " + destType + ": " + message) { IsConverterNotSupplied = isConverterNotSupplied; }

        public ConversionException(Type sourceType, Type destType, Exception innerException, bool isConverterNotSupplied = false) : base("Unable to convert " + sourceType + " to " + destType + ": " + innerException.Message, innerException) { IsConverterNotSupplied = isConverterNotSupplied; }
    }
}
