using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.ActivityPub.Models
{
    public class OrderedCollectionPage : ActivityPubBase
    {
        public override string Type => "OrderedCollectionPage";

        public string Id { get; set; }
        public IEnumerable<CreateActivity> OrderedItems { get; set; }
    }
}
