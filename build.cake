var target = Argument("target", "CI");
var configuration = Argument("configuration", "Release");

var artifactsDir = Directory("./artifacts");

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
        DotNetRestore(sln.FullPath);
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
        });
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var solutions = FindSolutions().ToList();
    if (!solutions.Any())
    {
        Information("No solution (*.slnx) found yet. Skipping pack.");
        return;
    }

    foreach (var sln in solutions)
    {
        Information($"Packing {sln.GetFilename()} ({configuration})...");
        DotNetPack(sln.FullPath, new DotNetPackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            OutputDirectory = artifactsDir,
        });
    }
});

Task("CI")
    .IsDependentOn("Clean")
    .IsDependentOn("Test");

RunTarget(target);
