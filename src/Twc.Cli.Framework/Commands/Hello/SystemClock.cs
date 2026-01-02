namespace Twc.Cli.Framework.Commands.Hello;

/// <summary>
///     System clock implementation.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}