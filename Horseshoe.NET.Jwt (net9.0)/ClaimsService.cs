namespace Horseshoe.NET.Jwt;

public static class ClaimsService
{
    public static IEnumerable<string> FilterRoles(IEnumerable<string>? roles, RoleClaimFilter roleClaimFilter)
    {
        var list = new List<string>();
        if (roles == null)
            return list;
        if (!roleClaimFilter.Any())
            return roles;
        foreach (var filt in roleClaimFilter.Filters)
        {
            if (filt.StartsWith('*'))
            {
                if (filt.EndsWith('*'))
                {
                    list.AddRange(roles.Where(r => r.ToUpper().Contains(filt.ToUpper()[1..^1])));
                }
                else
                {
                    list.AddRange(roles.Where(r => r.ToUpper().EndsWith(filt.ToUpper()[1..])));
                }
            }
            else if (filt.EndsWith('*'))  // most common usage: e.g. GROUP_PREFIX.*
            {
                list.AddRange(roles.Where(r => r.ToUpper().StartsWith(filt.ToUpper()[..^1])));
            }
            else
            {
                list.AddRange(roles.Where(r => string.Equals(r, filt, StringComparison.OrdinalIgnoreCase)));
            }
        }
        return list.ToArray();
    }

    public static IEnumerable<Claim> BuildClaims(string uniqueName, IEnumerable<string>? roles, RoleClaimFilter roleClaimFilter = default, string? firstName = null, string? lastName = null, string? displayName = null, string? email = null, NetworkCredential? cred = null)
    {
        var list = new List<Claim>
        {
            new Claim("unique_name", uniqueName)
        };
        if (firstName != null)
        {
            list.Add(new Claim(ClaimTypes.GivenName, firstName));
        }
        if (lastName != null)
        {
            list.Add(new Claim(ClaimTypes.Surname, lastName));
        }
        if (displayName != null)
        {
            list.Add(new Claim("commonname", displayName));
        }
        if (email != null)
        {
            list.Add(new Claim(ClaimTypes.Email, email));
        }
        if (roles != null && roles.Any())
        {
            list.AddRange
            (
                FilterRoles(roles, roleClaimFilter)
                    .Select(r =>
                        new Claim(ClaimTypes.Role, r)
                    )
            );
        }
        if (cred?.UserName != null)
        {
            list.Add(new Claim("full_unique_name", cred.GetFullyQualifiedUserId()));
        }
        return list;
    }
}
