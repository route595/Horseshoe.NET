using System;

namespace Horseshoe.NET.Jwt
{
    public class AccessTokenBody
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string UniqueName { get; set; }
        public string FullUniqueName { get; set; }  // specific to Horseshoe.NET
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string CommonName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
        public DateTime NotBeforeDateUTC { get; set; }
        public DateTime ExpirationDateUTC { get; set; }
        public DateTime IssueDateUTC { get; set; }
        public DateTime NotBeforeDate => NotBeforeDateUTC.ToLocalTime();
        public DateTime ExpirationDate => ExpirationDateUTC.ToLocalTime();
        public DateTime IssueDate => IssueDateUTC.ToLocalTime();
        public string AppType { get; set; }
        public string AppID { get; set; }
        public string AuthMethod { get; set; }
        public DateTime? AuthTime { get; set; }
        public string Version { get; set; }
        public string Scope { get; set; }

        public class Raw_RoleString
        {
#pragma warning disable IDE1006 // Naming Styles
            public string aud { get; set; }
            public string iss { get; set; }
            public string unique_name { get; set; }
            public string full_unique_name { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string commonname { get; set; }
            public string email { get; set; }
            public string role { get; set; }
            public long nbf { get; set; }
            public long exp { get; set; }
            public long iat { get; set; }
            public string apptype { get; set; }
            public string appid { get; set; }
            public string authmethod { get; set; }
            public string auth_time { get; set; }
            public string ver { get; set; }
            public string scp { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            public AccessTokenBody ToTokenBody()
            {
                return new AccessTokenBody
                {
                    Audience = aud,
                    Issuer = iss,
                    UniqueName = unique_name,
                    FullUniqueName = full_unique_name,
                    GivenName = given_name,
                    FamilyName = family_name,
                    CommonName = commonname,
                    Email = email,
                    Roles = role != null ? new[] { role } : null,
                    NotBeforeDateUTC = DateTime.UnixEpoch.AddSeconds(nbf),
                    ExpirationDateUTC = DateTime.UnixEpoch.AddSeconds(exp),
                    IssueDateUTC = DateTime.UnixEpoch.AddSeconds(iat),
                    AppType = apptype,
                    AppID = appid,
                    AuthMethod = authmethod,
                    AuthTime = auth_time != null ? DateTime.Parse(auth_time) : null as DateTime?,
                    Version = ver,
                    Scope = scp
                };
            }
        }

        public class Raw_RoleArray
        {
#pragma warning disable IDE1006 // Naming Styles
            public string aud { get; set; }
            public string iss { get; set; }
            public string unique_name { get; set; }
            public string full_unique_name { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string commonname { get; set; }
            public string email { get; set; }
            public string[] role { get; set; }
            public long nbf { get; set; }
            public long exp { get; set; }
            public long iat { get; set; }
            public string apptype { get; set; }
            public string appid { get; set; }
            public string authmethod { get; set; }
            public string auth_time { get; set; }
            public string ver { get; set; }
            public string scp { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            public AccessTokenBody ToTokenBody()
            {
                return new AccessTokenBody
                {
                    Audience = aud,
                    Issuer = iss,
                    UniqueName = unique_name,
                    GivenName = given_name,
                    FamilyName = family_name,
                    CommonName = commonname,
                    Email = email,
                    Roles = role,
                    NotBeforeDateUTC = DateTime.UnixEpoch.AddSeconds(nbf),
                    ExpirationDateUTC = DateTime.UnixEpoch.AddSeconds(exp),
                    IssueDateUTC = DateTime.UnixEpoch.AddSeconds(iat),
                    AppType = apptype,
                    AppID = appid,
                    AuthMethod = authmethod,
                    AuthTime = auth_time != null ? DateTime.Parse(auth_time) : null as DateTime?,
                    Version = ver,
                    Scope = scp
                };
            }
        }
    }
}
