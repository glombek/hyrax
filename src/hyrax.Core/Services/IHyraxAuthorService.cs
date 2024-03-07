using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;

namespace hyrax.Core.Services
{
    public interface IHyraxAuthorService
    {
        IEnumerable<IAuthor> Get();
        IAuthor? Get(string username);
    }
}
