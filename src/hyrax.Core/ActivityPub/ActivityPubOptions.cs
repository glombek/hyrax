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
        /// Number of items per followers page. Default: 20
        /// </summary>
        public int FollowersPageSize { get; set; } = 20;

        /// <summary>
        /// Filesystem path used by signature repository implementations.
        /// </summary>
        public string SignatureRepositoryPath { get; set; } = "./hyrax/signatures";

        /// <summary>
        /// Whether to automatically accept follow requests. Default: true
        /// </summary>
        public bool AutoAcceptFollows { get; set; } = true;

        /// <summary>
        /// Whether to verify HTTP signatures on incoming requests. Default: true
        /// </summary>
        public bool VerifySignatures { get; set; } = true;

        /// <summary>
        /// Whether to disable follower approval (don't expose followers list). Default: false
        /// </summary>
        public bool DisableFollowerApproval { get; set; } = false;

        /// <summary>
        /// Retry policy for outbound deliveries.
        /// </summary>
        public RetryPolicy? RetryPolicy { get; set; }
    }

    /// <summary>
    /// Retry policy configuration for outbound activity delivery.
    /// </summary>
    public class RetryPolicy
    {
        /// <summary>
        /// Maximum number of retry attempts. Default: 3
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Initial delay in milliseconds before first retry. Default: 1000
        /// </summary>
        public int InitialDelayMs { get; set; } = 1000;

        /// <summary>
        /// Backoff multiplier for exponential backoff. Default: 2.0
        /// </summary>
        public double BackoffMultiplier { get; set; } = 2.0;
    }
}
