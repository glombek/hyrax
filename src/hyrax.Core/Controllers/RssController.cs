using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using hyrax.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyrax.Core.Controllers
{
    public class RssController : Controller
    {
        private readonly IHyraxResourceLocatorService _hyraxResourceLocatorService;

        public RssController(IHyraxResourceLocatorService hyraxResourceLocatorService)
        {
            _hyraxResourceLocatorService = hyraxResourceLocatorService;
        }
        public ActionResult Get(string? culture)
        {
            var resources = _hyraxResourceLocatorService.GetResources(culture);
            var feed = new SyndicationFeed
            {
                //TODO: add publication info
                Items = resources.Select(res =>
                {
                    var item = new SyndicationItem
                    {
                        PublishDate = res.PublishDate,
                        Title = new TextSyndicationContent(res.Name, TextSyndicationContentKind.Plaintext),
                        Content = res.Abstract == null ? res.Content == null ? null : new TextSyndicationContent(res.Content?.Value, TextSyndicationContentKind.Html) : new TextSyndicationContent(res.Abstract, TextSyndicationContentKind.Plaintext),
                    };
                    foreach (var author in res.Authors)
                    {
                        item.Authors.Add(new SyndicationPerson() { Name = author.Name });
                    }
                    item.Links.Add(new SyndicationLink(res.Url));
                    foreach (var tag in res.Tags)
                    {
                        item.Categories.Add(new SyndicationCategory(tag));
                    }
                    return item;
                })
            };

            using var stream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true
            });

            var rssFormatter = new Rss20FeedFormatter(feed, false);
            rssFormatter.WriteTo(xmlWriter);
            xmlWriter.Flush();

            return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
        }
    }
}
