using Twc.Cli.Sdk;

namespace Twc.Cli.AppHost.Plugins;

/// <summary>
/// Snapshot of discovered plugin instances. Registered in DI to avoid repeated discovery.
/// </summary>
public sealed class PluginContext
{
    /// <summary>
    /// Creates a new plugin context.
    /// </summary>
    public PluginContext(IReadOnlyList<ITwcCliPlugin> plugins)
    {
        Plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));
    }

    /// <summary>
    /// Discovered plugin instances.
    /// </summary>
    public IReadOnlyList<ITwcCliPlugin> Plugins { get; }
}
