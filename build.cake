var target = Argument("target", "CI");
var configuration = Argument("configuration", "Release");

// Ensure Cake uses the dotnet available on PATH (or DOTNET_PATH if set), rather than a hardcoded /usr/local/bin/dotnet.
var dotnetPath = EnvironmentVariable("DOTNET_PATH") ?? "dotnet";

var artifactsDir = Directory("./artifacts");

// NuGet publish settings
var nugetSource = Argument("nugetSource", EnvironmentVariable("NUGET_SOURCE") ?? "https://api.nuget.org/v3/index.json");
var nugetApiKey = Argument("nugetApiKey", EnvironmentVariable("NUGET_API_KEY") ?? string.Empty);

// Versioning (tag-driven)
var explicitVersion = Argument("version", EnvironmentVariable("VERSION") ?? string.Empty);
var gitRefName = EnvironmentVariable("GITHUB_REF_NAME") ?? string.Empty; // e.g. v1.2.3

string? NormalizeVersion(string? versionOrTag)
{
    if (string.IsNullOrWhiteSpace(versionOrTag))
        return null;

    versionOrTag = versionOrTag.Trim();
    if (versionOrTag.StartsWith("refs/tags/", StringComparison.OrdinalIgnoreCase))
        versionOrTag = versionOrTag["refs/tags/".Length..];

    // Allow passing either vX.Y.Z or X.Y.Z
    if (versionOrTag.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        versionOrTag = versionOrTag[1..];

    return string.IsNullOrWhiteSpace(versionOrTag) ? null : versionOrTag;
}

var versionFromTag = NormalizeVersion(gitRefName);
var packageVersion = NormalizeVersion(explicitVersion) ?? versionFromTag;

// Only pack/publish these projects (v1 public surface)
var packProjects = new[]
{
    File("./src/Twc.Cli.AppHost/Twc.Cli.AppHost.csproj"),
    File("./src/Twc.Cli.Sdk/Twc.Cli.Sdk.csproj"),
};

IEnumerable<FilePath> FindSolutions()
{
    return GetFiles("./*.slnx")
        .Concat(GetFiles("./src/*.slnx"))
        .Concat(GetFiles("./**/*.slnx"))
        .Distinct();
}

Task("Clean")
    .Does(() =>
{
    EnsureDirectoryExists(artifactsDir);
    CleanDirectory(artifactsDir);
});

Task("Restore")
    .Does(() =>
{
    var solutions = FindSolutions().ToList();
    if (!solutions.Any())
    {
        Information("No solution (*.slnx) found yet. Skipping restore.");
        return;
    }

    foreach (var sln in solutions)
    {
        Information($"Restoring {sln.GetFilename()}...");
        DotNetRestore(sln.FullPath, new DotNetRestoreSettings
        {
            ToolPath = dotnetPath,
        });
    }
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var solutions = FindSolutions().ToList();
    if (!solutions.Any())
    {
        Information("No solution (*.slnx) found yet. Skipping build.");
        return;
    }

    foreach (var sln in solutions)
    {
        Information($"Building {sln.GetFilename()} ({configuration})...");
        DotNetBuild(sln.FullPath, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true,
            ToolPath = dotnetPath,
        });
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    var solutions = FindSolutions().ToList();
    if (!solutions.Any())
    {
        Information("No solution (*.slnx) found yet. Skipping tests.");
        return;
    }

    foreach (var sln in solutions)
    {
        Information($"Testing {sln.GetFilename()} ({configuration})...");
        DotNetTest(sln.FullPath, new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            ToolPath = dotnetPath,
        });
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(artifactsDir);

    if (string.IsNullOrWhiteSpace(packageVersion))
        throw new Exception("Missing package version. On CI, publish runs from tags like 'v1.2.3'. Locally, pass --version=1.2.3.");

    foreach (var proj in packProjects)
    {
        Information($"Packing {proj.GetFilename()} ({configuration}) version={packageVersion}...");
        DotNetPack(proj.FullPath, new DotNetPackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            OutputDirectory = artifactsDir,
            ToolPath = dotnetPath,
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Version", packageVersion)
                .WithProperty("PackageVersion", packageVersion)
                .WithProperty("AssemblyVersion", packageVersion)
                .WithProperty("FileVersion", packageVersion)
                .WithProperty("ContinuousIntegrationBuild", "true"),
        });
    }
});

Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
{
    if (string.IsNullOrWhiteSpace(nugetApiKey))
        throw new Exception("Missing NuGet API key. Set NUGET_API_KEY env var or pass --nugetApiKey=... to Cake.");

    var expected = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        $"Twc.Cli.AppHost.{packageVersion}.nupkg",
        $"Twc.Cli.Sdk.{packageVersion}.nupkg",
    };

    var packages = GetFiles(artifactsDir + "/**/*.nupkg")
        .Where(p => !p.FullPath.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase))
        .Where(p => expected.Contains(p.GetFilename().ToString()))
        .ToList();

    if (!packages.Any())
        throw new Exception($"No expected .nupkg files found in {artifactsDir}. Expected: {string.Join(", ", expected)}");

    Information($"Publishing {packages.Count} package(s) to {nugetSource}...");

    foreach (var pkg in packages)
    {
        Information($"Pushing {pkg.GetFilename()}...");
        DotNetNuGetPush(pkg.FullPath, new DotNetNuGetPushSettings
        {
            Source = nugetSource,
            ApiKey = nugetApiKey,
            SkipDuplicate = true,
            ToolPath = dotnetPath,
        });
    }
});

Task("Release")
    .IsDependentOn("CI")
    .IsDependentOn("Pack");

Task("CI")
    .IsDependentOn("Clean")
    .IsDependentOn("Test");

RunTarget(target);
