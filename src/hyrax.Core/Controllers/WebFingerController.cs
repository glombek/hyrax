using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hyrax.Core.Models.Implement;
using Microsoft.AspNetCore.Mvc;

namespace Umbraco.Community.UmbtivityHub.Controllers
{
    public class WebFingerController : Controller
    {
        public WebFingerController() {
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

            var res = Json(
                new
                {
                    subject = $"acct:me@{Domain}",
                    aliases = new string[] {
                        $"https://{Domain}/@me"
                    },
                    links = new []
                    {
                        new
                        {
                            rel = "self",
                            type = "application/activity+json",
                            href = $"https://{Domain}/activitypub/actor"
                        }
                    }
                }
                );

            res.ContentType = "application/activity+json";
            return res;
        }
    }
}
