using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace hyrax.Core.Startup
{
    public static class ApplicationExtensions
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
                            Action = "Get"
                        });
            });
        }

        public static void AddHyrax(this IServiceCollection services)
        {
            services.AddScoped<>
        }
    }
}
