using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.Sdk;

namespace Twc.Cli.Framework.Plugins;

/// <summary>
///     Convenience overloads for registering plugins.
/// </summary>
public static class PluginLoaderExtensions
{
    /// <summary>
    ///     Registers plugins into DI and the Spectre.Console.Cli command tree.
    /// </summary>
    public static CommandTypeCatalog RegisterAll(
        IEnumerable<ITwcCliPlugin> plugins,
        IServiceCollection services,
        IConfigurator configurator,
        SemanticVersion hostVersion)
    {
        if (plugins is null) throw new ArgumentNullException(nameof(plugins));

        var catalog = new PluginCatalog();
        foreach (var plugin in plugins)
            catalog.Add(plugin);

        return PluginLoader.RegisterAll(catalog, services, configurator, hostVersion);
    }
}