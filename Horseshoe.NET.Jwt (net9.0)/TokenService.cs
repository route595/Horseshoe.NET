namespace Horseshoe.NET.Jwt;

public static class TokenService
{
    static Regex TokenPattern { get; } = new Regex(@"^[0-9a-z_-]+\.[0-9a-z_-]+(\.[0-9a-z_-]+)?$", RegexOptions.IgnoreCase);
    static Regex NotTokenPattern { get; } = new Regex("[^0-9a-z._-]", RegexOptions.IgnoreCase);

    /// <summary>
    /// Separates, decodes and deserializes a Base64 encoded OAuth token
    /// </summary>
    /// <param name="encodedToken"></param>
    /// <returns>A <c>Token</c></returns>
    public static AccessToken ParseToken(string encodedToken)
    {
        if (encodedToken == null)
        {
            throw new ValidationException("encoded token cannot be null");
        }
        if (encodedToken.StartsWith("Bearer"))
        {
            encodedToken = encodedToken[6..].Trim();
        }
        if (!TokenPattern.IsMatch(encodedToken))
        {
            var invalidChars = NotTokenPattern.Matches(encodedToken)
                .Select(m => m.Value)
                .Distinct();
            throw invalidChars.Any()
                ? new ValidationException("encoded token contains invalid characters: [" + string.Join("", invalidChars) + "]")
                : new ValidationException("invalid encoded token");
        }
        var token = new AccessToken { EncodedToken = encodedToken };
        var split = encodedToken.Split('.');
        split[0] = Decode.Base64.String(split[0]);
        split[1] = Decode.Base64.String(split[1]);
        token.Header = DeserializeTokenHeader(split[0]);
        token.Body = DeserializeTokenBody(split[1]);
        token.RawDigitalSignature = split.Length > 2 ? split[2] : null;
        return token;
    }

    public static AccessTokenHeader DeserializeTokenHeader(string tokenHeaderString)
    {
        try
        {
            var rawHeader = Deserialize.Json<AccessTokenHeader.Raw>(tokenHeaderString);
            return rawHeader.ToTokenHeader();
        }
        catch (Exception ex)
        {
            throw new ValidationException("Error deserializing token header: " + ex.Message, ex);
        }
    }

    public static AccessTokenBody DeserializeTokenBody(string tokenBodyString)
    {
        Exception? deserializeJsonArrayException = null;
        try
        {
            var rawBody_roleArray = Deserialize.Json<AccessTokenBody.Raw_RoleArray>(tokenBodyString);
            return rawBody_roleArray.ToTokenBody();
        }
        catch (Exception ex)
        {
            deserializeJsonArrayException = ex;
        }

        try
        {
            var rawBody_roleString = Deserialize.Json<AccessTokenBody.Raw_RoleString>(tokenBodyString);
            return rawBody_roleString.ToTokenBody();
        }
        catch
        {
            throw deserializeJsonArrayException;
        }
    }

    public static string CreateAccessToken(string tokenKey, string uniqueName, IEnumerable<string>? roles, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256, Encoding? symmetricKeyEncoding = null)
    {
        var claims = ClaimsService.BuildClaims(uniqueName, roles);
        return CreateAccessToken(tokenKey, claims, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm, symmetricKeyEncoding: symmetricKeyEncoding);
    }

