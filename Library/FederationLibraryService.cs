using MediaBrowser.Controller.Library;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.Movies;
using System.Linq;
using BaseItem = MediaBrowser.Controller.Entities.BaseItem;
using BaseItemDto = MediaBrowser.Model.Dto.BaseItemDto;

namespace JellyfinFederationPlugin.Library
{
    public class FederationLibraryService
    {
        private readonly ILibraryManager _libraryManager;
        private readonly FederationRequestHandler _federationRequestHandler;
        private readonly ILogger _logger;

        public FederationLibraryService(ILibraryManager libraryManager, FederationRequestHandler federationRequestHandler, ILogger logger)
        {
            _libraryManager = libraryManager;
            _federationRequestHandler = federationRequestHandler;
            _logger = logger;
        }

        public async Task MergeFederatedLibrariesAsync()
        {
            var config = Plugin.Instance.Configuration;
            foreach (var server in config.FederatedServers)
            {
                var federatedItems = await _federationRequestHandler.GetFederatedLibrary(server);

                if (federatedItems != null && federatedItems.Any())
                {
                    _logger.LogInformation($"Fetched {federatedItems.Count} items from {server.ServerUrl}");

                    foreach (var federatedItem in federatedItems)
                    {
                        var virtualItem = CreateVirtualItem(federatedItem);
                        _libraryManager.AddMediaPath(virtualItem.Name, new MediaBrowser.Model.Configuration.MediaPathInfo(virtualItem.Path));
                    }
                }
            }
        }

        private BaseItem CreateVirtualItem(BaseItemDto remoteItem)
        {
            var movie = new Movie
            {
                Name = remoteItem.Name,
                Path = $"remote://{remoteItem.Id}",
            };

            return movie;
        }
    }
}
