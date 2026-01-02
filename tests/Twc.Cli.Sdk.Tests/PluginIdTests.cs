using Shouldly;
using Xunit;

namespace Twc.Cli.Sdk.Tests;

/// <summary>
/// Tests for <see cref="PluginId"/>.
/// </summary>
public sealed class PluginIdTests
{
    /// <summary>
    /// Verifies <see cref="PluginId.ToString"/> returns the underlying value.
    /// </summary>
    [Fact]
    public void ToString_returns_value()
    {
        var id = new PluginId("hello");

        id.ToString().ShouldBe("hello");
    }

    /// <summary>
    /// Verifies the constructor rejects null/empty/whitespace identifiers.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    public void Constructor_throws_for_null_empty_or_whitespace(string? value)
    {
        Should.Throw<ArgumentException>(() => new PluginId(value));
    }
}

