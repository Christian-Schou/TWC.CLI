using Microsoft.Extensions.Configuration;

namespace Twc.Cli.Framework.Commands.Hello;

/// <summary>
///     Formats greetings used by sample commands.
/// </summary>
public interface IGreetingService
{
    /// <summary>
    ///     Formats a greeting for the provided name.
    /// </summary>
    string FormatGreeting(string? name);
}

/// <summary>
///     Default greeting implementation.
/// </summary>
public sealed class GreetingService : IGreetingService
{
    private readonly IClock _clock;
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Creates a new greeting service.
    /// </summary>
    public GreetingService(IClock clock, IConfiguration configuration)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public string FormatGreeting(string? name)
    {
        var who = string.IsNullOrWhiteSpace(name) ? "world" : name.Trim();
        var greetingPrefix = _configuration["Greeting:Prefix"];
        if (string.IsNullOrWhiteSpace(greetingPrefix))
            greetingPrefix = "Hello";

        return $"{greetingPrefix}, {who}! (UTC: {_clock.UtcNow:O})";
    }
}