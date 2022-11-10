using Microsoft.Extensions.DependencyInjection;

namespace Horseshoe.NET.Caching
{
    public static class Extensions
    {
        public static IServiceCollection AddAppCache(this IServiceCollection services)
        {
            services.AddScoped<IAppCache, AppCache>();
            services.AddMemoryCache();
            return services;
        }
    }
}
