using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Horseshoe.NET.Http;

namespace TestMVC.ViewModels
{
    public class HomeIndexViewModel
    {
        public HttpRequest HttpRequest { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public string AbsoluteApplicationPath => HttpRequest.GetAbsoluteApplicationPath();
        public string AbsoluteApplicationPath_API => HttpRequest.GetAbsoluteApplicationPath(virtualSubpath: "/api");
    }
}
