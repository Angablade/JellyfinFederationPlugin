using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.MediaInfo;
using System;
using System.Linq;
using System.Threading.Tasks;
using JellyfinFederationPlugin.Library;

namespace JellyfinFederationPlugin.Api
{
    public class PlaybackProxyService
    {
        private readonly FederationRequestHandler _requestHandler;
        private readonly ILogger _logger;

        public PlaybackProxyService(FederationRequestHandler _requestHandler, ILogger logger)
        {
            this._requestHandler = _requestHandler;
            _logger = logger;
        }

        public async Task<StreamInfo> GetFederatedStream(BaseItem item)
        {
            if (item.Path.StartsWith("remote://"))
            {
                var mediaId = item.Path.Replace("remote://", "");

                if (Guid.TryParse(mediaId, out var mediaGuid))
                {
                    var federatedServer = new Configuration.PluginConfiguration.FederatedServer
                    {
                        ServerUrl = mediaId
                    };

                    var federatedLibraryItems = await _requestHandler.GetFederatedLibrary(federatedServer);

                    if (federatedLibraryItems != null)
                    {
                        var federatedItem = federatedLibraryItems.FirstOrDefault(x => x.Id == mediaGuid);
                        if (federatedItem != null)
                        {
                            var streamUrl = $"{federatedServer.ServerUrl}/Items/{federatedItem.Id}/Playback";

                            return new StreamInfo
                            {
                                MediaSource = new MediaSourceInfo
                                {
                                    Path = streamUrl,
                                    Protocol = MediaProtocol.Http
                                },
                                DeviceProfile = new DeviceProfile
                                {
                                    Name = "Federated Device",
                                    MaxStreamingBitrate = 100000000
                                }
                            };
                        }
                    }
                }
                else
                {
                    _logger.LogError($"Failed to parse mediaId '{mediaId}' as a Guid.");
                }
            }

            return null;
        }
    }
}
