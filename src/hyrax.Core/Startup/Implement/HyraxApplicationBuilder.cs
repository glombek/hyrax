using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace hyrax.Core.Startup.Implement
{
    internal class HyraxApplicationBuilder : IHyraxApplicationBuilder
    {
        public HyraxApplicationBuilder(IApplicationBuilder app) => App = app;

        public IApplicationBuilder App { get; }

        //IServiceProvider IApplicationBuilder.ApplicationServices { get => App.ApplicationServices; set => App.ApplicationServices = value }

        //IFeatureCollection IApplicationBuilder.ServerFeatures => App.ServerFeatures;

        //IDictionary<string, object?> IApplicationBuilder.Properties => App.Properties;

        //RequestDelegate IApplicationBuilder.Build()
        //{
        //    App.Build();
        //}

        //IApplicationBuilder IApplicationBuilder.New()
        //{
        //    App.New();
        //}

        //IApplicationBuilder IApplicationBuilder.Use(Func<RequestDelegate, RequestDelegate> middleware)
        //{
        //    App.Use(middleware);
        //}
    }
}
