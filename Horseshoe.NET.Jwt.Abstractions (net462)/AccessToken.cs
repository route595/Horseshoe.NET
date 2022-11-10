using System.Linq;

namespace Horseshoe.NET.Jwt
{
    public class AccessToken
    {
        public string EncodedToken { get; set; }
        public AccessTokenHeader Header { get; set; }
        public AccessTokenBody Body { get; set; }
        public string RawDigitalSignature { get; set; }

        public bool HasRole(string role, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(role))
                return false;

            return Body?.Roles?.Any(r => ignoreCase
                ? string.Equals(r, role, System.StringComparison.CurrentCultureIgnoreCase)
                : string.Equals(r, role)
            ) ?? false;
        }

        public override string ToString()
        {
            return EncodedToken ?? "";
        }
    }
}
