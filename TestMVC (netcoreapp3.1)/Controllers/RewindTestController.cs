using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using Horseshoe.NET;
using Horseshoe.NET.Http;
using Horseshoe.NET.Http.Mvc;
using TestMVC.ViewModels;

namespace TestMVC.Controllers
{
    public class RewindTestController : Controller
    {
        public IActionResult Index()
        {
            return View(new RewindTestIndexViewModel());
        }

        [HttpPost]
        [EnableBodyRewind]
        public IActionResult RewindTest(string message, int age, DateTime dateTime)
        {
            var model = new RewindTestIndexViewModel();
            try
            {
                model.RewindDisplayParams = new StringBuilder()
                    .AppendLine("Message: " + message)
                    .AppendLine("Age: " + age)
                    .AppendLine("Date/time: " + dateTime)
                    .ToString();
                model.RewindBody = Request.GetBodyAsString().Replace("&", Environment.NewLine + "&");
            }
            catch (Exception ex)
            {
                model.RewindError = ex.RenderMessage();
            }
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult NoRewindTest(string message, int age, DateTime dateTime)
        {
            var model = new RewindTestIndexViewModel();
            try
            {
                model.NoRewindDisplayParams = new StringBuilder()
                    .AppendLine("Message: " + message)
                    .AppendLine("Age: " + age)
                    .AppendLine("Date/time: " + dateTime)
                    .ToString();
                model.NoRewindBody = Request.GetBodyAsString().Replace("&", Environment.NewLine + "&");
            }
            catch (Exception ex)
            {
                model.NoRewindError = ex.RenderMessage();
            }
            return View("Index", model);
        }
    }
}
