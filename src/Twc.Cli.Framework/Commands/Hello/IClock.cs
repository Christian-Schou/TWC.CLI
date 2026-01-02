namespace Twc.Cli.Framework.Commands.Hello;

/// <summary>
///     Abstraction over time for deterministic tests.
/// </summary>
public interface IClock
{
    /// <summary>
    ///     Current UTC timestamp.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}