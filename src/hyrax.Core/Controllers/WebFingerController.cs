using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hyrax.Core.Models.Implement;
using hyrax.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Community.UmbtivityHub.Controllers
{
    public class WebFingerController : Controller
    {
        private readonly IHyraxAuthorService _authorService;

        public WebFingerController(IHyraxAuthorService authorService)
        {
            _authorService = authorService;
        }

        public string Domain
        {
            get
            {
                return Request.Host.Value;
            }
        }

        public ActionResult Get(string resource)
        {
            if(string.IsNullOrEmpty(resource))
            {
                return NotFound();
            }

            var acct = Regex.Match(resource, "^acct:(?<username>.*)@(?<host>.*)$");
            string? username = acct?.Groups["username"]?.Value;
            string? host = acct?.Groups["host"]?.Value;

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(host) || host != Domain)
            {
                return NotFound();
            }

            //TODO: check valid username
            var author = _authorService.Get(username);

            if (author == null)
            {
                return NotFound();
            }


            //Generate a url from the route
            var url = Url.Action("Actor", "ActivityPub", new { id = author.Username }, Request.Scheme, Domain);


            var res = Json(
                new
                {
                    subject = $"acct:me@{Domain}",
                    aliases = new string[] {
                        //$"https://{Domain}/@{author.Username}"
                    },
                    links = new []
                    {
                        new
                        {
                            rel = "self",
                            type = "application/activity+json",
                            href = url
                        }
                    }
                }
                );

            res.ContentType = "application/activity+json";
            return res;
        }
    }
}
