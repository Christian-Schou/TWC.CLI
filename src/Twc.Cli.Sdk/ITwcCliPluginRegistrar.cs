namespace Twc.Cli.Sdk;

/// <summary>
/// Host-provided registrar used by plugins to register services and contribute commands.
/// </summary>
public interface ITwcCliPluginRegistrar
{
    /// <summary>
    /// Adds services to the host service collection.
    /// 
    /// The SDK intentionally avoids referencing Microsoft.Extensions.DependencyInjection.
    /// </summary>
    void ConfigureServices(Action<IServiceRegistry> configure);

    /// <summary>
    /// Adds a command branch to the CLI's command tree.
    /// </summary>
    void ConfigureCommands(Action<ICommandRegistry> configure);

    /// <summary>
    /// Returns false when the plugin should not be loaded due to incompatibility.
    /// </summary>
    bool IsHostCompatible(PluginMetadata metadata);
}

/// <summary>
/// Minimal, SDK-defined service registration abstraction.
/// Host chooses the real DI container.
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    /// Registers a singleton factory.
    /// </summary>
    void AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class;

    /// <summary>
    /// Registers a transient factory.
    /// </summary>
    void AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class;

    /// <summary>
    /// Registers a prebuilt singleton instance.
    /// </summary>
    void AddSingletonInstance<TService>(TService instance) where TService : class;
}

/// <summary>
/// Minimal command registration abstraction (host owns Spectre integration).
/// </summary>
public interface ICommandRegistry
{
    /// <summary>
    /// Adds a leaf command.
    /// </summary>
    void AddCommand(string name, Type commandType, string? description = null);

    /// <summary>
    /// Adds a branch command group.
    /// </summary>
    void AddBranch(string name, Action<ICommandRegistry> configure, string? description = null);
}

