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
using Microsoft.AspNetCore.Mvc;

namespace hyrax.Core.Controllers
{
    public class ActivityPubController : Controller
    {
        private readonly IHyraxResourceLocatorService _resourceLocatorService;
        private readonly IHyraxAuthorService _authorService;

        public ActivityPubController(
            IHyraxResourceLocatorService resourceLocatorService,
            IHyraxAuthorService authorService
            )
        {
            _resourceLocatorService = resourceLocatorService;
            _authorService = authorService;
        }

        public ActionResult Actor(string id)
        {
            var author = _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(new Actor()
            {
                Id = Url.Action("Actor", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                PreferredUsername = author.Username,
                Inbox = Url.Action("Inbox", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty,
                Outbox = Url.Action("Outbox", "ActivityPub", new { id = author.Username }, Request.Scheme, Request.Host.Value) ?? string.Empty
            });
        }
    }
}
