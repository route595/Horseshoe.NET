using System;
using System.Security;

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
        /// <para>
        /// For example...
        /// <code>
        /// var pwd = new Password("mYpa$$w0rdr0ck$");
        /// </code>
        /// </para>
        /// If encrypted, please decrypt prior to supplying to this constructor or use the constructor with the 'getPassword' param.
        /// <para>
        /// For example...
        /// <code>
        /// var pwd = new Password(() => Decrypt.String("$t%u8!gc)dL;-+=Qqu!@%"));
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="password">A password <c>string</c>.</param>
        /// <seealso cref="Credential.Build(string, string, string)" />
        public Password(string password)
        {
            if (string.IsNullOrEmpty(password))
                SecurePassword = null;
            SecurePassword = TextUtil.ConvertToSecureString(password);
        }

        /// <summary>
        /// Creates a new <c>Password</c> by converting the password <c>string</c> from the supplied getter to a <c>SecureString</c>.
        /// <para>
        /// For example...
        /// <code>
        /// var pwd = new Password(() => Decrypt.String("$t%u8!gc)dL;-+=Qqu!@%"));
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="getPassword">A no-arg function for getting a password <c>string</c>.</param>
        /// <seealso cref="Credential.Build(string, string, string)" />
        public Password(Func<string> getPassword) : this(getPassword.Invoke())
        {
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
