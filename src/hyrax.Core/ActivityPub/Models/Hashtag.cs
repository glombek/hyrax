using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.ActivityPub.Models
{
    public class Hashtag : ActivityPubBase
    {
        public override string Type => "Hashtag";
        public string Name { get; set; }
    }
}
