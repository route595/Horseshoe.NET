![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.ActiveDirectory

Services for Active Directory authentication and LDAP querying.

## Code Examples

```c#
var userIdNameOrEmail = "djones1" -or- "david.jones@mybiz.com" -or- "Jones, David E. [Contractor]";
var ldapProperties = "name|cn|samaccountname|memberof";   // (optional)

var user = ADUtil.GetUser(userIdNameOrEmail, propertiesToLoad: ldapProperties);
-or-
var user = ADUtil.Authenticate(userIdNameOrEmail, "P@$$W0rd123", propertiesToLoad: ldapProperties);

user.GetGroups(orderByAdsPath: true);        // "All Contractors", "DB Admins", "Hiring Team"...
user.OU;                                     // "Contractors"
user.RawOU;                                  // LDAP://OU=Contractors,DC=mybiz,DC=com
ADUtil.ListOUs();                            // (lists all OUs in Active Directory)
ADEngine.GetDomainContext().ConnectedServer; // (lists name of connected domain controller)
```
