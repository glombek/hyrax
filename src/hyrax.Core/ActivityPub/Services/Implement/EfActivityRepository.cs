using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using hyrax.Core.ActivityPub.Models;

namespace hyrax.Core.ActivityPub.Services.Implement
{
    /// <summary>
    /// Entity Framework Core implementation of IActivityRepository.
    /// Provides persistent storage for activities and followers.
    /// </summary>
    public class EfActivityRepository : IActivityRepository
    {
        private readonly HyraxActivityDbContext _context;

        public EfActivityRepository(HyraxActivityDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task StoreIncoming(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<Activity?> GetActivity(string activityId)
        {
            return await _context.Activities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == activityId);
        }

        public async Task<(IReadOnlyList<Activity> activities, int totalCount)> GetOutboxPage(string authorId, int page, int pageSize)
        {
            var query = _context.Activities
                .Where(a => a.Status == "Processed" && a.Actor.Contains(authorId))
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            var activities = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (activities.AsReadOnly(), totalCount);
        }

        public async Task<int> GetOutboxCount(string authorId)
        {
            return await _context.Activities
                .Where(a => a.Status == "Processed" && a.Actor.Contains(authorId))
                .CountAsync();
        }

        public async Task UpdateActivityStatus(string activityId, string status, int retryCount, string? lastError = null)
        {
            var activity = await _context.Activities.FindAsync(activityId);
            if (activity != null)
            {
                activity.Status = status;
                activity.RetryCount = retryCount;
                activity.LastError = lastError;
                if (status == "Processed")
                {
                    activity.ProcessedAt = DateTimeOffset.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddFollower(Follower follower)
        {
            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFollower(string followerId)
        {
            var follower = await _context.Followers.FindAsync(followerId);
            if (follower != null)
            {
                _context.Followers.Remove(follower);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Follower?> GetFollower(string actorUri, string localAuthorId)
        {
            return await _context.Followers
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.ActorUri == actorUri && f.LocalAuthorId == localAuthorId);
        }

        public async Task<(IReadOnlyList<Follower> followers, int totalCount)> GetFollowersPage(string authorId, int page, int pageSize)
        {
            var query = _context.Followers
                .Where(f => f.LocalAuthorId == authorId && !f.Pending)
                .OrderByDescending(f => f.AcceptedAt);

            var totalCount = await query.CountAsync();
            var followers = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (followers.AsReadOnly(), totalCount);
        }

        public async Task<int> GetFollowerCount(string authorId)
        {
            return await _context.Followers
                .Where(f => f.LocalAuthorId == authorId && !f.Pending)
                .CountAsync();
        }

        public async Task<IReadOnlyList<Follower>> GetAllFollowers(string authorId)
        {
            var followers = await _context.Followers
                .Where(f => f.LocalAuthorId == authorId && !f.Pending)
                .AsNoTracking()
                .ToListAsync();
            return followers.AsReadOnly();
        }
    }
}
