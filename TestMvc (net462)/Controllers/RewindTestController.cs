using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Horseshoe.NET;
using Horseshoe.NET.Http;
using TestMvc.ViewModels;

namespace TestMvc.Controllers
{
    public class RewindTestController : Controller
    {
        // GET: RewindTest
        public ActionResult Index()
        {
            return View(new RewindTestIndexViewModel());
        }

        [HttpPost]
        public ActionResult RewindTest(string message, int age, DateTime dateTime)
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
    }
}