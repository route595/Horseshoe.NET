using System;

namespace Horseshoe.NET.ApplicationInsights
{
    public class TelemetryException : Exception
    {
        public TelemetryException(string message) : base(message) { }
        public TelemetryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
