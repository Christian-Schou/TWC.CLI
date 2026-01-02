using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Twc.Cli.Framework.Commands.Hello;

namespace Twc.Cli.Framework;

/// <summary>
///     Framework registration helpers.
/// </summary>
public static class TwcCliFrameworkServiceCollectionExtensions
{
    /// <summary>
    ///     Registers framework services into the provided <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddTwcCliFramework(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        // Sample services used by the sample command.
        services.AddSingleton<IClock, SystemClock>();
        services.AddTransient<IGreetingService, GreetingService>();

        // Register command types so Spectre resolves them from DI.
        services.AddTransient<HelloCommand>();

        return services;
    }

    /// <summary>
    ///     Registers framework commands into the provided Spectre configurator.
    /// </summary>
    public static IConfigurator AddTwcCliFrameworkCommands(this IConfigurator app)
    {
        if (app is null) throw new ArgumentNullException(nameof(app));

        app.AddCommand<HelloCommand>("hello")
            .WithDescription("Prints a greeting (sample command that validates DI wiring).");

        return app;
    }
}