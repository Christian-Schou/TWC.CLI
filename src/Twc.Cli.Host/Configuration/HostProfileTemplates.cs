namespace Twc.Cli.Host.Configuration;

/// <summary>
///     Hardcoded profile templates used to drive the interactive selection menus.
///     v1: This is intentionally hardcoded (no discovery) per the host-thin policy.
/// </summary>
public static class HostProfileTemplates
{
    /// <summary>
    ///     Known environments.
    /// </summary>
    public static IReadOnlyList<string> Environments { get; } = ["dev", "test", "prod"];

    /// <summary>
    ///     Known log levels.
    /// </summary>
    public static IReadOnlyList<string> LogLevels { get; } =
        ["Trace", "Debug", "Information", "Warning", "Error", "Critical"];

    /// <summary>
    ///     Known API base URLs.
    /// </summary>
    public static IReadOnlyList<string> ApiBaseUrls { get; } =
    [
        "https://api.example.local",
        "https://api.test.example.com",
        "https://api.example.com"
    ];
}