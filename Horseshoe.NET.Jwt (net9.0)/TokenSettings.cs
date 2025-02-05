namespace Horseshoe.NET.Jwt;

public static class TokenSettings
{
    private static string? _defaultTokenKey;
    public static string DefaultTokenKey
    {
        get => _defaultTokenKey
            ?? EnvVar.Get("HORSESHOENET_TOKEN_KEY")                         // source: Kubernetes YAML, etc.
            ?? Config.Get("UserSecrets:Horseshoe.NET:Jwt:TokenKey")         // source: user secrets
            ?? Config.Get("Horseshoe.NET:Jwt:TokenKey")                     // source: appsettings.json / user secrets
            ?? throw new JwtException("A token key was not found.  Try setting either TokenSettings.DefaultTokenKey or Horseshoe.NET:Jwt:TokenKey (appsettings.json).");
        set => _defaultTokenKey = value;
        
    }

    private static string? _defaultKeyId;
    public static string DefaultKeyId
    {
        get => _defaultKeyId
            ?? EnvVar.Get("HORSESHOENET_KEY_ID")                    // source: Kubernetes YAML, etc.
            ?? Config.Get("UserSecrets:Horseshoe.NET:Jwt:KeyId")    // source: user secrets
            ?? Config.Get("Horseshoe.NET:Jwt:KeyId")                // source: appsettings.json / user secrets
            ?? "0001";
        set => _defaultKeyId = value;
    }

    private static string? _defaultOAuthSourcedKeyId;
    public static string DefaultOAuthSourcedKeyId
    {
        get => _defaultOAuthSourcedKeyId
            ?? EnvVar.Get("HORSESHOENET_OAUTH_SOURCED_KEY_ID")                  // source: Kubernetes YAML, etc.
            ?? Config.Get("UserSecrets:Horseshoe.NET:Jwt:OAuthSourcedKeyId")    // source: user secrets
            ?? Config.Get("Horseshoe.NET:Jwt:OAuthSourcedKeyId")                // source: appsettings.json / user secrets
            ?? "0004";
        set => _defaultOAuthSourcedKeyId = value;
    }
}
