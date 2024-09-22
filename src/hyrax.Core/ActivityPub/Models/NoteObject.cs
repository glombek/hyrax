using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace hyrax.Core.ActivityPub.Models
{
    public class NoteObject : ActivityPubBase
    {
        public override string Type => "Note";

        public Uri Id { get; set; }
        public bool Sensitive { get; set; }
        public object InReplyTo { get; set; }
        public DateTimeOffset Published { get; set; }
        public Uri Url { get; set; }
        public string AttributedTo { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public HtmlString Content { get; set; }
        public IEnumerable<Hashtag> Tag { get; set; }
    }
}
