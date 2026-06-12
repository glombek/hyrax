using Microsoft.AspNetCore.Builder;

namespace hyrax.Core.Startup
{
    public static class ActivityPubApplicationBuilderExtensions
    {
        public static IHyraxApplicationBuilder WithActivityPub(this IHyraxApplicationBuilder hyrax)
        {
            hyrax.App.UseEndpoints(u =>
            {
                u.MapControllerRoute(
                    "Hyrax ActivityPub Other",
                    "/activitypub/{action}/{id?}",
                    new { Controller = "ActivityPub" }
                );
            });

            return hyrax;
        }
    }
}
