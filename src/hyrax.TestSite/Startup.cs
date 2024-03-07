using hyrax.Core.Models;
using hyrax.Core.Models.Implement;
using hyrax.Core.Services;
using hyrax.Core.Startup;
using hyrax.Umbraco;
using Hyrax.Umbraco.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace hyrax.TestSite
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337.
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddWebsite()
                .AddComposers()
                .Build();

            //var hyraxSingleAuthor = new Author("test", "Test");
            //services.AddHyrax((BlogPost blogPost, IHyraxAuthorService authorService) => new Resource(
            //        new Uri(blogPost.Url(mode: UrlMode.Absolute)),
            //        blogPost.Id.ToString(),
            //        blogPost.Name ?? string.Empty,
            //        hyraxSingleAuthor.AsEnumerableOfOne(),
            //        blogPost.PublishDate,
            //        blogPost.Tags ?? new string[] { },
            //        blogPost.Abstract,
            //        new Microsoft.AspNetCore.Html.HtmlString(blogPost.BodyText?.ToString())
            //    ),
            //    hyraxSingleAuthor);

            services.AddHyrax<BlogPost, HyraxUmbracoUserAuthorService>(
                (BlogPost blogPost, IHyraxAuthorService authorService) =>
                {
                    var author = authorService.Get(blogPost.CreatorId.ToString());

                    return new Resource(
                            new Uri(blogPost.Url(mode: UrlMode.Absolute)),
                            blogPost.Id.ToString(),
                            blogPost.Name ?? string.Empty,
                            author?.AsEnumerableOfOne() ?? new IAuthor[] { },
                            blogPost.PublishDate,
                            blogPost.Tags ?? new string[] { },
                            blogPost.Abstract,
                            new Microsoft.AspNetCore.Html.HtmlString(blogPost.BodyText?.ToString())
                            );
                });
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();
                })
                .WithEndpoints(u =>
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });

            app.UseHyrax();

        }
    }
}
