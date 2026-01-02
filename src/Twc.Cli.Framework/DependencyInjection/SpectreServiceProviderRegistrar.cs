using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Twc.Cli.Framework.DependencyInjection;

/// <summary>
///     Spectre.Console.Cli type registrar that resolves command instances from a Microsoft DI
///     <see cref="IServiceProvider" />.
///     This allows Spectre to create commands with constructor-injected services.
/// </summary>
public sealed class SpectreServiceProviderRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Creates a new registrar backed by the provided service collection.
    /// </summary>
    public SpectreServiceProviderRegistrar(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc />
    public ITypeResolver Build()
    {
        var provider = _services.BuildServiceProvider();
        return new SpectreServiceProviderResolver(provider);
    }

    /// <inheritdoc />
    public void Register(Type service, Type implementation)
    {
        if (service is null) throw new ArgumentNullException(nameof(service));
        if (implementation is null) throw new ArgumentNullException(nameof(implementation));

        _services.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterInstance(Type service, object implementation)
    {
        if (service is null) throw new ArgumentNullException(nameof(service));
        if (implementation is null) throw new ArgumentNullException(nameof(implementation));

        _services.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterLazy(Type service, Func<object> factory)
    {
        if (service is null) throw new ArgumentNullException(nameof(service));
        if (factory is null) throw new ArgumentNullException(nameof(factory));

        _services.AddSingleton(service, _ => factory());
    }

    private sealed class SpectreServiceProviderResolver : ITypeResolver, IDisposable
    {
        private readonly ServiceProvider _provider;

        public SpectreServiceProviderResolver(ServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void Dispose()
        {
            _provider.Dispose();
        }

        public object? Resolve(Type? type)
        {
            if (type is null)
                return null;

            // First try normal DI.
            var fromContainer = _provider.GetService(type);
            if (fromContainer is not null)
                return fromContainer;

            // Then try constructor injection even if the type isn't explicitly registered.
            try
            {
                return ActivatorUtilities.CreateInstance(_provider, type);
            }
            catch
            {
                return null;
            }
        }
    }
}