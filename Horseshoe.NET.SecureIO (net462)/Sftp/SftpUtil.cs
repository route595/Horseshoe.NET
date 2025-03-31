using System;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.IO.Ftp;

namespace Horseshoe.NET.SecureIO.Sftp
{
    public static class SftpUtil
    {
        public static SftpConnectionInfo ParseSftpConnectionString(string connectionString)
        {
            // example pseudo connection string: sftp://george@11.22.33.44/dir/subdir?encryptedPassword=akdj$8iO(d@1sd==
            if (connectionString == null) return null;

            var connectionInfo = new SftpConnectionInfo();
            if (connectionString.ToLower().StartsWith("sftp://"))
            {
                connectionString = connectionString.Substring(7);  // remove scheme to continue parsing
            }

            if (connectionString.Contains("://"))
            {
                throw new ValidationException("Invalid scheme: \"" + connectionString.Substring(0, connectionString.IndexOf("://")) + "\" - this method only accepts \"sftp://\"");
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
                        connectionInfo.Credentials = Credential.Build(connectionString.Substring(0, pos), () => Decrypt.String(connectionString.Substring(pos2 + 19)));
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
    }
}
