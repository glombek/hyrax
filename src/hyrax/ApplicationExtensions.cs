using Microsoft.AspNetCore.Builder;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using hyrax.Core;

namespace hyrax.Umbraco
{
    public static class ApplicationExtensions
    {
        public static IHyraxUmbracoApplicationBuilder UseHyrax(this IUmbracoApplicationBuilder app)
        {
            if (app is UmbracoApplicationBuilder uab)
            {
                uab.AppBuilder.UseHyrax();
            }
        }
    }
}
