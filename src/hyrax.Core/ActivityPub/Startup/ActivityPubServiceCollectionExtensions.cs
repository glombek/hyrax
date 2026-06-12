using hyrax.Core.ActivityPub;
using hyrax.Core.ActivityPub.Services;
using hyrax.Core.Services;
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
                opts.SignatureRepositoryPath = options.SignatureRepositoryPath;
            });

            // Register ActivityPub services here. Keep minimal implementation; implementer may expand.
            hyrax.Services.AddScoped<IActivityPubService, ActivityPubService>();

            // If signature repository is needed specifically for ActivityPub, register implementation here.
            // Example (existing implementation lives in hyrax.Core.Services.Implement):
            hyrax.Services.AddScoped<IHyraxSignatureRepositoryService>(sp =>
                new hyrax.Core.Services.Implement.HyraxFilesystemSignatureRepositoryService(options.SignatureRepositoryPath));

            return hyrax;
        }
    }
}
