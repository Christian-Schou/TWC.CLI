using Spectre.Console.Cli;

namespace Twc.Cli.Host.Plugins.Sample.Commands;

/// <summary>
///     Settings for <see cref="PluginHelloCommand" />.
/// </summary>
public sealed class PluginHelloCommandSettings : CommandSettings
{
}

/// <summary>
///     Sample plugin command.
/// </summary>
public sealed class PluginHelloCommand : Command<PluginHelloCommandSettings>
{
    private readonly SampleMessageProvider _messages;

    /// <summary>
    ///     Creates the command.
    /// </summary>
    public PluginHelloCommand(SampleMessageProvider messages)
    {
        _messages = messages ?? throw new ArgumentNullException(nameof(messages));
    }

    /// <inheritdoc />
    public override int Execute(CommandContext context, PluginHelloCommandSettings settings,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Plugin hello: {_messages.Message}");
        return 0;
    }
}