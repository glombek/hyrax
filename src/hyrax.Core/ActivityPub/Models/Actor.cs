using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.ActivityPub.Models
{
    internal class Actor:ActivityPubObject
    {
        public override string Type => "Person";

        public string PreferredUsername { get; set; }
        public string Id { get; set; }
        public string Inbox { get; set; }
        public string Outbox { get; set; }
    }
}
