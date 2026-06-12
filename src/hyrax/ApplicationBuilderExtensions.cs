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
using System;
using Examine;
using hyrax.Core.Startup.Implement;

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

        private static IHyraxBuilder AddHyraxResources<TResource>(this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxResourceLocatorService>((serviceProvider) =>
                new HyraxUmbracoResourceLocatorService<TResource>(serviceProvider.GetRequiredService<IUmbracoContextFactory>(),
                    serviceProvider.GetRequiredService<IHyraxAuthorService>(),
                    resourceMapping, serviceProvider.GetRequiredService<IExamineManager>()));
            // ActivityPub services are optional and should be registered via WithActivityPub
            services.AddScoped<IHyraxSignatureRepositoryService>((serviceProvider) =>
                new HyraxFilesystemSignatureRepositoryService("./umbraco/hyrax/"));

            return new HyraxBuilder(services);
        }

        /// <summary>
        /// WARNING: This uses the AutomaticAuthorService, which is inefficient and not recommended for use with ActivityPub. Use a different overload to specify available authors.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="services"></param>
        /// <param name="resourceMapping"></param>
        public static IHyraxBuilder AddHyrax<TResource>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService, HyraxAutomaticAuthorService>();

            return services.AddHyraxResources(resourceMapping);
        }

        public static IHyraxBuilder AddHyrax<TResource>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping,
            IAuthor singleAuthor) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService>((serviceProvider) =>
                new HyraxSingleAuthorAuthorService(singleAuthor));

            return services.AddHyraxResources(resourceMapping);
        }

        public static IHyraxBuilder AddHyrax<TResource, TAuthorService>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping) where TResource : class, IPublishedContent where TAuthorService : class, IHyraxAuthorService
        {
            services.AddScoped<IHyraxAuthorService, TAuthorService>();

            return services.AddHyraxResources(resourceMapping);
        }

        public static IHyraxBuilder AddHyrax<TResource>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping,
            Func<IServiceProvider, IHyraxAuthorService> authorServiceFactory) where TResource : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService>(authorServiceFactory);

            return services.AddHyraxResources(resourceMapping);
        }

        public static IHyraxBuilder AddHyrax<TResource, TAuthor>(
            this IServiceCollection services,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping,
            Func<TAuthor, IAuthor> authorMapping) where TResource : class, IPublishedContent where TAuthor : class, IPublishedContent
        {
            services.AddScoped<IHyraxAuthorService>((IServiceProvider serviceProvider) => new HyraxUmbracoContentAuthorService<TAuthor>(
                serviceProvider.GetRequiredService<IUmbracoContextFactory>(),
                authorMapping,
                serviceProvider.GetRequiredService<IExamineManager>()
                ));

            return services.AddHyraxResources(resourceMapping);
        }
    }
}
