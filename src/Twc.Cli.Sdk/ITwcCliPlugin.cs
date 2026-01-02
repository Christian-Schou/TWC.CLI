namespace Twc.Cli.Sdk;

/// <summary>
///     A plugin (distributed as a NuGet package) that contributes commands and services to a TWC CLI host.
/// </summary>
public interface ITwcCliPlugin
{
    /// <summary>
    ///     Static plugin metadata.
    /// </summary>
    PluginMetadata Metadata { get; }

    /// <summary>
    ///     Called by the host during bootstrapping to allow the plugin to register services.
    /// </summary>
    void RegisterServices(IServiceRegistry services);

    /// <summary>
    ///     Called by the host during bootstrapping to allow the plugin to register commands.
    /// </summary>
    void RegisterCommands(ICommandRegistry commands);
}