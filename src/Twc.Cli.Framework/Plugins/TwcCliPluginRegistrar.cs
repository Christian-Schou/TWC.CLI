// This file is intentionally excluded from compilation via Twc.Cli.Framework.csproj.
//
// It previously contained ITwcCliPluginRegistrar support, but the plugin contract was simplified to:
// - ITwcCliPlugin.RegisterServices(IServiceRegistry)
// - ITwcCliPlugin.RegisterCommands(ICommandRegistry)
//
// TODO: delete this file when repository tooling allows file deletion.

