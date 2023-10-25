using hyrax.Core.Services;
using hyrax.Core.Services.Implement;
using hyrax.Core.Startup.Implement;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace hyrax.Core.Startup
{
    public static class ApplicationBuilderExtensions
    {
        public static IHyraxApplicationBuilder UseHyrax(this IApplicationBuilder app)
        {
            app.UseEndpoints(u =>
            {

                u.MapControllerRoute(
                        "Hyrax ActivityPub WebFinger",
                        "/.well-known/webfinger",
                        new
                        {
                            Controller = "WebFinger",
                            Action = "Get"
                        });


                u.MapControllerRoute(
                        "Hyrax ActivityPub Other",
                        "/activitypub/{controller}",
                        new
                        {
                            Action = "Get"
                        });


                u.MapControllerRoute(
                        "Hyrax RSS",
                        "/rss",
                        new
                        {
                            Controller = "Rss",
                            Action = "Get"
                        });
            });

            return new HyraxApplicationBuilder(app);
        }

        public static void AddHyrax<THyraxResourceLocatorService>(this IServiceCollection services) where THyraxResourceLocatorService : class, IHyraxResourceLocatorService
        {
            services.AddScoped<IHyraxResourceLocatorService, THyraxResourceLocatorService>();
            services.AddScoped<IHyraxActivityService, HyraxActivityService>();
        }
    }
}
