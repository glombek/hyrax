using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub;
using hyrax.Core.ActivityPub.Models;
using hyrax.Core.ActivityPub.Services;
using hyrax.Core.ActivityPub.Services.Implement;
using Xunit;

namespace hyrax.Tests
{
    public class ActivityRepositoryTests
    {
        [Fact]
        public async Task InMemoryRepository_StoreAndRetrieve_RoundTrips()
        {
            // Arrange
            var repository = new InMemoryActivityRepository();
            var activity = new Activity
            {
                Id = "activity1",
                Type = "Create",
                Actor = "https://example.com/users/alice",
                Target = "https://example.com/posts/1",
                RawJson = """{"type":"Create"}""",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Received",
                RetryCount = 0
            };

            // Act
            await repository.StoreIncoming(activity);
            var retrieved = await repository.GetActivity("activity1");

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("activity1", retrieved.Id);
            Assert.Equal("Create", retrieved.Type);
        }

        [Fact]
        public async Task InMemoryRepository_GetFollowersPage_ReturnsPagedResults()
        {
            // Arrange
            var repository = new InMemoryActivityRepository();
            var follower1 = new Follower
            {
                Id = "follower1",
                ActorUri = "https://example.com/users/alice",
                LocalAuthorId = "author1",
                CreatedAt = DateTimeOffset.UtcNow,
                AcceptedAt = DateTimeOffset.UtcNow,
                Pending = false
            };
            var follower2 = new Follower
            {
                Id = "follower2",
                ActorUri = "https://example.com/users/bob",
                LocalAuthorId = "author1",
                CreatedAt = DateTimeOffset.UtcNow,
                AcceptedAt = DateTimeOffset.UtcNow,
                Pending = false
            };

            // Act
            await repository.AddFollower(follower1);
            await repository.AddFollower(follower2);
            var (followers, totalCount) = await repository.GetFollowersPage("author1", 0, 10);

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, followers.Count);
        }

        [Fact]
        public async Task InMemoryRepository_UpdateActivityStatus_UpdatesCorrectly()
        {
            // Arrange
            var repository = new InMemoryActivityRepository();
            var activity = new Activity
            {
                Id = "activity1",
                Type = "Create",
                Actor = "https://example.com/users/alice",
                RawJson = """{"type":"Create"}""",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Received",
                RetryCount = 0
            };

            await repository.StoreIncoming(activity);

            // Act
            await repository.UpdateActivityStatus("activity1", "Processed", 0);
            var updated = await repository.GetActivity("activity1");

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("Processed", updated.Status);
            Assert.NotNull(updated.ProcessedAt);
        }

        [Fact]
        public async Task InMemoryRepository_RemoveFollower_DeletesCorrectly()
        {
            // Arrange
            var repository = new InMemoryActivityRepository();
            var follower = new Follower
            {
                Id = "follower1",
                ActorUri = "https://example.com/users/alice",
                LocalAuthorId = "author1",
                CreatedAt = DateTimeOffset.UtcNow,
                Pending = false
            };

            await repository.AddFollower(follower);

            // Act
            await repository.RemoveFollower("follower1");
            var retrieved = await repository.GetFollower("https://example.com/users/alice", "author1");

            // Assert
            Assert.Null(retrieved);
        }
    }
}
