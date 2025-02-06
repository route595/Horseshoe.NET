![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Jwt

A set of common methods and classes for handling JWTs and a subset of basic OAuth operations.

## Code examples

```
// note: the same digital signature crypto key that creates the digital signature is also used to validate it
// note: roles may very well include Active Directory group memberships
TokenService.CreateAccessToken 
(
    tokenKey,            // e.g. encoding.GetBytes("ah476&ewj^!09")
	roles,               // e.g. { "All Contractors", "Domain Admin" }
	keyId,               // e.g. "0001"
	lifespanInSeconds,   // default is 3600 (1 hour)
	securityAlgorithm    // default is "HS256"
);                                            // -> "eyjg73ls0..." (encoded JWT)

// parse a token string
TokenService.ParseToken("eyjg73ls0...");      // -> token as instance of AccessToken
```
