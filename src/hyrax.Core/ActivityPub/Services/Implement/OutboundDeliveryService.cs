using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using hyrax.Core.ActivityPub.Models;
using Microsoft.Extensions.Options;

namespace hyrax.Core.ActivityPub.Services.Implement
{
    /// <summary>
    /// Implements outbound delivery of ActivityPub activities.
    /// Uses synchronous delivery with configurable retry policy and exponential backoff.
    /// </summary>
    public class OutboundDeliveryService : IOutboundDeliveryService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpSignatureService _signatureService;
        private readonly ActivityPubOptions _options;

        public OutboundDeliveryService(
            HttpClient httpClient,
            IHttpSignatureService signatureService,
            IOptions<ActivityPubOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _signatureService = signatureService ?? throw new ArgumentNullException(nameof(signatureService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<bool> Deliver(string actorId, string inboxUrl, string activity)
        {
            var maxRetries = _options.RetryPolicy?.MaxRetries ?? 3;
            var initialDelayMs = _options.RetryPolicy?.InitialDelayMs ?? 1000;
            var backoffMultiplier = _options.RetryPolicy?.BackoffMultiplier ?? 2.0;

            int retryCount = 0;
            while (true)
            {
                try
                {
                    // Sign the request
                    var signature = await _signatureService.SignRequest(actorId, inboxUrl, activity);

                    // Create the HTTP POST request
                    var content = new StringContent(activity, Encoding.UTF8, "application/activity+json");
                    var request = new HttpRequestMessage(HttpMethod.Post, inboxUrl)
                    {
                        Content = content
                    };

                    // Add signature header if available
                    if (!string.IsNullOrEmpty(signature))
                    {
                        request.Headers.Add("Signature", signature);
                    }

                    request.Headers.Add("User-Agent", "hyrax/1.0 (+http://hyrax.io)");

                    // Send the request
                    var response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    // Server returned error, might retry on certain status codes
                    if ((int)response.StatusCode >= 500)
                    {
                        // Server error, might retry
                        if (retryCount < maxRetries)
                        {
                            retryCount++;
                            var delayMs = (int)(initialDelayMs * Math.Pow(backoffMultiplier, retryCount - 1));
                            await Task.Delay(delayMs);
                            continue;
                        }
                    }

                    // Client or other error, don't retry
                    return false;
                }
                catch (Exception ex)
                {
                    // Network error, might retry
                    if (retryCount < maxRetries)
                    {
                        retryCount++;
                        var delayMs = (int)(initialDelayMs * Math.Pow(backoffMultiplier, retryCount - 1));
                        await Task.Delay(delayMs);
                        continue;
                    }

                    // Max retries exceeded
                    return false;
                }
            }
        }

        public async Task<int> DeliverToFollowers(string actorId, IReadOnlyList<Follower> followers, string activity)
        {
            var successCount = 0;

            foreach (var follower in followers)
            {
                if (!string.IsNullOrEmpty(follower.InboxUrl))
                {
                    try
                    {
                        var success = await Deliver(actorId, follower.InboxUrl, activity);
                        if (success)
                        {
                            successCount++;
                        }
                    }
                    catch
                    {
                        // Log and continue with next follower
                    }
                }
            }

            return successCount;
        }
    }
}
