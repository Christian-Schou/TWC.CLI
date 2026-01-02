namespace Twc.Cli.Updates.Providers.GitHub;

/// <summary>
/// Marker type for the GitHub updates provider assembly.
/// </summary>
public static class GitHubUpdatesProvider
{
    /// <summary>
    /// Provider name used in configuration.
    /// </summary>
    public const string ProviderName = "github";

    /// <summary>
    /// Gets the provider assembly.
    /// </summary>
    public static System.Reflection.Assembly Assembly => typeof(GitHubUpdatesProvider).Assembly;
}

