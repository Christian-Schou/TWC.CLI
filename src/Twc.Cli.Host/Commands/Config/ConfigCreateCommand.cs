using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using Twc.Cli.AppHost.Plugins;
using Twc.Cli.Host.Configuration;

namespace Twc.Cli.Host.Commands.Config;

/// <summary>
///     Interactive profile generator.
/// </summary>
[Obsolete("Use Twc.Cli.AppHost built-in config wizard via HostAppOptions.EnableConfigWizard.")]
public sealed class ConfigCreateCommand : Command<ConfigCreateCommandSettings>
{
    private readonly PluginContext _pluginContext;

    /// <summary>
    ///     Creates a new command instance.
    /// </summary>
    public ConfigCreateCommand(PluginContext pluginContext)
    {
        _pluginContext = pluginContext ?? throw new ArgumentNullException(nameof(pluginContext));
    }

    /// <inheritdoc />
    public override int Execute(CommandContext context, ConfigCreateCommandSettings settings,
        CancellationToken cancellationToken)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));

        var store = new ProfileStore(settings.ProfilesRoot);
        store.EnsureDefaultProfiles();

        // Determine target profile name
        var profileName = settings.Name;

        if (string.IsNullOrWhiteSpace(profileName) && !settings.NonInteractive)
        {
            var choices = store.ListProfiles().ToList();
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

        var hostProfile = settings.NonInteractive
            ? CreateDefaultProfile(profileName)
            : PromptForProfile(profileName);

        // Build root document
        var doc = new ProfileDocument
        {
            Core = hostProfile
        };

        // Apply plugin defaults from the already-discovered plugin set
        PluginConfigSchemaCollector.ApplyPluginDefaults(doc, _pluginContext.Plugins);

        var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });

        // Save
        if (!string.IsNullOrWhiteSpace(settings.Output))
        {
            var outputPath = Path.GetFullPath(settings.Output);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory());
            File.WriteAllText(outputPath, json);
            AnsiConsole.MarkupLine($"[green]Wrote profile[/] to {outputPath}");
            return 0;
        }

        var path = store.Save(profileName, hostProfile, settings.Overwrite);
        // Also write the full document to the prefixed profile filename
        File.WriteAllText(path, json);

        AnsiConsole.MarkupLine($"[green]Wrote profile[/] '{profileName}' to {path}");
        AnsiConsole.MarkupLine($"Profiles root: {store.ProfilesRoot}");
        return 0;
    }

    private static HostProfile CreateDefaultProfile(string profileName)
    {
        return new HostProfile { Name = profileName };
    }

    private static HostProfile PromptForProfile(string profileName)
    {
        // Start with a profile bound to the chosen name.
        var profile = new HostProfile { Name = profileName };

        // Allow editing fields.
        profile.Environment = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select environment")
                .AddChoices(HostProfileTemplates.Environments));

        profile.ApiBaseUrl = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select API base URL")
                .AddChoices(HostProfileTemplates.ApiBaseUrls));

        profile.LogLevel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select log level")
                .AddChoices(HostProfileTemplates.LogLevels));

        return profile;
    }
}