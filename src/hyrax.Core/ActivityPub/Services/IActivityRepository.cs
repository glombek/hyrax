using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub.Models;

namespace hyrax.Core.ActivityPub.Services
{
    /// <summary>
    /// Interface for persisting and retrieving ActivityPub activities and followers.
    /// Provides a pluggable abstraction over the storage backend.
    /// </summary>
    public interface IActivityRepository
    {
        // Activity Operations

        /// <summary>
        /// Stores an incoming activity in the repository.
        /// </summary>
        /// <param name="activity">The activity to store.</param>
        Task StoreIncoming(Activity activity);

        /// <summary>
        /// Retrieves an activity by its ID.
        /// </summary>
        /// <param name="activityId">The ID of the activity to retrieve.</param>
        /// <returns>The activity, or null if not found.</returns>
        Task<Activity?> GetActivity(string activityId);

        /// <summary>
        /// Gets a page of outbox activities for a given author, ordered newest first.
        /// </summary>
        /// <param name="authorId">The local author ID.</param>
        /// <param name="page">Page number (zero-based).</param>
        /// <param name="pageSize">Number of activities per page.</param>
        /// <returns>A tuple of (activities, totalCount).</returns>
        Task<(IReadOnlyList<Activity> activities, int totalCount)> GetOutboxPage(string authorId, int page, int pageSize);

        /// <summary>
        /// Gets the total count of outbox activities for a given author.
        /// </summary>
        /// <param name="authorId">The local author ID.</param>
        /// <returns>The total number of activities in the outbox.</returns>
        Task<int> GetOutboxCount(string authorId);

        /// <summary>
        /// Updates the status and processed timestamp of an activity.
        /// </summary>
        /// <param name="activityId">The activity ID.</param>
        /// <param name="status">The new status.</param>
        /// <param name="retryCount">The new retry count.</param>
        /// <param name="lastError">Optional error message if processing failed.</param>
        Task UpdateActivityStatus(string activityId, string status, int retryCount, string? lastError = null);

        // Follower Operations

        /// <summary>
        /// Adds or updates a follower for a given author.
        /// </summary>
        /// <param name="follower">The follower to add.</param>
        Task AddFollower(Follower follower);

        /// <summary>
        /// Removes a follower.
        /// </summary>
        /// <param name="followerId">The ID of the follower to remove.</param>
        Task RemoveFollower(string followerId);

        /// <summary>
        /// Retrieves a follower by their actor URI and local author ID.
        /// </summary>
        /// <param name="actorUri">The remote actor's URI.</param>
        /// <param name="localAuthorId">The local author ID.</param>
        /// <returns>The follower, or null if not found.</returns>
        Task<Follower?> GetFollower(string actorUri, string localAuthorId);

        /// <summary>
        /// Gets a page of followers for a given author.
        /// </summary>
        /// <param name="authorId">The local author ID.</param>
        /// <param name="page">Page number (zero-based).</param>
        /// <param name="pageSize">Number of followers per page.</param>
        /// <returns>A tuple of (followers, totalCount).</returns>
        Task<(IReadOnlyList<Follower> followers, int totalCount)> GetFollowersPage(string authorId, int page, int pageSize);

        /// <summary>
        /// Gets the total count of followers for a given author.
        /// </summary>
        /// <param name="authorId">The local author ID.</param>
        /// <returns>The total number of followers.</returns>
        Task<int> GetFollowerCount(string authorId);

        /// <summary>
        /// Gets all followers for a given author (for delivery purposes).
        /// </summary>
        /// <param name="authorId">The local author ID.</param>
        /// <returns>A list of all followers.</returns>
        Task<IReadOnlyList<Follower>> GetAllFollowers(string authorId);
    }
}
