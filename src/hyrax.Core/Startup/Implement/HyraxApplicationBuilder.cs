using Microsoft.AspNetCore.Builder;

namespace hyrax.Core.Startup.Implement
{
    internal class HyraxApplicationBuilder : IHyraxApplicationBuilder
    {
        public HyraxApplicationBuilder(IApplicationBuilder app) => App = app;

        public IApplicationBuilder App { get; }
    }
}