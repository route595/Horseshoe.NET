using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Configuration
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException() : base() { }
        public ConfigurationException(string message) : base(message) { }
    }
}
