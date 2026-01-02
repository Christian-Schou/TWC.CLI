using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.AppHost.Configuration;
using Twc.Cli.Framework;

namespace Twc.Cli.AppHost;

/// <summary>
/// Opinionated defaults for hosts.
///
/// The goal is to keep host apps very small: most wiring should live here.
/// </summary>
public static class HostDefaults
{
    /// <summary>
    /// Adds the built-in config wizard branch (`config`, `config create`).
    /// </summary>
    public static void AddConfigWizard<TCore>(
        HostAppBuilder builder,
        IConfigurator configurator,
        Func<string, TCore> createDefaultCore,
        Func<string, TCore> promptForCore)
        where TCore : class, new()
        => HostConfigModule.AddConfigCommands(builder, configurator, createDefaultCore, promptForCore);

    /// <summary>
    /// Adds framework defaults (services and default framework commands).
    ///
    /// Hosts can still override by registering afterward.
    /// </summary>
    public static void AddFrameworkDefaults(HostAppBuilder builder, IConfigurator configurator)
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));
        if (configurator is null) throw new ArgumentNullException(nameof(configurator));

        // Keep this in AppHost to avoid hosts needing to know about internal wiring.
        // We reference the framework project directly in this repo.
        builder.Services.AddTwcCliFramework();
        configurator.AddTwcCliFrameworkCommands();
    }
}
