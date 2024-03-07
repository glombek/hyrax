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
    public class HyraxUmbracoResourceLocatorService<TResource> : IHyraxResourceLocatorService where TResource : class, IPublishedContent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IHyraxAuthorService _authorService;
        private readonly Func<TResource, IHyraxAuthorService, IResource> _resourceMapping;

        public HyraxUmbracoResourceLocatorService(
            IUmbracoContextFactory umbracoContextFactory,
            IHyraxAuthorService authorService,
            Func<TResource, IHyraxAuthorService, IResource> resourceMapping)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _authorService = authorService;
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
                return publishedContent.Select(x => _resourceMapping(x, _authorService));
            }
        }
    }
}
