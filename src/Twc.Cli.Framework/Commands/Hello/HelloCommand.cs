using Spectre.Console.Cli;

namespace Twc.Cli.Framework.Commands.Hello;

/// <summary>
///     Minimal sample command used to validate Spectre.Console.Cli + DI wiring.
/// </summary>
public sealed class HelloCommand : Command<HelloCommandSettings>
{
    private readonly IGreetingService _greetingService;

    /// <summary>
    ///     Creates a new command.
    /// </summary>
    public HelloCommand(IGreetingService greetingService)
    {
        _greetingService = greetingService ?? throw new ArgumentNullException(nameof(greetingService));
    }

    /// <inheritdoc />
    public override int Execute(CommandContext context, HelloCommandSettings settings,
        CancellationToken cancellationToken)
    {
        var message = _greetingService.FormatGreeting(settings.Name);
        Console.WriteLine(message);
        return 0;
    }
}