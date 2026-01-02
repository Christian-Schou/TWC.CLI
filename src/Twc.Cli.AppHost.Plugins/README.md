# Twc.Cli.AppHost.Plugins

This package adds **plugin discovery + registration** for CLI tools built with `Twc.Cli.AppHost`.

## Install

**Host app (CLI tool):**

- `Twc.Cli.AppHost` (required)
- `Twc.Cli.AppHost.Plugins` (required for plugin discovery in v1)

**Plugin package:**

- `Twc.Cli.Sdk`

## Discovery model (v1)

Plugins are discovered from **referenced/loaded assemblies** using an assembly attribute:

```csharp
using Twc.Cli.Sdk;

[assembly: TwcCliPlugin(typeof(MyPlugin))]
```

The plugin type must implement `ITwcCliPlugin`. The host discovers these plugin types, instantiates them, and calls `Register(...)`.

