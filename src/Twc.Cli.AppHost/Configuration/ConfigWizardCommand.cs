using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using Twc.Cli.AppHost.Plugins;

namespace Twc.Cli.AppHost.Configuration;

/// <summary>
/// Interactive (or non-interactive) profile generator.
///
/// This command is host-agnostic: hosts provide the core settings type and the prompts.
/// </summary>
public sealed class ConfigWizardCommand<TCore> : Command<ConfigWizardCommandSettings>
    where TCore : class, new()
{
    private readonly string _appName;
    private readonly PluginContext? _pluginContext;
    private readonly Func<string, TCore> _createDefaultCore;
    private readonly Func<string, TCore> _promptForCore;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigWizardCommand(
        string appName,
        PluginContext? pluginContext,
        Func<string, TCore> createDefaultCore,
        Func<string, TCore> promptForCore,
        JsonSerializerOptions? jsonOptions = null)
    {
        _appName = string.IsNullOrWhiteSpace(appName) ? "twc" : appName.Trim();
        _pluginContext = pluginContext;
        _createDefaultCore = createDefaultCore ?? throw new ArgumentNullException(nameof(createDefaultCore));
        _promptForCore = promptForCore ?? throw new ArgumentNullException(nameof(promptForCore));
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions { WriteIndented = true };
    }

    public override int Execute(CommandContext context, ConfigWizardCommandSettings settings, CancellationToken cancellationToken)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));

        var store = new ProfileStore(appName: _appName, profilesRoot: settings.ProfilesRoot);
        store.EnsureDefaultProfiles(_ => _createDefaultCore(_));

        var profileName = settings.Name;

        if (string.IsNullOrWhiteSpace(profileName) && !settings.NonInteractive)
        {
            var choices = store.ListProfiles(_ => true).ToList();
            choices.Add("<create new>");

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a profile")
                    .AddChoices(choices));

            if (selected == "<create new>")
                profileName = AnsiConsole.Ask<string>("New profile name?", "custom");
            else
                profileName = selected;
        }

        if (string.IsNullOrWhiteSpace(profileName))
            profileName = "default";

        var core = settings.NonInteractive
            ? _createDefaultCore(profileName)
            : _promptForCore(profileName);

        var doc = new ProfileDocument<TCore>
        {
            Core = core,
        };

        if (_pluginContext is not null)
            PluginConfigSchemaCollector.ApplyPluginDefaults(doc, _pluginContext.Plugins);

        var json = JsonSerializer.Serialize(doc, _jsonOptions);

        if (!string.IsNullOrWhiteSpace(settings.Output))
        {
            var outputPath = Path.GetFullPath(settings.Output);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory());
            File.WriteAllText(outputPath, json);
            AnsiConsole.MarkupLine($"[green]Wrote profile[/] to {outputPath}");
            return 0;
        }

        var path = store.Save(profileName, doc, overwrite: settings.Overwrite);
        AnsiConsole.MarkupLine($"[green]Wrote profile[/] '{profileName}' to {path}");
        AnsiConsole.MarkupLine($"Profiles root: {store.ProfilesRoot}");
        return 0;
    }
}
