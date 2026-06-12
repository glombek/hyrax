using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub.Models;
using hyrax.Core.Models;

namespace hyrax.Core.ActivityPub.Services
{
    /// <summary>
    /// Handles incoming and outgoing ActivityPub activities.
    /// Processes activity types (Create, Follow, Accept, Undo, Like, Announce, Update, Delete).
    /// Manages activity persistence and follower relationships.
    /// </summary>
    public interface IHyraxActivityService
    {
        /// <summary>
        /// Handles an incoming ActivityPub activity.
        /// Parses the activity type, verifies the signature, persists it, and triggers appropriate handlers.
        /// </summary>
        /// <param name="recipient">The local author receiving this activity.</param>
        /// <param name="activityJson">The parsed JSON of the activity.</param>
        /// <param name="rawBody">The raw body content for signature verification.</param>
        Task HandleIncoming(IAuthor recipient, JsonElement activityJson, string rawBody);

        /// <summary>
        /// Creates an outgoing activity representation for a resource.
        /// Primarily for Create activities that reference existing resources.
        /// </summary>
        /// <param name="resource">The resource to create an activity for.</param>
        /// <returns>The activity JSON as a string.</returns>
        Task<string> CreateOutgoingForResource(IResource resource);

        /// <summary>
        /// Retrieves a stored activity by ID.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>The activity JSON, or null if not found.</returns>
        Task<string?> GetActivity(string id);

        /// <summary>
        /// Gets a page of activities from an author's outbox.
        /// </summary>
        /// <param name="authorId">The author ID.</param>
        /// <param name="page">Page number (zero-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A tuple of (activities, totalCount).</returns>
        Task<(IReadOnlyList<Activity> activities, int totalCount)> GetOutboxPage(string authorId, int page, int pageSize);

        /// <summary>
        /// Gets the total count of activities in an author's outbox.
        /// </summary>
        /// <param name="authorId">The author ID.</param>
        /// <returns>The total number of activities.</returns>
        Task<int> GetOutboxCount(string authorId);

        /// <summary>
        /// Gets a page of followers for an author.
        /// </summary>
        /// <param name="authorId">The author ID.</param>
        /// <param name="page">Page number (zero-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A tuple of (followers, totalCount).</returns>
        Task<(IReadOnlyList<Follower> followers, int totalCount)> GetFollowersPage(string authorId, int page, int pageSize);

        /// <summary>
        /// Gets the total count of followers for an author.
        /// </summary>
        /// <param name="authorId">The author ID.</param>
        /// <returns>The total number of followers.</returns>
        Task<int> GetFollowerCount(string authorId);

        /// <summary>
        /// Gets all followers for an author (for delivery operations).
        /// </summary>
        /// <param name="authorId">The author ID.</param>
        /// <returns>A collection of all followers.</returns>
        Task<IReadOnlyList<Follower>> GetAllFollowers(string authorId);
    }
}
