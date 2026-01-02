namespace Twc.Cli.Updates.Providers.Gitea;

/// <summary>
/// Marker type for the Gitea updates provider assembly.
/// </summary>
public static class GiteaUpdatesProvider
{
    /// <summary>
    /// Provider name used in configuration.
    /// </summary>
    public const string ProviderName = "gitea";

    /// <summary>
    /// Gets the provider assembly.
    /// </summary>
    public static System.Reflection.Assembly Assembly => typeof(GiteaUpdatesProvider).Assembly;
}

