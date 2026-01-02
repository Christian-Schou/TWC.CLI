namespace Twc.Cli.Sdk;

/// <summary>
///     Host/plugin compatibility checks.
/// </summary>
public static class HostCompatibility
{
    /// <summary>
    ///     Returns true if <paramref name="hostVersion" /> is within the plugin's required host version range.
    /// </summary>
    public static bool IsCompatible(SemanticVersion hostVersion, VersionRange requiredHostRange)
    {
        return requiredHostRange.Contains(hostVersion);
    }
}