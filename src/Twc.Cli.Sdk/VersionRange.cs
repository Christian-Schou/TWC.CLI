using System.Diagnostics.CodeAnalysis;

namespace Twc.Cli.Sdk;

/// <summary>
/// A minimal semantic version range used to declare compatible host versions.
/// </summary>
public sealed record VersionRange
{
    /// <summary>
    /// Inclusive minimum host version.
    /// </summary>
    public SemanticVersion? MinInclusive { get; }

    /// <summary>
    /// Exclusive maximum host version.
    /// </summary>
    public SemanticVersion? MaxExclusive { get; }

    /// <summary>
    /// Creates a version range.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the range is invalid.</exception>
    public VersionRange(SemanticVersion? minInclusive, SemanticVersion? maxExclusive)
    {
        if (minInclusive is not null && maxExclusive is not null && minInclusive >= maxExclusive)
            throw new ArgumentException("MinInclusive must be less than MaxExclusive.");

        MinInclusive = minInclusive;
        MaxExclusive = maxExclusive;
    }

    /// <summary>
    /// Returns a range that matches any host version.
    /// </summary>
    public static VersionRange Any { get; } = new(minInclusive: null, maxExclusive: null);

    /// <summary>
    /// Checks if a host version is supported by this range.
    /// </summary>
    public bool Contains(SemanticVersion hostVersion)
    {
        if (MinInclusive is not null && hostVersion < MinInclusive.Value)
            return false;

        if (MaxExclusive is not null && hostVersion >= MaxExclusive.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Parses a range in one of these forms:
    /// <list type="bullet">
    /// <item><description><c>*</c> (any)</description></item>
    /// <item><description><c>&gt;=1.2.3</c></description></item>
    /// <item><description><c>&gt;=1.2.3 &lt;2.0.0</c></description></item>
    /// </list>
    /// </summary>
    public static VersionRange Parse(string value)
    {
        if (!TryParse(value, out var range))
            throw new FormatException($"Invalid version range '{value}'. Expected '*', '>=x.y.z', or '>=x.y.z <a.b.c'.");

        return range;
    }

    /// <summary>
    /// Tries to parse a host compatibility range.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? value, [NotNullWhen(true)] out VersionRange? range)
    {
        range = null;

        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();
            if (trimmed == "*")
            {
                range = Any;
                return true;
            }

            var tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            SemanticVersion? min = null;
            SemanticVersion? max = null;

            foreach (var token in tokens)
            {
                if (token.StartsWith(">=", StringComparison.Ordinal))
                {
                    if (!SemanticVersion.TryParse(token[2..], out var v)) return false;
                    min = v;
                    continue;
                }

                if (token.StartsWith("<", StringComparison.Ordinal))
                {
                    if (!SemanticVersion.TryParse(token[1..], out var v)) return false;
                    max = v;
                    continue;
                }

                return false;
            }

            if (min is null && max is null)
                return false;

            range = new VersionRange(min, max);
            return true;
        }
        catch (ArgumentException)
        {
            // Normalizes all invalid boundaries to "false" for TryParse callers.
            range = null;
            return false;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (ReferenceEquals(this, Any) || (MinInclusive is null && MaxExclusive is null))
            return "*";

        if (MinInclusive is not null && MaxExclusive is not null)
            return $">={MinInclusive} <{MaxExclusive}";

        if (MinInclusive is not null)
            return $">={MinInclusive}";

        return $"<{MaxExclusive}";
    }
}
