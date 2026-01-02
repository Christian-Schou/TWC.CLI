using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.AppHost.Configuration;
using Twc.Cli.AppHost.Plugins;
using Twc.Cli.Framework.Plugins;

namespace Twc.Cli.AppHost;

/// <summary>
/// High-level entrypoints for building and running a CLI tool using Twc.Cli.AppHost.
/// </summary>
public static class HostApp
{
    /// <summary>
    /// Convenience helper that:
    /// - builds configuration
    /// - lets the caller register services (including IConfiguration)
    /// - configures Spectre commands
    /// - runs the app
    /// </summary>
    public static int Run(
        string[] args,
        HostAppInfo info,
        Action<HostAppBuilder, IConfiguration> configureServices,
        Action<IConfigurator> configureCommands)
    {
        if (args is null) throw new ArgumentNullException(nameof(args));
        if (info is null) throw new ArgumentNullException(nameof(info));
        if (configureServices is null) throw new ArgumentNullException(nameof(configureServices));
        if (configureCommands is null) throw new ArgumentNullException(nameof(configureCommands));

        var builder = new HostAppBuilder(info);
        var configuration = builder.BuildConfiguration(args);

        configureServices(builder, configuration);

        var app = builder.BuildCommandApp(configureCommands);
        return app.Run(args);
    }

    /// <summary>
    /// One-call host runner.
    ///
    /// This overload:
    /// - builds IConfiguration (json + env vars)
    /// - registers IConfiguration into DI
    /// - optionally discovers and registers plugins
    /// - invokes optional user hooks for additional services and commands
    /// - runs the Spectre.Console.Cli application
    /// </summary>
    public static int Run(string[] args, HostAppInfo info, HostAppOptions options)
    {
        if (args is null) throw new ArgumentNullException(nameof(args));
        if (info is null) throw new ArgumentNullException(nameof(info));
        if (options is null) throw new ArgumentNullException(nameof(options));

        var builder = new HostAppBuilder(info);
        var configuration = builder.BuildConfiguration(args);

        // Always register IConfiguration.
        builder.Services.AddSingleton<IConfiguration>(configuration);

        // Optional user service registrations.
        options.ConfigureServices?.Invoke(builder, configuration);

        // Discover plugins once so other command/service code can reuse it.
        PluginContext? pluginContext = null;
        if (options.EnablePlugins)
        {
            pluginContext = PluginContextBuilder.BuildFromLoadedAssemblies();
            builder.Services.AddSingleton(pluginContext);
        }

        var app = builder.BuildCommandApp(configurator =>
        {
            configurator.ValidateExamples();

            if (options.EnableFrameworkDefaults)
            {
                HostDefaults.AddFrameworkDefaults(builder, configurator);
            }

            // Register plugins after framework defaults so plugin commands appear alongside.
            if (options.EnablePlugins && pluginContext is not null)
            {
                var commandTypes = PluginLoaderExtensions.RegisterAll(pluginContext.Plugins, builder.Services, configurator, options.HostVersion);
                foreach (var commandType in commandTypes.CommandTypes)
                    builder.Services.AddTransient(commandType);
            }

            // Built-in `config` wizard wiring (optional)
            var wizard = options.ConfigWizard;
            if (wizard is null && options.EnableConfigWizard)
            {
                wizard = new ConfigWizardOptions
                {
                    CoreType = options.ConfigCoreType ?? throw new InvalidOperationException("EnableConfigWizard requires ConfigCoreType."),
                    CreateDefaultCore = options.CreateDefaultConfigCore ?? throw new InvalidOperationException("EnableConfigWizard requires CreateDefaultConfigCore."),
                    PromptForCore = options.PromptForConfigCore ?? throw new InvalidOperationException("EnableConfigWizard requires PromptForConfigCore."),
                };
            }

            if (wizard is not null)
            {
                var method = typeof(HostConfigModule)
                    .GetMethod(nameof(HostConfigModule.AddConfigCommands))
                    ?? throw new InvalidOperationException("Could not find HostConfigModule.AddConfigCommands.");

                var generic = method.MakeGenericMethod(wizard.CoreType);
                generic.Invoke(null, new object[] { builder, configurator, wizard.CreateDefaultCore, wizard.PromptForCore });
            }

            options.ConfigureCommands?.Invoke(configurator);
        });

        return app.Run(args);
    }
}
