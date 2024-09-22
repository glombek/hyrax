using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using hyrax.Core.ActivityPub.Models;
using hyrax.Core.Services;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Humanizer;
using Microsoft.AspNetCore.Html;

namespace hyrax.Core.Controllers
{
    public class ActivityPubController : Controller
    {
        private readonly IHyraxResourceLocatorService _resourceLocatorService;
        private readonly IHyraxAuthorService _authorService;
        private readonly IHyraxSignatureRepositoryService _signatureRepositoryService;
        private readonly IHyraxResourceLocatorService _hyraxResourceLocatorService;

        public ActivityPubController(
            IHyraxResourceLocatorService resourceLocatorService,
            IHyraxAuthorService authorService,
            IHyraxSignatureRepositoryService signatureRepositoryService, IHyraxResourceLocatorService hyraxResourceLocatorService)
        {
            _resourceLocatorService = resourceLocatorService;
            _authorService = authorService;
            _signatureRepositoryService = signatureRepositoryService;
            _hyraxResourceLocatorService = hyraxResourceLocatorService;
        }

        public async Task<ActionResult> Actor(string id)
        {
            var author = _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }

            var key = await _signatureRepositoryService.GetPublicKeyForAuthor(author);


            var actorId = Url.Action("Actor", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty;

            return Ok(new Actor()
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
            });
        }

        public async Task<ActionResult> Outbox(string id, int page = 1)
        {
            var author = _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }

            var actorId = Url.Action("Actor", "ActivityPub", new { id = author.Username }, Request.Scheme,
                Request.Host.Value) ?? string.Empty;

            var resources = _hyraxResourceLocatorService.GetResources(author: author);

            return Ok(new OrderedCollectionPage()
            {
                Id = Url.Action("Outbox", "ActivityPub", new { id, page }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                OrderedItems = resources.Select(x => new CreateActivity()
                {
                    Id = "https://umbracocommunity.social/users/joe/statuses/113112223161684772/activity",
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
                        //Summary = content warning
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
                        //AtomUri = x.Url,
                        Content = x.Content ?? HtmlString.Empty,
                        Tag = x.Tags.Select(tag=> new Hashtag()
                        {
                            //Href = "",
                            Name = $"#{tag.Dehumanize()}"
                        })
                    }
                }
                )
            });
        }
    }
}
