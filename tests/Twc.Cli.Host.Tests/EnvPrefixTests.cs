using Shouldly;
using Xunit;

namespace Twc.Cli.Host.Tests;

public sealed class EnvPrefixTests
{
    [Fact]
    public void Custom_env_prefix_is_used_for_environment_variable_configuration()
    {
        // GreetingService reads Greeting:Prefix from IConfiguration.
        // We'll provide that via env vars using a custom prefix and assert it takes effect.
        const string prefix = "ACME_";

        var result = HostCliRunner.RunWithEnvironment(
            new Dictionary<string, string?>
            {
                ["TWC_ENV_PREFIX"] = prefix,
                [prefix + "Greeting__Prefix"] = "Howdy",
            },
            "hello",
            "Ada");

        result.ExitCode.ShouldBe(0, $"STDOUT:\n{result.StdOut}\nSTDERR:\n{result.StdErr}");
        result.StdOut.ShouldContain("Howdy, Ada!");
    }
}
