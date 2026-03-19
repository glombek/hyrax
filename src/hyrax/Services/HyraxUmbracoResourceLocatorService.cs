using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examine;
using hyrax.Core.Models;
using hyrax.Core.Models.Implement;
using hyrax.Core.Services;
using Umbraco.Cms.Core.Media.EmbedProviders;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Extensions;

namespace Hyrax.Umbraco.Services
{
    public class HyraxUmbracoResourceLocatorService<TResource> : IHyraxResourceLocatorService where TResource : class, IPublishedContent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IHyraxAuthorService _authorService;
        private readonly Func<TResource, IHyraxAuthorService, Task<IResource>> _resourceMapping;
        private readonly IExamineManager _examineManager;

        public HyraxUmbracoResourceLocatorService(
            IUmbracoContextFactory umbracoContextFactory,
            IHyraxAuthorService authorService,
            Func<TResource, IHyraxAuthorService, Task<IResource>> resourceMapping,
            IExamineManager examineManager)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _authorService = authorService;
            _resourceMapping = resourceMapping;
            _examineManager = examineManager;
        }

        public async Task<IResource?> GetResource(string id)
        {
            using (var contextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var contentCache = contextRef.UmbracoContext.Content;
                if (contentCache == null)
                {
                    throw new Exception("Content cache is empty");
                }

                if (await contentCache.GetByIdAsync(int.Parse(id)) is not TResource content)
                {
                    return null;
                }

                var resource = await _resourceMapping(content, _authorService);
                return resource;
            }
        }

        public async Task<IEnumerable<IResource>> GetResources(string? culture = null, IAuthor? author = null)
        {
            var index = _examineManager.TryGetIndex("ExternalIndex", out var externalIndex) ? externalIndex : null;
            if (index == null)
            {
                throw new Exception("ExternalIndex not found");
            }

            // Get the alias from the PublishedModelAttribute
            var docTypeAlias = typeof(TResource).GetCustomAttributes(typeof(PublishedModelAttribute), true)
                .FirstOrDefault() is PublishedModelAttribute attr ? attr.ContentTypeAlias : null;

            if (docTypeAlias == null)
            {
                throw new Exception($"Alias for {nameof(TResource)} not found");
            }

            // Adjust the contentType alias as needed (e.g., "resource")
            var results = index.Searcher.CreateQuery("content")
                .NodeTypeAlias(docTypeAlias)
                .Execute();

            using (var contextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var contentCache = contextRef.UmbracoContext.Content;
                if (contentCache == null)
                {
                    throw new Exception("Content cache is empty");
                }

                // fetch all published content items (Task<IPublishedContent?>[])
                var contentTasks = results.Select(r => contentCache.GetByIdAsync(int.Parse(r.Id))).ToArray();
                // await them and filter to TResource
                var contents = (await Task.WhenAll(contentTasks)).OfType<TResource>().ToArray();

                // map each TResource -> Task<IResource>, then await all
                var resourceTasks = contents.Select(x => _resourceMapping(x, _authorService)).ToArray();
                var resources = await Task.WhenAll(resourceTasks);

                return resources.Where(x => author == null || x.Authors.Contains(author));
            }
        }
    }
}
