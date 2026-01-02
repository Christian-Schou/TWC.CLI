using Twc.Cli.Framework.Plugins;

namespace Twc.Cli.AppHost.Plugins;

/// <summary>
/// Builds a <see cref="PluginContext"/> from loaded assemblies.
/// </summary>
public static class PluginContextBuilder
{
    public static PluginContext BuildFromLoadedAssemblies()
    {
        var catalog = new PluginCatalog()
            .AddFromAssemblies(PluginDiscovery.GetLoadedAssemblies());

        return new PluginContext(catalog.Plugins);
    }
}

