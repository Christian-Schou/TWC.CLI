namespace Twc.Cli.Sdk;

/// <summary>
/// Represents a plugin identifier.
/// </summary>
public readonly record struct PluginId(string Value)
{
    /// <summary>
    /// Returns the identifier value.
    /// </summary>
    public override string ToString() => Value;
}
