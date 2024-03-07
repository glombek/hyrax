using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;

namespace hyrax.Core.Services.Implement
{
    public class HyraxAutomaticAuthorService : IHyraxAuthorService
    {
        private readonly IHyraxResourceLocatorService _resourceLocatorService;

        public HyraxAutomaticAuthorService(IHyraxResourceLocatorService resourceLocatorService)
        {
            _resourceLocatorService = resourceLocatorService;
        }

        public IEnumerable<IAuthor> Get()
        {
            return _resourceLocatorService.GetResources().SelectMany(r => r.Authors).Distinct();
        }

        public IAuthor? Get(string name)
        {
            return Get().FirstOrDefault(a => a.Name == name);
        }
    }
}
