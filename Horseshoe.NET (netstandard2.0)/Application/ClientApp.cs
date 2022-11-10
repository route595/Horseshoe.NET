using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

using Horseshoe.NET.Text;

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
            string consoleTitle = null;

            // load error prone variables
            try
            {
                consoleTitle = Zap.String(Console.Title);
            }
            finally { }

            // Start app type detection
            // Windows Desktop (e.g. Console, WinForms)
            if (consoleTitle != null && consoleTitle.ToLower().Contains("dotnet"))
            {
                messageTracker?.AppendLine("Found Console.Title containing \"dotnet\"");

                // Windows Presentation Foundation
                if (loadedAssemblies.Any(a => a.GetName().Name.Equals("Microsoft.WindowsDesktop.App.WPF")))
                {
                    messageTracker?.AppendLine("Found assembly Microsoft.WindowsDesktop.App.WPF");
                    return AppType.Wpf;
                }

                // Windows Forms
                if (loadedAssemblies.Any(a => a.GetName().Name.Equals("Microsoft.WindowsDesktop.App.WindowsForms")))
                {
                    messageTracker?.AppendLine("Found assembly Microsoft.WindowsDesktop.App.WindowsForms");
                    return AppType.WinForms;
                }

                return AppType.Console;
            }
            else if (consoleTitle != null && consoleTitle.ToLower().ContainsAny(new[] { "iis", "kestrel" }, out string contentFound, ignoreCase: true))
            {
                messageTracker?.AppendLine("Found Console.Title containing \"" + contentFound + "\"");
                var aspNetCoreAssembly = loadedAssemblies.SingleOrDefault(a => a.GetName().Name.Equals("Microsoft.AspNetCore"));
                if (aspNetCoreAssembly != null)
                {
                    messageTracker?.AppendLine("Found assembly Microsoft.AspNetCore");
                    var aspNetCoreHttpAssembly = loadedAssemblies.SingleOrDefault(a => a.GetName().Name.Equals("Microsoft.AspNetCore.Http.Abstractions"));
                    if (aspNetCoreHttpAssembly != null)
                    {
                        messageTracker?.AppendLine("Found assembly Microsoft.AspNetCore.Http");
                        var httpContextType = aspNetCoreHttpAssembly.GetType("Microsoft.AspNetCore.Http.HttpContext");
                        if (httpContextType != null)
                        {
                            messageTracker?.AppendLine("Found type Microsoft.AspNetCore.Http.HttpContext");
                            if (loadedAssemblies.Any(a => a.GetName().Name.Equals("Microsoft.AspNetCore.Mvc")))
                            {
                                messageTracker?.AppendLine("Found assembly Microsoft.AspNetCore.Mvc");
                                return AppType.Mvc;
                            }
                            return AppType.Web;
                        }
                        else
                        {
                            messageTracker?.AppendLine("Type Microsoft.AspNetCore.Http.HttpContext not found");
                        }
                    }
                    else
                    {
                        messageTracker?.AppendLine("Assembly Microsoft.AspNetCore.Http.Abstractions not found");
                    }
                }
                else
                {
                    messageTracker?.AppendLine("Assembly Microsoft.AspNetCore not found");
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
