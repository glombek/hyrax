using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyrax.Core.ActivityPub.Models
{
    /// <summary>
    /// Represents a follower relationship in the ActivityPub system.
    /// Tracks remote actors who have followed this server's local author.
    /// </summary>
    [Table("Followers", Schema = "hyrax")]
    public class Follower
    {
        /// <summary>
        /// Unique identifier for this follower record.
        /// </summary>
        [Key]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The remote actor's URI who is following.
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string ActorUri { get; set; } = string.Empty;

        /// <summary>
        /// The local author being followed.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string LocalAuthorId { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the follow was accepted.
        /// Null if still pending approval.
        /// </summary>
        public DateTimeOffset? AcceptedAt { get; set; }

        /// <summary>
        /// Flag indicating if this follow request is pending approval.
        /// Only relevant if follower approval is enabled.
        /// </summary>
        [Required]
        public bool Pending { get; set; }

        /// <summary>
        /// Timestamp when this follower record was created.
        /// </summary>
        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The inbox URL where activities should be delivered for this follower.
        /// Cached for performance to avoid repeated lookups.
        /// </summary>
        [StringLength(2000)]
        public string? InboxUrl { get; set; }
    }
}
