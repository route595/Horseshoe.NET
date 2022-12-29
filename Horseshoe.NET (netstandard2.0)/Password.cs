using System.Security;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// A multi-purpose, <c>string</c> and <c>SecureString</c>-compatible password handler.
    /// It stores all passwords in <c>SecureString</c> format, therefore, by convention, it only accepts 
    /// unencrypted password <c>string</c>s or <c>SecureString</c>s as input.  Technically, it can't tell the 
    /// difference between encrypted and unencrypted.  So, if client code knowingly creates a <c>Password</c> 
    /// from an encrypted password <c>string</c> please utilize the constructor and set <c>isEncrytped = true</c>.
    /// </summary>
    public readonly struct Password
    {
        /// <summary>
        /// A secure password.
        /// </summary>
        public SecureString SecurePassword { get; }

        /// <summary>
        /// Indicates whether this value is storing a <c>SecureString</c> password.
        /// </summary>
        public bool HasSecurePassword =>
            SecurePassword != null;

        /// <summary>
        /// Creates a new <c>Password</c> by converting the supplied password <c>string</c> to a <c>SecureString</c>.
        /// If encrypted, <c>SecurePassword</c> is derived directly from decrypting <c>password</c> which attempts
        /// to leave no memory footprint exposing the unsecure, unencrypted password.
        /// </summary>
        /// <param name="password">A password <c>string</c>.</param>
        /// <param name="isEncrypted">A flag indicating whether the supplied password <c>string</c> is encrypted.</param>
        /// <param name="cryptoOptions">Options for password decryption, if applicable.</param>
        /// <seealso cref="Credential.Build(string, string, bool, CryptoOptions, string)" />
        public Password(string password, bool isEncrypted = false, CryptoOptions cryptoOptions = null)
        {
            if (isEncrypted)
            {
                SecurePassword = Decrypt.SecureString(password, cryptoOptions);
            }
            else
            {
                SecurePassword = TextUtil.ConvertToSecureString(password);
            }
        }

        /// <summary>
        /// Creates a new <c>Password</c> from the supplied <c>SecureString</c>.
        /// </summary>
        /// <param name="securePassword">A <c>SecureString</c> password.</param>
        public Password(SecureString securePassword)
        {
            SecurePassword = securePassword;
        }

        /// <summary>
        /// Converts the stored password to a <c>strrng</c>.
        /// </summary>
        /// <returns>A plain password</returns>
        public string ToUnsecurePassword() => HasSecurePassword
            ? TextUtil.ConvertToUnsecureString(SecurePassword)
            : null;

        /// <summary>
        /// Converts the stored password to an encrypted <c>strrng</c>.
        /// </summary>
        /// <returns>An encrypted password</returns>
        public string ToEncryptedPassword(CryptoOptions cryptoOptions) => HasSecurePassword
            ? Encrypt.String(TextUtil.ConvertToUnsecureString(SecurePassword), cryptoOptions)
            : null;

        /// <summary>
        /// A default value representing a non-existent password.
        /// </summary>
        public static Password NoPassword { get; } = default;

        /// <summary>
        /// Implicitly casts a string as a <c>Password</c>.
        /// </summary>
        /// <param name="password">A password <c>string</c>.</param>
        public static implicit operator Password(string password) => new Password(password);

        /// <summary>
        /// Implicitly casts a <c>SecureString</c> as a <c>Password</c>.
        /// </summary>
        /// <param name="securePassword">A secure password.</param>
        public static implicit operator Password(SecureString securePassword) => new Password(securePassword);

        /// <summary>
        /// Implicitly casts a <c>Password</c> back to a <c>SecureString</c>.
        /// </summary>
        /// <param name="password">A <c>Password</c>.</param>
        public static implicit operator SecureString(Password password) => password.SecurePassword;
    }
}
