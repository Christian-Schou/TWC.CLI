# Twc.Cli.Sdk (plugin contract)

This package defines the **public plugin contract** between a TWC CLI host and plugins shipped as **NuGet packages**.

## Design goals

- No runtime DLL scanning in v1 (explicit registration only)
- Host can reference the SDK **without pulling in Spectre dependencies**
- SDK exposes minimal abstractions for:
  - plugin identity + metadata
  - host compatibility rules
  - registering commands + services

## Plugin registration

A plugin implements `ITwcCliPlugin`:

- `Metadata` declares identity, plugin version, and the required host version range.
- `Register(registrar)` is called by the host. The plugin registers services and contributes command branches.

## Compatibility

Plugins declare their supported host range via `PluginMetadata.RequiresHostVersionRange` (`VersionRange`).

Range syntax supported by the SDK:

- `*` any host version
- `>=1.2.3`
- `>=1.2.3 <2.0.0`

### Host behavior on incompatibility

When the host discovers a plugin reference, it should:

1. Validate compatibility: `registrar.IsHostCompatible(plugin.Metadata)` (or `HostCompatibility.IsCompatible(hostVersion, requiredRange)`)
2. If incompatible, do **not** load the plugin and surface a friendly error.

## Command registration abstraction

Plugins register commands via `ICommandRegistry`.

The SDK does not define Spectre types; instead, the host adapts `ICommandRegistry` to Spectre.Cli internally.

## Service registration abstraction

Plugins register services via `IServiceRegistry` (a minimal abstraction). The host can adapt it to Microsoft DI (or any container).

