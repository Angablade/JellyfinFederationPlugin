using MediaBrowser.Model.Plugins;
using System.Collections.Generic;

namespace JellyfinFederationPlugin.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public List<FederatedServer> FederatedServers { get; set; } = new List<FederatedServer>();

        public class FederatedServer
        {
            public string ServerUrl { get; set; }
            public string ApiKey { get; set; }
            public int Port { get; set; }
        }
    }
}
