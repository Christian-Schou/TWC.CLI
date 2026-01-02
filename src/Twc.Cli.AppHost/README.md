# Twc.Cli.AppHost

Reusable **Spectre.Console.Cli + Microsoft.Extensions.DependencyInjection** bootstrapper for building CLI tools.

This project is intended to be packaged and published to NuGet so tool authors can consume it from their own console apps.

## What to install

### Host app (CLI tool)

Install:

- `Twc.Cli.AppHost`

### Plugin package

Install:

- `Twc.Cli.Sdk`

## What it provides

- A `HostAppBuilder` that:
  - builds `IConfiguration` from JSON + env vars
  - wires `IServiceCollection`
  - creates a `CommandApp` with a Spectre DI registrar
- A high-level `HostApp.Run(...)` helper to get a CLI running with minimal boilerplate
- Plugin discovery helpers under `Twc.Cli.AppHost.Plugins` (v1: referenced/loaded assemblies)

## Notes

- Global options like `--config`/`--env-prefix` are parsed by the host builder (pre-Spectre) so they can influence early configuration.
- Profile/config wizard UX should live in a separate package so hosts can opt-in.
