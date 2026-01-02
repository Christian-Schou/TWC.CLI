using Twc.Cli.Shared;

namespace Twc.Cli.Sdk;

/// <summary>
/// Represents a plugin identifier.
/// </summary>
public readonly record struct PluginId
{
    /// <summary>
    /// Creates a new <see cref="PluginId"/>.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
    public PluginId(string? value)
    {
        Value = Guard.NotNullOrWhiteSpace(value, nameof(value));
    }

    /// <summary>
    /// Gets the underlying identifier value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
