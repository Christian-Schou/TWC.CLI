namespace Twc.Cli.Host.Configuration;

/// <summary>
///     Represents the host-owned configuration model that can be materialized into a JSON profile.
///     This intentionally lives in the host project (not framework) so the host remains the policy owner
///     of what configuration exists and how it is authored.
/// </summary>
public sealed class HostProfile
{
    /// <summary>
    ///     Friendly profile name.
    /// </summary>
    public string Name { get; set; } = "default";

    /// <summary>
    ///     Runtime environment selection.
    /// </summary>
    public string Environment { get; set; } = "dev";

    /// <summary>
    ///     API base URL.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.example.local";

    /// <summary>
    ///     Log verbosity.
    /// </summary>
    public string LogLevel { get; set; } = "Information";
}