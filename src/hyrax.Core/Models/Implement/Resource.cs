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
            string name,
            IAuthor author,
            DateTimeOffset publishDate,
            IEnumerable<string> tags,
            string? contentAbstract = null,
            HtmlString? content = null
            )
        {
            Url = url;
            Name = name;
            Author = author;
            PublishDate = publishDate;
            Tags = tags;
            Abstract = contentAbstract;
            Content = content;
        }

        public Uri Url { get; set; }

        public string Name { get; set; }

        public string? Abstract { get; set; }

        public HtmlString? Content { get; set; }

        public IAuthor Author { get; set; }

        public DateTimeOffset PublishDate { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
