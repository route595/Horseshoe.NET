namespace Horseshoe.NET.Configuration
{
    public class NoConfigurationServiceException : ConfigurationException
    {
        public NoConfigurationServiceException(string message = "Configuration service not loaded.  Try one of the following: Config.Load(...) or Config.ConfigurationAccessor = () => ...") : base(message) { }
    }
}
