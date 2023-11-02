using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace hyrax.Core.Models
{
    public interface IResource
    {
        Uri Url { get; }
        string Id { get; }
        string Name { get; }
        string? Abstract { get; }
        HtmlString? Content { get; }
        IEnumerable<IAuthor> Authors { get; }
        DateTimeOffset PublishDate { get; }
        IEnumerable<string> Tags { get; }
    }
}
