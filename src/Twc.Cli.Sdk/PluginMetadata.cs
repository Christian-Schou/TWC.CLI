namespace Twc.Cli.Sdk;

/// <summary>
///     Plugin metadata declared by a plugin package.
/// </summary>
/// <param name="Id">The unique plugin identifier.</param>
/// <param name="DisplayName">A human friendly name.</param>
/// <param name="Version">The plugin version.</param>
/// <param name="RequiresHostVersionRange">Semantic version range of the host supported by this plugin.</param>
public sealed record PluginMetadata(
    PluginId Id,
    PluginDisplayName DisplayName,
    SemanticVersion Version,
    VersionRange RequiresHostVersionRange);