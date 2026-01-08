using System.Net;

namespace Horseshoe.NET.Jwt
{
    public class AdfsBasedOAuthQuery
    {
        public NetworkCredential Credentials { get; set; }
        public string RoleClaimFilter { get; set; }
    }
}
