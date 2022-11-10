namespace Horseshoe.NET.Configuration
{
    public class NoConfigurationException : ConfigurationException
    {
        public NoConfigurationException(string message) : base(message) { }
    }
}
