using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.AppHost.Plugins;

namespace Twc.Cli.AppHost.Configuration;

/// <summary>
/// Optional built-in configuration/profile command wiring for hosts.
/// </summary>
public static class HostConfigModule
{
    /// <summary>
    /// Adds a built-in `config` command branch that generates profile JSON.
    /// </summary>
    public static void AddConfigCommands<TCore>(
        HostAppBuilder builder,
        IConfigurator configurator,
        Func<string, TCore> createDefaultCore,
        Func<string, TCore> promptForCore)
        where TCore : class, new()
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));
        if (configurator is null) throw new ArgumentNullException(nameof(configurator));
        if (createDefaultCore is null) throw new ArgumentNullException(nameof(createDefaultCore));
        if (promptForCore is null) throw new ArgumentNullException(nameof(promptForCore));

        // Command type (constructor-injected)
        builder.Services.AddSingleton<Func<string, TCore>>(createDefaultCore);
        builder.Services.AddSingleton<Func<string, TCore>>(promptForCore);
        builder.Services.AddTransient<ConfigWizardCommand<TCore>>(sp =>
        {
            var pluginCtx = sp.GetService<PluginContext>();
            var defaultFactory = sp.GetRequiredService<Func<string, TCore>>();
            var promptFactory = sp.GetRequiredService<Func<string, TCore>>();
            return new ConfigWizardCommand<TCore>(builder.Info.ApplicationName, pluginCtx, defaultFactory, promptFactory);
        });

        configurator.AddBranch("config", b =>
        {
            b.SetDescription("Configuration profile commands");
            b.SetDefaultCommand<ConfigWizardCommand<TCore>>();

            b.AddCommand<ConfigWizardCommand<TCore>>("create")
                .WithDescription("Interactively generate a JSON profile (wizard).")
                .WithExample("config", "create", "--output", "<file>.json");
        });
    }
}
