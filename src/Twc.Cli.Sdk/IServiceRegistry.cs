namespace Twc.Cli.Sdk;

/// <summary>
///     Minimal, SDK-defined service registration abstraction.
///     Host chooses the real DI container.
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    ///     Registers a singleton factory.
    /// </summary>
    void AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class;

    /// <summary>
    ///     Registers a transient factory.
    /// </summary>
    void AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class;

    /// <summary>
    ///     Registers a prebuilt singleton instance.
    /// </summary>
    void AddSingletonInstance<TService>(TService instance) where TService : class;
}