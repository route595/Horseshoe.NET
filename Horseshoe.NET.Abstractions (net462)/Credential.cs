using System;
using System.Net;

namespace Horseshoe.NET
{
    /// <summary>
    /// A generic user crediential designed to be compatible with all types of credentials across
    /// the Horseshoe.NET platform (e.g. network credentials, database credentials, etc.).
    /// </summary>
    public readonly struct Credential
    {
        /// <summary>
        /// The user name or ID.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// The user password.
        /// </summary>
        public Password Password { get; }

        /// <summary>
        /// The network domain.  Applies mainly to credentialed HTTP requests.
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// Creates a new <c>Credential</c>.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain">Optional, the network domain.</param>
        public Credential(string userName, Password password, string domain = null)
        {
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        /// <summary>
        /// Builds a <c>Credential</c> from its constituent parts.
        /// </summary>
        /// <param name="userName">A user name or ID.</param>
        /// <param name="password">A password <c>string</c>.</param>
        /// <param name="domain">Optional, the network domain.</param>
        /// <returns>A <c>Credential</c> or <c>null</c> if <c>userName == null</c>.</returns>
        public static Credential? Build(string userName, string password, string domain = null)
        {
            return Build(userName, new Password(password), domain: domain);
        }

        /// <summary>
        /// Builds a <c>Credential</c> from its constituent parts.
        /// </summary>
        /// <param name="userName">A user name or ID.</param>
        /// <param name="getPassword">A no-arg function for getting a password <c>string</c>.</param>
        /// <param name="domain">Optional, the network domain.</param>
        /// <returns>A <c>Credential</c> or <c>null</c> if <c>userName == null</c>.</returns>
        public static Credential? Build(string userName, Func<string> getPassword, string domain = null)
        {
            return Build(userName, new Password(getPassword), domain: domain);
        }

        /// <summary>
        /// Builds a <c>Credential</c> from its constituent parts.
        /// </summary>
        /// <param name="userName">A user name or ID.</param>
        /// <param name="password">A <c>Password</c> (can substitute with a password <c>string</c> or <c>SecureString</c>).</param>
        /// <param name="domain">Optional, the network domain.</param>
        /// <returns>A <c>Credential</c> or <c>null</c> if <c>userName == null</c>.</returns>
        public static Credential? Build(string userName, Password password, string domain = null)
        {
            if (userName == null)
                return null;
            return new Credential(userName, password, domain: domain);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string pwdMsg = Password.HasSecurePassword
                ? "has-password"
                : "no-password";

            return UserName + " [" + pwdMsg + "]" + (Domain != null ? " @" + Domain : "");
        }

        /// <summary>
        /// Converts this <c>Credential</c> to a <c>NetworkCredential</c>.
        /// </summary>
        /// <returns></returns>
        public NetworkCredential ToNetworkCredential()
        {
            return Domain != null
                ? new NetworkCredential(UserName, Password, Domain)
                : new NetworkCredential(UserName, Password);
        }

        /// <summary>
        /// Implicitly casts this <c>Credential</c> as a <c>NetworkCredential</c>.
        /// </summary>
        /// <param name="credentials"></param>
        public static implicit operator NetworkCredential(Credential credentials) => credentials.ToNetworkCredential();
    }
}
