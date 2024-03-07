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

namespace Hyrax.Umbraco.Services
{
    public class HyraxUmbracoContentAuthorService<TAuthor> : IHyraxAuthorService where TAuthor : class, IPublishedContent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly Func<TAuthor, IAuthor> _authorMapping;

        public HyraxUmbracoContentAuthorService(
            IUmbracoContextFactory umbracoContextFactory,
            IHyraxAuthorService authorService,
            Func<TAuthor, IAuthor> authorMapping)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _authorMapping = authorMapping;
        }

        public IEnumerable<IAuthor> Get()
        {
            //TODO: more efficient with examine?

            using (var contextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                if (contextRef.UmbracoContext.Content == null)
                {
                    throw new Exception("Content cache is empty");
                }

                //TODO: Culture?
                var publishedContent = contextRef.UmbracoContext.Content.GetAtRoot().DescendantsOrSelf<TAuthor>();
                return publishedContent.Select(x => _authorMapping(x));
            }
        }

        public IAuthor? Get(string username)
        {
            //TODO: more efficient with examine?

            return Get().FirstOrDefault(a => a.Username == username);
        }
    }
}
