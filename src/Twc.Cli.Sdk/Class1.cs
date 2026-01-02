namespace Twc.Cli.Sdk;

/// <summary>
/// Represents a plugin identifier.
/// </summary>
public readonly record struct PluginId
{
    /// <summary>
    /// Gets the underlying identifier value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new <see cref="PluginId"/>.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
    public PluginId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plugin identifier cannot be null, empty, or whitespace.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Returns the identifier value.
    /// </summary>
    public override string ToString() => Value;
}
