using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.SecureIO.Sftp
{
    public class SftpConnectionInfo
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public string ServerPath { get; set; } = "";
        public Credential? Credentials { get; set; } 

        public override string ToString()
        {
            return "sftp://" +
                (Credentials.HasValue ? Credentials.Value.UserName + "@" : "") +
                TextUtil.Reveal(Server) +
                (Port.HasValue ? ":" + Port : "") +
                "/" + (ServerPath ?? "") +
                (Credentials.HasValue ? (Credentials.Value.HasSecurePassword ? "?password=<secure>" : (Credentials.Value.IsEncryptedPassword ? "?encryptedPassword=" + Credentials.Value.Password : "?password=" + (Credentials.Value.Password == null ? "[null]" : "******"))) : "");
        }
    }
}
