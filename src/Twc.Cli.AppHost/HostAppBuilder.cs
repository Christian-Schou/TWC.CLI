using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.Framework.DependencyInjection;

namespace Twc.Cli.AppHost;

/// <summary>
/// Builder for composing a Spectre.Console.Cli + DI host.
/// </summary>
public sealed class HostAppBuilder
{
    public HostAppInfo Info { get; }
    public IServiceCollection Services { get; }

    public HostAppBuilder(HostAppInfo info)
    {
        Info = info ?? throw new ArgumentNullException(nameof(info));
        Services = new ServiceCollection();
    }

    public IConfiguration BuildConfiguration(string[] args)
    {
        var configFile = ResolveConfigFile(args, Info.DefaultConfigFileName);
        var envPrefix = ResolveEnvPrefix(args, Info.DefaultEnvPrefix);

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFile, optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: envPrefix)
            .Build();
    }

    public CommandApp BuildCommandApp(Action<IConfigurator> configure)
    {
        var registrar = new SpectreServiceProviderRegistrar(Services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName(Info.ApplicationName);
            configure(config);
        });

        return app;
    }

    private static string ResolveConfigFile(string[] args, string defaultFileName)
        => TryGetOptionValue(args, "--config") ?? TryGetOptionValue(args, "-c")
           ?? Environment.GetEnvironmentVariable("TWC_CONFIG_FILE")
           ?? defaultFileName;

    private static string ResolveEnvPrefix(string[] args, string defaultPrefix)
    {
        var prefix = TryGetOptionValue(args, "--env-prefix") ?? TryGetOptionValue(args, "-p")
                    ?? Environment.GetEnvironmentVariable("TWC_ENV_PREFIX")
                    ?? defaultPrefix;

        prefix = prefix.Trim();
        if (prefix.Length == 0)
            return defaultPrefix;

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

