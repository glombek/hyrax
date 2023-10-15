using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.Models
{
    public interface IPublication
    {
        string Name { get; }
        string? Description { get; }
        Uri? Url { get; }
        string Language { get; }
    }
}
