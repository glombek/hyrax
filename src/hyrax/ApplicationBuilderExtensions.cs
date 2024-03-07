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
        //public static IHyraxUmbracoApplicationBuilder UseHyrax(this IUmbracoApplicationBuilder umbracoApplicationBuilder, IApplicationBuilder applicationBuilder)
        //{
        //    var appBuilder = applicationBuilder.UseHyrax();
        //    return new HyraxUmbracoApplicationBuilder(umbracoApplicationBuilder, appBuilder);
        //}

        //public static IHyraxUmbracoApplicationBuilder UseHyrax(this IUmbracoApplicationBuilder umbracoApplicationBuilder)
        //{
        //    if (umbracoApplicationBuilder is UmbracoApplicationBuilder uab)
        //    {
        //        return uab.UseHyrax(uab.AppBuilder);
        //    }
        //    throw new ArgumentException("Must be of type Umbraco.Cms.Web.Common.ApplicationBuilder, otherwise use the overload accepting an IApplicationBuilder.", "umbracoApplicationBuilder");
        //}

        private static void AddHyraxResources<TResource>(this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, IResource> resourceMapping) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxResourceLocatorService>((serviceProvider) =>
                new HyraxUmbracoResourceLocatorService<TResource>(serviceProvider.GetRequiredService<IUmbracoContextFactory>(),
                    serviceProvider.GetRequiredService<IHyraxAuthorService>(),
                    resourceMapping));
            services.AddScoped<IHyraxActivityService, HyraxActivityService>();
        }

        /// <summary>
        /// WARNING: This uses the AutomaticAuthorService, which is inefficient and not recommended for use with ActivityPub. Use a different overload to specify available authors.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="services"></param>
        /// <param name="resourceMapping"></param>
        public static void AddHyrax<TResource>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, IResource> resourceMapping) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService, HyraxAutomaticAuthorService>();

            services.AddHyraxResources(resourceMapping);
        }

        public static void AddHyrax<TResource>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, IResource> resourceMapping,
            IAuthor singleAuthor) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService>((serviceProvider) =>
                new HyraxSingleAuthorAuthorService(singleAuthor));

            services.AddHyraxResources(resourceMapping);
        }

        public static void AddHyrax<TResource, TAuthorService>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, IResource> resourceMapping) where TResource : class, IPublishedContent where TAuthorService : class, IHyraxAuthorService
        {
            services.AddScoped<IHyraxAuthorService, TAuthorService>();

            services.AddHyraxResources(resourceMapping);
        }

        public static void AddHyrax<TResource>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, IResource> resourceMapping,
            Func<IServiceProvider, IHyraxAuthorService> authorServiceFactory) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService>(authorServiceFactory);

            services.AddHyraxResources(resourceMapping);
        }
    }
}
