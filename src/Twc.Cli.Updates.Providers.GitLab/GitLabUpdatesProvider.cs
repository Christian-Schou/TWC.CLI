namespace Twc.Cli.Updates.Providers.GitLab;

/// <summary>
/// Marker type for the GitLab updates provider assembly.
/// </summary>
public static class GitLabUpdatesProvider
{
    /// <summary>
    /// Provider name used in configuration.
    /// </summary>
    public const string ProviderName = "gitlab";

    /// <summary>
    /// Gets the provider assembly.
    /// </summary>
    public static System.Reflection.Assembly Assembly => typeof(GitLabUpdatesProvider).Assembly;
}

