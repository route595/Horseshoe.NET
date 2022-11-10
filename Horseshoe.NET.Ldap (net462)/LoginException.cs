using System;

using Novell.Directory.Ldap;

namespace Horseshoe.NET.Ldap
{
    public class LoginException : Exception
    {
        public bool HasInnerLdapException =>
            InnerException != null;

        public LdapException InnerLdapException =>
            HasInnerLdapException
            ? InnerException as LdapException
            : null;

        public LoginException(string message) : base(message) { }

        public LoginException(string message, LdapException innerException) : base(message, innerException) { }
    }
}
