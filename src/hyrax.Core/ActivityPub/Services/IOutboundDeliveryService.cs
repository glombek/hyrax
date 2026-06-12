using System;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub.Models;

namespace hyrax.Core.ActivityPub.Services
{
    /// <summary>
    /// Handles outbound delivery of ActivityPub activities to remote followers.
    /// Supports configurable retry policies and exponential backoff.
    /// </summary>
    public interface IOutboundDeliveryService
    {
        /// <summary>
        /// Delivers an activity to a specific inbox URL.
        /// Performs synchronous delivery with configured retry policy.
        /// </summary>
        /// <param name="actorId">The local actor performing the activity.</param>
        /// <param name="inboxUrl">The remote inbox URL to deliver to.</param>
        /// <param name="activity">The activity JSON to deliver.</param>
        /// <returns>True if delivery succeeded within retries, false otherwise.</returns>
        Task<bool> Deliver(string actorId, string inboxUrl, string activity);

        /// <summary>
        /// Delivers an activity to all follower inboxes for an author.
        /// </summary>
        /// <param name="actorId">The local actor performing the activity.</param>
        /// <param name="followers">The followers to deliver to.</param>
        /// <param name="activity">The activity JSON to deliver.</param>
        /// <returns>The number of successful deliveries.</returns>
        Task<int> DeliverToFollowers(string actorId, System.Collections.Generic.IReadOnlyList<Follower> followers, string activity);
    }
}
