using Twc.Cli.Sdk;

namespace Twc.Cli.Host.Plugins.Sample;

/// <summary>
///     Demonstrates how a plugin can contribute config entries.
/// </summary>
public sealed class SamplePluginConfigSchema : IConfigSchemaContributor
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
}