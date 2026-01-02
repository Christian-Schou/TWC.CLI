namespace Twc.Cli.Updates.Providers.AzureDevOps;

/// <summary>
/// Marker type for the Azure DevOps updates provider assembly.
/// </summary>
public static class AzureDevOpsUpdatesProvider
{
    /// <summary>
    /// Provider name used in configuration.
    /// </summary>
    public const string ProviderName = "azuredevops";

    /// <summary>
    /// Gets the provider assembly.
    /// </summary>
    public static System.Reflection.Assembly Assembly => typeof(AzureDevOpsUpdatesProvider).Assembly;
}

