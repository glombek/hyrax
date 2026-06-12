using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyrax.Core.ActivityPub.Models
{
    /// <summary>
    /// Represents a stored ActivityPub activity for tracking and persistence.
    /// Stores both the structured activity type and raw JSON for archival.
    /// </summary>
    [Table("Activities", Schema = "hyrax")]
    public class Activity
    {
        /// <summary>
        /// Unique identifier for this activity record.
        /// </summary>
        [Key]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ActivityPub activity type (e.g., "Create", "Follow", "Like", "Announce", "Update", "Delete", "Accept", "Undo").
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The actor URI who performed this activity.
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string Actor { get; set; } = string.Empty;

        /// <summary>
        /// The target of this activity (e.g., the object being liked, followed, etc.).
        /// </summary>
        [StringLength(2000)]
        public string? Target { get; set; }

        /// <summary>
        /// The complete raw JSON representation of the activity as received.
        /// </summary>
        [Required]
        public string RawJson { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when this activity was received and stored.
        /// </summary>
        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when this activity was last processed / handled.
        /// Null if not yet processed.
        /// </summary>
        public DateTimeOffset? ProcessedAt { get; set; }

        /// <summary>
        /// Status of activity processing: Received, Processing, Processed, Failed.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Received";

        /// <summary>
        /// Number of retry attempts for processing this activity.
        /// </summary>
        [Required]
        public int RetryCount { get; set; }

        /// <summary>
        /// Last error message if processing failed.
        /// </summary>
        public string? LastError { get; set; }
    }
}
