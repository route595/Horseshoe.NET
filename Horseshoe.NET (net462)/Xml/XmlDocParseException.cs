namespace Horseshoe.NET.Xml
{
    /// <summary>
    /// Represents compiled XML documentation (e.g. XML produced by the C# compiler) featuring the ability to parse XML documentation 
    /// </summary>
    public class XmlDocParseException : System.Exception
    {
        /// <summary>
        /// Exception to be thrown when certain unexpected conditions are encountered while parsing XML doc 
        /// </summary>
        /// <param name="message"></param>
        public XmlDocParseException(string message) : base(message) { }
    }
}
