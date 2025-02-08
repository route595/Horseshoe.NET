using Horseshoe.NET.Text;

namespace Horseshoe.NET.SecureIO.Sftp
{
    public class SftpConnectionInfo
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public string ServerPath { get; set; } = "";
        public Credential? Credentials { get; set; } 

        public string ToConnectionSring()
        {
            return "sftp://" +
                (Credentials.HasValue ? Credentials.Value.UserName + "@" : "") +
                TextUtil.Reveal(Server) +
                (Port.HasValue ? ":" + Port : "") +
                "/" + (ServerPath ?? "") +
                (Credentials.HasValue ? "?password=" + Credentials.Value.Password.ToUnsecurePassword() : "");
        }

        public override string ToString()
        {
            return "sftp://" +
                (Credentials.HasValue ? Credentials.Value.UserName + "@" : "") +
                TextUtil.Reveal(Server) +
                (Port.HasValue ? ":" + Port : "") +
                "/" + (ServerPath ?? "") +
                (Credentials.HasValue ? "?password=*******" : "");
        }

        /// <summary>
        /// Implicitly converts pseudo connection strings to <c>SftpConnectionInfo</c>
        /// </summary>
        /// <param name="pseudoConnectionString"></param>
        public static implicit operator SftpConnectionInfo(string pseudoConnectionString) => SftpUtil.ParseSftpConnectionString(pseudoConnectionString);
    }
}
