using hyrax.Core;
using hyrax.Core.Startup;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace hyrax.Umbraco
{
    public interface IHyraxUmbracoApplicationBuilder
    {
        IHyraxApplicationBuilder HyraxApplicationBuilder { get; }
        IUmbracoApplicationBuilder UmbracoApplicationBuilder { get; }
    }
}
