using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public class OracleCredentialInterface
    {
        public OracleCredential OracleCredentials { get; }

        public OracleCredentialInterface(OracleCredential orclCredentials)
        {
            OracleCredentials = orclCredentials;
        }

        public OracleCredentialInterface(Credential credentials)
        {
            OracleCredentials = ToOracleCredentials(credentials);
        }

        public static OracleCredential ToOracleCredentials(Credential? credentials)
        {
            if (!credentials.HasValue)
                return null;
            return new OracleCredential(credentials.Value.UserName, credentials.Value.Password);
        }

        public override string ToString()
        {
            return OracleCredentials != null
                ? OracleCredentials.UserId + " [" + (OracleCredentials.Password != null ? "secure-password" : "no-password") + "]"
                : "[null-credentials]";
        }

        public static implicit operator OracleCredentialInterface(Credential credentials) => new OracleCredentialInterface(credentials);
        public static implicit operator OracleCredentialInterface(OracleCredential orclCredentials) => new OracleCredentialInterface(orclCredentials);
        public static implicit operator OracleCredential(OracleCredentialInterface orclCredentialInterface) => orclCredentialInterface.OracleCredentials;
    }
}
