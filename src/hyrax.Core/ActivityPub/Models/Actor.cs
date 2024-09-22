using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Controllers;

namespace hyrax.Core.ActivityPub.Models
{
    internal class Actor : ActivityPubBase
    {
        public override string Type => "Person";

        public string PreferredUsername { get; set; }
        public string Id { get; set; }
        public string Inbox { get; set; }
        public string Outbox { get; set; }
        public PublicKey PublicKey { get; set; }
    }
}
