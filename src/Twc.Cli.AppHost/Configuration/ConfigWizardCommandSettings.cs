using Spectre.Console.Cli;

namespace Twc.Cli.AppHost.Configuration;

/// <summary>
/// Settings for <see cref="ConfigWizardCommand{TCore}"/>.
/// </summary>
#pragma warning disable CA1819 // Populated by Spectre via reflection.
#pragma warning disable IDE0051 // Reflection-based binding.
public sealed class ConfigWizardCommandSettings : CommandSettings
{
    /// <summary>
    /// Name of the profile to create or update.
    /// If omitted, the wizard will prompt.
    /// </summary>
    [CommandOption("--name <NAME>")]
    public string? Name { get; set; }

    /// <summary>
    /// Output file path for the generated profile JSON.
    /// If specified, the profile will be written to this exact path rather than the profile store.
    /// </summary>
    [CommandOption("--output|-o <FILE>")]
    public string? Output { get; set; }

    /// <summary>
    /// Root directory for profile storage (defaults to user home directory).
    /// </summary>
    [CommandOption("--profiles-root <DIR>")]
    public string? ProfilesRoot { get; set; }

    /// <summary>
    /// Force overwrite if the profile already exists.
    /// </summary>
    [CommandOption("--overwrite")]
    public bool Overwrite { get; set; }

    /// <summary>
    /// If specified, the command will not prompt and will generate a profile based on defaults.
    /// Useful for tests/automation.
    /// </summary>
    [CommandOption("--non-interactive")]
    public bool NonInteractive { get; set; }
}
#pragma warning restore IDE0051
#pragma warning restore CA1819

