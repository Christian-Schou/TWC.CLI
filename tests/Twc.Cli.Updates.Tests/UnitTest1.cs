using NSubstitute;
using Shouldly;
using Xunit;

namespace Twc.Cli.Updates.Tests;

/// <summary>
/// Smoke tests proving the test infrastructure works.
/// </summary>
public sealed class TestStyleSamples
{
    /// <summary>
    ///     
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// 
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }

    /// <summary>
    /// Verifies NSubstitute and Shouldly behave as expected.
    /// </summary>
    [Fact]
    public void Can_substitute_and_assert()
    {
        var clock = Substitute.For<IClock>();
        var now = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
        clock.UtcNow.Returns(now);

        clock.UtcNow.ShouldBe(now);
    }
}
