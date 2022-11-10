using System;

namespace Horseshoe.NET
{
    public class EnvironmentVariableNotFoundException : Exception
    {
        public EnvironmentVariableNotFoundException() : base() { }
        public EnvironmentVariableNotFoundException(string varName) : base(varName + " not found") { }
    }
}
