using System.Security;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;
using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public class OracleCredentialInterface
    {
        public OracleCredential OracleCredential { get; }

        public OracleCredentialInterface(OracleCredential orclCredentials)
        {
            OracleCredential = orclCredentials;
        }

        public OracleCredentialInterface(Credential credentials, CryptoOptions cryptoOptions = null)
        {
            OracleCredential = ToOracleCredential(credentials, cryptoOptions: cryptoOptions);
        }

        public static OracleCredential ToOracleCredential(Credential credentials, CryptoOptions cryptoOptions = null)
        {
            if (credentials.HasSecurePassword)
                return new OracleCredential(credentials.UserName, credentials.SecurePassword);

            if (credentials.Password != null)
            {
                SecureString securePassword;
                if (credentials.IsEncryptedPassword)
                {
                    securePassword = Decrypt.SecureString(credentials.Password, options: cryptoOptions ?? _options);
                }
                else
                {
                    TextUtil.ConvertToSecureString(credentials.Password);
                    securePassword = TextUtil.ConvertToSecureString(credentials.Password);
                }
                return new OracleCredential(credentials.UserName, securePassword);
            }
            return new OracleCredential(credentials.UserName, null);
        }

        public override string ToString()
        {
            return OracleCredential != null
                ? OracleCredential.UserId + " [" + (OracleCredential.Password != null ? "secure-password" : "no-password") + "]"
                : "[null-crededentials]";
        }

        static CryptoOptions _options;

        public static void SetCryptoOptions(CryptoOptions options)
        {
            _options = options;
        }

        public static implicit operator OracleCredentialInterface(Credential credentials) => new OracleCredentialInterface(credentials);
        public static implicit operator OracleCredentialInterface(OracleCredential orclCredentials) => new OracleCredentialInterface(orclCredentials);
        public static implicit operator OracleCredential(OracleCredentialInterface orclCredentialInterface) => orclCredentialInterface.OracleCredential;
    }
}
