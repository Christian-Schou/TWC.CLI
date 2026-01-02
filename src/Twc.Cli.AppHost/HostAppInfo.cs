namespace Twc.Cli.AppHost;

/// <summary>
/// Host-level identity for a CLI tool using the app host package.
/// </summary>
public sealed record HostAppInfo(
    string ApplicationName,
    string DefaultConfigFileName = "twc.json",
    string DefaultEnvPrefix = "TWC_");
