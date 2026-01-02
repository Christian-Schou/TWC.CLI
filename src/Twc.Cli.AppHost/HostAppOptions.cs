using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;
using Twc.Cli.Sdk;

namespace Twc.Cli.AppHost;

/// <summary>
/// Options for the one-call host runner.
/// </summary>
public sealed class HostAppOptions
{
    /// <summary>
    /// Host version used for plugin compatibility checks.
    /// </summary>
    public required SemanticVersion HostVersion { get; init; }

    /// <summary>
    /// Whether to auto-discover and register plugins.
    /// </summary>
    public bool EnablePlugins { get; init; } = true;

    /// <summary>
    /// Optional hook for additional host service registrations.
    /// Called after IConfiguration is created and registered.
    /// </summary>
    public Action<HostAppBuilder, IConfiguration>? ConfigureServices { get; init; }

    /// <summary>
    /// Optional hook for adding host commands/branches.
    /// Called after plugins (if enabled) have registered their commands.
    /// </summary>
    public Action<IConfigurator>? ConfigureCommands { get; init; }

    /// <summary>
    /// Enable the built-in `config` wizard command.
    /// </summary>
    public bool EnableConfigWizard { get; init; }

    /// <summary>
    /// Core profile type used as the <c>Core</c> section of the generated profile JSON.
    /// If <see cref="EnableConfigWizard"/> is true, this must be set.
    /// </summary>
    public Type? ConfigCoreType { get; init; }

    /// <summary>
    /// Factory for creating a default core profile for a given profile name.
    /// Must match <see cref="ConfigCoreType"/>.
    /// </summary>
    public Delegate? CreateDefaultConfigCore { get; init; }

    /// <summary>
    /// Factory for interactively prompting for a core profile.
    /// Must match <see cref="ConfigCoreType"/>.
    /// </summary>
    public Delegate? PromptForConfigCore { get; init; }

    /// <summary>
    /// If true, the host will auto-register the framework services and default framework commands.
    /// </summary>
    public bool EnableFrameworkDefaults { get; init; } = true;

    /// <summary>
    /// Optional strongly-typed config wizard setup. If provided, the config wizard is enabled.
    /// Prefer this over using <see cref="EnableConfigWizard"/> with reflection fields.
    /// </summary>
    public ConfigWizardOptions? ConfigWizard { get; init; }
}

/// <summary>
/// Strongly-typed options for the built-in config wizard.
/// </summary>
public sealed class ConfigWizardOptions
{
    public required Type CoreType { get; init; }
    public required Delegate CreateDefaultCore { get; init; }
    public required Delegate PromptForCore { get; init; }
}
