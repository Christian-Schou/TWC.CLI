namespace Twc.Cli.Sdk;

/// <summary>
///     Optional plugin surface for contributing configuration schema to the host.
///     The host uses this to generate/update JSON profiles so plugin settings appear automatically.
/// </summary>
public interface IConfigSchemaContributor
{
    /// <summary>
    ///     Returns one or more configuration entries the plugin wants to add to the profile.
    /// </summary>
    IEnumerable<ConfigEntry> GetConfigEntries();
}

/// <summary>
///     Describes a single configuration key and a suggested default value.
/// </summary>
public sealed class ConfigEntry
{
    /// <summary>
    ///     Configuration key within the plugin section.
    ///     Example: "Message".
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    ///     Friendly label for interactive UIs.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    ///     Optional help text for interactive UIs.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Suggested default value.
    /// </summary>
    public required ConfigValue DefaultValue { get; init; }

    /// <summary>
    ///     Whether the value should be treated as sensitive.
    /// </summary>
    public bool IsSecret { get; init; }

    /// <summary>
    ///     Optional allowed values (for selection prompts).
    /// </summary>
    public IReadOnlyList<ConfigValue>? AllowedValues { get; init; }
}

/// <summary>
///     A JSON-serializable config value.
/// </summary>
public abstract record ConfigValue
{
    /// <summary>
    ///     Base type for config values.
    /// </summary>
    private protected ConfigValue()
    {
    }

    /// <summary>
    ///     A string config value.
    /// </summary>
    public sealed record String(string? Value) : ConfigValue;

    /// <summary>
    ///     A boolean config value.
    /// </summary>
    public sealed record Boolean(bool Value) : ConfigValue;

    /// <summary>
    ///     An integer config value.
    /// </summary>
    public sealed record Integer(int Value) : ConfigValue;
}