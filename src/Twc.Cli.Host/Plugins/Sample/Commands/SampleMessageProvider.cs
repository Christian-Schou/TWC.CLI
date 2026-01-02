namespace Twc.Cli.Host.Plugins.Sample.Commands;

/// <summary>
///     Trivial service used by the plugin command to validate DI.
/// </summary>
public sealed class SampleMessageProvider
{
    /// <summary>
    ///     Creates a message provider.
    /// </summary>
    public SampleMessageProvider(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    ///     Message returned by the provider.
    /// </summary>
    public string Message { get; }
}