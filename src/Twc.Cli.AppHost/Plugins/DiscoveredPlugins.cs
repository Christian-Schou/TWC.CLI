using Twc.Cli.Framework.Plugins;
using Twc.Cli.Sdk;

namespace Twc.Cli.AppHost.Plugins;

/// <summary>
/// Result of plugin discovery.
/// </summary>
public sealed record DiscoveredPlugins(PluginCatalog Catalog, CommandTypeCatalog CommandTypes)
{
    /// <summary>
    /// Discovered plugin instances.
    /// </summary>
    public IEnumerable<ITwcCliPlugin> Plugins => Catalog.EnumeratePlugins();
}
