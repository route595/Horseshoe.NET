using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Application
{
    public static class ClientApp
    {
        static AppType? _appType;

        /// <summary>
        /// Gets or sets the app type.  If not set it is derived based on criteria detected at runtime.
        /// </summary>
        public static AppType AppType
        {
            get
            {
                return _appType
                    ?? _Config.GetNEnum<AppType>("Horseshoe.NET:AppType")
                    ?? DetectAppType()
                    ?? default;
            }
            set
            {
                _appType = value;
            }
        }

        /// <summary>
        /// Used internally by this and other Horseshoe.NET utility assemblies
        /// </summary>
        /// <returns></returns>
        public static AppType? DetectAppType(StringBuilder messageTracker = null)
        {
            // all detectable app types are user interactive
            messageTracker?.AppendLine("Environment.UserInteractive = " + Environment.UserInteractive);
            if (!Environment.UserInteractive) return null;

            // declare variables
            var loadedAssemblies = Assemblies.List();

            // Start app type detection
            // Windows Desktop (e.g. Console, WinForms)
            if (Environment.CommandLine.ToLower().Contains((AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName).ToLower()))
            {
                messageTracker?.AppendLine("Environment.CommandLine contains AppDomain.BaseDirectory+FriendlyName");

                // declare win variables
                string consoleTitle = null;

                // load win variables
                try
                {
                    consoleTitle = Zap.String(Console.Title);
                }
                finally { }

                // Console
                if (consoleTitle != null)
                {
                    messageTracker?.AppendLine("Found Console.Title");
                    return AppType.Console;
                }
                else
                {
                    messageTracker?.AppendLine("Console.Title not found");
                }

                // WPF +  WinForms
                if (Process.GetCurrentProcess().MainWindowHandle == IntPtr.Zero)
                {
                    messageTracker?.AppendLine("Process.MainWindowHandle = IntPtr.Zero");

                    if (loadedAssemblies.Any(a => a.GetName().Name.Equals("PresentationFramework")))
                    {
                        messageTracker?.AppendLine("Found assembly PresentationFramework");
                        return AppType.Wpf;
                    }
                    else
                    {
                        messageTracker?.AppendLine("Assembly PresentationFramework not found");
                    }

                    if (loadedAssemblies.Any(a => a.GetName().Name.Equals("System.Windows.Forms")))
                    {
                        messageTracker?.AppendLine("Found assembly System.Windows.Forms");
                        return AppType.WinForms;
                    }
                    else
                    {
                        messageTracker?.AppendLine("Assembly System.Windows.Forms not found");
                    }
                    return AppType.WindowsDesktop;
                }
                else
                {
                    messageTracker?.AppendLine("Process.MainWindowHandle != IntPtr.Zero");
                }
            }
            else
            {
                messageTracker?.AppendLine("AppDomain.DomainManager != null");

                // Web (e.g. WebForms, MVC)
                var systemWebAssembly = loadedAssemblies.SingleOrDefault(a => a.GetName().Name.Equals("System.Web"));
                if (systemWebAssembly != null)
                {
                    messageTracker?.AppendLine("Found assembly System.Web");

                    if (loadedAssemblies.Any(a => a.GetName().Name.Equals("System.Web.Mvc")))
                    {
                        messageTracker?.AppendLine("Found assembly System.Web.Mvc");
                        return AppType.Mvc;
                    }

                    var httpContextType = systemWebAssembly.GetType("System.Web.HttpContext");
                    if (httpContextType != null)
                    {
                        messageTracker?.AppendLine("Found type System.Web.HttpContext");
                        var currentProp = httpContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                        if (currentProp != null)
                        {
                            messageTracker?.AppendLine("Found property HttpContext.Current");
                            var current = currentProp.GetValue(null);
                            if (current != null)
                            {
                                messageTracker?.AppendLine("Property HttpContext.Current != null");
                                return AppType.WebForms;
                            }
                            else
                            {
                                messageTracker?.AppendLine("HttpContext.Current = null");
                            }
                        }
                        else
                        {
                            messageTracker?.AppendLine("Property HttpContext.Current not found");
                        }
                    }
                    else
                    {
                        messageTracker?.AppendLine("Type HttpContext not found");
                    }
                    return AppType.Web;
                }
                else
                {
                    messageTracker?.AppendLine("Assembly System.Web not found");
                }
            }

            return null;
        }

        private static AppMode? _appMode;

        /// <summary>
        /// Gets or sets the application mode (e.g. Development, Test, Production...)
        /// </summary>
        public static AppMode? AppMode
        {
            get
            {
                return _appMode
                    ?? _Config.GetNEnum<AppMode>("Horseshoe.NET:AppMode");
            }
            set
            {
                _appMode = value;
            }
        }

        /// <summary>
        /// Gets the IP address of the local machine
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable()) return null;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                .ToString();
        }
    }
}
