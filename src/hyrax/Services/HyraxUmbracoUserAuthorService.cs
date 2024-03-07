using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;
using hyrax.Core.Models.Implement;
using hyrax.Core.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Hyrax.Umbraco.Services
{
    public class HyraxUmbracoUserAuthorService : IHyraxAuthorService
    {
        private readonly IUserService _umbracoUserService;
        private readonly IShortStringHelper _shortStringHelper;

        public HyraxUmbracoUserAuthorService(IUserService umbracoUserService, IShortStringHelper shortStringHelper)
        {
            _umbracoUserService = umbracoUserService;
            _shortStringHelper = shortStringHelper;
        }

        private IAuthor? ConvertToAuthor(IUser? user)
        {
            if (user == null)
            {
                return null;
            }

            //TODO: Get a unique "username" from the user
            return new Author((user.Name ?? user.Email).ToUrlSegment(_shortStringHelper), user.Name ?? user.Email);
        }

        public IEnumerable<IAuthor> Get()
        {
            return _umbracoUserService.GetAll(0, int.MaxValue, out _).Select(ConvertToAuthor).WhereNotNull();
        }

        public IAuthor? Get(string username)
        {
            //Cannot use GetByUsername as we're generating our own usernames from the name
            //return ConvertToAuthor(_umbracoUserService.GetByUsername(id));

            //ensure username is converted
            username = username.ToUrlSegment(_shortStringHelper);

            return Get().FirstOrDefault(u => u.Username == username);
        }
    }
}
