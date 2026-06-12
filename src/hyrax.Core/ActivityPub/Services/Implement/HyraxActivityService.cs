using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using hyrax.Core.Models;
using hyrax.Core.ActivityPub.Models;
using Microsoft.Extensions.Options;

namespace hyrax.Core.ActivityPub.Services.Implement
{
    /// <summary>
    /// Implements core ActivityPub activity handling.
    /// Processes all activity types and manages followers.
    /// </summary>
    public class HyraxActivityService : IHyraxActivityService
    {
        private readonly IActivityRepository _repository;
        private readonly IHttpSignatureService _signatureService;
        private readonly ActivityPubOptions _options;

        public HyraxActivityService(
            IActivityRepository repository,
            IHttpSignatureService signatureService,
            IOptions<ActivityPubOptions> options)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _signatureService = signatureService ?? throw new ArgumentNullException(nameof(signatureService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task HandleIncoming(IAuthor recipient, JsonElement activityJson, string rawBody)
        {
            // Parse the activity
            var activityType = GetJsonProperty(activityJson, "type");
            var actor = GetJsonProperty(activityJson, "actor");

            if (string.IsNullOrEmpty(activityType) || string.IsNullOrEmpty(actor))
            {
                throw new InvalidOperationException("Activity missing required 'type' or 'actor' property");
            }

            // Verify HTTP signature if configured
            if (_options.VerifySignatures)
            {
                var isValid = await _signatureService.VerifySignature(actor, rawBody);
                if (!isValid)
                {
                    throw new InvalidOperationException($"Invalid HTTP signature from {actor}");
                }
            }

            // Create and store the activity record
            var activityId = Guid.NewGuid().ToString();
            var activity = new Activity
            {
                Id = activityId,
                Type = activityType,
                Actor = actor,
                Target = GetJsonProperty(activityJson, "object"),
                RawJson = JsonSerializer.Serialize(activityJson),
                CreatedAt = DateTimeOffset.UtcNow,
                Status = "Processing",
                RetryCount = 0
            };

            await _repository.StoreIncoming(activity);

            try
            {
                // Dispatch to appropriate handler based on activity type
                switch (activityType)
                {
                    case "Create":
                        await HandleCreate(activity, activityJson, recipient);
                        break;
                    case "Follow":
                        await HandleFollow(activity, activityJson, recipient);
                        break;
                    case "Like":
                        await HandleLike(activity, activityJson, recipient);
                        break;
                    case "Announce":
                        await HandleAnnounce(activity, activityJson, recipient);
                        break;
                    case "Update":
                        await HandleUpdate(activity, activityJson, recipient);
                        break;
                    case "Delete":
                        await HandleDelete(activity, activityJson, recipient);
                        break;
                    case "Accept":
                        await HandleAccept(activity, activityJson, recipient);
                        break;
                    case "Undo":
                        await HandleUndo(activity, activityJson, recipient);
                        break;
                    default:
                        // Unknown activity type - log and mark as processed
                        break;
                }

                await _repository.UpdateActivityStatus(activityId, "Processed", 0);
            }
            catch (Exception ex)
            {
                var newRetryCount = activity.RetryCount + 1;
                var willRetry = newRetryCount < 3; // Default 3 retries
                var newStatus = willRetry ? "Received" : "Failed";

                await _repository.UpdateActivityStatus(activityId, newStatus, newRetryCount, ex.Message);

                if (!willRetry)
                {
                    throw;
                }
            }
        }

        private async Task HandleCreate(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            // Create activities reference existing resources
            // We don't create new content automatically
            // Just acknowledge receipt
            await Task.CompletedTask;
        }

        private async Task HandleFollow(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            var actor = GetJsonProperty(activityJson, "actor");

            // Check if we should auto-accept follows
            if (_options.AutoAcceptFollows)
            {
                // Create or update follower record
                var follower = await _repository.GetFollower(actor, recipient.Username);
                if (follower == null)
                {
                    follower = new Follower
                    {
                        Id = Guid.NewGuid().ToString(),
                        ActorUri = actor,
                        LocalAuthorId = recipient.Username,
                        CreatedAt = DateTimeOffset.UtcNow,
                        Pending = false,
                        AcceptedAt = DateTimeOffset.UtcNow
                    };
                    await _repository.AddFollower(follower);
                }
                else if (follower.Pending)
                {
                    follower.Pending = false;
                    follower.AcceptedAt = DateTimeOffset.UtcNow;
                    // Update in repository
                    await _repository.RemoveFollower(follower.Id);
                    await _repository.AddFollower(follower);
                }

                // Send Accept activity
                await SendAcceptFollow(activity, actor, recipient);
            }
            else
            {
                // Create pending follower
                var follower = await _repository.GetFollower(actor, recipient.Username);
                if (follower == null)
                {
                    follower = new Follower
                    {
                        Id = Guid.NewGuid().ToString(),
                        ActorUri = actor,
                        LocalAuthorId = recipient.Username,
                        CreatedAt = DateTimeOffset.UtcNow,
                        Pending = true
                    };
                    await _repository.AddFollower(follower);
                }
            }
        }

        private async Task HandleLike(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            // Like activities just reference a liked object
            // For now, we just store them
            await Task.CompletedTask;
        }

        private async Task HandleAnnounce(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            // Announce activities are shares/boosts
            // Store them for tracking but don't create duplicates
            await Task.CompletedTask;
        }

        private async Task HandleUpdate(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            // Update activities modify existing objects
            // We don't auto-sync updates
            await Task.CompletedTask;
        }

        private async Task HandleDelete(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            // Delete activities remove an object
            // We don't auto-delete content
            await Task.CompletedTask;
        }

        private async Task HandleAccept(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            // Accept activities confirm that a previous activity was accepted
            // Typically used to confirm follow requests
            await Task.CompletedTask;
        }

        private async Task HandleUndo(Activity activity, JsonElement activityJson, IAuthor recipient)
        {
            var @object = GetJsonProperty(activityJson, "object");
            if (@object != null && @object.StartsWith("http"))
            {
                // This might be undoing a follow
                // Check if there's a follower with this actor
                var actor = GetJsonProperty(activityJson, "actor");
                var follower = await _repository.GetFollower(actor, recipient.Username);
                if (follower != null)
                {
                    await _repository.RemoveFollower(follower.Id);
                }
            }
        }

        private async Task SendAcceptFollow(Activity activity, string followerActor, IAuthor recipient)
        {
            // This would be handled by the outbound delivery service
            // For now, just placeholder
            await Task.CompletedTask;
        }

        public async Task<string> CreateOutgoingForResource(IResource resource)
        {
            var activity = new
            {
                context = new[] { "https://www.w3.org/ns/activitystreams", "https://w3id.org/security/v1" },
                type = "Create",
                id = $"{resource.Url}#create",
                actor = resource.Authors?.FirstOrDefault()?.Username,
                published = resource.PublishDate.ToString("o"),
                @object = new
                {
                    type = "Note",
                    id = resource.Url,
                    name = resource.Name,
                    content = resource.Content?.Value,
                    inReplyTo = (string?)null,
                    attributedTo = resource.Authors?.Select(a => a.Username),
                    published = resource.PublishDate.ToString("o")
                }
            };

            return JsonSerializer.Serialize(activity);
        }

        public async Task<string?> GetActivity(string id)
        {
            var activity = await _repository.GetActivity(id);
            if (activity == null)
            {
                return null;
            }

            return activity.RawJson;
        }

        public async Task<(IReadOnlyList<Activity> activities, int totalCount)> GetOutboxPage(string authorId, int page, int pageSize)
        {
            return await _repository.GetOutboxPage(authorId, page, pageSize);
        }

        public async Task<int> GetOutboxCount(string authorId)
        {
            return await _repository.GetOutboxCount(authorId);
        }

        public async Task<(IReadOnlyList<Follower> followers, int totalCount)> GetFollowersPage(string authorId, int page, int pageSize)
        {
            return await _repository.GetFollowersPage(authorId, page, pageSize);
        }

        public async Task<int> GetFollowerCount(string authorId)
        {
            return await _repository.GetFollowerCount(authorId);
        }

        public async Task<IReadOnlyList<Follower>> GetAllFollowers(string authorId)
        {
            return await _repository.GetAllFollowers(authorId);
        }

        private static string? GetJsonProperty(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var property))
            {
                return property.GetString();
            }
            return null;
        }
    }
}
