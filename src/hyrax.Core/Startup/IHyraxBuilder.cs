using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace hyrax.Core.Startup
{
    public interface IHyraxBuilder
    {
        IServiceCollection Services { get; }
    }
}
