using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hyrax.Core.Models;
using hyrax.Core.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Extensions;
using Examine;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Core;

namespace Hyrax.Umbraco.Services
{
    public class HyraxUmbracoContentAuthorService<TAuthor> : IHyraxAuthorService where TAuthor : class, IPublishedContent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly Func<TAuthor, IAuthor> _authorMapping;
        private readonly IExamineManager _examineManager;

        public HyraxUmbracoContentAuthorService(
            IUmbracoContextFactory umbracoContextFactory,
            Func<TAuthor, IAuthor> authorMapping,
            IExamineManager examineManager)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _authorMapping = authorMapping;
            _examineManager = examineManager;
        }

        public async Task<IEnumerable<IAuthor>> Get()
        {
            // Use Examine ExternalIndex for author lookup
            var index = _examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var externalIndex) ? externalIndex : null;
            if (index == null)
            {
                throw new Exception("ExternalIndex not found");
            }

            // Adjust the contentType alias as needed (e.g., "author")
            var results = index.Searcher.CreateQuery("content")
                .NodeTypeAlias("author")
                .Execute();

            using (var contextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var contentCache = contextRef.UmbracoContext.Content ?? throw new Exception("Content cache is empty");

                var authors = results.Select(async r => await contentCache.GetByIdAsync(int.Parse(r.Id))).OfType<TAuthor>().Select(x => _authorMapping(x));
                return authors;
            }
        }

        public async Task<IAuthor?> Get(string username)
        {
            return (await Get()).FirstOrDefault(a => a.Username == username);
        }
    }
}
