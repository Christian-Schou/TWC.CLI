param(
    [string]$Target = "CI",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

dotnet tool restore

dotnet cake build.cake --target=$Target --configuration=$Configuration
