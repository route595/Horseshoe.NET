using System.Data.SqlClient;

namespace Horseshoe.NET.SqlDb
{
    public class SqlCredentialInterface
    {
        public SqlCredential SqlCredentials { get; }

        public SqlCredentialInterface(SqlCredential sqlCredentials)
        {
            SqlCredentials = sqlCredentials;
        }

        public SqlCredentialInterface(Credential credentials)
        {
            SqlCredentials = ToSqlCredential(credentials);
        }

        public static SqlCredential ToSqlCredential(Credential? credentials)
        {
            if (!credentials.HasValue)
                return null;
            return new SqlCredential(credentials.Value.UserName, credentials.Value.Password);
        }

        public override string ToString()
        {
            return SqlCredentials != null
                ? SqlCredentials.UserId + " [" + (SqlCredentials.Password != null ? "secure-password" : "no-password") + "]"
                : "[null-credentials]";
        }

        public static implicit operator SqlCredentialInterface(Credential credentials) => new SqlCredentialInterface(credentials);
        public static implicit operator SqlCredentialInterface(SqlCredential sqlCredentials) => new SqlCredentialInterface(sqlCredentials);
        public static implicit operator SqlCredential(SqlCredentialInterface sqlCredentialInterface) => sqlCredentialInterface.SqlCredentials;
    }
}
