using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Horseshoe.NET.Http.Mvc
{
    public static class Extensions
    {
        public static void AddMvcWithRewind(this IServiceCollection services, Action<MvcOptions> setupAction = null)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<BodyRewindEnabledFilter>();
                setupAction?.Invoke(options);
            });
        }
    }
}
