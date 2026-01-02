namespace Twc.Cli.AppHost.Configuration;

/// <summary>
/// Root JSON document for generated profiles.
/// </summary>
public sealed class ProfileDocument<TCore>
    where TCore : class, new()
{
    /// <summary>
    /// Host-owned settings.
    /// </summary>
    public TCore Core { get; set; } = new();

    /// <summary>
    /// Plugin settings, keyed by plugin id.
    /// </summary>
    public Dictionary<string, Dictionary<string, object?>> PluginSettings { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}

