using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JellyfinFederationPlugin
{
    public class FederationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        private readonly List<FederatedServer> _federatedServers = new List<FederatedServer>();

        public FederationService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<MediaItem>> GetFederatedMediaAsync(string libraryName)
        {
            var mediaItems = new List<MediaItem>();

            foreach (var server in _federatedServers)
            {
                try
                {
                    var url = $"{server.Url}/Libraries/{libraryName}/Items?api_key={server.ApiKey}";
                    _logger.LogInformation($"Fetching media from federated server: {server.Url}");

                    var response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var items = JsonSerializer.Deserialize<List<MediaItem>>(content);
                        mediaItems.AddRange(items);
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to fetch media from {server.Url}. Status Code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching media from {server.Url}: {ex.Message}");
                }
            }

            return mediaItems;
        }

        public void AddFederatedServer(string url, string apiKey)
        {
            _federatedServers.Add(new FederatedServer
            {
                Url = url,
                ApiKey = apiKey
            });

            _logger.LogInformation($"Added federated server: {url}");
        }

        public void RemoveFederatedServer(string url)
        {
            _federatedServers.RemoveAll(s => s.Url == url);
            _logger.LogInformation($"Removed federated server: {url}");
        }

        public class FederatedServer
        {
            public string Url { get; set; }
            public string ApiKey { get; set; }
        }

        public class MediaItem
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string MediaType { get; set; }
        }
    }
}
