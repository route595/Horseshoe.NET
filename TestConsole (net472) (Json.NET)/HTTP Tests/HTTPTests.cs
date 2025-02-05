using System;
using System.Collections.Generic;
using System.Net;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Http;
using Horseshoe.NET.Text;

namespace TestConsole.HTTPTests
{
    class HTTPTests : RoutineX
    {
        private string ProxyUrl { get; }
        private int ProxyPort { get; }
        private string ProxyUser { get; }
        private string ProxyPassword { get; }

        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Web Service GET",
                () =>
                {
                    try
                    {
                        var apiResponse = Get.AsJson<WebServiceResponse<string>>
                        (
                            "https://site.com/service/endpoint", 
                            alterHeaders: (hdrs) =>
                            {
                                hdrs.Add(HttpRequestHeader.Authorization, "Bearer " + "blabla");
                            }
                            //handleResponse: (metaData, stream) =>
                            //{
                            //    metaData.StatusCode == HttpStatusCode.
                            //}
                        );
                        Console.WriteLine("Response: " + apiResponse.Data);
                    }
                    catch(Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Web Service GET (JSON >> WebServiceResponse<IEnumerable<string>>)",
                () =>
                {
                    try
                    {
                        var apiResponse = Get.AsJson<WebServiceResponse<IEnumerable<string>>>
                        (
                            "https://site.com/service/endpoint"
                        );
                        Console.WriteLine("Response: " + string.Join(", ", apiResponse.Data));
                    }
                    catch(Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Web Service GET (JSON >> WebServiceResponse)",
                () =>
                {
                    try
                    {
                        var apiResponse = Get.AsJson<WebServiceResponse>
                        (
                            "https://site.com/service/endpoint"
                        );
                        if (apiResponse.Status == WebServiceResponseStatus.Error)
                        {
                            throw apiResponse.GetReconstitutedException();
                        }
                        Console.WriteLine("Response (" + apiResponse.Data.GetType().FullName + "): " + apiResponse.Data);
                    }
                    catch(Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Web Service GET (non-auth)",
                () =>
                {
                    try
                    {
                        var apiResponse = Get.AsString
                        (
                            "https://site.com/service/endpoint"
                        );
                        Console.WriteLine("Response: " + apiResponse);
                    }
                    catch(Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Web Service GET (non-exist)",
                () =>
                {
                    try
                    {
                        var apiResponse = Get.AsString
                        (
                            "https://site.com/service/endpoint",
                            alterHeaders: (hdrs) =>
                            {
                                hdrs.Add(HttpRequestHeader.Authorization, "Bearer " + "blabla");
                            }
                            //handleResponse: (metaData, stream) =>
                            //{
                            //    metaData.StatusCode == HttpStatusCode.
                            //}
                        );
                        Console.WriteLine("Response: " + apiResponse);
                    }
                    catch(Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Web Service GET w/ Proxy",
                () =>
                {
                    try
                    {
                        var apiResponse = Get.AsString
                        (
                            "https://jsonplaceholder.typicode.com/todos/1",
                            proxyAddress: "http://proxypool:9119",
                            proxyCredentials: new Credential("me", "my-proxy-pass")
                        );
                        Console.WriteLine("Response: " + apiResponse);
                    }
                    catch(Exception ex)
                    {
                        RenderX.Exception(ex);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Web Service w/ Header",
                () =>
                {
                    var wshUrl = "http://localhost:58917/fake/path";
                    Console.WriteLine("calling " + wshUrl + "...");
                    string text = Get.AsString
                    (
                        wshUrl,
                        alterHeaders: (hdrs) =>
                        {
                            hdrs.Add("My-Custom-Header", "Frankenstein");
                        },
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(text);
                }
            ),
            BuildMenuRoutine
            (
                "Web Service POST",
                () =>
                {
                    var postResponse = Get.AsJson<WebServiceResponse<string>>
                    (
                        "https://site.com/service/endpoint",
                        //content: new { Property1 = "Value1", Property2 = "Value2" },  // content removed from Get
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    if (postResponse.Status == WebServiceResponseStatus.Ok)
                    {
                        Console.WriteLine("response: " + postResponse.Data);
                    }
                    else
                    {
                        Console.WriteLine("exception: " + postResponse.Exception.Message);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Download document as string",
                () =>
                {
                    var docUrl = "https://www.example.com/index.html";
                    Console.WriteLine("downloading " + docUrl + "...");
                    string text = WebDocument.AsText
                    (
                        docUrl,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(text, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        proxyAddress: ProxyUrl,
                        proxyPort: ProxyPort,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.WriteLine((int)response.StatusCode + " (" + response.StatusCode + ")"); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test stream (google.com)",
                () =>
                {
                    using (var stream = WebDocument.AsStream
                    (
                        "https://www.google.com",
                        proxyAddress: ProxyUrl,
                        proxyPort: ProxyPort,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    ))
                    {
                        var buf = new byte[497];
                        int bytesRead = stream.Read(buf, 0, 497);
                        Console.WriteLine(System.Text.Encoding.Default.GetString(buf) + TruncateMarker.LongEllipsis);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com) (SSL3)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        securityProtocol: SecurityProtocolType.Ssl3,
                        proxyAddress: ProxyUrl,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com) (TLS)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        securityProtocol: SecurityProtocolType.Tls,
                        proxyAddress: ProxyUrl,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com) (TLS 1.1)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        securityProtocol: SecurityProtocolType.Tls11,
                        proxyAddress: ProxyUrl,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com) (TLS 1.2)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        securityProtocol: SecurityProtocolType.Tls12,
                        proxyAddress: ProxyUrl,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com) (TLS or TLS 1.2)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        securityProtocol: SecurityProtocolType.Tls | SecurityProtocolType.Tls12,
                        proxyAddress: ProxyUrl,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            ),
            BuildMenuRoutine
            (
                "https test (google.com) (SSL3 or TLS 1.2)",
                () =>
                {
                    var html = WebDocument.AsText
                    (
                        "https://www.google.com",
                        securityProtocol: SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12,
                        proxyAddress: ProxyUrl,
                        proxyCredentials: ProxyUser != null ? new Credential(ProxyUser, ProxyPassword) : null as Credential?,
                        handleResponse: (response, consumerResponse) => { Console.Write(response.StatusCode + " "); }
                    );
                    Console.WriteLine(TextUtil.Crop(html, 500, truncateMarker: TruncateMarker.LongEllipsis));
                }
            )
        };
    }
}
