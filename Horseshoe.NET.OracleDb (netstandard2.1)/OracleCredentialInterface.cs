using Oracle.ManagedDataAccess.Client;

namespace Horseshoe.NET.OracleDb
{
    public class OracleCredentialInterface
    {
        private OracleCredential _oracleCredentials;
        private Credential? _credentials;

        public OracleCredentialInterface(OracleCredential orclCredentials)
        {
            _oracleCredentials = orclCredentials;
        }

        public OracleCredentialInterface(Credential credentials)
        {
            _credentials = credentials;
        }

        public OracleCredential ToOracleCredentials()
        {
            return _oracleCredentials ?? ToOracleCredentials(_credentials);
        }

        public Credential? ToCredentials()
        {
            return _credentials ?? ToCredentials(_oracleCredentials);
        }

        public static Credential? ToCredentials(OracleCredential oracleCredentials)
        {
            if (oracleCredentials == null)
                return null;
            return new Credential(oracleCredentials.UserId, oracleCredentials.Password);
        }

        public static OracleCredential ToOracleCredentials(Credential? credentials)
        {
            if (!credentials.HasValue)
                return null;
            return new OracleCredential(credentials.Value.UserName, credentials.Value.Password);
        }

        public override string ToString()
        {
            if (_oracleCredentials != null)
                return _oracleCredentials.UserId + " [" + (_oracleCredentials.Password != null ? "secure-password" : "no-password") + "]";
            if (_credentials.HasValue)
                return _credentials.ToString();
            return "[null-crededentials]";
        }

        public static implicit operator OracleCredentialInterface(Credential credentials) => new OracleCredentialInterface(credentials);
        public static implicit operator OracleCredentialInterface(OracleCredential orclCredentials) => new OracleCredentialInterface(orclCredentials);
        public static implicit operator OracleCredential(OracleCredentialInterface orclCredentialInterface) => orclCredentialInterface.ToOracleCredentials();
    }
}
