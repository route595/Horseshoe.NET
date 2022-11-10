using System.Data.SqlClient;
using System.Security;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb
{
    public class SqlCredentialInterface
    {
        public SqlCredential SqlCredential { get; }

        public SqlCredentialInterface(SqlCredential sqlCredentials)
        {
            SqlCredential = sqlCredentials;
        }

        public SqlCredentialInterface(Credential credentials, CryptoOptions cryptoOptions = null)
        {
            SqlCredential = ToSqlCredential(credentials, cryptoOptions: cryptoOptions);
        }

        public static SqlCredential ToSqlCredential(Credential credentials, CryptoOptions cryptoOptions = null)
        {
            if (credentials.HasSecurePassword)
                return new SqlCredential(credentials.UserName, credentials.SecurePassword);

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
                return new SqlCredential(credentials.UserName, securePassword);
            }
            return new SqlCredential(credentials.UserName, null);
        }

        public override string ToString()
        {
            return SqlCredential != null
                ? SqlCredential.UserId + " [" + (SqlCredential.Password != null ? "secure-password" : "no-password") + "]"
                : "[null-crededentials]";
        }

        static CryptoOptions _options;

        public static void SetCryptoOptions(CryptoOptions options)
        {
            _options = options;
        }

        public static implicit operator SqlCredentialInterface(Credential credentials) => new SqlCredentialInterface(credentials);
        public static implicit operator SqlCredentialInterface(SqlCredential sqlCredentials) => new SqlCredentialInterface(sqlCredentials);
        public static implicit operator SqlCredential(SqlCredentialInterface sqlCredentialInterface) => sqlCredentialInterface.SqlCredential;
    }
}
