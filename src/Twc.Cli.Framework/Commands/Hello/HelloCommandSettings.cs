using Spectre.Console.Cli;

namespace Twc.Cli.Framework.Commands.Hello;

/// <summary>
///     Settings for the <see cref="HelloCommand" />.
/// </summary>
public sealed class HelloCommandSettings : CommandSettings
{
    /// <summary>
    ///     Optional name to greet.
    /// </summary>
    [CommandArgument(0, "[name]")]
    public string? Name { get; init; }
}