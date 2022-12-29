namespace Horseshoe.NET.Ftp
{
    /// <summary>
    /// FTP connection info
    /// </summary>
    public class FtpConnectionInfo
    {
        /// <summary>
        /// FTP server name or DNS alias
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Optional TCP port
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// The virtual path on the FTP server to set as the current directory
        /// </summary>
        public string ServerPath { get; set; } = "/";

        /// <summary>
        /// Set to <c>true</c> to enable SSL on the FTP connection
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// The FTP login username and password
        /// </summary>
        public Credential? Credentials { get; set; } 

        /// <summary>
        /// Display this <c>FtpConnectionInfo</c> as a pseudo connection string 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FtpUtil.BuildConnectionString(this, hidePassword: true);
        }
    }
}
