using System.Diagnostics.CodeAnalysis;

namespace Twc.Cli.Sdk;

/// <summary>
///     A minimal semantic version implementation (MAJOR.MINOR.PATCH).
/// </summary>
public readonly record struct SemanticVersion : IComparable<SemanticVersion>
{
    /// <summary>
    ///     Creates a semantic version.
    /// </summary>
    public SemanticVersion(int major, int minor, int patch)
    {
        if (major < 0) throw new ArgumentOutOfRangeException(nameof(major));
        if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor));
        if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch));

        Major = major;
        Minor = minor;
        Patch = patch;
    }

    /// <summary>
    ///     Major version.
    /// </summary>
    public int Major { get; }

    /// <summary>
    ///     Minor version.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    ///     Patch version.
    /// </summary>
    public int Patch { get; }

    /// <inheritdoc />
    public int CompareTo(SemanticVersion other)
    {
        var major = Major.CompareTo(other.Major);
        if (major != 0) return major;

        var minor = Minor.CompareTo(other.Minor);
        if (minor != 0) return minor;

        return Patch.CompareTo(other.Patch);
    }

    /// <summary>
    ///     Parses a semantic version string in the form <c>MAJOR.MINOR.PATCH</c>.
    /// </summary>
    public static SemanticVersion Parse(string value)
    {
        if (!TryParse(value, out var result))
            throw new FormatException($"Invalid semantic version '{value}'. Expected 'MAJOR.MINOR.PATCH'.");

        return result;
    }

    /// <summary>
    ///     Tries to parse a semantic version string in the form <c>MAJOR.MINOR.PATCH</c>.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? value, out SemanticVersion version)
    {
        version = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var parts = value.Trim().Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var major) || major < 0) return false;
        if (!int.TryParse(parts[1], out var minor) || minor < 0) return false;
        if (!int.TryParse(parts[2], out var patch) || patch < 0) return false;

        version = new SemanticVersion(major, minor, patch);
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    /// <summary>
    ///     Returns true when <paramref name="left" /> is greater than <paramref name="right" />.
    /// </summary>
    public static bool operator >(SemanticVersion left, SemanticVersion right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    ///     Returns true when <paramref name="left" /> is less than <paramref name="right" />.
    /// </summary>
    public static bool operator <(SemanticVersion left, SemanticVersion right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    ///     Returns true when <paramref name="left" /> is greater than or equal to <paramref name="right" />.
    /// </summary>
    public static bool operator >=(SemanticVersion left, SemanticVersion right)
    {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    ///     Returns true when <paramref name="left" /> is less than or equal to <paramref name="right" />.
    /// </summary>
    public static bool operator <=(SemanticVersion left, SemanticVersion right)
    {
        return left.CompareTo(right) <= 0;
    }
}