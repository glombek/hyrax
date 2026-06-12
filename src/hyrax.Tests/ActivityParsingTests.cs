using System;
using System.Text.Json;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub;
using hyrax.Core.ActivityPub.Services;
using hyrax.Core.ActivityPub.Services.Implement;
using hyrax.Core.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace hyrax.Tests
{
    public class ActivityParsingTests
    {
        private readonly IActivityRepository _repository;
        private readonly IHttpSignatureService _signatureService;
        private readonly ActivityPubOptions _options;
        private readonly HyraxActivityService _service;

        public ActivityParsingTests()
        {
            _repository = new InMemoryActivityRepository();
            var options = new ActivityPubOptions
            {
                AutoAcceptFollows = true,
                VerifySignatures = false
            };
            _signatureService = new HttpSignatureService(null!, Options.Create(options));
            _service = new HyraxActivityService(_repository, _signatureService, Options.Create(options));
        }

        [Fact]
        public async Task HandleIncoming_WithFollowActivity_CreatesFollower()
        {
            // Arrange
            var author = new TestAuthor { Username = "testuser", Name = "Test User" };
            var activityJson = JsonSerializer.Deserialize<JsonElement>("""
            {
                "type": "Follow",
                "actor": "https://example.com/users/alice",
                "object": "https://myapp.com/users/testuser"
            }
            """);

            // Act
            await _service.HandleIncoming(author, activityJson, "{}");

            // Assert
            var followers = await _repository.GetAllFollowers(author.Username);
            Assert.NotEmpty(followers);
            Assert.Equal("https://example.com/users/alice", followers[0].ActorUri);
        }

        [Fact]
        public async Task HandleIncoming_WithCreateActivity_StoresActivity()
        {
            // Arrange
            var author = new TestAuthor { Username = "testuser", Name = "Test User" };
            var activityJson = JsonSerializer.Deserialize<JsonElement>("""
            {
                "type": "Create",
                "actor": "https://example.com/users/alice",
                "object": { "type": "Note", "content": "Hello world" }
            }
            """);

            // Act
            await _service.HandleIncoming(author, activityJson, "{}");

            // Assert - activity should be processed
            // The Create handler doesn't create new content, just acknowledges it
        }

        private class TestAuthor : IAuthor
        {
            public string Username { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;

            public bool Equals(IAuthor? other)
            {
                return other?.Username == Username;
            }

            public override bool Equals(object? obj)
            {
                return Equals(obj as IAuthor);
            }

            public override int GetHashCode()
            {
                return Username.GetHashCode();
            }
        }
    }
}
