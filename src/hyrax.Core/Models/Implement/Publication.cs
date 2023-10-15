using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.Models.Implement
{
    public class Publication : IPublication
    {
        public Publication(string name, string language, string? description = null, Uri? url = null)
        {
            Name = name;
            Language = language;
            Description = description;
            Url = url;
        }
        public string Name { get; set; }

        public string? Description { get; set; }

        public Uri? Url { get; set; }

        public string Language { get; set; }
    }
}
