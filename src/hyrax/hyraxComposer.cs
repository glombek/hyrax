using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace hyrax.Umbraco
{
    internal class hyraxComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ManifestFilters().Append<HyraxManifestFilter>();
        }
    }
}
