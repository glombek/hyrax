using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace hyrax.Core.Models.Implement
{
    public class Resource : IResource
    {
        public Resource(
            Uri url,
            string id,
            string name,
            IEnumerable<IAuthor> authors,
            DateTimeOffset publishDate,
            IEnumerable<string> tags,
            string? contentAbstract = null,
            HtmlString? content = null
            )
        {
            Url = url;
            Id = id;
            Name = name;
            Authors = authors;
            PublishDate = publishDate;
            Tags = tags;
            Abstract = contentAbstract;
            Content = content;
        }

        public Uri Url { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string? Abstract { get; set; }

        public HtmlString? Content { get; set; }

        public IEnumerable<IAuthor> Authors { get; set; }

        public DateTimeOffset PublishDate { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
