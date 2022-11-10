using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Horseshoe.NET
{
    public static class Machine
    {
        public static string IPAddress => IPv4Address ?? IPv6Address;

        public static string IPv4Address => IPv4Addresses.FirstOrDefault();

        public static string IPv6Address => IPv6Addresses.FirstOrDefault();

        static string[] _ipAddresses;
        public static string[] IPAddresses
        {
            get
            {
                if (_ipAddresses == null)
                {
                    _ipAddresses = Dns.GetHostEntry("").AddressList.Select(a => a.ToString()).ToArray();
                }
                return _ipAddresses;
            }
        }

        static Regex IPv4Pattern = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}");
        public static string[] IPv4Addresses => IPAddresses
            .Where(a => IPv4Pattern.IsMatch(a))
            .ToArray();

        public static string[] IPv6Addresses => IPAddresses
            .Where(a => !IPv4Pattern.IsMatch(a))
            .ToArray();

        public static string Name => Environment.MachineName;

        public static string FullyQualifiedName
        {
            get
            {
                try
                {
                    return Dns.GetHostEntry("").HostName;
                }
                catch { }
                return null;
            }
        }
    }
}
