using Twc.Cli.Host.Plugins.Sample.Commands;
using Twc.Cli.Sdk;

namespace Twc.Cli.Host.Plugins.Sample;

/// <summary>
///     Minimal sample plugin used to validate the host plugin catalog + registration pipeline.
/// </summary>
public sealed class SamplePlugin : ITwcCliPlugin, IConfigSchemaContributor
{
    /// <inheritdoc />
    public IEnumerable<ConfigEntry> GetConfigEntries()
    {
        yield return new ConfigEntry
        {
            Key = "Message",
            DisplayName = "Greeting message",
            Description = "Message printed by the sample plugin.",
            DefaultValue = new ConfigValue.String("Hello from plugin config")
        };
    }

    /// <inheritdoc />
    public PluginMetadata Metadata { get; } = new(
        new PluginId("twc.sample"),
        new PluginDisplayName("Sample Plugin"),
        new SemanticVersion(0, 1, 0),
        VersionRange.Any);

    /// <inheritdoc />
    public void RegisterServices(IServiceRegistry services)
    {
        services.AddSingletonInstance(new SampleMessageProvider("plugin says hi"));
    }

    /// <inheritdoc />
    public void RegisterCommands(ICommandRegistry commands)
    {
        commands.AddCommand("plugin-hello", typeof(PluginHelloCommand),
            "Hello from a plugin (validates plugin registration).");
    }
}