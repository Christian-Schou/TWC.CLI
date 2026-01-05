# TWC.CLI

![CodeRabbit Pull Request Reviews](https://img.shields.io/coderabbit/prs/github/Christian-Schou/TWC.CLI?utm_source=oss&utm_medium=github&utm_campaign=Christian-Schou%2FTWC.CLI&labelColor=171717&color=FF570A&link=https%3A%2F%2Fcoderabbit.ai&label=CodeRabbit+Reviews)

A .NET 10 CLI framework built on Spectre.Console + Spectre.Console.Cli.

## Documentation

Developer docs live in [`/docs`](./docs/README.md).

## Goals
- Provide a reusable host pipeline (DI, logging, consistent UX)
- Support **plugins as NuGet packages** (from any repository) that contribute command branches
- Keep business logic in plugins so they can be versioned independently
- Provide configuration + credential services usable by plugins
- Support automatic update checks using user-configured providers (GitHub/GitLab/Gitea/Azure DevOps)

## Plugin model (NuGet)
Plugins are regular NuGet packages referenced by the consuming CLI project. The consuming CLI publishes **one** package (typically a .NET tool) and manages plugin versions via normal NuGet dependency management.

There is no runtime “drop-in DLL” plugin loading in v1.

## Repository structure
- `architecture/` — architecture notes and Mermaid diagrams (sequence, context, components, ADRs)

## Build
This repository uses Cake (cakebuild.net) as the build entrypoint and is designed to run on GitHub Actions.

The .NET SDK version is pinned in `global.json` and GitHub Actions uses the same file.

Commands:
- macOS/Linux: `./build.sh --target=CI`
- Windows: `./build.ps1 -Target CI`

The scripts restore local dotnet tools (`dotnet tool restore`) and then run Cake.

## SDK (plugin contract)

The public NuGet plugin contract lives in `src/Twc.Cli.Sdk`. See:
- `src/Twc.Cli.Sdk/README.md`
