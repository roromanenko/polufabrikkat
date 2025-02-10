using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Polufabrikkat.Core.ApiClients
{
    public class UnsplashApiClient : IUnsplashApiClient
    {
        private readonly UnsplashApiOptions _apiOptions;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UnsplashApiClient> _logger;

        public UnsplashApiClient(HttpClient httpClient, ILogger<UnsplashApiClient> logger, IOptions<UnsplashApiOptions> apiOptions)
        {
            _apiOptions = apiOptions.Value;
            if (string.IsNullOrWhiteSpace(_apiOptions.ApiKey))
            {
                throw new InvalidOperationException("Unsplash Access Key is not configured.");
            }

            _httpClient = httpClient;

            _logger = logger;
        }

        public async Task<string> GetRandomImageUrlAsync(string query)
        {
            var requestUrl = $"https://api.unsplash.com/photos/random?query={Uri.EscapeDataString(query)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Client-ID", _apiOptions.ApiKey);

            _logger.LogInformation($"Requesting image with query: {query}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<UnsplashApiResponse>(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return content.Urls.Regular;
        }
    }
}
