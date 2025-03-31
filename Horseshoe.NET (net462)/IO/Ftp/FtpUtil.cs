using System;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Ftp
{
    /// <summary>
    /// A set of utility methods for supporting FTP operations
    /// </summary>
    public static class FtpUtil
    {
        /// <summary>
        /// Parses an FTP connection string 
        /// </summary>
        /// <param name="connectionString">A pseudo connection string for FTP</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        /// <remarks>
        /// Pseudo connection strings 
        /// <para>
        /// Example #1: ftp://george@11.22.33.44/dir/subdir?password=akdj$8iO(d@1sd
        /// </para>
        /// <para>
        /// Example #2: ftp://george@11.22.33.44/dir/subdir?encryptedPassword=a6bd9cf8a07dbc15d==
        /// </para>
        /// </remarks>
        public static FtpConnectionInfo ParseFtpConnectionString(string connectionString)
        {
            if (connectionString == null) return null;

            var connectionInfo = new FtpConnectionInfo();
            if (connectionString.ToLower().StartsWith("ftp://"))
            {
                connectionString = connectionString.Substring(6);  // remove scheme to continue parsing
            }
            else if (connectionString.ToLower().StartsWith("ftps://"))
            {
                connectionString = connectionString.Substring(7);  // remove scheme to continue parsing
                connectionInfo.EnableSsl = true;
            }

            if (connectionString.Contains("://"))
            {
                throw new ValidationException("Invalid scheme: \"" + connectionString.Substring(0, connectionString.IndexOf("://")) + "\" - this method only accepts \"ftp://\"");
            }

            var pos = connectionString.IndexOf("@");  // @ = user name / server address separator
            if (pos > 0)
            {
                var pos2 = connectionString.ToLower().IndexOf("?password=");
                if (pos2 > pos)
                {
                    connectionInfo.Credentials = new Credential(connectionString.Substring(0, pos), connectionString.Substring(pos2 + 10));
                }
                else
                {
                    pos2 = connectionString.ToLower().IndexOf("?encryptedpassword=");
                    if (pos2 > pos)
                    {
                        var pwd = connectionString.Substring(pos2 + 19);
                        connectionInfo.Credentials = Credential.Build(connectionString.Substring(0, pos), () => Decrypt.String(pwd));
                    }
                    else if (!string.Equals(connectionString.Substring(0, pos), "anonymous", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ValidationException("No password was found for user '" + connectionString.Substring(0, pos) + "': make sure the connection string ends with \"?password=*****\" or \"?encryptedPassword=*****\"");
                    }
                }

                if (pos2 > pos)
                {
                    connectionString = connectionString.Substring(0, pos2);  // remove password to continue parsing
                }
                connectionString = connectionString.Substring(pos + 1);  // remove username to continue parsing
            }

            pos = connectionString.IndexOf("/");
            if (pos > 0)
            {
                connectionInfo.Server = connectionString.Substring(0, pos);
                connectionInfo.ServerPath = connectionString.Substring(pos + 1);
            }
            else
            {
                connectionInfo.Server = connectionString;
                connectionInfo.ServerPath = FtpSettings.DefaultServerPath;
            }

            pos = connectionInfo.Server.IndexOf(":");
            if (pos > 0)
            {
                if (int.TryParse(connectionInfo.Server.Substring(pos + 1), out int port))
                {
                    connectionInfo.Port = port;
                }
                else
                {
                    throw new ValidationException("Invalid port: \"" + connectionInfo.Server.Substring(pos + 1) + "\"");
                }
                connectionInfo.Server = connectionInfo.Server.Substring(0, pos);
            }

            return connectionInfo;
        }

        /// <summary>
        /// Builds a pseudo connection string
        /// </summary>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="hidePassword">hide the password</param>
        /// <returns></returns>
        public static string BuildConnectionString(FtpConnectionInfo connectionInfo, bool hidePassword = false)
        {
            if (connectionInfo == null)
                return null;
            return BuildConnectionString(connectionInfo.Server, connectionInfo.Port, connectionInfo.ServerPath, connectionInfo.EnableSsl, connectionInfo.Credentials, hidePassword: hidePassword);
        }

        /// <summary>
        /// Builds a pseudo connection string
        /// </summary>
        /// <param name="server">FTP server name or DNS alias</param>
        /// <param name="port">optional TCP port</param>
        /// <param name="serverPath">the virtual path on the FTP server to set as the current directory</param>
        /// <param name="enableSsl">set to <c>true</c> to enable SSL on the FTP connection</param>
        /// <param name="credentials">the FTP login username and password</param>
        /// <param name="hidePassword">hide the password</param>
        /// <returns></returns>
        public static string BuildConnectionString(string server, int? port, string serverPath, bool enableSsl, Credential? credentials, bool hidePassword = false)
        {
            var strb = new StringBuilder(enableSsl ? "ftps://" : "ftp://")
                .AppendIf(credentials.HasValue, credentials.Value.UserName + "@")
                .Append(server)
                .AppendIf(port.HasValue, ":" + port)
                .Append(serverPath);

            if (credentials.HasValue)
            {
                strb.Append("?password=")
                    .Append(hidePassword ? "<password>" : credentials.Value.Password.ToUnsecurePassword());
            }
            return strb.ToString();
        }
    }
}
