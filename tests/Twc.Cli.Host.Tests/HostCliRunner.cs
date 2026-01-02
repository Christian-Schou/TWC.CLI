using System.Diagnostics;

namespace Twc.Cli.Host.Tests;

internal static class HostCliRunner
{
    public static (int ExitCode, string StdOut, string StdErr) Run(params string[] args)
        => RunWithEnvironment(environment: null, args);

    public static (int ExitCode, string StdOut, string StdErr) RunWithEnvironment(
        IReadOnlyDictionary<string, string?>? environment,
        params string[] args)
    {
        var hostProject = FindHostProjectPath();

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        if (environment is not null)
        {
            foreach (var kvp in environment)
            {
                psi.Environment[kvp.Key] = kvp.Value;
            }
        }

        psi.ArgumentList.Add("run");
        psi.ArgumentList.Add("--project");
        psi.ArgumentList.Add(hostProject);
        psi.ArgumentList.Add("--no-build");
        psi.ArgumentList.Add("--");

        foreach (var arg in args)
            psi.ArgumentList.Add(arg);

        using var p = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start dotnet process.");

        var stdout = p.StandardOutput.ReadToEnd();
        var stderr = p.StandardError.ReadToEnd();

        p.WaitForExit();
        return (p.ExitCode, stdout, stderr);
    }

    private static string FindHostProjectPath()
    {
        // We can't rely on relative paths from the test output folder.
        // Instead, walk upwards until we find the repo root that contains src/Twc.Cli.Host.
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "Twc.Cli.Host", "Twc.Cli.Host.csproj");
            if (File.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }

        throw new FileNotFoundException("Could not locate Twc.Cli.Host.csproj by walking up from test output folder.");
    }
}
