![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Jwt

A set of common methods and classes for handling JWTs and a subset of basic OAuth operations.

## Code Examples

```c#
// parse a token
// note: If ADFS '/token' provided the JWT roles will include Active Directory group memberships
TokenService.ParseToken("eyjg73ls0...");      // -> token as instance of AccessToken
```
