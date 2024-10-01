using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using JellyfinFederationPlugin.Configuration;
using JellyfinFederationPlugin.Library;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using System;
using System.Globalization;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public override string Name => "Jellyfin Federation Plugin";
    public override string Description => "Enables federation across Jellyfin servers for streaming media without syncing files.";
    public override Guid Id => new Guid("ab6fad0e-ffcb-469c-b1b0-b11f79ddf447");
   

    public static Plugin Instance { get; private set; }

    private readonly FederationLibraryService _federationLibraryService;
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger<Plugin> _logger;

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILibraryManager libraryManager, ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;

        _libraryManager = libraryManager;
        _logger = logger;

        var federationRequestHandler = new FederationRequestHandler(_logger);

                _federationLibraryService = new FederationLibraryService(
                    _libraryManager,
                    federationRequestHandler,
                    _logger
                );

        Initialize();
    }

    private void Initialize()
    {
         _ = _federationLibraryService.MergeFederatedLibrariesAsync();
    }

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
               {
            new PluginPageInfo
            {
                Name = this.Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Web.ConfigurationPage.html", GetType().Namespace)
            }
        };
    }
}
