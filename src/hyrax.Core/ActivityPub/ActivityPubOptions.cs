using System;

namespace hyrax.Core.ActivityPub
{
    public class ActivityPubOptions
    {
        /// <summary>
        /// Number of items per outbox page. Default: 20
        /// </summary>
        public int OutboxPageSize { get; set; } = 20;

        /// <summary>
        /// Filesystem path used by signature repository implementations.
        /// </summary>
        public string SignatureRepositoryPath { get; set; } = "./hyrax/signatures";

        // Add additional toggles or configuration options for ActivityPub here.
    }
}
