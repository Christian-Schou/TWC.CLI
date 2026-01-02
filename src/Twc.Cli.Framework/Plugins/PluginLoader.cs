using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.Sdk;

namespace Twc.Cli.Framework.Plugins;

/// <summary>
///     Loads plugins from an explicit <see cref="PluginCatalog" />.
/// </summary>
public static class PluginLoader
{
    /// <summary>
    ///     Registers all plugins into DI and the CLI command tree.
    ///     Incompatible plugins are skipped.
    /// </summary>
    public static CommandTypeCatalog RegisterAll(
        PluginCatalog catalog,
        IServiceCollection services,
        IConfigurator config,
        SemanticVersion hostVersion)
    {
        if (catalog is null) throw new ArgumentNullException(nameof(catalog));
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (config is null) throw new ArgumentNullException(nameof(config));

        var commandTypes = new CommandTypeCatalog();

        foreach (var plugin in catalog.Plugins)
        {
            if (!HostCompatibility.IsCompatible(hostVersion, plugin.Metadata.RequiresHostVersionRange))
                continue;

            // Services first so command constructors can resolve plugin dependencies.
            plugin.RegisterServices(new MicrosoftServiceRegistry(services));
            plugin.RegisterCommands(new SpectreCommandRegistry(config, commandTypes));
        }

        return commandTypes;
    }
}