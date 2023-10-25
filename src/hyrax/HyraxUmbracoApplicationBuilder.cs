using hyrax.Core;
using hyrax.Core.Startup;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace hyrax.Umbraco
{
    public class HyraxUmbracoApplicationBuilder : IHyraxUmbracoApplicationBuilder
    {
        public HyraxUmbracoApplicationBuilder(IUmbracoApplicationBuilder umbracoApplicationBuilder, IHyraxApplicationBuilder hyraxApplicationBuilder)
        {
            UmbracoApplicationBuilder = umbracoApplicationBuilder;
            HyraxApplicationBuilder = hyraxApplicationBuilder;
        }

        public IUmbracoApplicationBuilder UmbracoApplicationBuilder { get; }
        public IHyraxApplicationBuilder HyraxApplicationBuilder { get; }
    }
}
