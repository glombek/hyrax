using System.Threading.Tasks;
using hyrax.Core.Models;

namespace hyrax.Core.ActivityPub.Services
{
    public interface IActivityPubService
    {
        Task HandleActivityAsync(IAuthor author, object activity);

        Task<string> GetPublicKeyForAuthorAsync(IAuthor author);
    }
}
