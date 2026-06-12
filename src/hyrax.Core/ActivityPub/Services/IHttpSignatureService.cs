using System.Threading.Tasks;

namespace hyrax.Core.ActivityPub.Services
{
    /// <summary>
    /// Verifies HTTP signatures for ActivityPub requests.
    /// </summary>
    public interface IHttpSignatureService
    {
        /// <summary>
        /// Verifies the HTTP signature of a request.
        /// </summary>
        /// <param name="actorUri">The URI of the actor making the request.</param>
        /// <param name="rawBody">The raw request body.</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        Task<bool> VerifySignature(string actorUri, string rawBody);

        /// <summary>
        /// Signs a request body for outgoing deliveries.
        /// </summary>
        /// <param name="actorId">The local actor ID to sign the request as.</param>
        /// <param name="targetUrl">The target URL for the delivery.</param>
        /// <param name="body">The body to sign.</param>
        /// <returns>The signature header value to include in the request.</returns>
        Task<string> SignRequest(string actorId, string targetUrl, string body);
    }
}
