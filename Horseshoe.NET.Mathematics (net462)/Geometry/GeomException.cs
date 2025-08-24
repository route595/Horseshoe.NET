using System;

using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Mathematics.Geometry
{
    /// <summary>
    /// An exception related to an angle, side or other attribute of a geometric shape
    /// </summary>
    public class GeomException : Exception
    {
        private static string MessageRelayGroup { get; } = typeof(GeomException).Namespace;

        /// <summary>
        /// Creates a new <c>GeometricException</c>
        /// </summary>
        /// <param name="message">A message</param>
        /// <param name="inlineWithMessages">
        /// If <c>true</c> and if message relay is active, indents this exception at the same level as the last relayed message.  Default is <c>false</c>.  
        /// </param>
        public GeomException(string message, bool inlineWithMessages = false) : base(message) 
        {
            SystemMessageRelay.RelayException(this, group: MessageRelayGroup, inlineWithMessages: inlineWithMessages);
        }
        
        //public GeometricException(string message, Exception innerException) : base(message, innerException) { }
    }
}
