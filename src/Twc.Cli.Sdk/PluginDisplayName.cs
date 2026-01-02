namespace Twc.Cli.Sdk;

/// <summary>
/// Human-friendly plugin name.
/// </summary>
public readonly record struct PluginDisplayName
{
    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new <see cref="PluginDisplayName"/>.
    /// </summary>
    /// <param name="value">The display name.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
    public PluginDisplayName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plugin display name cannot be null, empty, or whitespace.", nameof(value));

        Value = value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}

