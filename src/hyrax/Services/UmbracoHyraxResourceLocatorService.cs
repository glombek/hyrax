using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;
using hyrax.Core.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Extensions;

namespace Hyrax.Umbraco.Services
{
    public class UmbracoHyraxResourceLocatorService<TResource> : IHyraxResourceLocatorService where TResource : class, IPublishedContent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly Func<TResource, IResource> _resourceMapping;

        public UmbracoHyraxResourceLocatorService(IUmbracoContextFactory umbracoContextFactory, Func<TResource, IResource> resourceMapping)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _resourceMapping = resourceMapping;
        }
        public IEnumerable<IResource> GetResources(string? culture = null)
        {
            using (var contextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if(contextRef.UmbracoContext.Content == null)
                {
                    throw new Exception("Content cache is empty");
                }

                var publishedContent = contextRef.UmbracoContext.Content.GetAtRoot(culture).DescendantsOrSelf<TResource>();
                return publishedContent.Select(x => _resourceMapping(x));
            }
        }
    }
}
