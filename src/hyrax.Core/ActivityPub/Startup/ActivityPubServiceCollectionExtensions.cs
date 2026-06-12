using hyrax.Core.ActivityPub;
using hyrax.Core.ActivityPub.Services;
using hyrax.Core.ActivityPub.Services.Implement;
using hyrax.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace hyrax.Core.Startup
{
    public static class ActivityPubServiceCollectionExtensions
    {
        public static IHyraxBuilder WithActivityPub(this IHyraxBuilder hyrax)
        {
            return WithActivityPub(hyrax, x => { });
        }

        public static IHyraxBuilder WithActivityPub(this IHyraxBuilder hyrax, Action<ActivityPubOptions> configure)
        {
            var options = new ActivityPubOptions();
            configure?.Invoke(options);

            hyrax.Services.Configure<ActivityPubOptions>(opts =>
            {
                opts.OutboxPageSize = options.OutboxPageSize;
                opts.FollowersPageSize = options.FollowersPageSize;
                opts.SignatureRepositoryPath = options.SignatureRepositoryPath;
                opts.AutoAcceptFollows = options.AutoAcceptFollows;
                opts.VerifySignatures = options.VerifySignatures;
                opts.DisableFollowerApproval = options.DisableFollowerApproval;
                opts.RetryPolicy = options.RetryPolicy;
            });

            // Register ActivityPub services here. Keep minimal implementation; implementer may expand.
            hyrax.Services.AddScoped<IActivityPubService, ActivityPubService>();
            hyrax.Services.AddScoped<IHyraxActivityService, HyraxActivityService>();
            hyrax.Services.AddScoped<IHttpSignatureService, HttpSignatureService>();

            // Register activity repository - default is in-memory
            hyrax.Services.AddScoped<IActivityRepository, InMemoryActivityRepository>();

            // Register HttpClient for outbound delivery
            hyrax.Services.AddHttpClient<OutboundDeliveryService>();
            hyrax.Services.AddScoped<IOutboundDeliveryService>(sp =>
                sp.GetRequiredService<OutboundDeliveryService>());

            // If signature repository is needed specifically for ActivityPub, register implementation here.
            // Example (existing implementation lives in hyrax.Core.Services.Implement):
            hyrax.Services.AddScoped<IHyraxSignatureRepositoryService>(sp =>
                new hyrax.Core.Services.Implement.HyraxFilesystemSignatureRepositoryService(options.SignatureRepositoryPath));

            return hyrax;
        }

        /// <summary>
        /// Configures Entity Framework Core persistence for ActivityPub activities and followers.
        /// </summary>
        public static IHyraxBuilder UseHyraxActivityPersistence(
            this IHyraxBuilder hyrax,
            Action<ActivityPersistenceOptions> configure)
        {
            var options = new ActivityPersistenceOptions();
            configure?.Invoke(options);

            // Register EF Core context and repository
            if (options.UseEFCore)
            {
                if (options.UseSqlServer && !string.IsNullOrEmpty(options.SqlServerConnectionString))
                {
                    hyrax.Services.AddDbContext<HyraxActivityDbContext>(opts =>
                        opts.UseSqlServer(options.SqlServerConnectionString));
                }
                else if (options.UseSqlite && !string.IsNullOrEmpty(options.SqliteConnectionString))
                {
                    hyrax.Services.AddDbContext<HyraxActivityDbContext>(opts =>
                        opts.UseSqlite(options.SqliteConnectionString));
                }

                hyrax.Services.AddScoped<IActivityRepository, EfActivityRepository>();
            }

            return hyrax;
        }
    }

    /// <summary>
    /// Options for configuring persistence providers.
    /// </summary>
    public class ActivityPersistenceOptions
    {
        public bool UseEFCore { get; set; } = false;
        public bool UseSqlServer { get; set; } = false;
        public bool UseSqlite { get; set; } = false;
        public string SqlServerConnectionString { get; set; } = string.Empty;
        public string SqliteConnectionString { get; set; } = string.Empty;
    }
}
