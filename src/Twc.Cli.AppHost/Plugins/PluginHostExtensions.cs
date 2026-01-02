using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.Framework.Plugins;
using Twc.Cli.Sdk;

namespace Twc.Cli.AppHost.Plugins;

/// <summary>
/// Convenience helpers for registering plugins in a Spectre.Console.Cli + Microsoft DI host.
///
/// v1: plugins are discovered from referenced/loaded assemblies via <see cref="TwcCliPluginAttribute"/>.
/// </summary>
public static class PluginHostExtensions
{
    /// <summary>
    /// Discovers plugins from currently loaded assemblies and registers them into DI and the command tree.
    /// Returns the discovered plugins and command types.
    /// </summary>
    public static DiscoveredPlugins DiscoverAndRegisterPlugins(
        this IServiceCollection services,
        IConfigurator configurator,
        SemanticVersion hostVersion)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configurator is null) throw new ArgumentNullException(nameof(configurator));

        var catalog = new PluginCatalog()
            .AddFromAssemblies(PluginDiscovery.GetLoadedAssemblies());

        var commandTypes = PluginLoader.RegisterAll(catalog, services, configurator, hostVersion);
        return new DiscoveredPlugins(catalog, commandTypes);
    }
}

