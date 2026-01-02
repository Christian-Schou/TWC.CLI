namespace Twc.Cli.Shared;

/// <summary>
/// Simple guard helpers for validating arguments.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Ensures <paramref name="value"/> is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name used in the thrown exception.</param>
    /// <returns>The original <paramref name="value"/> when valid.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
    public static string NotNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);

        return value;
    }
}
