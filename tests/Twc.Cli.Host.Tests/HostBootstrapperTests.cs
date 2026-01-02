using Shouldly;
using Xunit;

namespace Twc.Cli.Host.Tests;

public sealed class HostBootstrapperTests
{
    [Fact]
    public void Running_without_args_shows_help_and_exits_successfully()
    {
        var app = HostBootstrapper.BuildCommandApp([]);

        using var output = new StringWriter();
        using var error = new StringWriter();
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            Console.SetOut(output);
            Console.SetError(error);

            var exitCode = app.Run(Array.Empty<string>());

            exitCode.ShouldBe(0);
            output.ToString().ShouldContain("USAGE");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    [Fact]
    public void Hello_command_runs_and_uses_DI_to_format_message()
    {
        Environment.SetEnvironmentVariable("TWC_ENV_PREFIX", null);
        Environment.SetEnvironmentVariable("TWC_Greeting__Prefix", null);
        Environment.SetEnvironmentVariable("ACME_Greeting__Prefix", null);

        var app = HostBootstrapper.BuildCommandApp([]);

        using var output = new StringWriter();
        using var error = new StringWriter();
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            Console.SetOut(output);
            Console.SetError(error);

            var exitCode = app.Run(["hello", "Ada"]);

            exitCode.ShouldBe(0);
            output.ToString().ShouldContain("Hello, Ada!");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    [Fact]
    public void Plugin_command_is_registered_and_can_execute()
    {
        var result = HostCliRunner.Run("plugin-hello");

        result.ExitCode.ShouldBe(0, $"STDOUT:\n{result.StdOut}\nSTDERR:\n{result.StdErr}");
    }
}
