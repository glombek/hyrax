using System.Threading.Tasks;
using hyrax.Core.Services;
using hyrax.Core.Models;

namespace hyrax.Core.ActivityPub.Services
{
    public class ActivityPubService : IActivityPubService
    {
        private readonly IHyraxSignatureRepositoryService _signatureRepositoryService;

        public ActivityPubService(IHyraxSignatureRepositoryService signatureRepositoryService)
        {
            _signatureRepositoryService = signatureRepositoryService;
        }

        public Task HandleActivityAsync(IAuthor author, object activity)
        {
            // TODO: implement queuing and delivery of incoming activities (background processing)
            return Task.CompletedTask;
        }

        public async Task<string> GetPublicKeyForAuthorAsync(IAuthor author)
        {
            // Keep minimal: defer to configured signature repository service if available.
            return await _signatureRepositoryService.GetPublicKeyForAuthor(author);
        }
    }
}
