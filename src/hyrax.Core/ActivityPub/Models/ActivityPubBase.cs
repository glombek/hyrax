using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hyrax.Core.ActivityPub.Models
{
    public abstract class ActivityPubBase
    {
        [JsonPropertyName("@context")]
        public string[] _Context { get; } = new string[] { "https://www.w3.org/ns/activitystreams",
            "https://w3id.org/security/v1" };

        public abstract string Type { get; }
    }
}
