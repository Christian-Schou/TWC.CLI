namespace Twc.Cli.Host.Configuration;

/// <summary>
///     Root JSON document we generate for configuration profiles.
///     Host owns strongly-typed settings; plugin settings live under <see cref="PluginSettings" />.
/// </summary>
public sealed class ProfileDocument
{
    /// <summary>
    ///     Host-owned settings.
    /// </summary>
    public HostProfile Core { get; set; } = new();

    /// <summary>
    ///     Plugin settings, keyed by plugin id.
    /// </summary>
    public Dictionary<string, Dictionary<string, object?>> PluginSettings { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}