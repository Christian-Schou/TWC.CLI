using System.Text.Json;
using Shouldly;
using Xunit;

namespace Twc.Cli.Host.Tests;

public sealed class ConfigWizardTests
{
    [Fact]
    public void Config_bootstraps_default_profiles_and_can_create_custom_profile_non_interactive()
    {
        var tempDir = Directory.CreateTempSubdirectory("twc-cli-profiles-");
        try
        {
            var prefix = HostInfo.ApplicationName;

            // First run: should bootstrap dev/test/prod.
            var bootstrap = HostCliRunner.Run("config", "--non-interactive", "--profiles-root", tempDir.FullName, "--name", "dev", "--overwrite");
            bootstrap.ExitCode.ShouldBe(0, $"STDOUT:\n{bootstrap.StdOut}\nSTDERR:\n{bootstrap.StdErr}");

            File.Exists(Path.Combine(tempDir.FullName, $"{prefix}.dev.json")).ShouldBeTrue();
            File.Exists(Path.Combine(tempDir.FullName, $"{prefix}.test.json")).ShouldBeTrue();
            File.Exists(Path.Combine(tempDir.FullName, $"{prefix}.prod.json")).ShouldBeTrue();

            // Create custom
            var custom = HostCliRunner.Run("config", "--non-interactive", "--profiles-root", tempDir.FullName, "--name", "myteam");
            custom.ExitCode.ShouldBe(0, $"STDOUT:\n{custom.StdOut}\nSTDERR:\n{custom.StdErr}");

            var customFile = Path.Combine(tempDir.FullName, $"{prefix}.myteam.json");
            File.Exists(customFile).ShouldBeTrue();

            using var doc = JsonDocument.Parse(File.ReadAllText(customFile));

            // Core section
            doc.RootElement.GetProperty("Core").GetProperty("Name").GetString().ShouldBe("myteam");

            // Plugin section (sample plugin contributes Message)
            var plugins = doc.RootElement.GetProperty("PluginSettings");
            plugins.GetProperty("twc.sample").GetProperty("Message").GetString().ShouldBe("Hello from plugin config");
        }
        finally
        {
            try { tempDir.Delete(recursive: true); } catch { /* ignore */ }
        }
    }

    [Fact]
    public void Non_profile_json_files_are_not_treated_as_profiles()
    {
        var tempDir = Directory.CreateTempSubdirectory("twc-cli-profiles-");
        try
        {
            var prefix = HostInfo.ApplicationName;

            // Write a random JSON file that should NOT be treated as a profile.
            File.WriteAllText(Path.Combine(tempDir.FullName, "random.json"), "{\"foo\":123}");

            // Run config once to bootstrap defaults.
            var result = HostCliRunner.Run("config", "--non-interactive", "--profiles-root", tempDir.FullName, "--name", "dev", "--overwrite");
            result.ExitCode.ShouldBe(0, $"STDOUT:\n{result.StdOut}\nSTDERR:\n{result.StdErr}");

            // Ensure defaults exist but random.json remains.
            File.Exists(Path.Combine(tempDir.FullName, $"{prefix}.dev.json")).ShouldBeTrue();
            File.Exists(Path.Combine(tempDir.FullName, "random.json")).ShouldBeTrue();

            // Create another profile to ensure the command path works with extra json present.
            var custom = HostCliRunner.Run("config", "--non-interactive", "--profiles-root", tempDir.FullName, "--name", "myteam", "--overwrite");
            custom.ExitCode.ShouldBe(0, $"STDOUT:\n{custom.StdOut}\nSTDERR:\n{custom.StdErr}");
            File.Exists(Path.Combine(tempDir.FullName, $"{prefix}.myteam.json")).ShouldBeTrue();
        }
        finally
        {
            try { tempDir.Delete(recursive: true); } catch { /* ignore */ }
        }
    }
}
