using Twc.Cli.Framework.Plugins;

namespace Twc.Cli.Host.Plugins;

/// <summary>
///     Host-owned plugin catalog.
///     v1.1: uses assembly attributes for discovery of referenced plugin packages.
/// </summary>
public static class HostPluginCatalog
{
    /// <summary>
    ///     Builds the plugin catalog.
    /// </summary>
    public static PluginCatalog Build()
    {
        return new PluginCatalog()
            .AddFromAssemblies(PluginDiscovery.GetLoadedAssemblies());
    }
}