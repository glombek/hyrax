using Microsoft.AspNetCore.Builder;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using hyrax.Core.Startup;
using Umbraco.Cms.Core.Models.ContentEditing;
using hyrax.Core.Services.Implement;
using hyrax.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using hyrax.Core.Models;
using Hyrax.Umbraco.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace hyrax.Umbraco
{
    public static class ApplicationBuilderExtensions
    {
        public static IHyraxUmbracoApplicationBuilder UseHyrax(this IUmbracoApplicationBuilder umbracoApplicationBuilder, IApplicationBuilder applicationBuilder)
        {
            var appBuilder = applicationBuilder.UseHyrax();
            return new HyraxUmbracoApplicationBuilder(umbracoApplicationBuilder, appBuilder);
        }

        public static IHyraxUmbracoApplicationBuilder UseHyrax(this IUmbracoApplicationBuilder umbracoApplicationBuilder)
        {
            if (umbracoApplicationBuilder is UmbracoApplicationBuilder uab)
            {
                return uab.UseHyrax(uab.AppBuilder);
            }
            throw new ArgumentException("Must be of type Umbraco.Cms.Web.Common.ApplicationBuilder, otherwise use the overload accepting an IApplicationBuilder.", "umbracoApplicationBuilder");
        }

        public static void AddHyrax<TResource>(this IServiceCollection services, Func<TResource, IResource> resourceMapping) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxResourceLocatorService>((serviceProvider) => new UmbracoHyraxResourceLocatorService<TResource>(serviceProvider.GetRequiredService<IUmbracoContextFactory>(), resourceMapping));
            services.AddScoped<IHyraxActivityService, HyraxActivityService>();
        }
    }
}
