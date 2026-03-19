using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.ActivityPub.Models
{
    public class CreateActivity : ActivityPubBase
    {
        public override string Type => "Create";
        // Keep this as a plain object so System.Text.Json will include the concrete
        // derived properties (polymorphic serialization is limited otherwise).
        public object Object { get; set; }
        public string Id { get; set; }
        public string Actor { get; set; }
        public DateTimeOffset Published { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
    }
}
