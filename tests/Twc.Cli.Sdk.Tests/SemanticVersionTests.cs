using Shouldly;
using Xunit;

namespace Twc.Cli.Sdk.Tests;

/// <summary>
/// Tests for <see cref="SemanticVersion"/>.
/// </summary>
public sealed class SemanticVersionTests
{
    /// <summary>
    /// Verifies semantic version parsing.
    /// </summary>
    [Fact]
    public void Parse_parses_major_minor_patch()
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
    public void Compares_correctly()
    {
        new SemanticVersion(1, 0, 0).ShouldBeLessThan(new SemanticVersion(2, 0, 0));
        new SemanticVersion(1, 1, 0).ShouldBeGreaterThan(new SemanticVersion(1, 0, 9));
        new SemanticVersion(1, 0, 2).ShouldBeGreaterThan(new SemanticVersion(1, 0, 1));
    }
}

