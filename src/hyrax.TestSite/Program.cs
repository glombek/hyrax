using hyrax.Core.Models;
using hyrax.Core.Models.Implement;
using hyrax.Core.Services;
using hyrax.Core.Startup;
using hyrax.Umbraco;
using Hyrax.Umbraco.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// Make Kestrel listen on the local.notacu.lt host (HTTPS) in addition to existing URLs
builder.WebHost.UseUrls("https://local.notacu.lt:443", "https://localhost:44358", "http://localhost:26995");



builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

builder.Services.AddHyrax<BlogPost, HyraxUmbracoUserAuthorService>(
    async (BlogPost blogPost, IHyraxAuthorService authorService) =>
    {
        var author = await authorService.Get(blogPost.CreatorName().ToLower());

        return new Resource(
                    new Uri(blogPost.Url(mode: UrlMode.Absolute)),
                    blogPost.Id.ToString(),
                    blogPost.Name ?? string.Empty,
                    author is null ? new IAuthor[] { } : Enumerable.Repeat(author, 1),
                    blogPost.PublishDate,
                    blogPost.Tags ?? new string[] { },
                    blogPost.Abstract,
                    new Microsoft.AspNetCore.Html.HtmlString(blogPost.BodyText?.ToString())
                    );
    }).WithActivityPub();

// Hard-coded signle author
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

// Umbraco content as authors
//services.AddHyrax<BlogPost, UmbracoAuthor>(
//    (blogPost, authorService) =>
//    {
//        var author = authorService.Get(blogPost.CreatorId.ToString());

//        return new Resource(
//            new Uri(blogPost.Url(mode: UrlMode.Absolute)),
//            blogPost.Id.ToString(),
//            blogPost.Name ?? string.Empty,
//            author?.AsEnumerableOfOne() ?? new IAuthor[] { },
//            blogPost.PublishDate,
//            blogPost.Tags ?? new string[] { },
//            blogPost.Abstract,
//            new Microsoft.AspNetCore.Html.HtmlString(blogPost.BodyText?.ToString())
//        );
//    },
//    author => new hyrax.Core.Models.Implement.Author(author.Username, author.Name)
//    );

WebApplication app = builder.Build();

await app.BootUmbracoAsync();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

app.UseHyrax()
    .WithActivityPub();

await app.RunAsync();
