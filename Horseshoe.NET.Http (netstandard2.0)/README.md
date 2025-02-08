![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Http

Web service and web API boilerplate code.

## Code Examples

```c#
// basic HTTP call
var apiResponse = Get.AsJson<WebServiceResponse<string>>
(
    "https://site.com/service/endpoint"
);
apiResponse.Data;    // { "requestedItems" : [ { "name": "Item ABC"}, { "name": "Item DEF"}... ] }

// HTTP call with JWT authorization
var apiResponse = Get.AsJson<WebServiceResponse<string>>
(
    "https://site.com/service/endpoint", 
    alterHeaders: (hdrs) =>
        hdrs.Add(HttpRequestHeader.Authorization, "Bearer " + "blabla")
);
apiResponse.Data;    // { "authorizedItems" : [ { "name": "Item ABC"}, { "name": "Item DEF"}... ] }
```
