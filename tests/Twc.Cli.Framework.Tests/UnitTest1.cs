using NSubstitute;
using Shouldly;
using Xunit;

namespace Twc.Cli.Framework.Tests;

/// <summary>
/// Smoke tests proving the test infrastructure works.
/// </summary>
public sealed class TestStyleSamples
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFoo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Get();
    }

    /// <summary>
    /// Verifies NSubstitute and Shouldly behave as expected.
    /// </summary>
    [Fact]
    public void NSubstitute_and_Shouldly_work()
    {
        var foo = Substitute.For<IFoo>();
        foo.Get().Returns(42);

        foo.Get().ShouldBe(42);
    }
}