    public static string CreateAccessToken(byte[] tokenKeyBytes, string uniqueName, IEnumerable<string>? roles, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256)
    {
        var claims = ClaimsService.BuildClaims(uniqueName, roles);
        return CreateAccessToken(tokenKeyBytes, claims, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAccessToken(string tokenKey, IEnumerable<Claim> claims, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256, Encoding? symmetricKeyEncoding = null)
    {
        tokenKey ??= TokenSettings.DefaultTokenKey;
        return CreateAccessToken((symmetricKeyEncoding ?? Encoding.Default).GetBytes(tokenKey), claims, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAccessToken(byte[] tokenKeyBytes, IEnumerable<Claim> claims, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(lifespanInSeconds),
            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(tokenKeyBytes) { KeyId = keyId },
                securityAlgorithm
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string CreateAdfsBasedToken(AdfsBasedOAuthQuery query, string tokenKey, IEnumerable<string> roles, string? firstName = null, string? lastName = null, string? displayName = null, string? email = null, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256, Encoding? symmetricKeyEncoding = null)
    {
        tokenKey ??= TokenSettings.DefaultTokenKey;
        var tokenKeyBytes = (symmetricKeyEncoding ?? Encoding.Default).GetBytes(tokenKey);
        return CreateAdfsBasedToken(query, tokenKeyBytes, roles, firstName: firstName, lastName: lastName, displayName: displayName, email: email, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAdfsBasedToken(AdfsBasedOAuthQuery query, byte[] tokenKeyBytes, IEnumerable<string> roles, string? firstName = null, string? lastName = null, string? displayName = null, string? email = null, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256)
    {
        return CreateAdfsBasedToken(tokenKeyBytes, query.Credentials.UserName, roles, roleClaimFilter: query.RoleClaimFilter, firstName: firstName, lastName: lastName, displayName: displayName, email: email, cred: query.Credentials, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAdfsBasedToken(AccessToken token, string tokenKey, RoleClaimFilter roleClaimFilter = default, NetworkCredential? cred = null, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256, Encoding? symmetricKeyEncoding = null)
    {
        tokenKey ??= TokenSettings.DefaultTokenKey;
        var tokenKeyBytes = (symmetricKeyEncoding ?? Encoding.Default).GetBytes(tokenKey);
        return CreateAdfsBasedToken(token, tokenKeyBytes, roleClaimFilter: roleClaimFilter, cred: cred, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAdfsBasedToken(AccessToken token, byte[] tokenKeyBytes, RoleClaimFilter roleClaimFilter = default, NetworkCredential? cred = null, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256)
    {
        var pos = token.Body.CommonName.IndexOf(',');
        return CreateAdfsBasedToken(tokenKeyBytes, token.Body.UniqueName, token.Body.Roles, roleClaimFilter: roleClaimFilter, firstName: pos > 1 ? token.Body.CommonName[(pos + 1)..].Trim() : null, lastName: pos > 1 ? token.Body.CommonName[..pos] : null, displayName: token.Body.CommonName, email: token.Body.Email, cred: cred, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAdfsBasedToken(string tokenKey, string uniqueName, IEnumerable<string> roles, RoleClaimFilter roleClaimFilter = default, string? firstName = null, string? lastName = null, string? displayName = null, string? email = null, NetworkCredential? cred = null, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256, Encoding? symmetricKeyEncoding = null)
    {
        tokenKey ??= TokenSettings.DefaultTokenKey;
        var tokenKeyBytes = (symmetricKeyEncoding ?? Encoding.Default).GetBytes(tokenKey);
        return CreateAdfsBasedToken(tokenKeyBytes, uniqueName, roles, roleClaimFilter: roleClaimFilter, firstName: firstName, lastName: lastName, displayName: displayName, email: email, cred: cred, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static string CreateAdfsBasedToken(byte[] tokenKeyBytes, string uniqueName, IEnumerable<string> roles, RoleClaimFilter roleClaimFilter = default, string? firstName = null, string? lastName = null, string? displayName = null, string? email = null, NetworkCredential? cred = null, string? keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256)
    {
        var claims = ClaimsService.BuildClaims(uniqueName, roles, roleClaimFilter: roleClaimFilter, firstName: firstName, lastName: lastName, displayName: displayName, email: email, cred: cred);
        return CreateAccessToken(tokenKeyBytes, claims, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
    }

    public static byte[] GetKeyFromBase64(string keyTextBase64)
    {
        return Convert.FromBase64String(keyTextBase64);
    }

    public static byte[] GetKeyFromText(string keyText)
    {
        return Encoding.ASCII.GetBytes(keyText);
    }

    public static byte[] GenerateKey(int size = 32)
    {
        var now = DateTime.Now;
        var rand = new Random(now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond);
        var array = new byte[size];
        for (int i = 0; i < size; i++)
        {
            //array[i] = i == 0
            //    ? GetRandomAlphanumericByte(rand, setId: 1)
            //    : GetRandomAlphanumericByte(rand);
            array[i] = GetRandomAlphanumericByte(rand);
        }
        return array;
    }

    static byte GetRandomAlphanumericByte(Random rand, int? setId = default)
    {
        var set = setId ?? rand.Next(0, 3);
        return set switch
        {
            0 => (byte)rand.Next(48, 58),  // num
            1 => (byte)rand.Next(65, 91),  // ucase
            2 => (byte)rand.Next(97, 123), // lcase
            _ => (byte)'#',
        };
    }

    public static string KeyToText(byte[] key)
    {
        return Encoding.ASCII.GetString(key);
    }

    public static string KeyToBase64(byte[] key)
    {
        return Convert.ToBase64String(key);
    }
}
