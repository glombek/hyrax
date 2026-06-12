using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub.Models;

namespace hyrax.Core.ActivityPub.Services.Implement
{
    /// <summary>
    /// In-memory implementation of IActivityRepository for development and testing.
    /// Not suitable for production use.
    /// </summary>
    public class InMemoryActivityRepository : IActivityRepository
    {
        private readonly Dictionary<string, Activity> _activities = new();
        private readonly Dictionary<string, Follower> _followers = new();
        private readonly object _activityLock = new();
        private readonly object _followerLock = new();

        public Task StoreIncoming(Activity activity)
        {
            lock (_activityLock)
            {
                _activities[activity.Id] = activity;
            }
            return Task.CompletedTask;
        }

        public Task<Activity?> GetActivity(string activityId)
        {
            lock (_activityLock)
            {
                _activities.TryGetValue(activityId, out var activity);
                return Task.FromResult(activity);
            }
        }

        public Task<(IReadOnlyList<Activity> activities, int totalCount)> GetOutboxPage(string authorId, int page, int pageSize)
        {
            lock (_activityLock)
            {
                var allActivities = _activities.Values
                    .Where(a => a.Status == "Processed" && a.Actor.Contains(authorId))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();

                var totalCount = allActivities.Count;
                var pagedActivities = allActivities
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Task.FromResult<(IReadOnlyList<Activity>, int)>((pagedActivities.AsReadOnly(), totalCount));
            }
        }

        public Task<int> GetOutboxCount(string authorId)
        {
            lock (_activityLock)
            {
                var count = _activities.Values
                    .Count(a => a.Status == "Processed" && a.Actor.Contains(authorId));
                return Task.FromResult(count);
            }
        }

        public Task UpdateActivityStatus(string activityId, string status, int retryCount, string? lastError = null)
        {
            lock (_activityLock)
            {
                if (_activities.TryGetValue(activityId, out var activity))
                {
                    activity.Status = status;
                    activity.RetryCount = retryCount;
                    activity.LastError = lastError;
                    if (status == "Processed")
                    {
                        activity.ProcessedAt = DateTimeOffset.UtcNow;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task AddFollower(Follower follower)
        {
            lock (_followerLock)
            {
                _followers[follower.Id] = follower;
            }
            return Task.CompletedTask;
        }

        public Task RemoveFollower(string followerId)
        {
            lock (_followerLock)
            {
                _followers.Remove(followerId);
            }
            return Task.CompletedTask;
        }

        public Task<Follower?> GetFollower(string actorUri, string localAuthorId)
        {
            lock (_followerLock)
            {
                var follower = _followers.Values
                    .FirstOrDefault(f => f.ActorUri == actorUri && f.LocalAuthorId == localAuthorId);
                return Task.FromResult(follower);
            }
        }

        public Task<(IReadOnlyList<Follower> followers, int totalCount)> GetFollowersPage(string authorId, int page, int pageSize)
        {
            lock (_followerLock)
            {
                var allFollowers = _followers.Values
                    .Where(f => f.LocalAuthorId == authorId && !f.Pending)
                    .OrderByDescending(f => f.AcceptedAt)
                    .ToList();

                var totalCount = allFollowers.Count;
                var pagedFollowers = allFollowers
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Task.FromResult<(IReadOnlyList<Follower>, int)>((pagedFollowers.AsReadOnly(), totalCount));
            }
        }

        public Task<int> GetFollowerCount(string authorId)
        {
            lock (_followerLock)
            {
                var count = _followers.Values
                    .Count(f => f.LocalAuthorId == authorId && !f.Pending);
                return Task.FromResult(count);
            }
        }

        public Task<IReadOnlyList<Follower>> GetAllFollowers(string authorId)
        {
            lock (_followerLock)
            {
                var followers = _followers.Values
                    .Where(f => f.LocalAuthorId == authorId && !f.Pending)
                    .ToList()
                    .AsReadOnly();
                return Task.FromResult<IReadOnlyList<Follower>>(followers);
            }
        }
    }
}
