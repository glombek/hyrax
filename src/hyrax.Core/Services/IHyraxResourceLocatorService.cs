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
        IEnumerable<IResource> GetResources(string? culture = null);
    }
}
