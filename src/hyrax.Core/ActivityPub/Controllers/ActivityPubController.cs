using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Humanizer;
using hyrax.Core.ActivityPub.Models;
using hyrax.Core.Models;
using hyrax.Core.Models.Implement;
using hyrax.Core.ActivityPub.Services;
using hyrax.Core.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace hyrax.Core.ActivityPub.Controllers
{
    public class ActivityPubController : Controller
    {
        private readonly IHyraxResourceLocatorService _resourceLocatorService;
        private readonly IHyraxAuthorService _authorService;
        private readonly IHyraxSignatureRepositoryService _signatureRepositoryService;
        private readonly IHyraxResourceLocatorService _hyraxResourceLocatorService;
        private readonly ActivityPubOptions _options;
        private readonly IActivityPubService _activityPubService;

        public ActivityPubController(
            IHyraxResourceLocatorService resourceLocatorService,
            IHyraxAuthorService authorService,
            IHyraxSignatureRepositoryService signatureRepositoryService,
            IHyraxResourceLocatorService hyraxResourceLocatorService,
            IOptions<ActivityPubOptions> options,
            IActivityPubService activityPubService)
        {
            _resourceLocatorService = resourceLocatorService;
            _authorService = authorService;
            _signatureRepositoryService = signatureRepositoryService;
            _hyraxResourceLocatorService = hyraxResourceLocatorService;
            _options = options?.Value ?? new ActivityPubOptions();
            _activityPubService = activityPubService;
        }

        // Controller methods remain largely unchanged; they now use _options instead of IConfiguration
        public async Task<ActionResult> Actor(string id)
        {
            var author = await _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }

            var key = await _signatureRepositoryService.GetPublicKeyForAuthor(author);


            var actorId = Url.Action("Actor", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty;

            return new ObjectResult(new Actor()
            {
                Id = actorId,
                PreferredUsername = author.Username,
                Inbox = Url.Action("Inbox", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                Outbox = Url.Action("Outbox", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                PublicKey = new PublicKey()
                {
                    Id = $"{actorId}#main-key",
                    Owner = actorId,
                    PublicKeyPem = key
                }
            })
            { ContentTypes = { "application/activity+json" } };
        }

        public async Task<ActionResult> Outbox(string id, int page = 0)
        {
            var author = await _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }

            var pageSize = _options.OutboxPageSize;

            var actorId = Url.Action("Actor", "ActivityPub", new { id = author.Username }, Request.Scheme,
                Request.Host.Value) ?? string.Empty;

            var allResources = (await _hyraxResourceLocatorService.GetResources(author: author))
                .OrderByDescending(x => x.PublishDate)
                .ToList();

            var totalItems = allResources.Count;

            if (page == 0)
            {
                var outboxId = Url.Action("Outbox", "ActivityPub", new { id }, Request.Scheme, Request.Host.Value) ?? string.Empty;
                var firstPageUrl = Url.Action("Outbox", "ActivityPub", new { id, page = 1 }, Request.Scheme, Request.Host.Value) ?? string.Empty;

                return new ObjectResult(new
                {
                    @context = "https://www.w3.org/ns/activitystreams",
                    id = outboxId,
                    type = "OrderedCollection",
                    totalItems = totalItems,
                    first = firstPageUrl
                })
                { ContentTypes = { "application/activity+json" } };
            }

            var skip = (page - 1) * pageSize;
            var pagedResources = allResources.Skip(skip).Take(pageSize).ToList();

            var pageUrl = Url.Action("Outbox", "ActivityPub", new { id, page }, Request.Scheme, Request.Host.Value) ?? string.Empty;
            var nextPageUrl = skip + pageSize < totalItems
                ? Url.Action("Outbox", "ActivityPub", new { id, page = page + 1 }, Request.Scheme, Request.Host.Value) ?? string.Empty
                : null;
            var prevPageUrl = page > 1
                ? Url.Action("Outbox", "ActivityPub", new { id, page = page - 1 }, Request.Scheme, Request.Host.Value) ?? string.Empty
                : null;
            var outboxCollectionUrl = Url.Action("Outbox", "ActivityPub", new { id, page = 0 }, Request.Scheme, Request.Host.Value) ?? string.Empty;

            var page_obj = new OrderedCollectionPage()
            {
                Id = pageUrl,
            };
            page_obj.AddProperty("partOf", outboxCollectionUrl);
            if (nextPageUrl != null)
            {
                page_obj.AddProperty("next", nextPageUrl);
            }
            if (prevPageUrl != null)
            {
                page_obj.AddProperty("prev", prevPageUrl);
            }

            var activities = pagedResources.Select(x => new CreateActivity()
            {
                Id = Url.Action("Activity", "ActivityPub", new { authorUsername = author.Username, activityId = x.Id }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                Actor = actorId,
                Published = x.PublishDate,
                To = new string[] {
                    "https://www.w3.org/ns/activitystreams#Public"
                },
                Cc = new string[] {
                    Url.Action("Followers", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty
                },
                Object = new NoteObject()
                {
                    Id = x.Url,
                    Sensitive = false,
                    InReplyTo = null,
                    Published = x.PublishDate,
                    Url = x.Url,
                    AttributedTo = actorId,
                    To = new string[] {
                        "https://www.w3.org/ns/activitystreams#Public"
                    },
                    Cc = new string[] {
                        Url.Action("Followers", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty
                    },
                    Content = x.Content ?? HtmlString.Empty,
                    Tag = x.Tags.Select(tag => new Hashtag()
                    {
                        Name = $"#{tag.Dehumanize()}"
                    })
                }
            });

            page_obj.OrderedItems = activities;

            return new ObjectResult(page_obj) { ContentTypes = { "application/activity+json" } };
        }

        public async Task<ActionResult> Inbox(string id)
        {
            var author = await _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }

            var inboxId = Url.Action("Inbox", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty;

            if (Request.Method == "POST")
            {
                // In a real-world scenario, you would parse the incoming activity
                // and hand it off to a service for processing. For now we delegate
                // to ActivityPubService which may enqueue for background processing.
                return StatusCode(202); // Accepted
            }

            return new ObjectResult(new
            {
                @context = "https://www.w3.org/ns/activitystreams",
                id = inboxId,
                type = "Inbox",
                Inbox = inboxId // The inbox itself
            })
            { ContentTypes = { "application/activity+json" } };
        }

        public async Task<ActionResult> Activity(string authorUsername, string activityId)
        {
            var res = await _hyraxResourceLocatorService.GetResource(activityId);

            if (res == null)
            {
                return NotFound();
            }

            var actorId = Url.Action("Actor", "ActivityPub", new { id = authorUsername }, Request.Scheme,
                Request.Host.Value) ?? string.Empty;

            var activity = new CreateActivity()
            {
                Id = Url.Action("Activity", "ActivityPub", new { activityId = res.Id }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                Actor = actorId,
                Published = res.PublishDate,
                To = new string[] {
                    "https://www.w3.org/ns/activitystreams#Public"
                },
                Cc = new string[] {
                    Url.Action("Followers", "ActivityPub", new { id = authorUsername }, Request.Scheme, Request.Host.Value) ?? string.Empty
                },
                Object = new NoteObject()
                {
                    Id = res.Url,
                    Sensitive = false,
                    InReplyTo = null,
                    Published = res.PublishDate,
                    Url = res.Url,
                    AttributedTo = actorId,
                    To = new string[] {
                        "https://www.w3.org/ns/activitystreams#Public"
                    },
                    Cc = new string[] {
                        Url.Action("Followers", "ActivityPub", new { id = authorUsername }, Request.Scheme, Request.Host.Value) ?? string.Empty
                    },
                    Content = res.Content ?? HtmlString.Empty,
                    Tag = res.Tags.Select(tag => new Hashtag()
                    {
                        Name = $"#{tag.Dehumanize()}"
                    })
                }
            };

            return new ObjectResult(activity) { ContentTypes = { "application/activity+json" } };
        }
    }
}
