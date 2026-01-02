# Twc.Cli.AppHost (plugins module)

`Twc.Cli.AppHost` includes an internal "plugins" module under the `Twc.Cli.AppHost.Plugins` namespace.

## Discovery model (v1)

Plugins are discovered from **referenced/loaded assemblies** using an assembly attribute:

```csharp
using Twc.Cli.Sdk;

[assembly: TwcCliPlugin(typeof(MyPlugin))]
```

The plugin type must implement `ITwcCliPlugin`. The host discovers these plugin types, instantiates them, and calls `Register(...)`.

