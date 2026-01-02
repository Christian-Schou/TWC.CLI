using Shouldly;
using Xunit;

namespace Twc.Cli.Sdk.Tests;

/// <summary>
/// Tests for <see cref="VersionRange"/>.
/// </summary>
public sealed class VersionRangeTests
{
    /// <summary>
    /// Verifies version range '*' contains any version.
    /// </summary>
    [Fact]
    public void Any_contains_any_version()
    {
        VersionRange.Any.Contains(new SemanticVersion(0, 0, 0)).ShouldBeTrue();
        VersionRange.Any.Contains(new SemanticVersion(999, 0, 0)).ShouldBeTrue();
        VersionRange.Any.ToString().ShouldBe("*");
    }

    /// <summary>
    /// Verifies parsing and containment for common range syntax.
    /// </summary>
    [Fact]
    public void Parse_supports_min_and_max()
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
    public void Parse_rejects_invalid_inputs(string range)
    {
        Should.Throw<FormatException>(() => VersionRange.Parse(range));
    }
}

