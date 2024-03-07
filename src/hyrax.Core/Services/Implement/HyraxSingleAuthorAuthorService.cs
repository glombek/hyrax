using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;

namespace hyrax.Core.Services.Implement
{
    public class HyraxSingleAuthorAuthorService : IHyraxAuthorService
    {
        private readonly IAuthor _author;

        public HyraxSingleAuthorAuthorService(IAuthor author) => _author = author;

        public IEnumerable<IAuthor> Get()
        {
            return new[] { _author };
        }

        public IAuthor? Get(string name)
        {
            if (name == _author.Name)
            {
                return _author;
            }

            return null;
        }
    }
}
