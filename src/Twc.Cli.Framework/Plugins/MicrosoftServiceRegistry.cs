using Microsoft.Extensions.DependencyInjection;
using Twc.Cli.Sdk;

namespace Twc.Cli.Framework.Plugins;

internal sealed class MicrosoftServiceRegistry : IServiceRegistry
{
    private readonly IServiceCollection _services;

    public MicrosoftServiceRegistry(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public void AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
    {
        _services.AddSingleton<TService>(factory);
    }

    public void AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class
    {
        _services.AddTransient<TService>(factory);
    }

    public void AddSingletonInstance<TService>(TService instance) where TService : class
    {
        _services.AddSingleton(instance);
    }
}