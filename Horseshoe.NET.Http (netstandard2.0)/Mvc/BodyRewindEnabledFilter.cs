using System.Linq;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Horseshoe.NET.Http.Mvc
{
    internal class BodyRewindEnabledFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // do nothing
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor.MethodInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(EnableBodyRewindAttribute)))
            {
                context.HttpContext.Request.EnableBuffering();
                var bodyControl = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (bodyControl != null)
                {
                    bodyControl.AllowSynchronousIO = true;
                }
            }
        }
    }
}
