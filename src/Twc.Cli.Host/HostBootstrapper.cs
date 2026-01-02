using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.AppHost.Plugins;
using Twc.Cli.Framework;
using Twc.Cli.Framework.DependencyInjection;
using Twc.Cli.Framework.Plugins;
using Twc.Cli.Sdk;

namespace Twc.Cli.Host;

/// <summary>
///     Thin host bootstrapper:
///     - Loads configuration
///     - Wires DI
///     - Registers framework services/commands (and later, plugins)
///     - Builds a Spectre.Console.Cli <see cref="CommandApp" />
/// </summary>
public static class HostBootstrapper
{
    private const string DefaultConfigFileName = "twc.json";
    private const string DefaultEnvPrefix = "TWC_";

    /// <summary>
    ///     Builds a configured <see cref="CommandApp" />.
    /// </summary>
    public static CommandApp BuildCommandApp(string[] args)
    {
        var configFile = ResolveConfigFile(args);
        var envPrefix = ResolveEnvPrefix(args);

        // Configuration (minimal defaults; host stays thin)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFile, true, false)
            .AddEnvironmentVariables(envPrefix)
            .Build();

        // DI
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        // Discover plugins once and store in DI for reuse (config wizard, etc.)
        var pluginContext = PluginContextBuilder.BuildFromLoadedAssemblies();
        services.AddSingleton(pluginContext);

        // Framework services + sample command
        services.AddTwcCliFramework();

        // Spectre app with DI
        var registrar = new SpectreServiceProviderRegistrar(services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName(HostInfo.ApplicationName);
            config.ValidateExamples();


            // Framework commands
            config.AddTwcCliFrameworkCommands();

            // Plugins: register discovered plugins into DI + command tree.
            var hostVersion = new SemanticVersion(0, 1, 0);
            var commandTypes = PluginLoaderExtensions.RegisterAll(pluginContext.Plugins, services, config, hostVersion);

            foreach (var commandType in commandTypes.CommandTypes)
                services.AddTransient(commandType);
        });

        return app;
    }

    private static string ResolveConfigFile(string[] args)
    {
        // Priority: --config <file> (or --config=<file>) -> env var -> default
        var fromArgs = TryGetOptionValue(args, "--config")
                       ?? TryGetOptionValue(args, "-c");

        if (!string.IsNullOrWhiteSpace(fromArgs))
            return fromArgs;

        var fromEnv = Environment.GetEnvironmentVariable("TWC_CONFIG_FILE");
        if (!string.IsNullOrWhiteSpace(fromEnv))
            return fromEnv;

        return DefaultConfigFileName;
    }

    private static string ResolveEnvPrefix(string[] args)
    {
        // Priority: --env-prefix <prefix> (or --env-prefix=<prefix>) -> env var -> default
        var fromArgs = TryGetOptionValue(args, "--env-prefix")
                       ?? TryGetOptionValue(args, "-p");

        if (!string.IsNullOrWhiteSpace(fromArgs))
            return EnsureEnvPrefixFormat(fromArgs);

        var fromEnv = Environment.GetEnvironmentVariable("TWC_ENV_PREFIX");
        if (!string.IsNullOrWhiteSpace(fromEnv))
            return EnsureEnvPrefixFormat(fromEnv);

        return DefaultEnvPrefix;
    }

    private static string EnsureEnvPrefixFormat(string prefix)
    {
        // Microsoft.Extensions.Configuration expects the prefix to match exactly.
        // Most prefixes are specified with a trailing underscore (e.g., TWC_).
        prefix = prefix.Trim();
        if (prefix.Length == 0)
            return DefaultEnvPrefix;

        return prefix.EndsWith('_') ? prefix : prefix + "_";
    }

    private static string? TryGetOptionValue(string[] args, string option)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, option, StringComparison.Ordinal))
            {
                if (i + 1 < args.Length)
                    return args[i + 1];
                return null;
            }

            if (arg.StartsWith(option + "=", StringComparison.Ordinal))
                return arg[(option.Length + 1)..];
        }

        return null;
    }
}