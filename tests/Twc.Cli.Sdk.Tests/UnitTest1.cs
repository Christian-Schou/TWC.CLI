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

    /// <summary>
    /// Verifies semantic version parsing.
    /// </summary>
    [Fact]
    public void SemanticVersion_Parse_parses_major_minor_patch()
    {
        var v = SemanticVersion.Parse("1.2.3");

        v.Major.ShouldBe(1);
        v.Minor.ShouldBe(2);
        v.Patch.ShouldBe(3);
        v.ToString().ShouldBe("1.2.3");
    }

    /// <summary>
    /// Verifies semantic version ordering.
    /// </summary>
    [Fact]
    public void SemanticVersion_compares_correctly()
    {
        new SemanticVersion(1, 0, 0).ShouldBeLessThan(new SemanticVersion(2, 0, 0));
        new SemanticVersion(1, 1, 0).ShouldBeGreaterThan(new SemanticVersion(1, 0, 9));
        new SemanticVersion(1, 0, 2).ShouldBeGreaterThan(new SemanticVersion(1, 0, 1));
    }

    /// <summary>
    /// Verifies version range '*' contains any version.
    /// </summary>
    [Fact]
    public void VersionRange_any_contains_any_version()
    {
        VersionRange.Any.Contains(new SemanticVersion(0, 0, 0)).ShouldBeTrue();
        VersionRange.Any.Contains(new SemanticVersion(999, 0, 0)).ShouldBeTrue();
        VersionRange.Any.ToString().ShouldBe("*");
    }

    /// <summary>
    /// Verifies parsing and containment for common range syntax.
    /// </summary>
    [Fact]
    public void VersionRange_Parse_supports_min_and_max()
    {
        var range = VersionRange.Parse(">=1.2.3 <2.0.0");

        range.Contains(new SemanticVersion(1, 2, 3)).ShouldBeTrue();
        range.Contains(new SemanticVersion(1, 9, 9)).ShouldBeTrue();
        range.Contains(new SemanticVersion(2, 0, 0)).ShouldBeFalse();
        range.Contains(new SemanticVersion(1, 2, 2)).ShouldBeFalse();
        range.ToString().ShouldBe(">=1.2.3 <2.0.0");
    }

    /// <summary>
    /// Verifies invalid range strings are rejected.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1.2.3")]
    [InlineData(">=1.2")]
    [InlineData("<1")]
    [InlineData(">=2.0.0 <2.0.0")]
    [InlineData("~=1.2.3")]
    public void VersionRange_Parse_rejects_invalid_inputs(string range)
    {
        Should.Throw<FormatException>(() => VersionRange.Parse(range));
    }
}
