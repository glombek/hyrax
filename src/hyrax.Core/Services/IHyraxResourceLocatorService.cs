using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.Models;

namespace hyrax.Core.Services
{
    public interface IHyraxResourceLocatorService
    {
        Task<IResource?> GetResource(string id);
        Task<IEnumerable<IResource>> GetResources(string? culture = null, IAuthor? author = null);
    }
}
