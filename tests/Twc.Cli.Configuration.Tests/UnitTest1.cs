namespace Twc.Cli.Configuration.Tests;

using System;
using Shouldly;
using Shared;
using Xunit;

/// <summary>
/// Tests for <see cref="Guard"/>.
/// </summary>
public sealed class GuardTests
{
    /// <summary>
    /// Returns the value when it is not null/whitespace.
    /// </summary>
    [Fact]
    public void NotNullOrWhiteSpace_returns_value_when_valid()
    {
        Guard.NotNullOrWhiteSpace("ok", nameof(Guard)).ShouldBe("ok");
    }

    /// <summary>
    /// Throws when the value is null.
    /// </summary>
    [Fact]
    public void NotNullOrWhiteSpace_throws_when_null()
    {
        var ex = Should.Throw<ArgumentException>(() => Guard.NotNullOrWhiteSpace(null, "value"));
        ex.ParamName.ShouldBe("value");
    }

    /// <summary>
    /// Throws when the value is empty or whitespace.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void NotNullOrWhiteSpace_throws_when_whitespace(string input)
    {
        var ex = Should.Throw<ArgumentException>(() => Guard.NotNullOrWhiteSpace(input, "value"));
        ex.ParamName.ShouldBe("value");
    }
}
