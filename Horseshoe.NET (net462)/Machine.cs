using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Horseshoe.NET
{
    /// <summary>
    /// A set of properties pertaining to the local computer / device.
    /// </summary>
    public static class Machine
    {
        /// <summary>
        /// Machine's IP address
        /// </summary>
        public static string IPAddress => IPv4Address ?? IPv6Address;

        /// <summary>
        /// Machine's first IPv4 address
        /// </summary>
        public static string IPv4Address => IPv4Addresses.FirstOrDefault();

        /// <summary>
        /// Machine's first IPv6 address
        /// </summary>
        public static string IPv6Address => IPv6Addresses.FirstOrDefault();

        static string[] _ipAddresses;

        /// <summary>
        /// Machine's IP addresses
        /// </summary>
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

        static readonly Regex IPv4Pattern = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}");

        /// <summary>
        /// Machine's IPv4 addresses
        /// </summary>
        public static string[] IPv4Addresses => IPAddresses
            .Where(a => IPv4Pattern.IsMatch(a))
            .ToArray();

        /// <summary>
        /// Machine's IPv6 addresses
        /// </summary>
        public static string[] IPv6Addresses => IPAddresses
            .Where(a => !IPv4Pattern.IsMatch(a))
            .ToArray();

        /// <summary>
        /// Machine name
        /// </summary>
        public static string Name => Environment.MachineName;

        /// <summary>
        /// Machine's DNS name
        /// </summary>
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
