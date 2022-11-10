using System;
using System.Net;
using System.Security;

using Horseshoe.NET.Crypto;

namespace Horseshoe.NET
{
    public partial struct Credential
    {
        public string UserName { get; }

        public string Password { get; }

        public bool IsEncryptedPassword { get; }

        public SecureString SecurePassword { get; }

        public bool HasSecurePassword => SecurePassword != null;

        public string Domain { get; }

        private Credential(string userName, string domain = null)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = null;
            IsEncryptedPassword = false;
            SecurePassword = null;
            Domain = domain;
        }

        public Credential(string userName, string password, bool isEncryptedPassword = false, string domain = null) : this(userName, domain: domain)
        {
            Password = password;
            IsEncryptedPassword = password != null && isEncryptedPassword;
        }

        public Credential(string userName, SecureString securePassword, string domain = null) : this(userName, domain: domain)
        {
            SecurePassword = securePassword ?? throw new ArgumentNullException(nameof(securePassword));
        }

        public static Credential? Build(string userName, string password, bool isEncryptedPassword = false, string domain = null)
        {
            if (userName == null)
                return null;
            return new Credential(userName, password, isEncryptedPassword: isEncryptedPassword, domain: domain);
        }

        public static Credential? Build(string userName, SecureString securePassword, string domain = null)
        {
            if (userName == null)
                return null;
            return new Credential(userName, securePassword, domain: domain);
        }

        public override string ToString()
        {
            string pwdMsg = "no-password";
            if (HasSecurePassword)
            {
                pwdMsg = "secure-password";
            }
            else if (IsEncryptedPassword)
            {
                pwdMsg = "encrypted-password";
            }
            else if (Password != null)
            {
                pwdMsg = "plaintext-password";
            }
            return UserName + " [" + pwdMsg + "]" + (Domain != null ? " @" + Domain : "");
        }

        public NetworkCredential ToNetworkCredential()
        {
            if (HasSecurePassword)
            {
                return Domain != null
                    ? new NetworkCredential(UserName, SecurePassword, Domain)
                    : new NetworkCredential(UserName, SecurePassword);
            }
            else if (IsEncryptedPassword)
            {
                return Domain != null
                    ? new NetworkCredential(UserName, Decrypt.SecureString(Password), Domain)
                    : new NetworkCredential(UserName, Decrypt.SecureString(Password));
            }
            return Domain != null
                ? new NetworkCredential(UserName, Password, Domain)
                : new NetworkCredential(UserName, Password);
        }

        public static implicit operator NetworkCredential(Credential credentials) => credentials.ToNetworkCredential();
    }
}
