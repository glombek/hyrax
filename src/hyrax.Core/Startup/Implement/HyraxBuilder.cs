using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace hyrax.Core.Startup.Implement
{
    public class HyraxBuilder:IHyraxBuilder
    {
        public HyraxBuilder(IServiceCollection services) => Services = services;

        public IServiceCollection Services { get; }
    }
}
